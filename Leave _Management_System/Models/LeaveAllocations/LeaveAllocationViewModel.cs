using Leave__Management_System.Models.LeaveTypes;
using Leave__Management_System.Models.Periods;
using System;
using System.ComponentModel.DataAnnotations;

namespace Leave__Management_System.Models.LeaveAllocations
{
    public class LeaveAllocationViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Number of Days")]
        public int NumberOfDays { get; set; }

        [Display(Name = "Remaining Days")]
        public int RemainingDays { get; set; }

        [Display(Name = "Allocation Period")]
        public PeriodViewModel Period { get; set; } = new PeriodViewModel();

        // Keep both complex and simple fields for view compatibility
        public LeaveTypeReadOnlyVM LeaveType { get; set; } = new LeaveTypeReadOnlyVM();
        public int LeaveTypeId { get; set; }
        public string LeaveTypeName { get; set; } = string.Empty;

        public int PeriodId { get; set; }
        public DateOnly PeriodStart { get; set; }
        public DateOnly PeriodEnd { get; set; }
    }
}
