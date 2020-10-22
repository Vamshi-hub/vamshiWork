using Microsoft.EntityFrameworkCore.Migrations;

namespace astorWorkDAO.Migrations
{
    public partial class new_columns_addedto_MaterialMaster : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "Area",
                table: "MaterialMaster",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Dimensions",
                table: "MaterialMaster",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "Length",
                table: "MaterialMaster",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Area",
                table: "MaterialMaster");

            migrationBuilder.DropColumn(
                name: "Dimensions",
                table: "MaterialMaster");

            migrationBuilder.DropColumn(
                name: "Length",
                table: "MaterialMaster");
        }
    }
}
