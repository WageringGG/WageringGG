using Microsoft.EntityFrameworkCore.Migrations;

namespace WageringGG.Server.Migrations
{
    public partial class Members : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WagerChallenges_StellarAccount_AccountId",
                table: "WagerChallenges");

            migrationBuilder.DropTable(
                name: "StellarAccount");

            migrationBuilder.DropTable(
                name: "WagerApprovals");

            migrationBuilder.DropTable(
                name: "WagerChallengeBids");

            migrationBuilder.DropTable(
                name: "WagerHostBids");

            migrationBuilder.DropIndex(
                name: "IX_WagerChallenges_AccountId",
                table: "WagerChallenges");

            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "WagerChallenges");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Wagers",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "WagerChallenges",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Tournaments",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint");

            migrationBuilder.CreateTable(
                name: "WagerMembers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WagerId = table.Column<int>(nullable: false),
                    ProfileId = table.Column<string>(nullable: true),
                    ChallengeId = table.Column<int>(nullable: true),
                    IsHost = table.Column<bool>(nullable: false),
                    IsApproved = table.Column<bool>(nullable: true),
                    ReceivablePercentage = table.Column<byte>(nullable: false),
                    PayablePercentage = table.Column<byte>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WagerMembers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WagerMembers_WagerChallenges_ChallengeId",
                        column: x => x.ChallengeId,
                        principalTable: "WagerChallenges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WagerMembers_Profiles_ProfileId",
                        column: x => x.ProfileId,
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WagerMembers_Wagers_WagerId",
                        column: x => x.WagerId,
                        principalTable: "Wagers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WagerMembers_ChallengeId",
                table: "WagerMembers",
                column: "ChallengeId");

            migrationBuilder.CreateIndex(
                name: "IX_WagerMembers_ProfileId",
                table: "WagerMembers",
                column: "ProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_WagerMembers_WagerId",
                table: "WagerMembers",
                column: "WagerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WagerMembers");

            migrationBuilder.AlterColumn<byte>(
                name: "Status",
                table: "Wagers",
                type: "tinyint",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<byte>(
                name: "Status",
                table: "WagerChallenges",
                type: "tinyint",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<int>(
                name: "AccountId",
                table: "WagerChallenges",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<byte>(
                name: "Status",
                table: "Tournaments",
                type: "tinyint",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.CreateTable(
                name: "StellarAccount",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Asset = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Balance = table.Column<decimal>(type: "decimal(18,7)", nullable: false),
                    SecretSeed = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StellarAccount", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WagerChallengeBids",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Approved = table.Column<bool>(type: "bit", nullable: true),
                    ChallengeId = table.Column<int>(type: "int", nullable: false),
                    PayablePt = table.Column<byte>(type: "tinyint", nullable: false),
                    ProfileId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ReceivablePt = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WagerChallengeBids", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WagerChallengeBids_WagerChallenges_ChallengeId",
                        column: x => x.ChallengeId,
                        principalTable: "WagerChallenges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WagerChallengeBids_Profiles_ProfileId",
                        column: x => x.ProfileId,
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WagerHostBids",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Approved = table.Column<bool>(type: "bit", nullable: true),
                    PayablePt = table.Column<byte>(type: "tinyint", nullable: false),
                    ProfileId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ReceivablePt = table.Column<byte>(type: "tinyint", nullable: false),
                    WagerId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WagerHostBids", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WagerHostBids_Profiles_ProfileId",
                        column: x => x.ProfileId,
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WagerHostBids_Wagers_WagerId",
                        column: x => x.WagerId,
                        principalTable: "Wagers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WagerApprovals",
                columns: table => new
                {
                    HostId = table.Column<int>(type: "int", nullable: false),
                    ChallengeId = table.Column<int>(type: "int", nullable: false),
                    Approved = table.Column<bool>(type: "bit", nullable: true)
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
                name: "IX_WagerChallenges_AccountId",
                table: "WagerChallenges",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_WagerApprovals_ChallengeId",
                table: "WagerApprovals",
                column: "ChallengeId");

            migrationBuilder.CreateIndex(
                name: "IX_WagerChallengeBids_ChallengeId",
                table: "WagerChallengeBids",
                column: "ChallengeId");

            migrationBuilder.CreateIndex(
                name: "IX_WagerChallengeBids_ProfileId",
                table: "WagerChallengeBids",
                column: "ProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_WagerHostBids_ProfileId",
                table: "WagerHostBids",
                column: "ProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_WagerHostBids_WagerId",
                table: "WagerHostBids",
                column: "WagerId");

            migrationBuilder.AddForeignKey(
                name: "FK_WagerChallenges_StellarAccount_AccountId",
                table: "WagerChallenges",
                column: "AccountId",
                principalTable: "StellarAccount",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
