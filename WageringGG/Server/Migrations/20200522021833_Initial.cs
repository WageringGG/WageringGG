using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WageringGG.Server.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: false),
                    NormalizedName = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Profiles",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    DisplayName = table.Column<string>(maxLength: 12, nullable: false),
                    NormalizedDisplayName = table.Column<string>(nullable: true),
                    IsVerified = table.Column<bool>(nullable: false),
                    PublicKey = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Profiles", x => x.Id);
                    table.UniqueConstraint("AK_Profiles_DisplayName", x => x.DisplayName);
                });

            migrationBuilder.CreateTable(
                name: "Rules",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Message = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tournaments",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Status = table.Column<byte>(nullable: false),
                    GameId = table.Column<int>(nullable: false),
                    Description = table.Column<string>(maxLength: 50, nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    IsPrivate = table.Column<bool>(nullable: false),
                    Entry = table.Column<decimal>(type: "decimal(18,7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tournaments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tournaments_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Wagers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Status = table.Column<byte>(nullable: false),
                    GameId = table.Column<int>(nullable: false),
                    Description = table.Column<string>(maxLength: 50, nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    IsPrivate = table.Column<bool>(nullable: false),
                    MinimumWager = table.Column<decimal>(type: "decimal(18,7)", nullable: true),
                    MaximumWager = table.Column<decimal>(type: "decimal(18,7)", nullable: true),
                    ChallengeCount = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wagers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Wagers_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Connections",
                columns: table => new
                {
                    ConnectionId = table.Column<string>(nullable: false),
                    ProfileId = table.Column<string>(nullable: true),
                    UserAgent = table.Column<string>(nullable: false),
                    Connected = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Connections", x => x.ConnectionId);
                    table.ForeignKey(
                        name: "FK_Connections_Profiles_ProfileId",
                        column: x => x.ProfileId,
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProfileId = table.Column<string>(nullable: true),
                    Message = table.Column<string>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    DataModel = table.Column<byte>(nullable: false),
                    Data = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notifications_Profiles_ProfileId",
                        column: x => x.ProfileId,
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Ratings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GameId = table.Column<int>(nullable: false),
                    ProfileId = table.Column<int>(nullable: false),
                    ProfileId1 = table.Column<string>(nullable: true),
                    Value = table.Column<decimal>(type: "decimal(9,2)", nullable: false),
                    GamesPlayed = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ratings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ratings_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Ratings_Profiles_ProfileId1",
                        column: x => x.ProfileId1,
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WagerChallenges",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Status = table.Column<byte>(nullable: false),
                    WagerId = table.Column<int>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,7)", nullable: false),
                    IsAccepted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WagerChallenges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WagerChallenges_Wagers_WagerId",
                        column: x => x.WagerId,
                        principalTable: "Wagers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WagerHostBids",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProfileId = table.Column<string>(nullable: true),
                    Approved = table.Column<bool>(nullable: true),
                    ReceivablePt = table.Column<byte>(nullable: false),
                    PayablePt = table.Column<byte>(nullable: false),
                    IsOwner = table.Column<bool>(nullable: false),
                    WagerId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WagerHostBids", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WagerHostBids_Profiles_ProfileId",
                        column: x => x.ProfileId,
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WagerHostBids_Wagers_WagerId",
                        column: x => x.WagerId,
                        principalTable: "Wagers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WagerRules",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RuleId = table.Column<int>(nullable: false),
                    WagerId = table.Column<int>(nullable: false)
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

            migrationBuilder.CreateTable(
                name: "WagerChallengeBids",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProfileId = table.Column<string>(nullable: true),
                    Approved = table.Column<bool>(nullable: true),
                    ReceivablePt = table.Column<byte>(nullable: false),
                    PayablePt = table.Column<byte>(nullable: false),
                    IsOwner = table.Column<bool>(nullable: false),
                    ChallengeId = table.Column<int>(nullable: false)
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
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Games",
                columns: new[] { "Id", "Name", "NormalizedName" },
                values: new object[] { 1, "Fortnite", "fortnite" });

            migrationBuilder.CreateIndex(
                name: "IX_Connections_ProfileId",
                table: "Connections",
                column: "ProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_Games_NormalizedName",
                table: "Games",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_ProfileId",
                table: "Notifications",
                column: "ProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_Profiles_NormalizedDisplayName",
                table: "Profiles",
                column: "NormalizedDisplayName",
                unique: true,
                filter: "[NormalizedDisplayName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Ratings_GameId",
                table: "Ratings",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_Ratings_ProfileId1",
                table: "Ratings",
                column: "ProfileId1");

            migrationBuilder.CreateIndex(
                name: "IX_Tournaments_GameId",
                table: "Tournaments",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_WagerChallengeBids_ChallengeId",
                table: "WagerChallengeBids",
                column: "ChallengeId");

            migrationBuilder.CreateIndex(
                name: "IX_WagerChallengeBids_ProfileId",
                table: "WagerChallengeBids",
                column: "ProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_WagerChallenges_WagerId",
                table: "WagerChallenges",
                column: "WagerId");

            migrationBuilder.CreateIndex(
                name: "IX_WagerHostBids_ProfileId",
                table: "WagerHostBids",
                column: "ProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_WagerHostBids_WagerId",
                table: "WagerHostBids",
                column: "WagerId");

            migrationBuilder.CreateIndex(
                name: "IX_WagerRules_RuleId",
                table: "WagerRules",
                column: "RuleId");

            migrationBuilder.CreateIndex(
                name: "IX_WagerRules_WagerId",
                table: "WagerRules",
                column: "WagerId");

            migrationBuilder.CreateIndex(
                name: "IX_Wagers_GameId",
                table: "Wagers",
                column: "GameId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Connections");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "Ratings");

            migrationBuilder.DropTable(
                name: "Tournaments");

            migrationBuilder.DropTable(
                name: "WagerChallengeBids");

            migrationBuilder.DropTable(
                name: "WagerHostBids");

            migrationBuilder.DropTable(
                name: "WagerRules");

            migrationBuilder.DropTable(
                name: "WagerChallenges");

            migrationBuilder.DropTable(
                name: "Profiles");

            migrationBuilder.DropTable(
                name: "Rules");

            migrationBuilder.DropTable(
                name: "Wagers");

            migrationBuilder.DropTable(
                name: "Games");
        }
    }
}
