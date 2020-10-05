using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WageringGG.Server.Migrations
{
    public partial class WagerHostMember : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WagerMembers_WagerChallenges_ChallengeId",
                table: "WagerMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_Wagers_StellarAccount_AccountId",
                table: "Wagers");

            migrationBuilder.DropIndex(
                name: "IX_Wagers_AccountId",
                table: "Wagers");

            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "Wagers");

            migrationBuilder.DropColumn(
                name: "PayablePercentage",
                table: "WagerMembers");

            migrationBuilder.DropColumn(
                name: "ReceivablePercentage",
                table: "WagerMembers");

            migrationBuilder.AlterColumn<int>(
                name: "ChallengeId",
                table: "WagerMembers",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Payable",
                table: "WagerMembers",
                type: "decimal(18,7)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Receivable",
                table: "WagerMembers",
                type: "decimal(18,7)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<bool>(
                name: "IsAccepted",
                table: "WagerChallenges",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AddColumn<string>(
                name: "AccountId",
                table: "WagerChallenges",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProfileId = table.Column<string>(nullable: true),
                    Date = table.Column<DateTime>(nullable: false),
                    FromAddress = table.Column<string>(nullable: true),
                    ToAddress = table.Column<string>(nullable: true),
                    Amount = table.Column<decimal>(nullable: false),
                    Data = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transactions_Profiles_ProfileId",
                        column: x => x.ProfileId,
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WagerHosts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WagerId = table.Column<int>(nullable: false),
                    ProfileId = table.Column<string>(nullable: true),
                    IsApproved = table.Column<bool>(nullable: true),
                    Payable = table.Column<decimal>(type: "decimal(18,7)", nullable: false),
                    Receivable = table.Column<decimal>(type: "decimal(18,7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WagerHosts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WagerHosts_Profiles_ProfileId",
                        column: x => x.ProfileId,
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WagerHosts_Wagers_WagerId",
                        column: x => x.WagerId,
                        principalTable: "Wagers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WagerChallenges_AccountId",
                table: "WagerChallenges",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_ProfileId",
                table: "Transactions",
                column: "ProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_WagerHosts_ProfileId",
                table: "WagerHosts",
                column: "ProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_WagerHosts_WagerId",
                table: "WagerHosts",
                column: "WagerId");

            migrationBuilder.AddForeignKey(
                name: "FK_WagerChallenges_StellarAccount_AccountId",
                table: "WagerChallenges",
                column: "AccountId",
                principalTable: "StellarAccount",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WagerMembers_WagerChallenges_ChallengeId",
                table: "WagerMembers",
                column: "ChallengeId",
                principalTable: "WagerChallenges",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WagerChallenges_StellarAccount_AccountId",
                table: "WagerChallenges");

            migrationBuilder.DropForeignKey(
                name: "FK_WagerMembers_WagerChallenges_ChallengeId",
                table: "WagerMembers");

            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.DropTable(
                name: "WagerHosts");

            migrationBuilder.DropIndex(
                name: "IX_WagerChallenges_AccountId",
                table: "WagerChallenges");

            migrationBuilder.DropColumn(
                name: "Payable",
                table: "WagerMembers");

            migrationBuilder.DropColumn(
                name: "Receivable",
                table: "WagerMembers");

            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "WagerChallenges");

            migrationBuilder.AddColumn<string>(
                name: "AccountId",
                table: "Wagers",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ChallengeId",
                table: "WagerMembers",
                type: "int",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<byte>(
                name: "PayablePercentage",
                table: "WagerMembers",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "ReceivablePercentage",
                table: "WagerMembers",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AlterColumn<bool>(
                name: "IsAccepted",
                table: "WagerChallenges",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Wagers_AccountId",
                table: "Wagers",
                column: "AccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_WagerMembers_WagerChallenges_ChallengeId",
                table: "WagerMembers",
                column: "ChallengeId",
                principalTable: "WagerChallenges",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Wagers_StellarAccount_AccountId",
                table: "Wagers",
                column: "AccountId",
                principalTable: "StellarAccount",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
