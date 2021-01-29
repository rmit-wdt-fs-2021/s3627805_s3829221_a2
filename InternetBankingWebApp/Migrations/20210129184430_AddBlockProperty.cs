using Microsoft.EntityFrameworkCore.Migrations;

namespace InternetBankingWebApp.Migrations
{
    public partial class AddBlockProperty : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsBlocked",
                table: "Logins",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsBlocked",
                table: "BillPays",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsBlocked",
                table: "Logins");

            migrationBuilder.DropColumn(
                name: "IsBlocked",
                table: "BillPays");
        }
    }
}
