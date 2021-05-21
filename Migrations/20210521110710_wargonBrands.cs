using Microsoft.EntityFrameworkCore.Migrations;

namespace SortexAdminV._1.Migrations
{
    public partial class wargonBrands : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Märke",
                table: "WargonBrands",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_WargonBrands",
                table: "WargonBrands",
                column: "Märke");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_WargonBrands",
                table: "WargonBrands");

            migrationBuilder.AlterColumn<string>(
                name: "Märke",
                table: "WargonBrands",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
