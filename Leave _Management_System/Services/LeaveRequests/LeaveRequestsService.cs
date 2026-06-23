using Leave__Management_System.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Leave__Management_System.Services.LeaveRequests
{
    public class LeaveRequestsService : ILeaveRequestsService
    {
        private readonly ApplicationDbContext _context;

        public LeaveRequestsService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<LeaveRequest?> GetRequestByIdAsync(int id)
        {
            return await _context.LeaveRequests
                .Include(r => r.Employee)
                .Include(r => r.LeaveType)
                .Include(r => r.Period)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<List<LeaveRequest>> GetPendingRequestsAsync()
        {
            return await _context.LeaveRequests
                .Where(r => r.Status == LeaveRequestStatus.Pending)
                .Include(r => r.Employee)
                .Include(r => r.LeaveType)
                .Include(r => r.Period)
                .ToListAsync();
        }

        public async Task<LeaveRequest> CreateRequestAsync(LeaveRequest request)
        {
            _context.LeaveRequests.Add(request);
            await _context.SaveChangesAsync();
            return request;
        }

        public async Task<bool> ApproveAsync(int id, string approverId)
        {
            var request = await _context.LeaveRequests.FindAsync(id);
            if (request == null || request.Status != LeaveRequestStatus.Pending) return false;

            var allocation = await _context.LeaveAllocations
                .FirstOrDefaultAsync(a => a.EmployeeId == request.EmployeeId && a.LeaveTypeId == request.LeaveTypeId && a.PeriodId == request.PeriodId);

            if (allocation == null) return false;

            allocation.RemainingDays = Math.Max(0, allocation.RemainingDays - request.NumberOfDays);
            request.Status = LeaveRequestStatus.Approved;
            request.DecisionDate = DateTime.UtcNow;
            request.ApprovedById = approverId;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DenyAsync(int id, string approverId)
        {
            var request = await _context.LeaveRequests.FindAsync(id);
            if (request == null || request.Status != LeaveRequestStatus.Pending) return false;

            request.Status = LeaveRequestStatus.Denied;
            request.DecisionDate = DateTime.UtcNow;
            request.ApprovedById = approverId;

            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Get all leave requests for a specific employee
        /// </summary>
        public async Task<List<LeaveRequest>> GetEmployeeRequestsAsync(string employeeId)
        {
            if (string.IsNullOrEmpty(employeeId))
                return new List<LeaveRequest>();

            return await _context.LeaveRequests
                .Where(r => r.EmployeeId == employeeId)
                .Include(r => r.LeaveType)
                .Include(r => r.Period)
                .OrderByDescending(r => r.RequestDate)
                .ToListAsync();
        }

        /// <summary>
        /// Get leave requests for an employee within a date range
        /// </summary>
        public async Task<List<LeaveRequest>> GetEmployeeRequestsByDateRangeAsync(string employeeId, DateOnly startDate, DateOnly endDate)
        {
            if (string.IsNullOrEmpty(employeeId))
                return new List<LeaveRequest>();

            return await _context.LeaveRequests
                .Where(r => r.EmployeeId == employeeId && r.StartDate >= startDate && r.EndDate <= endDate)
                .Include(r => r.LeaveType)
                .Include(r => r.Period)
                .OrderByDescending(r => r.RequestDate)
                .ToListAsync();
        }

        /// <summary>
        /// Get leave requests for an employee by status
        /// </summary>
        public async Task<List<LeaveRequest>> GetEmployeeRequestsByStatusAsync(string employeeId, LeaveRequestStatus status)
        {
            if (string.IsNullOrEmpty(employeeId))
                return new List<LeaveRequest>();

            return await _context.LeaveRequests
                .Where(r => r.EmployeeId == employeeId && r.Status == status)
                .Include(r => r.LeaveType)
                .Include(r => r.Period)
                .OrderByDescending(r => r.RequestDate)
                .ToListAsync();
        }

        /// <summary>
        /// Get pending requests for a specific leave type
        /// </summary>
        public async Task<List<LeaveRequest>> GetPendingRequestsByLeaveTypeAsync(int leaveTypeId)
        {
            return await _context.LeaveRequests
                .Where(r => r.LeaveTypeId == leaveTypeId && r.Status == LeaveRequestStatus.Pending)
                .Include(r => r.Employee)
                .Include(r => r.Period)
                .OrderByDescending(r => r.RequestDate)
                .ToListAsync();
        }

        /// <summary>
        /// Get overlapping leave requests for an employee
        /// </summary>
        public async Task<List<LeaveRequest>> GetOverlappingRequestsAsync(string employeeId, DateOnly startDate, DateOnly endDate, int? excludeRequestId = null)
        {
            if (string.IsNullOrEmpty(employeeId))
                return new List<LeaveRequest>();

            var query = _context.LeaveRequests.AsQueryable();

            // Filter by employee and date overlap
            query = query.Where(r => r.EmployeeId == employeeId &&
                !(r.EndDate < startDate || r.StartDate > endDate) &&
                (r.Status == LeaveRequestStatus.Approved || r.Status == LeaveRequestStatus.Pending));

            // Exclude specific request if provided
            if (excludeRequestId.HasValue)
                query = query.Where(r => r.Id != excludeRequestId.Value);

            return await query
                .Include(r => r.LeaveType)
                .Include(r => r.Period)
                .OrderByDescending(r => r.StartDate)
                .ToListAsync();
        }

        /// <summary>
        /// Count total days requested by an employee in a period
        /// </summary>
        public async Task<int> GetTotalDaysRequestedAsync(string employeeId, int periodId)
        {
            if (string.IsNullOrEmpty(employeeId))
                return 0;

            return await _context.LeaveRequests
                .Where(r => r.EmployeeId == employeeId && 
                    r.PeriodId == periodId && 
                    (r.Status == LeaveRequestStatus.Approved || r.Status == LeaveRequestStatus.Pending))
                .SumAsync(r => r.NumberOfDays);
        }
    }
}
