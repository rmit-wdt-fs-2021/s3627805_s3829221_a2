using Microsoft.EntityFrameworkCore.Migrations;

namespace InternetBankingWebApp.Migrations
{
    public partial class AddIsFailedProperty : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsFailed",
                table: "BillPays",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsFailed",
                table: "BillPays");
        }
    }
}
