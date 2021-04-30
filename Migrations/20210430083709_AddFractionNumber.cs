using Microsoft.EntityFrameworkCore.Migrations;

namespace SortexAdminV._1.Migrations
{
    public partial class AddFractionNumber : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Number",
                table: "Fractions",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Number",
                table: "Fractions");
        }
    }
}
