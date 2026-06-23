using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Leave__Management_System.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAdditionalInformationToLeaveRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LeaveRequests_AspNetUsers_ApprovedById",
                table: "LeaveRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_LeaveRequests_AspNetUsers_EmployeeId",
                table: "LeaveRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_LeaveRequests_LeaveTypes_LeaveTypeId",
                table: "LeaveRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_LeaveRequests_Periods_PeriodId",
                table: "LeaveRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_SupervisionAssignments_AspNetUsers_EmployeeId",
                table: "SupervisionAssignments");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "LeaveRequests",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "AdditionalInformation",
                table: "LeaveRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.InsertData(
                table: "LeaveRequests",
                columns: new[] { "Id", "AdditionalInformation", "ApprovedById", "DecisionDate", "EmployeeId", "EndDate", "LeaveTypeId", "NumberOfDays", "PeriodId", "RequestDate", "StartDate" },
                values: new object[] { 1, null, null, null, "a1b2c3d4-5e6f-7g8h-9i0j-1k2l3m4n5o6p", new DateOnly(2026, 7, 3), 1, 3, 1, new DateTime(2026, 6, 23, 20, 52, 29, 275, DateTimeKind.Utc).AddTicks(1487), new DateOnly(2026, 6, 30) });

            migrationBuilder.AddForeignKey(
                name: "FK_LeaveRequests_AspNetUsers_ApprovedById",
                table: "LeaveRequests",
                column: "ApprovedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LeaveRequests_AspNetUsers_EmployeeId",
                table: "LeaveRequests",
                column: "EmployeeId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LeaveRequests_LeaveTypes_LeaveTypeId",
                table: "LeaveRequests",
                column: "LeaveTypeId",
                principalTable: "LeaveTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LeaveRequests_Periods_PeriodId",
                table: "LeaveRequests",
                column: "PeriodId",
                principalTable: "Periods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SupervisionAssignment_Employee",
                table: "SupervisionAssignments",
                column: "EmployeeId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LeaveRequests_AspNetUsers_ApprovedById",
                table: "LeaveRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_LeaveRequests_AspNetUsers_EmployeeId",
                table: "LeaveRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_LeaveRequests_LeaveTypes_LeaveTypeId",
                table: "LeaveRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_LeaveRequests_Periods_PeriodId",
                table: "LeaveRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_SupervisionAssignment_Employee",
                table: "SupervisionAssignments");

            migrationBuilder.DeleteData(
                table: "LeaveRequests",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DropColumn(
                name: "AdditionalInformation",
                table: "LeaveRequests");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "LeaveRequests",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_LeaveRequests_AspNetUsers_ApprovedById",
                table: "LeaveRequests",
                column: "ApprovedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LeaveRequests_AspNetUsers_EmployeeId",
                table: "LeaveRequests",
                column: "EmployeeId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LeaveRequests_LeaveTypes_LeaveTypeId",
                table: "LeaveRequests",
                column: "LeaveTypeId",
                principalTable: "LeaveTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LeaveRequests_Periods_PeriodId",
                table: "LeaveRequests",
                column: "PeriodId",
                principalTable: "Periods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SupervisionAssignments_AspNetUsers_EmployeeId",
                table: "SupervisionAssignments",
                column: "EmployeeId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
