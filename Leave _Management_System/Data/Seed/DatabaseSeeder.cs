using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Leave__Management_System.Data.Seed
{
    public static class DatabaseSeeder
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var services = scope.ServiceProvider;

            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            var db = services.GetRequiredService<ApplicationDbContext>();
            var allocationsService = services.GetService<Leave__Management_System.Services.LeaveAllocation.ILeaveAllocationsService>();

            // Roles
            var roles = new[] { "Administrator", "Supervisor", "Employee" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Admin user
            var adminEmail = "admin@example.com";
            var admin = await userManager.FindByEmailAsync(adminEmail);
            if (admin == null)
            {
                admin = new ApplicationUser
                {
                    UserName = "admin",
                    Email = adminEmail,
                    EmailConfirmed = true,
                    FirstName = "System",
                    LastName = "Admin",
                    DateOfBirth = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-30))
                };
                var res = await userManager.CreateAsync(admin, "Admin123!");
                if (res.Succeeded)
                    await userManager.AddToRoleAsync(admin, "Administrator");
            }

            // Supervisor user
            var supEmail = "supervisor@example.com";
            var sup = await userManager.FindByEmailAsync(supEmail);
            if (sup == null)
            {
                sup = new ApplicationUser
                {
                    UserName = "supervisor",
                    Email = supEmail,
                    EmailConfirmed = true,
                    FirstName = "Default",
                    LastName = "Supervisor",
                    DateOfBirth = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-35))
                };
                var res = await userManager.CreateAsync(sup, "Supervisor123!");
                if (res.Succeeded)
                    await userManager.AddToRoleAsync(sup, "Supervisor");
            }

            // Employee users
            var employeeEmails = new[] { "employee1@example.com", "employee2@example.com" };
            foreach (var e in employeeEmails)
            {
                var user = await userManager.FindByEmailAsync(e);
                if (user == null)
                {
                    user = new ApplicationUser
                    {
                        UserName = e.Split('@')[0],
                        Email = e,
                        EmailConfirmed = true,
                        FirstName = e.Split('@')[0],
                        LastName = "Employee",
                        DateOfBirth = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-28))
                    };
                    var res = await userManager.CreateAsync(user, "Employee123!");
                    if (res.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user, "Employee");
                    }
                }
            }

            // Create a Period for current year if missing
            var currentYear = DateTime.UtcNow.Year;
            if (!db.Periods.Any(p => p.EndDate.Year == currentYear))
            {
                var start = DateOnly.FromDateTime(new DateTime(currentYear, 1, 1));
                var end = DateOnly.FromDateTime(new DateTime(currentYear, 12, 31));
                db.Periods.Add(new Period { Name = currentYear.ToString(), StartDate = start, EndDate = end });
                await db.SaveChangesAsync();
            }

            // Seed LeaveTypes if missing
            if (!db.LeaveTypes.Any())
            {
                db.LeaveTypes.AddRange(
                    new LeaveType { Name = "Annual", NumberOfDays = 21 },
                    new LeaveType { Name = "Sick", NumberOfDays = 10 },
                    new LeaveType { Name = "Maternity", NumberOfDays = 90 }
                );
                await db.SaveChangesAsync();
            }

            // Assign supervisor to employees via SupervisionAssignment table if not present
            var supervisorUser = await userManager.FindByEmailAsync(supEmail);
            foreach (var e in employeeEmails)
            {
                var emp = await userManager.FindByEmailAsync(e);
                if (emp != null && supervisorUser != null && !db.SupervisionAssignments.Any(sa => sa.SupervisorId == supervisorUser.Id && sa.EmployeeId == emp.Id))
                {
                    db.SupervisionAssignments.Add(new SupervisionAssignment { SupervisorId = supervisorUser.Id, EmployeeId = emp.Id });
                }
            }
            await db.SaveChangesAsync();

            // Ensure allocations exist for seeded users
            if (allocationsService != null)
            {
                var allUsers = userManager.Users.ToList();
                foreach (var user in allUsers)
                {
                    var hasAlloc = db.LeaveAllocations.Any(a => a.EmployeeId == user.Id && a.Period.EndDate.Year == currentYear);
                    if (!hasAlloc)
                    {
                        await allocationsService.AllocateLeave(user.Id);
                    }
                }
            }

            // Create sample leave requests for employee1 if none exist
            var emp1 = await userManager.FindByEmailAsync(employeeEmails.First());
            var lt = db.LeaveTypes.FirstOrDefault();
            var period = db.Periods.FirstOrDefault(p => p.EndDate.Year == currentYear);
            if (emp1 != null && lt != null && period != null && !db.LeaveRequests.Any(lr => lr.EmployeeId == emp1.Id))
            {
                db.LeaveRequests.Add(new LeaveRequest
                {
                    EmployeeId = emp1.Id,
                    LeaveTypeId = lt.Id,
                    PeriodId = period.Id,
                    StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7)),
                    EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10)),
                    NumberOfDays = 3,
                    RequestDate = DateTime.UtcNow,
                    Status = LeaveRequestStatus.Pending
                });
                await db.SaveChangesAsync();
            }
        }
    }
}
