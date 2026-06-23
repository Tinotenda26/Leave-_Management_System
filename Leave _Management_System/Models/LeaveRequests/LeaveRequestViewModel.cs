using System;

namespace Leave__Management_System.Models.LeaveRequests
{
    public class LeaveRequestViewModel
    {
        public int Id { get; set; }
        public string EmployeeId { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
        public int LeaveTypeId { get; set; }
        public string LeaveTypeName { get; set; } = string.Empty;
        public int PeriodId { get; set; }
        public string PeriodName { get; set; } = string.Empty;
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public int NumberOfDays { get; set; }
        public DateTime RequestDate { get; set; }
        public Leave__Management_System.Data.LeaveRequestStatus Status { get; set; }
        public string? ApprovedById { get; set; }
        public string ApprovedByName { get; set; } = string.Empty;
        public DateTime? DecisionDate { get; set; }

        /// <summary>
        /// Additional information/remarks provided by the employee
        /// </summary>
        public string? AdditionalInformation { get; set; }
    }
}
