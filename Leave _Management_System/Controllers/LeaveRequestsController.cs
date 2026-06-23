using Leave__Management_System.Common;
using Leave__Management_System.Data;
using Leave__Management_System.Models.LeaveAllocations;
using Leave__Management_System.Models.LeaveRequests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Leave__Management_System.Controllers
{
    [Authorize]
    public class LeaveRequestsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public LeaveRequestsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Employee: create a leave request for a specific allocation
        public async Task<IActionResult> Create(int id)
        {
            if (id <= 0)
            {
                TempData["Error"] = "Invalid allocation selected.";
                return RedirectToAction("Details", "LeaveAllocation");
            }

            var allocation = await _context.LeaveAllocations
                .Include(a => a.LeaveType)
                .Include(a => a.Period)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (allocation == null)
            {
                TempData["Error"] = "Selected allocation was not found.";
                return RedirectToAction("Details", "LeaveAllocation");
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            if (allocation.EmployeeId != user.Id)
            {
                TempData["Error"] = "You are not authorized to request leave for this allocation.";
                return RedirectToAction("Details", "LeaveAllocation");
            }

            // prevent requesting when no remaining days
            if (allocation.RemainingDays <= 0)
            {
                TempData["Error"] = "No remaining days available for this allocation.";
                return RedirectToAction("Details", "LeaveAllocation");
            }

            // Load all available leave types for the current period
            var leaveTypes = await _context.LeaveTypes.ToListAsync();

            var vm = new LeaveRequestCreateVM
            {
                LeaveAllocationId = allocation.Id,
                LeaveTypeId = allocation.LeaveTypeId,
                LeaveTypeName = allocation.LeaveType?.Name ?? string.Empty,
                PeriodId = allocation.PeriodId,
                PeriodStart = allocation.Period?.StartDate ?? default,
                PeriodEnd = allocation.Period?.EndDate ?? default,
                MaxDays = Math.Max(allocation.RemainingDays, allocation.NumberOfDays),
                OriginalDays = allocation.NumberOfDays,
                LeaveTypeOptions = leaveTypes.Select(lt => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Value = lt.Id.ToString(),
                    Text = lt.Name,
                    Selected = (lt.Id == allocation.LeaveTypeId)
                }).ToList()
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LeaveRequestCreateVM model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var allocation = await _context.LeaveAllocations
                .Include(a => a.LeaveType)
                .Include(a => a.Period)
                .FirstOrDefaultAsync(a => a.Id == model.LeaveAllocationId);

            if (allocation == null)
            {
                ModelState.AddModelError(string.Empty, "Selected allocation was not found.");
                return View(model);
            }

            if (allocation.EmployeeId != user.Id) return Forbid();

            // Re-populate display fields so view can show them on validation errors
            model.LeaveTypeName = allocation.LeaveType?.Name ?? string.Empty;
            model.PeriodStart = allocation.Period?.StartDate ?? default;
            model.PeriodEnd = allocation.Period?.EndDate ?? default;
            model.MaxDays = Math.Max(allocation.RemainingDays, allocation.NumberOfDays);
            model.OriginalDays = allocation.NumberOfDays;
            model.PeriodId = allocation.PeriodId;

            // Check if ModelState is valid (includes IValidatableObject validation)
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (allocation.RemainingDays <= 0)
            {
                ModelState.AddModelError(string.Empty, "No remaining days available for this allocation.");
                return View(model);
            }

            // Create the leave request
            var leaveRequest = new LeaveRequest
            {
                EmployeeId = user.Id,
                LeaveTypeId = model.LeaveTypeId,
                PeriodId = allocation.PeriodId,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                NumberOfDays = model.NumberOfDays,
                RequestDate = DateTime.UtcNow,
                Status = LeaveRequestStatus.Pending,
                AdditionalInformation = model.AdditionalInformation?.Trim()
            };

            _context.LeaveRequests.Add(leaveRequest);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Your leave request has been submitted successfully.";
            return RedirectToAction("Details", "LeaveAllocation");
        }

        // Supervisor: list pending requests
        [Authorize(Roles = Roles.Supervisor)]
        public async Task<IActionResult> Index(string? sortOrder, string? filter)
        {
            var query = _context.LeaveRequests
                .Include(r => r.LeaveType)
                .Include(r => r.Employee)
                .Include(r => r.Period)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter))
            {
                query = query.Where(r => r.Employee!.UserName!.Contains(filter) || r.LeaveType!.Name.Contains(filter));
            }

            query = sortOrder switch
            {
                "date" => query.OrderByDescending(r => r.RequestDate),
                "type" => query.OrderBy(r => r.LeaveType!.Name),
                _ => query.OrderByDescending(r => r.RequestDate)
            };

            var list = await query.ToListAsync();

            // Map to view models
            var vms = list.Select(r => new LeaveRequestViewModel
            {
                Id = r.Id,
                EmployeeId = r.EmployeeId,
                EmployeeName = r.Employee != null ? $"{r.Employee.FirstName} {r.Employee.LastName}" : r.Employee?.UserName ?? string.Empty,
                LeaveTypeId = r.LeaveTypeId,
                LeaveTypeName = r.LeaveType?.Name ?? string.Empty,
                PeriodId = r.PeriodId,
                PeriodName = r.Period?.Name ?? string.Empty,
                StartDate = r.StartDate,
                EndDate = r.EndDate,
                NumberOfDays = r.NumberOfDays,
                RequestDate = r.RequestDate,
                Status = r.Status,
                ApprovedById = r.ApprovedById,
                DecisionDate = r.DecisionDate,
                AdditionalInformation = r.AdditionalInformation
            }).ToList();

            return View(vms);
        }

        [Authorize(Roles = Roles.Supervisor)]
        public async Task<IActionResult> Details(int id)
        {
            var r = await _context.LeaveRequests
                .Include(rq => rq.Employee)
                .Include(rq => rq.LeaveType)
                .Include(rq => rq.Period)
                .FirstOrDefaultAsync(rq => rq.Id == id);

            if (r == null) return NotFound();

            var vm = new LeaveRequestViewModel
            {
                Id = r.Id,
                EmployeeId = r.EmployeeId,
                EmployeeName = r.Employee != null ? $"{r.Employee.FirstName} {r.Employee.LastName}" : r.Employee?.UserName ?? string.Empty,
                LeaveTypeId = r.LeaveTypeId,
                LeaveTypeName = r.LeaveType?.Name ?? string.Empty,
                PeriodId = r.PeriodId,
                PeriodName = r.Period?.Name ?? string.Empty,
                StartDate = r.StartDate,
                EndDate = r.EndDate,
                NumberOfDays = r.NumberOfDays,
                RequestDate = r.RequestDate,
                Status = r.Status,
                ApprovedById = r.ApprovedById,
                DecisionDate = r.DecisionDate,
                ApprovedByName = r.ApprovedBy != null ? $"{r.ApprovedBy.FirstName} {r.ApprovedBy.LastName}" : string.Empty,
                AdditionalInformation = r.AdditionalInformation
            };

            return View(vm);
        }

        [HttpPost]
        [Authorize(Roles = Roles.Supervisor)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            var request = await _context.LeaveRequests.FindAsync(id);
            if (request == null) return NotFound();

            if (request.Status != LeaveRequestStatus.Pending) return BadRequest("Request already processed.");

            var allocation = await _context.LeaveAllocations
                .FirstOrDefaultAsync(a => a.EmployeeId == request.EmployeeId && a.LeaveTypeId == request.LeaveTypeId && a.PeriodId == request.PeriodId);

            if (allocation == null) return BadRequest("No allocation found for this request.");

            allocation.RemainingDays = Math.Max(0, allocation.RemainingDays - request.NumberOfDays);
            request.Status = LeaveRequestStatus.Approved;
            request.DecisionDate = DateTime.UtcNow;
            request.ApprovedById = _userManager.GetUserId(User);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize(Roles = Roles.Supervisor)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deny(int id)
        {
            var request = await _context.LeaveRequests.FindAsync(id);
            if (request == null) return NotFound();

            if (request.Status != LeaveRequestStatus.Pending) return BadRequest("Request already processed.");

            request.Status = LeaveRequestStatus.Denied;
            request.DecisionDate = DateTime.UtcNow;
            request.ApprovedById = _userManager.GetUserId(User);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
