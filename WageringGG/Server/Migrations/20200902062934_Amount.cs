using Microsoft.EntityFrameworkCore.Migrations;

namespace WageringGG.Server.Migrations
{
    public partial class Amount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaximumWager",
                table: "Wagers");

            migrationBuilder.DropColumn(
                name: "MinimumWager",
                table: "Wagers");

            migrationBuilder.DropColumn(
                name: "Amount",
                table: "WagerChallenges");

            migrationBuilder.AddColumn<decimal>(
                name: "Amount",
                table: "Wagers",
                type: "decimal(18,7)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_WagerMembers_IsHost",
                table: "WagerMembers",
                column: "IsHost");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WagerMembers_IsHost",
                table: "WagerMembers");

            migrationBuilder.DropColumn(
                name: "Amount",
                table: "Wagers");

            migrationBuilder.AddColumn<decimal>(
                name: "MaximumWager",
                table: "Wagers",
                type: "decimal(18,7)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MinimumWager",
                table: "Wagers",
                type: "decimal(18,7)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Amount",
                table: "WagerChallenges",
                type: "decimal(18,7)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
