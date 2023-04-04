using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DwitTech.AccountService.Data.Migrations
{
    public partial class AddedValidationCode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Roles_RolesId",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ValidationCode",
                table: "ValidationCode");

            migrationBuilder.RenameTable(
                name: "ValidationCode",
                newName: "ValidationCodes");

            migrationBuilder.RenameColumn(
                name: "RolesId",
                table: "Users",
                newName: "RoleId");

            migrationBuilder.RenameIndex(
                name: "IX_Users_RolesId",
                table: "Users",
                newName: "IX_Users_RoleId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ValidationCodes",
                table: "ValidationCodes",
                column: "Id");

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedOnUtc",
                value: new DateTime(2023, 3, 28, 17, 37, 24, 857, DateTimeKind.Utc).AddTicks(20));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedOnUtc",
                value: new DateTime(2023, 3, 28, 17, 37, 24, 857, DateTimeKind.Utc).AddTicks(24));

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Roles_RoleId",
                table: "Users",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Roles_RoleId",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ValidationCodes",
                table: "ValidationCodes");

            migrationBuilder.RenameTable(
                name: "ValidationCodes",
                newName: "ValidationCode");

            migrationBuilder.RenameColumn(
                name: "RoleId",
                table: "Users",
                newName: "RolesId");

            migrationBuilder.RenameIndex(
                name: "IX_Users_RoleId",
                table: "Users",
                newName: "IX_Users_RolesId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ValidationCode",
                table: "ValidationCode",
                column: "Id");

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedOnUtc",
                value: new DateTime(2023, 3, 25, 7, 4, 26, 14, DateTimeKind.Utc).AddTicks(4804));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedOnUtc",
                value: new DateTime(2023, 3, 25, 7, 4, 26, 14, DateTimeKind.Utc).AddTicks(4809));

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Roles_RolesId",
                table: "Users",
                column: "RolesId",
                principalTable: "Roles",
                principalColumn: "Id");
        }
    }
}
