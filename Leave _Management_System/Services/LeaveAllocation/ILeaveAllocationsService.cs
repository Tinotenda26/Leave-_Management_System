using System.Collections.Generic;
using System.Threading.Tasks;
using Leave__Management_System.Models.LeaveAllocations;

namespace Leave__Management_System.Services.LeaveAllocation
{
    public interface ILeaveAllocationsService
    {
        /// <summary>
        /// Allocate leave for a new employee across all leave types in the current period
        /// </summary>
        Task AllocateLeave(string employeeId);

        /// <summary>
        /// Get all allocations and employee details for a specific employee
        /// </summary>
        Task<EmployeeAllocationVM> GetEmployeeAllocations(string? userId);

        /// <summary>
        /// Get list of all employees
        /// </summary>
        Task<List<EmployeeListViewModel>> GetEmployees();

        /// <summary>
        /// Allocate the newly created leave type to all active employees for current period
        /// </summary>
        Task AllocateLeaveTypeToAllEmployees(int leaveTypeId);

        /// <summary>
        /// Allocate all active leave types to all employees for the newly created period
        /// </summary>
        Task AllocatePeriodToAllEmployees(int periodId);

        /// <summary>
        /// Ensure all missing leave type allocations are created for an employee
        /// Useful when a new employee is added or a new leave type is created
        /// </summary>
        Task EnsureAllLeaveTypeAllocationsForEmployee(string employeeId);
    }
}

