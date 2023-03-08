using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DwitTech.AccountService.Data.Migrations
{
    public partial class UpdatesMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "activationDetails");

            migrationBuilder.DropColumn(
                name: "ActivationDetail",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ActivationDetailCreated",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "Lastname",
                table: "Users",
                newName: "LastName");

            migrationBuilder.RenameColumn(
                name: "Firstname",
                table: "Users",
                newName: "FirstName");

            migrationBuilder.RenameColumn(
                name: "ActivationDetailExpires",
                table: "Users",
                newName: "CreatedOnUtc");

            migrationBuilder.CreateTable(
                name: "ValidationCodes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false),
                    Channel = table.Column<int>(type: "integer", nullable: false),
                    CodeType = table.Column<int>(type: "integer", nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ValidationCodes", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ValidationCodes");

            migrationBuilder.RenameColumn(
                name: "LastName",
                table: "Users",
                newName: "Lastname");

            migrationBuilder.RenameColumn(
                name: "FirstName",
                table: "Users",
                newName: "Firstname");

            migrationBuilder.RenameColumn(
                name: "CreatedOnUtc",
                table: "Users",
                newName: "ActivationDetailExpires");

            migrationBuilder.AddColumn<string>(
                name: "ActivationDetail",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "ActivationDetailCreated",
                table: "Users",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "activationDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ActivationCode = table.Column<string>(type: "text", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Expires = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedOnUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_activationDetails", x => x.Id);
                });
        }
    }
}
