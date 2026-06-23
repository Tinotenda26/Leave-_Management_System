using System.ComponentModel.DataAnnotations;

namespace Leave__Management_System.Models.LeaveAllocations
{
    public class AllocationEditViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Employee ID")]
        public string EmployeeId { get; set; } = string.Empty;

        [Display(Name = "Employee Name")]
        public string EmployeeName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Leave Type")]
        public int LeaveTypeId { get; set; }

        [Display(Name = "Leave Type Name")]
        public string LeaveTypeName { get; set; } = string.Empty;

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Original days must be 0 or greater")]
        [Display(Name = "Original Number of Days")]
        public int NumberOfDays { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Remaining days must be 0 or greater")]
        [Display(Name = "Remaining Days")]
        public int RemainingDays { get; set; }

        [Required]
        [Display(Name = "Period")]
        public int PeriodId { get; set; }

        [Display(Name = "Period Name")]
        public string PeriodName { get; set; } = string.Empty;

        [Display(Name = "Period Start")]
        public DateOnly PeriodStart { get; set; }

        [Display(Name = "Period End")]
        public DateOnly PeriodEnd { get; set; }
    }
}
