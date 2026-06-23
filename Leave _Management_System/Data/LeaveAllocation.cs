namespace Leave__Management_System.Data
{
    public class LeaveAllocation : BaseEntity
    {
        public int Id { get; set; }

        // Navigation properties should be nullable and not pre-initialized to avoid accidental inserts
        public LeaveType? LeaveType { get; set; }
        public int LeaveTypeId { get; set; }

        public ApplicationUser? Employee { get; set; }
        public string EmployeeId { get; set; } = string.Empty;

        public Period? Period { get; set; }
        public int PeriodId { get; set; }

        public int NumberOfDays { get; set; }

        // RemainingDays tracks how many days the employee still has available
        public int RemainingDays { get; set; }
    }
}
