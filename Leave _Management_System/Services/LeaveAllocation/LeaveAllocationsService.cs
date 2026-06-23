using System;
using System.Threading.Tasks;
using Leave__Management_System.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Leave__Management_System.Models.LeaveAllocations;
using Microsoft.AspNetCore.Identity;
using AutoMapper;

namespace Leave__Management_System.Services.LeaveAllocation
{
    public class LeaveAllocationsService : ILeaveAllocationsService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public LeaveAllocationsService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager, IMapper mapper)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task AllocateLeave(string employeeId)
        {
            var leaveTypes = await _context.LeaveTypes.ToListAsync();

            // get the current period based on the year
            var currentDate = DateTime.Now;
            var period = await _context.Periods
                .Where(q => q.EndDate.Year == currentDate.Year)
                .OrderByDescending(q => q.EndDate)
                .FirstOrDefaultAsync();

            // If no period found, skip allocation
            if (period == null)
                return;

            var monthsRemaining = period.EndDate.Month - currentDate.Month;

            foreach (var leaveType in leaveTypes)
            {
                // Set to fixed 60 days for each leave type allocation
                var calculated = 60;
                var leaveAllocation = new Leave__Management_System.Data.LeaveAllocation
                {
                    EmployeeId = employeeId,
                    LeaveTypeId = leaveType.Id,
                    PeriodId = period.Id,
                    NumberOfDays = calculated,
                    RemainingDays = calculated
                };

                _context.Add(leaveAllocation);
            }

            await _context.SaveChangesAsync();
        }

        public async Task<List<Data.LeaveAllocation>> GetAllocations(string userId)
        {
            string employedId = string.Empty;
            if (!string.IsNullOrEmpty(userId))
            {
                employedId = userId;
            }
            else
            {
                var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext?.User);
                employedId = user?.Id;
            }

            if (string.IsNullOrEmpty(employedId))
                return new List<Data.LeaveAllocation>();

            var currentDate = DateTime.Now;
            var leaveAllocations = await _context.LeaveAllocations
                .Where(q => q.EmployeeId == employedId && q.Period.EndDate.Year == DateTime.Now.Year)
                .Include(q => q.LeaveType)
                .Include(q => q.Period)
                .Include(q => q.Employee)
                .AsNoTracking()
                .ToListAsync();
            return leaveAllocations;
        }
        public async Task<EmployeeAllocationVM> GetEmployeeAllocations(string? userId)
        {
            var user = string.IsNullOrEmpty(userId) ? await _userManager.GetUserAsync(_httpContextAccessor.HttpContext?.User) : await _userManager.FindByIdAsync(userId);
            if (user == null)
                return new EmployeeAllocationVM();

            // Ensure all leave types have allocations for this employee
            try
            {
                await EnsureAllLeaveTypeAllocationsForEmployee(user.Id);
            }
            catch (Exception ex)
            {
                // Log but don't fail if allocation ensuring fails
            }

            var allocations = await GetAllocations(userId);
            var allocationVMs = _mapper.Map<List<LeaveAllocationViewModel>>(allocations);

            var vm = new EmployeeAllocationVM
            {
                EmployeeId = user.Id,
                EmployeeName = $"{user.FirstName} {user.LastName}",
                FirstName = user.FirstName,
                LastName = user.LastName,
                EmailAddress = user.Email,
                DateOfBirth = user.DateOfBirth,
                Allocations = allocations.Select(a => new LeaveAllocationViewModel
                {
                    Id = a.Id,
                    LeaveTypeId = a.LeaveTypeId,
                    LeaveTypeName = a.LeaveType?.Name ?? string.Empty,
                    PeriodId = a.PeriodId,
                    PeriodStart = a.Period?.StartDate ?? default,
                    PeriodEnd = a.Period?.EndDate ?? default,
                    NumberOfDays = a.NumberOfDays,
                    RemainingDays = a.RemainingDays
                }).ToList()
            };

            // Ensure all leave types are present in the allocations view
            var currentPeriod = await _context.Periods
                .Where(p => p.EndDate.Year == DateTime.Now.Year)
                .OrderByDescending(p => p.EndDate)
                .FirstOrDefaultAsync();
            var leaveTypes = await _context.LeaveTypes.ToListAsync();

            var presentLeaveTypeIds = vm.Allocations.Select(x => x.LeaveTypeId).ToHashSet();

            foreach (var lt in leaveTypes)
            {
                if (!presentLeaveTypeIds.Contains(lt.Id))
                {
                    vm.Allocations.Add(new LeaveAllocationViewModel
                    {
                        Id = 0,
                        LeaveTypeId = lt.Id,
                        LeaveTypeName = lt.Name,
                        PeriodId = currentPeriod?.Id ?? 0,
                        PeriodStart = currentPeriod?.StartDate ?? default,
                        PeriodEnd = currentPeriod?.EndDate ?? default,
                        NumberOfDays = 0,
                        RemainingDays = 0
                    });
                }
            }

            // Optionally order allocations by LeaveTypeName
            vm.Allocations = vm.Allocations.OrderBy(a => a.LeaveTypeName).ToList();

            return vm;
        }
        public async Task<List<EmployeeListViewModel>> GetEmployees()
        {
            var users = await _userManager.GetUsersInRoleAsync(Roles.Employee);
            var employees = _mapper.Map<List<ApplicationUser>, List<EmployeeListViewModel>>(users.ToList());
            return employees;
        }

        /// <summary>
        /// Allocates a newly created leave type to all active employees for the current period
        /// Uses the same accrual calculation as AllocateLeave
        /// </summary>
        public async Task AllocateLeaveTypeToAllEmployees(int leaveTypeId)
        {
            var leaveType = await _context.LeaveTypes.FindAsync(leaveTypeId);
            if (leaveType == null)
                throw new ArgumentException($"LeaveType with ID {leaveTypeId} not found");

            // Get current period
            var currentDate = DateTime.Now;
            var period = await _context.Periods
                .Where(p => p.EndDate.Year == currentDate.Year)
                .OrderByDescending(p => p.EndDate)
                .FirstOrDefaultAsync();

            if (period == null)
                throw new InvalidOperationException($"No period found for year {currentDate.Year}");

            // Get all employees
            var employees = await _userManager.GetUsersInRoleAsync(Roles.Employee);

            // Fixed 60 days for each allocation
            var calculatedDays = 60;

            // Create or update allocations for each employee
            foreach (var employee in employees)
            {
                var existingAllocation = await _context.LeaveAllocations
                    .FirstOrDefaultAsync(a => a.EmployeeId == employee.Id
                        && a.LeaveTypeId == leaveTypeId
                        && a.PeriodId == period.Id);

                if (existingAllocation == null)
                {
                    var allocation = new Leave__Management_System.Data.LeaveAllocation
                    {
                        EmployeeId = employee.Id,
                        LeaveTypeId = leaveTypeId,
                        PeriodId = period.Id,
                        NumberOfDays = calculatedDays,
                        RemainingDays = calculatedDays
                    };
                    _context.LeaveAllocations.Add(allocation);
                }
                else
                {
                    // Update existing allocation to reflect new leave type settings
                    var delta = calculatedDays - existingAllocation.NumberOfDays;
                    existingAllocation.NumberOfDays = calculatedDays;
                    // adjust remaining by delta but not below zero
                    existingAllocation.RemainingDays = Math.Max(0, existingAllocation.RemainingDays + delta);
                    _context.LeaveAllocations.Update(existingAllocation);
                }
            }

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Allocates all active leave types to all employees for a newly created period
        /// Uses the same accrual calculation as AllocateLeave
        /// </summary>
        public async Task AllocatePeriodToAllEmployees(int periodId)
        {
            var period = await _context.Periods.FindAsync(periodId);
            if (period == null)
                throw new ArgumentException($"Period with ID {periodId} not found");

            // Get all leave types
            var leaveTypes = await _context.LeaveTypes.ToListAsync();
            if (!leaveTypes.Any())
                throw new InvalidOperationException("No leave types found to allocate");

            // Get all employees
            var employees = await _userManager.GetUsersInRoleAsync(Roles.Employee);

            // Fixed 60 days for each allocation
            var calculatedDays = 60;

            // Create allocations for each employee and leave type combination
            foreach (var employee in employees)
            {
                foreach (var leaveType in leaveTypes)
                {
                    // Check if allocation already exists
                    var existingAllocation = await _context.LeaveAllocations
                        .FirstOrDefaultAsync(a => a.EmployeeId == employee.Id 
                            && a.LeaveTypeId == leaveType.Id 
                            && a.PeriodId == periodId);

                    if (existingAllocation == null)
                    {
                        var allocation = new Leave__Management_System.Data.LeaveAllocation
                        {
                            EmployeeId = employee.Id,
                            LeaveTypeId = leaveType.Id,
                            PeriodId = periodId,
                            NumberOfDays = calculatedDays,
                            RemainingDays = calculatedDays
                        };
                        _context.LeaveAllocations.Add(allocation);
                    }
                    else
                    {
                        var delta = calculatedDays - existingAllocation.NumberOfDays;
                        existingAllocation.NumberOfDays = calculatedDays;
                        existingAllocation.RemainingDays = Math.Max(0, existingAllocation.RemainingDays + delta);
                        _context.LeaveAllocations.Update(existingAllocation);
                    }
                }
            }

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Ensure all leave types have allocations for a specific employee
        /// </summary>
        public async Task EnsureAllLeaveTypeAllocationsForEmployee(string employeeId)
        {
            if (string.IsNullOrEmpty(employeeId))
                return;

            // Get current period
            var currentDate = DateTime.Now;
            var period = await _context.Periods
                .Where(p => p.EndDate.Year == currentDate.Year)
                .OrderByDescending(p => p.EndDate)
                .FirstOrDefaultAsync();

            if (period == null)
                return;

            // Get all leave types
            var leaveTypes = await _context.LeaveTypes.ToListAsync();
            if (!leaveTypes.Any())
                return;

            // Fixed 60 days for each allocation
            var calculatedDays = 60;

            // Ensure allocation exists for each leave type
            foreach (var leaveType in leaveTypes)
            {
                var existingAllocation = await _context.LeaveAllocations
                    .FirstOrDefaultAsync(a => a.EmployeeId == employeeId
                        && a.LeaveTypeId == leaveType.Id
                        && a.PeriodId == period.Id);

                if (existingAllocation == null)
                {
                    var allocation = new Leave__Management_System.Data.LeaveAllocation
                    {
                        EmployeeId = employeeId,
                        LeaveTypeId = leaveType.Id,
                        PeriodId = period.Id,
                        NumberOfDays = calculatedDays,
                        RemainingDays = calculatedDays
                    };
                    _context.LeaveAllocations.Add(allocation);
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}

                    