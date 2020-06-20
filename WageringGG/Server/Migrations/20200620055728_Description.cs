using Microsoft.EntityFrameworkCore.Migrations;

namespace WageringGG.Server.Migrations
{
    public partial class Description : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WagerRules");

            migrationBuilder.DropTable(
                name: "Rules");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Wagers",
                maxLength: 250,
                nullable: false,
                defaultValue: "this is the default description.");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Tournaments",
                maxLength: 250,
                nullable: false,
                defaultValue: "this is the default description.");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Wagers");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Tournaments");

            migrationBuilder.CreateTable(
                name: "Rules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Message = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WagerRules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RuleId = table.Column<int>(type: "int", nullable: false),
                    WagerId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WagerRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WagerRules_Rules_RuleId",
                        column: x => x.RuleId,
                        principalTable: "Rules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WagerRules_Wagers_WagerId",
                        column: x => x.WagerId,
                        principalTable: "Wagers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WagerRules_RuleId",
                table: "WagerRules",
                column: "RuleId");

            migrationBuilder.CreateIndex(
                name: "IX_WagerRules_WagerId",
                table: "WagerRules",
                column: "WagerId");
        }
    }
}
