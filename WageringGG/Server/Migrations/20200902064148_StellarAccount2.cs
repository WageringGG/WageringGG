using Microsoft.EntityFrameworkCore.Migrations;

namespace WageringGG.Server.Migrations
{
    public partial class StellarAccount2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AccountId",
                table: "Wagers",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "StellarAccount",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Balance = table.Column<decimal>(type: "decimal(18,7)", nullable: false),
                    SecretSeed = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StellarAccount", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Wagers_AccountId",
                table: "Wagers",
                column: "AccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_Wagers_StellarAccount_AccountId",
                table: "Wagers",
                column: "AccountId",
                principalTable: "StellarAccount",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Wagers_StellarAccount_AccountId",
                table: "Wagers");

            migrationBuilder.DropTable(
                name: "StellarAccount");

            migrationBuilder.DropIndex(
                name: "IX_Wagers_AccountId",
                table: "Wagers");

            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "Wagers");
        }
    }
}
