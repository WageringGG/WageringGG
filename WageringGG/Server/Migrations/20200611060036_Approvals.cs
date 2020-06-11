using Microsoft.EntityFrameworkCore.Migrations;

namespace WageringGG.Server.Migrations
{
    public partial class Approvals : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_Profiles_DisplayName",
                table: "Profiles");

            migrationBuilder.AlterColumn<string>(
                name: "DisplayName",
                table: "Profiles",
                maxLength: 12,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(12)",
                oldMaxLength: 12);

            migrationBuilder.CreateTable(
                name: "WagerApprovals",
                columns: table => new
                {
                    HostId = table.Column<int>(nullable: false),
                    ChallengeId = table.Column<int>(nullable: false),
                    Approved = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WagerApprovals", x => new { x.HostId, x.ChallengeId });
                    table.ForeignKey(
                        name: "FK_WagerApprovals_WagerChallenges_ChallengeId",
                        column: x => x.ChallengeId,
                        principalTable: "WagerChallenges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WagerApprovals_WagerHostBids_HostId",
                        column: x => x.HostId,
                        principalTable: "WagerHostBids",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_WagerApprovals_ChallengeId",
                table: "WagerApprovals",
                column: "ChallengeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WagerApprovals");

            migrationBuilder.AlterColumn<string>(
                name: "DisplayName",
                table: "Profiles",
                type: "nvarchar(12)",
                maxLength: 12,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 12,
                oldNullable: true);

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Profiles_DisplayName",
                table: "Profiles",
                column: "DisplayName");
        }
    }
}
