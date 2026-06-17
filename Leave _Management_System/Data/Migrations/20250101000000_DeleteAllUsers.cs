using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Leave__Management_System.Data.Migrations
{
    /// <inheritdoc />
    public partial class DeleteAllUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Delete all user roles first (foreign key constraint)
            migrationBuilder.Sql("DELETE FROM [AspNetUserRoles];");

            // Delete all user claims
            migrationBuilder.Sql("DELETE FROM [AspNetUserClaims];");

            // Delete all user logins
            migrationBuilder.Sql("DELETE FROM [AspNetUserLogins];");

            // Delete all user tokens
            migrationBuilder.Sql("DELETE FROM [AspNetUserTokens];");

            // Finally, delete all users
            migrationBuilder.Sql("DELETE FROM [AspNetUsers];");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // If rolling back, recreate the seed user
            migrationBuilder.Sql(@"
                INSERT INTO [AspNetUsers] 
                ([Id], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], 
                 [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], 
                 [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount], [FirstName], 
                 [LastName], [DateOfBirth])
                VALUES 
                ('a1b2c3d4-5e6f-7g8h-9i0j-1k2l3m4n5o6p', 'employee', 'EMPLOYEE', 'employee@example.com', 
                 'EMPLOYEE@EXAMPLE.COM', 1, 'AQAAAAIAAYagAAAAEAiJwz8DpkLC0eLYPgqbEOl/bMmzLY/qkcIdVn8F13QAj87leX5qsHZn3GgU8oGeJg==', 
                 '417c46fd-2fea-4209-9597-95ac13b5303a', 'db1eaee3-82d5-4588-80e9-64540fc68818', 
                 NULL, 0, 0, NULL, 0, 0, 'Default', 'User', CAST(GETDATE() AS date));
            ");

            migrationBuilder.Sql(@"
                INSERT INTO [AspNetUserRoles] ([UserId], [RoleId])
                VALUES ('a1b2c3d4-5e6f-7g8h-9i0j-1k2l3m4n5o6p', '98b1bfb4-3b1c-4d31-9858-56324e082b6d');
            ");
        }
    }
}
