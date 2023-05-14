using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DwitTech.AccountService.Data.Migrations
{
    public partial class ChangedZipCodeToCountryCodeInUserEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ZipCode",
                table: "Users",
                newName: "CountryCode");

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedOnUtc",
                value: new DateTime(2023, 5, 14, 12, 4, 4, 921, DateTimeKind.Utc).AddTicks(5862));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedOnUtc",
                value: new DateTime(2023, 5, 14, 12, 4, 4, 921, DateTimeKind.Utc).AddTicks(5867));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CountryCode",
                table: "Users",
                newName: "ZipCode");

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedOnUtc",
                value: new DateTime(2023, 4, 14, 23, 18, 8, 496, DateTimeKind.Utc).AddTicks(4052));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedOnUtc",
                value: new DateTime(2023, 4, 14, 23, 18, 8, 496, DateTimeKind.Utc).AddTicks(4054));
        }
    }
}
