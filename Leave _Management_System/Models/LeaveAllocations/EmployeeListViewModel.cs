namespace Leave__Management_System.Models.LeaveAllocations
{
    public class EmployeeListViewModel
    {
        public string EmployeeId { get; set; } = string.Empty;

        public string Id => EmployeeId;

        public string EmployeeName { get; set; } = string.Empty;

        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [Display(Name = "Email Address")]
        public string EmailAddress { get; set; }
    }
}
