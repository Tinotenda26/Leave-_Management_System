using Leave__Management_System.Services.LeaveAllocation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Leave__Management_System.Data;
using Microsoft.EntityFrameworkCore;
using Leave__Management_System.Models.LeaveAllocations;
using Microsoft.AspNetCore.Identity;
using Leave__Management_System.Common;
using AutoMapper;

namespace Leave__Management_System.Controllers
{
    [Authorize]
    public class LeaveAllocationController : Controller
    {
        private readonly ILeaveAllocationsService _leaveAllocationsService;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public LeaveAllocationController(ILeaveAllocationsService leaveAllocationsService, ApplicationDbContext context, UserManager<ApplicationUser> userManager, IMapper mapper)
        {
            _leaveAllocationsService = leaveAllocationsService;
            _context = context;
            _userManager = userManager;
            _mapper = mapper;
        }

        // Employee: view own allocations
        public async Task<IActionResult> Details(string? userId)
        {
            var vm = await _leaveAllocationsService.GetEmployeeAllocations(userId);
            return View(vm);
        }

        // Supervisor: view allocations for all users
        [Authorize(Roles = Roles.Supervisor)]
        public async Task<IActionResult> Index()
        {
            var allocations = await _context.LeaveAllocations
                .Include(a => a.LeaveType)
                .Include(a => a.Period)
                .Include(a => a.Employee)
                .ToListAsync();

            var grouped = allocations
                .GroupBy(a => a.EmployeeId)
                .Select(g => new UserAllocationsViewModel
                {
                    EmployeeId = g.Key,
                    EmployeeName = g.First().Employee != null ? ($"{g.First().Employee.FirstName} {g.First().Employee.LastName}") : string.Empty,
                    Email = g.First().Employee?.Email ?? string.Empty,
                    Allocations = g.Select(a => new LeaveAllocationViewModel
                    {
                        Id = a.Id,
                        LeaveTypeId = a.LeaveTypeId,
                        LeaveTypeName = a.LeaveType?.Name ?? string.Empty,
                        NumberOfDays = a.NumberOfDays,
                        RemainingDays = a.RemainingDays,
                        PeriodId = a.PeriodId,
                        PeriodStart = a.Period?.StartDate ?? default,
                        PeriodEnd = a.Period?.EndDate ?? default
                    }).ToList()
                })
                .ToList();

            return View(grouped);
        }

        // Supervisor: view all allocations in a single page for management
        [Authorize(Roles = Roles.Supervisor)]
        public async Task<IActionResult> AllAllocations(string? employeeFilter = "")
        {
            var allocations = await _context.LeaveAllocations
                .Include(a => a.LeaveType)
                .Include(a => a.Period)
                .Include(a => a.Employee)
                .ToListAsync();

            // Filter by employee if provided
            if (!string.IsNullOrWhiteSpace(employeeFilter))
            {
                allocations = allocations
                    .Where(a => a.Employee.FirstName.Contains(employeeFilter, StringComparison.OrdinalIgnoreCase) ||
                                a.Employee.LastName.Contains(employeeFilter, StringComparison.OrdinalIgnoreCase) ||
                                a.Employee.Email.Contains(employeeFilter, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            var vms = allocations.Select(a => new AllocationEditViewModel
            {
                Id = a.Id,
                EmployeeId = a.EmployeeId,
                EmployeeName = a.Employee != null ? $"{a.Employee.FirstName} {a.Employee.LastName}" : string.Empty,
                LeaveTypeId = a.LeaveTypeId,
                LeaveTypeName = a.LeaveType?.Name ?? string.Empty,
                NumberOfDays = a.NumberOfDays,
                RemainingDays = a.RemainingDays,
                PeriodId = a.PeriodId,
                PeriodName = a.Period?.Name ?? string.Empty,
                PeriodStart = a.Period?.StartDate ?? default,
                PeriodEnd = a.Period?.EndDate ?? default
            }).ToList();

            ViewData["EmployeeFilter"] = employeeFilter;
            return View(vms);
        }

        // Supervisor: edit single allocation
        [Authorize(Roles = Roles.Supervisor)]
        [HttpGet]
        public async Task<IActionResult> EditAllocation(int id)
        {
            var allocation = await _context.LeaveAllocations
                .Include(a => a.LeaveType)
                .Include(a => a.Period)
                .Include(a => a.Employee)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (allocation == null)
                return NotFound();

            var vm = new AllocationEditViewModel
            {
                Id = allocation.Id,
                EmployeeId = allocation.EmployeeId,
                EmployeeName = allocation.Employee != null ? $"{allocation.Employee.FirstName} {allocation.Employee.LastName}" : string.Empty,
                LeaveTypeId = allocation.LeaveTypeId,
                LeaveTypeName = allocation.LeaveType?.Name ?? string.Empty,
                NumberOfDays = allocation.NumberOfDays,
                RemainingDays = allocation.RemainingDays,
                PeriodId = allocation.PeriodId,
                PeriodName = allocation.Period?.Name ?? string.Empty,
                PeriodStart = allocation.Period?.StartDate ?? default,
                PeriodEnd = allocation.Period?.EndDate ?? default
            };

            // If request is AJAX (modal load), return partial view without layout (no navbar)
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_EditAllocationPartial", vm);
            }

            return View(vm);
        }

        // Supervisor: save allocation changes
        [Authorize(Roles = Roles.Supervisor)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAllocation(int id, AllocationEditViewModel model)
        {
            if (id != model.Id)
                return NotFound();

            if (!ModelState.IsValid)
                return View(model);

            var allocation = await _context.LeaveAllocations.FindAsync(id);
            if (allocation == null)
                return NotFound();

            // Update only NumberOfDays and RemainingDays
            allocation.NumberOfDays = model.NumberOfDays;
            allocation.RemainingDays = model.RemainingDays;

            try
            {
                _context.Update(allocation);
                await _context.SaveChangesAsync();

                // If AJAX request, return JSON success to close modal and refresh client
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = true, message = $"Allocation updated successfully for {model.EmployeeName}" });
                }

                TempData["Success"] = $"Allocation updated successfully for {model.EmployeeName}";
                return RedirectToAction(nameof(AllAllocations));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    // return the partial view again so modal can display errors
                    return PartialView("_EditAllocationPartial", model);
                }

                TempData["Error"] = "An error occurred while updating the allocation.";
                return View(model);
            }
        }
    }
}
