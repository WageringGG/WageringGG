using Microsoft.EntityFrameworkCore.Migrations;

namespace WageringGG.Server.Migrations
{
    public partial class StellarAccount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AccountId",
                table: "WagerChallenges",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "StellarAccount",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Asset = table.Column<string>(nullable: true),
                    AccountId = table.Column<string>(nullable: true),
                    SecretSeed = table.Column<string>(nullable: true),
                    Balance = table.Column<decimal>(type: "decimal(18,7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StellarAccount", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WagerChallenges_AccountId",
                table: "WagerChallenges",
                column: "AccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_WagerChallenges_StellarAccount_AccountId",
                table: "WagerChallenges",
                column: "AccountId",
                principalTable: "StellarAccount",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WagerChallenges_StellarAccount_AccountId",
                table: "WagerChallenges");

            migrationBuilder.DropTable(
                name: "StellarAccount");

            migrationBuilder.DropIndex(
                name: "IX_WagerChallenges_AccountId",
                table: "WagerChallenges");

            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "WagerChallenges");
        }
    }
}
