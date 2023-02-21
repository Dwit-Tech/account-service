using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DwitTech.AccountService.Data.Migrations
{
    public partial class InitialValidationCode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ValidationCode",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false),
                    ActivationType = table.Column<int>(type: "integer", nullable: false),
                    ActivationChannel = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ValidationCode", x => x.UserId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ValidationCode");
        }
    }
}
