using Leave__Management_System.Data;
using Leave__Management_System.Models.LeaveRequests;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Leave__Management_System.Services.LeaveRequests
{
    public interface ILeaveRequestsService
    {
        Task<LeaveRequest?> GetRequestByIdAsync(int id);
        Task<List<LeaveRequest>> GetPendingRequestsAsync();
        Task<LeaveRequest> CreateRequestAsync(LeaveRequest request);
        Task<bool> ApproveAsync(int id, string approverId);
        Task<bool> DenyAsync(int id, string approverId);

        /// <summary>
        /// Get all leave requests for a specific employee
        /// </summary>
        Task<List<LeaveRequest>> GetEmployeeRequestsAsync(string employeeId);

        /// <summary>
        /// Get leave requests for an employee within a date range
        /// </summary>
        Task<List<LeaveRequest>> GetEmployeeRequestsByDateRangeAsync(string employeeId, DateOnly startDate, DateOnly endDate);

        /// <summary>
        /// Get leave requests for an employee by status
        /// </summary>
        Task<List<LeaveRequest>> GetEmployeeRequestsByStatusAsync(string employeeId, LeaveRequestStatus status);

        /// <summary>
        /// Get pending requests for a specific leave type
        /// </summary>
        Task<List<LeaveRequest>> GetPendingRequestsByLeaveTypeAsync(int leaveTypeId);

        /// <summary>
        /// Get overlapping leave requests for an employee
        /// </summary>
        Task<List<LeaveRequest>> GetOverlappingRequestsAsync(string employeeId, DateOnly startDate, DateOnly endDate, int? excludeRequestId = null);

        /// <summary>
        /// Count total days requested by an employee in a period
        /// </summary>
        Task<int> GetTotalDaysRequestedAsync(string employeeId, int periodId);
    }
}
