using Microsoft.AspNetCore.Mvc.Rendering;

namespace Leave__Management_System.Models.LeaveAllocations
{
    public class EmployeeCreateViewModel
    {
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Password { get; set; } = "Password123!"; // default strong password suggestion
        public DateTime? DateOfBirth { get; set; }

        // Supervisor assignment
        public string? SupervisorId { get; set; }
        public List<SelectListItem> Supervisors { get; set; } = new List<SelectListItem>();
    }
}
