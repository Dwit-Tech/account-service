using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DwitTech.AccountService.Data.Migrations
{
    public partial class AddedValidationCodeEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ValidationCodes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: true),
                    CodeType = table.Column<int>(type: "integer", nullable: false),
                    NotificationChannel = table.Column<int>(type: "integer", nullable: false),
                    CreatedOnUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedOnUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ValidationCodes", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedOnUtc",
                value: new DateTime(2023, 4, 12, 18, 29, 10, 756, DateTimeKind.Utc).AddTicks(4355));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedOnUtc",
                value: new DateTime(2023, 4, 12, 18, 29, 10, 756, DateTimeKind.Utc).AddTicks(4359));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ValidationCodes");

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedOnUtc",
                value: new DateTime(2023, 4, 12, 18, 27, 5, 370, DateTimeKind.Utc).AddTicks(5072));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedOnUtc",
                value: new DateTime(2023, 4, 12, 18, 27, 5, 370, DateTimeKind.Utc).AddTicks(5075));
        }
    }
}
