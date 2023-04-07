using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DwitTech.AccountService.Data.Migrations
{
    public partial class UpdatedUsersLoginToUserLogins : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UsersLogin",
                table: "UsersLogin");

            migrationBuilder.DropColumn(
                name: "PassWord",
                table: "Users");

            migrationBuilder.RenameTable(
                name: "UsersLogin",
                newName: "UserLogins");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserLogins",
                table: "UserLogins",
                column: "Id");

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedOnUtc",
                value: new DateTime(2023, 4, 6, 14, 18, 43, 295, DateTimeKind.Utc).AddTicks(3469));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedOnUtc",
                value: new DateTime(2023, 4, 6, 14, 18, 43, 295, DateTimeKind.Utc).AddTicks(3473));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserLogins",
                table: "UserLogins");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Users");

            migrationBuilder.RenameTable(
                name: "UserLogins",
                newName: "UsersLogin");

            migrationBuilder.AddColumn<string>(
                name: "PassWord",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UsersLogin",
                table: "UsersLogin",
                column: "Id");

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedOnUtc",
                value: new DateTime(2023, 4, 1, 16, 46, 48, 230, DateTimeKind.Utc).AddTicks(9160));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedOnUtc",
                value: new DateTime(2023, 4, 1, 16, 46, 48, 230, DateTimeKind.Utc).AddTicks(9167));
        }
    }
}
