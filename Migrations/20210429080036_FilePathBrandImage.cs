using Microsoft.EntityFrameworkCore.Migrations;

namespace SortexAdminV._1.Migrations
{
    public partial class FilePathBrandImage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FilePath",
                table: "BrandImages",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FilePath",
                table: "BrandImages");
        }
    }
}
