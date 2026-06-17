using Microsoft.EntityFrameworkCore;

namespace Leave__Management_System.Services;

/// <summary>
/// Service to manage user data deletion operations
/// </summary>
public interface IUserDeletionService
{
    /// <summary>
    /// Delete all user records from the database
    /// </summary>
    Task DeleteAllUsersAsync();

    /// <summary>
    /// Delete all users except those with specific IDs (useful for seed data)
    /// </summary>
    Task DeleteAllUsersExceptAsync(params string[] userIdsToKeep);

    /// <summary>
    /// Delete users by email pattern
    /// </summary>
    Task DeleteUsersByEmailPatternAsync(string emailPattern);

    /// <summary>
    /// Get count of all users in the database
    /// </summary>
    Task<int> GetUserCountAsync();
}

public class UserDeletionService : IUserDeletionService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<UserDeletionService> _logger;

    public UserDeletionService(ApplicationDbContext context, ILogger<UserDeletionService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Delete all user records from the database
    /// </summary>
    public async Task DeleteAllUsersAsync()
    {
        try
        {
            // Get count before deletion
            var userCount = await _context.Users.CountAsync();

            if (userCount == 0)
            {
                _logger.LogInformation("No users found in the database to delete.");
                return;
            }

            _logger.LogInformation($"Starting deletion of {userCount} user records...");

            // Delete all user-related data in the correct order due to foreign key constraints
            await _context.Database.ExecuteSqlRawAsync("DELETE FROM [AspNetUserRoles];");
            _logger.LogDebug("Deleted user roles");

            await _context.Database.ExecuteSqlRawAsync("DELETE FROM [AspNetUserClaims];");
            _logger.LogDebug("Deleted user claims");

            await _context.Database.ExecuteSqlRawAsync("DELETE FROM [AspNetUserLogins];");
            _logger.LogDebug("Deleted user logins");

            await _context.Database.ExecuteSqlRawAsync("DELETE FROM [AspNetUserTokens];");
            _logger.LogDebug("Deleted user tokens");

            await _context.Database.ExecuteSqlRawAsync("DELETE FROM [AspNetUsers];");
            _logger.LogDebug("Deleted users");

            _logger.LogInformation($"Successfully deleted {userCount} user records from the database.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting all users.");
            throw;
        }
    }

    /// <summary>
    /// Delete all users except those with specific IDs (useful for seed data)
    /// </summary>
    public async Task DeleteAllUsersExceptAsync(params string[] userIdsToKeep)
    {
        try
        {
            if (userIdsToKeep == null || userIdsToKeep.Length == 0)
            {
                await DeleteAllUsersAsync();
                return;
            }

            var userCount = await _context.Users
                .Where(u => !userIdsToKeep.Contains(u.Id))
                .CountAsync();

            if (userCount == 0)
            {
                _logger.LogInformation("No users to delete (all protected users are in the exclude list).");
                return;
            }

            _logger.LogInformation($"Starting deletion of {userCount} user records (excluding {userIdsToKeep.Length} protected users)...");

            var userIdsToDeleteList = await _context.Users
                .Where(u => !userIdsToKeep.Contains(u.Id))
                .Select(u => u.Id)
                .ToListAsync();

            if (!userIdsToDeleteList.Any())
            {
                _logger.LogInformation("No users found to delete.");
                return;
            }

            // Delete user roles for users not in the keep list
            var userRolesToDelete = await _context.UserRoles
                .Where(ur => userIdsToDeleteList.Contains(ur.UserId))
                .ToListAsync();
            _context.UserRoles.RemoveRange(userRolesToDelete);

            // Delete user claims
            var userClaimsToDelete = await _context.UserClaims
                .Where(uc => userIdsToDeleteList.Contains(uc.UserId))
                .ToListAsync();
            _context.UserClaims.RemoveRange(userClaimsToDelete);

            // Delete user logins
            var userLoginsToDelete = await _context.UserLogins
                .Where(ul => userIdsToDeleteList.Contains(ul.UserId))
                .ToListAsync();
            _context.UserLogins.RemoveRange(userLoginsToDelete);

            // Delete user tokens
            var userTokensToDelete = await _context.UserTokens
                .Where(ut => userIdsToDeleteList.Contains(ut.UserId))
                .ToListAsync();
            _context.UserTokens.RemoveRange(userTokensToDelete);

            // Delete users
            var usersToDelete = await _context.Users
                .Where(u => userIdsToDeleteList.Contains(u.Id))
                .ToListAsync();
            _context.Users.RemoveRange(usersToDelete);

            await _context.SaveChangesAsync();

            _logger.LogInformation($"Successfully deleted {userCount} user records from the database.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting users with exclusion list.");
            throw;
        }
    }

    /// <summary>
    /// Delete users by email pattern
    /// </summary>
    public async Task DeleteUsersByEmailPatternAsync(string emailPattern)
    {
        try
        {
            var usersToDelete = await _context.Users
                .Where(u => u.Email != null && u.Email.Contains(emailPattern))
                .ToListAsync();

            if (!usersToDelete.Any())
            {
                _logger.LogInformation($"No users found matching email pattern: {emailPattern}");
                return;
            }

            _logger.LogInformation($"Found {usersToDelete.Count} users matching pattern '{emailPattern}'. Starting deletion...");

            var userIds = usersToDelete.Select(u => u.Id).ToList();

            // Delete user roles
            var userRolesToDelete = await _context.UserRoles
                .Where(ur => userIds.Contains(ur.UserId))
                .ToListAsync();
            _context.UserRoles.RemoveRange(userRolesToDelete);

            // Delete user claims
            var userClaimsToDelete = await _context.UserClaims
                .Where(uc => userIds.Contains(uc.UserId))
                .ToListAsync();
            _context.UserClaims.RemoveRange(userClaimsToDelete);

            // Delete user logins
            var userLoginsToDelete = await _context.UserLogins
                .Where(ul => userIds.Contains(ul.UserId))
                .ToListAsync();
            _context.UserLogins.RemoveRange(userLoginsToDelete);

            // Delete user tokens
            var userTokensToDelete = await _context.UserTokens
                .Where(ut => userIds.Contains(ut.UserId))
                .ToListAsync();
            _context.UserTokens.RemoveRange(userTokensToDelete);

            // Delete users
            _context.Users.RemoveRange(usersToDelete);

            await _context.SaveChangesAsync();

            _logger.LogInformation($"Successfully deleted {usersToDelete.Count} users matching pattern '{emailPattern}'.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while deleting users by email pattern: {emailPattern}");
            throw;
        }
    }

    /// <summary>
    /// Get count of all users in the database
    /// </summary>
    public async Task<int> GetUserCountAsync()
    {
        return await _context.Users.CountAsync();
    }
}
