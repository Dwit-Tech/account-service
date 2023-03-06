using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DwitTech.AccountService.Data.Migrations
{
    public partial class UpdateMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Roles",
                table: "Roles");

            migrationBuilder.AddColumn<string>(
                name: "Roles_Description",
                table: "Roles",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Roles_Name",
                table: "Roles",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Roles_Description", "Roles_Name" },
                values: new object[] { "", "" });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Roles_Description", "Roles_Name" },
                values: new object[] { "", "" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Roles_Description",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "Roles_Name",
                table: "Roles");

            migrationBuilder.AddColumn<int>(
                name: "Roles",
                table: "Roles",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
