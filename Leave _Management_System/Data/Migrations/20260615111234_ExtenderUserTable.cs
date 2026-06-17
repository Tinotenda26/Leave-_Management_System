using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Leave__Management_System.Data.Migrations
{
    /// <inheritdoc />
    public partial class ExtenderUserTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "DateOfBirth",
                table: "AspNetUsers",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "a1b2c3d4-5e6f-7g8h-9i0j-1k2l3m4n5o6p",
                columns: new[] { "DateOfBirth", "FirstName", "LastName","PasswordHash","SecurityStamp" },
                values: new object[] { new DateOnly(2026, 6, 15), "Default", "User","AQAAAAIAAYagAAAAEAiJwz8DpkLC0eLYPgqbEOl/bMmzLY/qkcIdVn8F13QAj87leX5qsHZn3GgU8oGeJg==", "417c46fd-2fea-4209-9597-95ac13b5303a" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateOfBirth",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "AspNetUsers");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "a1b2c3d4-5e6f-7g8h-9i0j-1k2l3m4n5o6p",
                columns: new[] { "PasswordHash", "SecurityStamp" },
                values: new object[] { "AQAAAAIAAYagAAAAEAiJwz8DpkLC0eLYPgqbEOl/bMmzLY/qkcIdVn8F13QAj87leX5qsHZn3GgU8oGeJg==", "417c46fd-2fea-4209-9597-95ac13b5303a" });
        }
    }
}
