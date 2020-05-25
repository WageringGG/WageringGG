using Microsoft.EntityFrameworkCore.Migrations;

namespace WageringGG.Server.Migrations
{
    public partial class Games : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Games",
                columns: new[] { "Id", "Name", "NormalizedName" },
                values: new object[] { 2, "Modern Warfare", "modern-warfare" });

            migrationBuilder.InsertData(
                table: "Games",
                columns: new[] { "Id", "Name", "NormalizedName" },
                values: new object[] { 3, "Apex Legends", "apex-legends" });

            migrationBuilder.InsertData(
                table: "Games",
                columns: new[] { "Id", "Name", "NormalizedName" },
                values: new object[] { 4, "Valorant", "valorant" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Games",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Games",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Games",
                keyColumn: "Id",
                keyValue: 4);
        }
    }
}
