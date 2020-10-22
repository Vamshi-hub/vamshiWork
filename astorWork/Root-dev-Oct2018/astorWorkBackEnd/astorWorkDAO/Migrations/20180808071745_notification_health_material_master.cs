using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace astorWorkDAO.Migrations
{
    public partial class notification_health_material_master : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MaterialMaster_RebarShapeMaster_RebarShapeID",
                table: "MaterialMaster");

            migrationBuilder.DropTable(
                name: "RebarShapeMaster");

            migrationBuilder.DropIndex(
                name: "IX_MaterialMaster_RebarShapeID",
                table: "MaterialMaster");

            migrationBuilder.DropColumn(
                name: "Dimensions",
                table: "MaterialMaster");

            migrationBuilder.DropColumn(
                name: "OrderQuantity",
                table: "MaterialMaster");

            migrationBuilder.DropColumn(
                name: "RebarShapeID",
                table: "MaterialMaster");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Dimensions",
                table: "MaterialMaster",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OrderQuantity",
                table: "MaterialMaster",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RebarShapeID",
                table: "MaterialMaster",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "RebarShapeMaster",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ShapeCode = table.Column<string>(nullable: true),
                    ShapeFileName = table.Column<string>(nullable: true),
                    SidesToEnable = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RebarShapeMaster", x => x.ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MaterialMaster_RebarShapeID",
                table: "MaterialMaster",
                column: "RebarShapeID");

            migrationBuilder.AddForeignKey(
                name: "FK_MaterialMaster_RebarShapeMaster_RebarShapeID",
                table: "MaterialMaster",
                column: "RebarShapeID",
                principalTable: "RebarShapeMaster",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
