using Leave__Management_System.Models.LeaveTypes;
using System.ComponentModel.DataAnnotations;
namespace Leave_Management_System.Models.LeaveTypes;

public class LeaveTypeEditVM : BaseLeaveTypeVM
{
    [Required]
    [Length(4, 150, ErrorMessage = "Name must be between 4 and 150 characters")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Range(1, 90, ErrorMessage = "Number of days must be between 1 and 90")]
    [Display(Name = "Maximum Allocation of Days")]
    public int NumberOfDays { get; set; }
}