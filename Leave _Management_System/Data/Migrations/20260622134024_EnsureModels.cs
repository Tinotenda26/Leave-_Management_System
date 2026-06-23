using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Leave__Management_System.Data.Migrations
{
    /// <inheritdoc />
    public partial class EnsureModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LeaveAllocations_AspNetUsers_EmployeeId1",
                table: "LeaveAllocations");

            migrationBuilder.DropIndex(
                name: "IX_LeaveAllocations_EmployeeId1",
                table: "LeaveAllocations");

            migrationBuilder.DropColumn(
                name: "EmployeeId1",
                table: "LeaveAllocations");

            migrationBuilder.AlterColumn<string>(
                name: "EmployeeId",
                table: "LeaveAllocations",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "a1b2c3d4-5e6f-7g8h-9i0j-1k2l3m4n5o6p",
                column: "DateOfBirth",
                value: new DateOnly(2026, 6, 22));

            migrationBuilder.CreateIndex(
                name: "IX_LeaveAllocations_EmployeeId",
                table: "LeaveAllocations",
                column: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_LeaveAllocations_AspNetUsers_EmployeeId",
                table: "LeaveAllocations",
                column: "EmployeeId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LeaveAllocations_AspNetUsers_EmployeeId",
                table: "LeaveAllocations");

            migrationBuilder.DropIndex(
                name: "IX_LeaveAllocations_EmployeeId",
                table: "LeaveAllocations");

            migrationBuilder.AlterColumn<int>(
                name: "EmployeeId",
                table: "LeaveAllocations",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "EmployeeId1",
                table: "LeaveAllocations",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "a1b2c3d4-5e6f-7g8h-9i0j-1k2l3m4n5o6p",
                column: "DateOfBirth",
                value: new DateOnly(2026, 6, 21));

            migrationBuilder.CreateIndex(
                name: "IX_LeaveAllocations_EmployeeId1",
                table: "LeaveAllocations",
                column: "EmployeeId1");

            migrationBuilder.AddForeignKey(
                name: "FK_LeaveAllocations_AspNetUsers_EmployeeId1",
                table: "LeaveAllocations",
                column: "EmployeeId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
