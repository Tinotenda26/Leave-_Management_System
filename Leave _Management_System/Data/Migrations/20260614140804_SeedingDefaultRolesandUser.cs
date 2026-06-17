using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Leave__Management_System.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedingDefaultRolesandUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "98b1bfb4-3b1c-4d31-9858-56324e082b6d", "a21ba486-bddf-4903-958c-513b756bdf5b", "Employee", "EMPLOYEE" },
                    { "b3c4b1d5-2a9c-4e01-8f67-4d321e987654", "1f5b2ec5-10d8-4978-833f-01dbd9e5a3eb", "Supervisor", "SUPERVISOR" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "a1b2c3d4-5e6f-7g8h-9i0j-1k2l3m4n5o6p", 0, "db1eaee3-82d5-4588-80e9-64540fc68818", "employee@example.com", true, false, null, "EMPLOYEE@EXAMPLE.COM", "EMPLOYEE", "AQAAAAIAAYagAAAAEAiJwz8DpkLC0eLYPgqbEOl/bMmzLY/qkcIdVn8F13QAj87leX5qsHZn3GgU8oGeJg==", null, false, "417c46fd-2fea-4209-9597-95ac13b5303a", false, "employee" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "98b1bfb4-3b1c-4d31-9858-56324e082b6d", "a1b2c3d4-5e6f-7g8h-9i0j-1k2l3m4n5o6p" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b3c4b1d5-2a9c-4e01-8f67-4d321e987654");

            // No Administrator role seeded in this migration - only Employee and Supervisor


            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "98b1bfb4-3b1c-4d31-9858-56324e082b6d", "a1b2c3d4-5e6f-7g8h-9i0j-1k2l3m4n5o6p" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "98b1bfb4-3b1c-4d31-9858-56324e082b6d");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "a1b2c3d4-5e6f-7g8h-9i0j-1k2l3m4n5o6p");
        }
    }
}
