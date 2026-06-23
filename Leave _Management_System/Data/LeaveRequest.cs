using System;

namespace Leave__Management_System.Data
{
    public enum LeaveRequestStatus
    {
        Pending = 0,
        Approved = 1,
        Denied = 2
    }

    public class LeaveRequest : BaseEntity
    {
        public int Id { get; set; }

        public string EmployeeId { get; set; } = string.Empty;
        public ApplicationUser? Employee { get; set; }

        public int LeaveTypeId { get; set; }
        public LeaveType? LeaveType { get; set; }

        public int PeriodId { get; set; }
        public Period? Period { get; set; }

        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }

        public int NumberOfDays { get; set; }

        public DateTime RequestDate { get; set; } = DateTime.UtcNow;

        public LeaveRequestStatus Status { get; set; } = LeaveRequestStatus.Pending;

        public string? ApprovedById { get; set; }
        public ApplicationUser? ApprovedBy { get; set; }
        public DateTime? DecisionDate { get; set; }

        /// <summary>
        /// Additional information/remarks provided by the employee when requesting leave
        /// </summary>
        public string? AdditionalInformation { get; set; }
    }
}
