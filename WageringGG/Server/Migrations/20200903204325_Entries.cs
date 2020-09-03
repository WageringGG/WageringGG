using Microsoft.EntityFrameworkCore.Migrations;

namespace WageringGG.Server.Migrations
{
    public partial class Entries : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Entries",
                table: "WagerMembers",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Entries",
                table: "WagerMembers");
        }
    }
}
