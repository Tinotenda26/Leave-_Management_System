using System.Collections.Generic;

namespace Leave__Management_System.Models.LeaveAllocations
{
    public class UserAllocationsViewModel
    {
        public string EmployeeId { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public List<LeaveAllocationViewModel> Allocations { get; set; } = new List<LeaveAllocationViewModel>();
    }
}
