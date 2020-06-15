using Microsoft.EntityFrameworkCore.Migrations;

namespace WageringGG.Server.Migrations
{
    public partial class Title : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Wagers");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Tournaments");

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Wagers",
                maxLength: 50,
                nullable: false,
                defaultValue: "Title");

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Tournaments",
                maxLength: 50,
                nullable: false,
                defaultValue: "Title");

            migrationBuilder.InsertData(
                table: "Games",
                columns: new[] { "Id", "Name", "NormalizedName" },
                values: new object[] { 5, "Fifa 20", "fifa20" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Games",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Wagers");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Tournaments");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Wagers",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Tournaments",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }
    }
}
