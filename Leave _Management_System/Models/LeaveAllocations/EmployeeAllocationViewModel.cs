using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Leave__Management_System.Models.LeaveAllocations
{
    public class EmployeeAllocationViewModel : EmployeeListViewModel
    {

        [Display(Name = "Date of Birth")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [DataType(DataType.Date)]
        public DateOnly DateOfBirth { get; set; }


        // Preferred property name used by services/controllers
        public List<LeaveAllocationViewModel> Allocations { get; set; } = new List<LeaveAllocationViewModel>();

        // Backwards-compatible alias for older views/code expecting LeaveAllocations
        public List<LeaveAllocationViewModel> LeaveAllocations => Allocations;
    }

    // Keep the original VM name used by services/controllers for compatibility
    public class EmployeeAllocationVM : EmployeeAllocationViewModel
    {
    }
}
