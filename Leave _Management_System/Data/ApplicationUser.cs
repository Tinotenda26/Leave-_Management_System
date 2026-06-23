using Microsoft.AspNetCore.Identity;
namespace Leave__Management_System.Data;
// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateOnly DateOfBirth { get; set; }

    /// <summary>
    /// Navigation property: Supervision assignments where this user is the supervisor
    /// </summary>
    public ICollection<SupervisionAssignment> SupervisedEmployees { get; set; } = new List<SupervisionAssignment>();

    /// <summary>
    /// Navigation property: Supervision assignments where this user is the employee
    /// </summary>
    public ICollection<SupervisionAssignment> Supervisors { get; set; } = new List<SupervisionAssignment>();
}
