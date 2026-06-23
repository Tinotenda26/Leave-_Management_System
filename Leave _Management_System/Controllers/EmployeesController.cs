using Leave__Management_System.Common;
using Leave__Management_System.Services.LeaveAllocation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Leave__Management_System.Data;
using Leave__Management_System.Models.LeaveAllocations;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Leave__Management_System.Controllers
{
    [Authorize(Roles = Roles.Supervisor + "," + Roles.Administrator)]
    public class EmployeesController : Controller
    {

        private readonly ILeaveAllocationsService _leaveAllocationsService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public EmployeesController(ILeaveAllocationsService leaveAllocationsService, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _leaveAllocationsService = leaveAllocationsService;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: /Employees
        public async Task<IActionResult> Index()
        {
            var employees = await _leaveAllocationsService.GetEmployees();
            return View(employees);
        }

        // GET: /Employees/Create
        public async Task<IActionResult> Create()
        {
            var model = new EmployeeCreateViewModel();

            // Populate supervisors list
            var supervisorRole = await _roleManager.FindByNameAsync(Roles.Supervisor);
            if (supervisorRole != null)
            {
                var supervisors = await _userManager.GetUsersInRoleAsync(Roles.Supervisor);
                model.Supervisors = supervisors.Select(s => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Value = s.Id,
                    Text = $"{s.FirstName} {s.LastName} ({s.Email})"
                }).ToList();
            }

            // Add empty option
            model.Supervisors.Insert(0, new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Value = string.Empty,
                Text = "-- Select a Supervisor (Optional) --"
            });

            return View(model);
        }

        // POST: /Employees/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EmployeeCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Re-populate supervisors list on validation error
                var supervisorRole = await _roleManager.FindByNameAsync(Roles.Supervisor);
                if (supervisorRole != null)
                {
                    var supervisors = await _userManager.GetUsersInRoleAsync(Roles.Supervisor);
                    model.Supervisors = supervisors.Select(s => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                    {
                        Value = s.Id,
                        Text = $"{s.FirstName} {s.LastName} ({s.Email})"
                    }).ToList();
                }
                model.Supervisors.Insert(0, new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Value = string.Empty,
                    Text = "-- Select a Supervisor (Optional) --"
                });
                return View(model);
            }

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                DateOfBirth = model.DateOfBirth.HasValue ? DateOnly.FromDateTime(model.DateOfBirth.Value) : default
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                foreach (var err in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, err.Description);
                }
                return View(model);
            }

            // ensure role exists
            if (!await _roleManager.RoleExistsAsync(Roles.Employee))
            {
                await _roleManager.CreateAsync(new IdentityRole(Roles.Employee));
            }

            await _userManager.AddToRoleAsync(user, Roles.Employee);

            // Assign supervisor if provided
            if (!string.IsNullOrEmpty(model.SupervisorId))
            {
                var db = HttpContext.RequestServices.GetService<ApplicationDbContext>();
                if (db != null)
                {
                    var assignment = new SupervisionAssignment
                    {
                        SupervisorId = model.SupervisorId,
                        EmployeeId = user.Id,
                        AssignmentDate = DateTime.UtcNow,
                        IsActive = true
                    };
                    db.SupervisionAssignments.Add(assignment);
                    await db.SaveChangesAsync();
                }
            }

            // allocate leave for new user
            try
            {
                await _leaveAllocationsService.AllocateLeave(user.Id);
            }
            catch
            {
                // ignore allocation errors for now
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: /Employees/Delete/{id}
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();
            var vm = new EmployeeListViewModel
            {
                EmployeeId = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                EmployeeName = $"{user.FirstName} {user.LastName}",
                EmailAddress = user.Email
            };
            return View(vm);
        }

        // POST: /Employees/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();
            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                // show errors
                TempData["Error"] = string.Join("; ", result.Errors.Select(e => e.Description));
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
