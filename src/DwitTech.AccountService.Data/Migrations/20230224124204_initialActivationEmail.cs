using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DwitTech.AccountService.Data.Migrations
{
    public partial class initialActivationEmail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "ValidationCode",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedOnUtc",
                table: "ValidationCode",
                type: "timestamp with time zone",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Id",
                table: "ValidationCode");

            migrationBuilder.DropColumn(
                name: "ModifiedOnUtc",
                table: "ValidationCode");
        }
    }
}
