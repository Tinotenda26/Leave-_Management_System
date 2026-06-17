using Leave__Management_System.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Leave__Management_System.Controllers;

/// <summary>
/// Administrative controller for managing user data deletion operations
/// Warning: These endpoints should only be accessible to administrators
/// </summary>
[Authorize(Roles = "Administrator")]
[ApiController]
[Route("api/[controller]")]
public class UserManagementController : ControllerBase
{
    private readonly IUserDeletionService _userDeletionService;
    private readonly ILogger<UserManagementController> _logger;

    public UserManagementController(IUserDeletionService userDeletionService, ILogger<UserManagementController> logger)
    {
        _userDeletionService = userDeletionService;
        _logger = logger;
    }

    /// <summary>
    /// Get the current count of users in the database
    /// </summary>
    [HttpGet("user-count")]
    public async Task<IActionResult> GetUserCount()
    {
        try
        {
            var count = await _userDeletionService.GetUserCountAsync();
            return Ok(new { message = "User count retrieved successfully", userCount = count });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user count");
            return StatusCode(500, new { error = "Error retrieving user count", details = ex.Message });
        }
    }

    /// <summary>
    /// Delete all users from the database (DANGEROUS - use with caution!)
    /// </summary>
    [HttpDelete("delete-all-users")]
    public async Task<IActionResult> DeleteAllUsers([FromQuery] bool confirm = false)
    {
        if (!confirm)
        {
            return BadRequest(new 
            { 
                error = "Confirmation required", 
                message = "This action will delete ALL user records from the database. To confirm, add ?confirm=true parameter." 
            });
        }

        try
        {
            var userCountBefore = await _userDeletionService.GetUserCountAsync();
            _logger.LogWarning($"Admin {User.Identity?.Name} initiated deletion of {userCountBefore} users");

            await _userDeletionService.DeleteAllUsersAsync();

            var userCountAfter = await _userDeletionService.GetUserCountAsync();
            return Ok(new 
            { 
                message = "All users successfully deleted", 
                usersDeletedCount = userCountBefore,
                usersRemainingCount = userCountAfter
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting all users");
            return StatusCode(500, new { error = "Error deleting users", details = ex.Message });
        }
    }

    /// <summary>
    /// Delete users by email pattern
    /// Example: /api/usermanagement/delete-by-email-pattern?pattern=test@
    /// </summary>
    [HttpDelete("delete-by-email-pattern")]
    public async Task<IActionResult> DeleteByEmailPattern([FromQuery] string pattern)
    {
        if (string.IsNullOrWhiteSpace(pattern))
        {
            return BadRequest(new { error = "Pattern parameter is required" });
        }

        try
        {
            _logger.LogWarning($"Admin {User.Identity?.Name} initiated deletion of users matching pattern: {pattern}");
            await _userDeletionService.DeleteUsersByEmailPatternAsync(pattern);
            return Ok(new { message = $"Users matching pattern '{pattern}' have been deleted successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error deleting users by email pattern: {pattern}");
            return StatusCode(500, new { error = "Error deleting users", details = ex.Message });
        }
    }

    /// <summary>
    /// Delete all users except specified ones (useful for seed/system users)
    /// Example: POST with JSON body containing list of user IDs to keep
    /// </summary>
    [HttpPost("delete-all-except")]
    public async Task<IActionResult> DeleteAllExcept([FromBody] DeleteAllExceptRequest request)
    {
        if (request?.UserIdsToKeep == null || request.UserIdsToKeep.Count == 0)
        {
            return BadRequest(new { error = "UserIdsToKeep list is required and cannot be empty" });
        }

        try
        {
            _logger.LogWarning($"Admin {User.Identity?.Name} initiated deletion of all users except {request.UserIdsToKeep.Count} protected users");
            await _userDeletionService.DeleteAllUsersExceptAsync(request.UserIdsToKeep.ToArray());
            return Ok(new { message = "All users except protected ones have been deleted successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting users with exclusion list");
            return StatusCode(500, new { error = "Error deleting users", details = ex.Message });
        }
    }
}

/// <summary>
/// Request model for deleting all users except specific ones
/// </summary>
public class DeleteAllExceptRequest
{
    public List<string> UserIdsToKeep { get; set; } = new();
}
