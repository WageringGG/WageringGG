using Microsoft.EntityFrameworkCore.Migrations;

namespace WageringGG.Server.Migrations
{
    public partial class NoOwner : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsOwner",
                table: "WagerHostBids");

            migrationBuilder.DropColumn(
                name: "IsOwner",
                table: "WagerChallengeBids");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsOwner",
                table: "WagerHostBids",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsOwner",
                table: "WagerChallengeBids",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
