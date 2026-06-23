using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Leave__Management_System.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRemainingDaysToLeaveAllocation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RemainingDays",
                table: "LeaveAllocations",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RemainingDays",
                table: "LeaveAllocations");
        }
    }
}
