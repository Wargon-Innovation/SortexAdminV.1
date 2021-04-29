using Microsoft.EntityFrameworkCore.Migrations;

namespace SortexAdminV._1.Migrations
{
    public partial class AddFilePathColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FilePath",
                table: "TrendImages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FilePath",
                table: "Modeboards",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FilePath",
                table: "TrendImages");

            migrationBuilder.DropColumn(
                name: "FilePath",
                table: "Modeboards");
        }
    }
}
