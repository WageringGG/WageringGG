using Microsoft.EntityFrameworkCore.Migrations;

namespace WageringGG.Server.Migrations
{
    public partial class AddFeatures : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WagerChallengeBids_Profiles_ProfileId",
                table: "WagerChallengeBids");

            migrationBuilder.DropForeignKey(
                name: "FK_WagerHostBids_Profiles_ProfileId",
                table: "WagerHostBids");

            migrationBuilder.AlterColumn<string>(
                name: "ProfileId",
                table: "WagerHostBids",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ProfileId",
                table: "WagerChallengeBids",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_WagerChallengeBids_Profiles_ProfileId",
                table: "WagerChallengeBids",
                column: "ProfileId",
                principalTable: "Profiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WagerHostBids_Profiles_ProfileId",
                table: "WagerHostBids",
                column: "ProfileId",
                principalTable: "Profiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WagerChallengeBids_Profiles_ProfileId",
                table: "WagerChallengeBids");

            migrationBuilder.DropForeignKey(
                name: "FK_WagerHostBids_Profiles_ProfileId",
                table: "WagerHostBids");

            migrationBuilder.AlterColumn<string>(
                name: "ProfileId",
                table: "WagerHostBids",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "ProfileId",
                table: "WagerChallengeBids",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AddForeignKey(
                name: "FK_WagerChallengeBids_Profiles_ProfileId",
                table: "WagerChallengeBids",
                column: "ProfileId",
                principalTable: "Profiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WagerHostBids_Profiles_ProfileId",
                table: "WagerHostBids",
                column: "ProfileId",
                principalTable: "Profiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
