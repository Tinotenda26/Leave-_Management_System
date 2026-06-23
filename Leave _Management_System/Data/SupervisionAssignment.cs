namespace Leave__Management_System.Data
{
    /// <summary>
    /// Represents the assignment of an employee to a supervisor for leave management oversight.
    /// This allows supervisors to manage and approve leave requests for their assigned employees.
    /// </summary>
    public class SupervisionAssignment : BaseEntity
    {
        public int Id { get; set; }

        /// <summary>
        /// The supervisor who oversees the employee
        /// </summary>
        public string SupervisorId { get; set; } = string.Empty;
        public ApplicationUser? Supervisor { get; set; }

        /// <summary>
        /// The employee assigned to this supervisor
        /// </summary>
        public string EmployeeId { get; set; } = string.Empty;
        public ApplicationUser? Employee { get; set; }

        /// <summary>
        /// When this assignment was created
        /// </summary>
        public DateTime AssignmentDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Whether this assignment is currently active
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// When the assignment was deactivated (if applicable)
        /// </summary>
        public DateTime? DeactivatedDate { get; set; }
    }
}
