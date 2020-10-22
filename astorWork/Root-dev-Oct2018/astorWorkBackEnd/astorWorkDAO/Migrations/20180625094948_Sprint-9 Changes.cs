using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace astorWorkDAO.Migrations
{
    public partial class Sprint9Changes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PageMaster_ModuleMaster_ModuleMasterID",
                table: "PageMaster");

            migrationBuilder.AddColumn<int>(
                name: "BatchNumber",
                table: "TrackerMaster",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsEditable",
                table: "RoleMaster",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "ProjectMaster",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "EstimatedEndDate",
                table: "ProjectMaster",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "EstimatedStartDate",
                table: "ProjectMaster",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<int>(
                name: "ProjectManagerID",
                table: "ProjectMaster",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ModuleMasterID",
                table: "PageMaster",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsEditable",
                table: "MaterialStageMaster",
                nullable: false,
                defaultValue: false);

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

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "LocationMaster",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "LocationMaster",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "VendorID",
                table: "LocationMaster",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "RebarShapeMaster",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ShapeCode = table.Column<string>(nullable: true),
                    SidesToEnable = table.Column<string>(nullable: true),
                    ShapeFileName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RebarShapeMaster", x => x.ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectMaster_ProjectManagerID",
                table: "ProjectMaster",
                column: "ProjectManagerID");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialMaster_RebarShapeID",
                table: "MaterialMaster",
                column: "RebarShapeID");

            migrationBuilder.CreateIndex(
                name: "IX_LocationMaster_VendorID",
                table: "LocationMaster",
                column: "VendorID");

            migrationBuilder.AddForeignKey(
                name: "FK_LocationMaster_VendorMaster_VendorID",
                table: "LocationMaster",
                column: "VendorID",
                principalTable: "VendorMaster",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MaterialMaster_RebarShapeMaster_RebarShapeID",
                table: "MaterialMaster",
                column: "RebarShapeID",
                principalTable: "RebarShapeMaster",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PageMaster_ModuleMaster_ModuleMasterID",
                table: "PageMaster",
                column: "ModuleMasterID",
                principalTable: "ModuleMaster",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectMaster_UserMaster_ProjectManagerID",
                table: "ProjectMaster",
                column: "ProjectManagerID",
                principalTable: "UserMaster",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LocationMaster_VendorMaster_VendorID",
                table: "LocationMaster");

            migrationBuilder.DropForeignKey(
                name: "FK_MaterialMaster_RebarShapeMaster_RebarShapeID",
                table: "MaterialMaster");

            migrationBuilder.DropForeignKey(
                name: "FK_PageMaster_ModuleMaster_ModuleMasterID",
                table: "PageMaster");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectMaster_UserMaster_ProjectManagerID",
                table: "ProjectMaster");

            migrationBuilder.DropTable(
                name: "RebarShapeMaster");

            migrationBuilder.DropIndex(
                name: "IX_ProjectMaster_ProjectManagerID",
                table: "ProjectMaster");

            migrationBuilder.DropIndex(
                name: "IX_MaterialMaster_RebarShapeID",
                table: "MaterialMaster");

            migrationBuilder.DropIndex(
                name: "IX_LocationMaster_VendorID",
                table: "LocationMaster");

            migrationBuilder.DropColumn(
                name: "BatchNumber",
                table: "TrackerMaster");

            migrationBuilder.DropColumn(
                name: "IsEditable",
                table: "RoleMaster");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "ProjectMaster");

            migrationBuilder.DropColumn(
                name: "EstimatedEndDate",
                table: "ProjectMaster");

            migrationBuilder.DropColumn(
                name: "EstimatedStartDate",
                table: "ProjectMaster");

            migrationBuilder.DropColumn(
                name: "ProjectManagerID",
                table: "ProjectMaster");

            migrationBuilder.DropColumn(
                name: "IsEditable",
                table: "MaterialStageMaster");

            migrationBuilder.DropColumn(
                name: "Dimensions",
                table: "MaterialMaster");

            migrationBuilder.DropColumn(
                name: "OrderQuantity",
                table: "MaterialMaster");

            migrationBuilder.DropColumn(
                name: "RebarShapeID",
                table: "MaterialMaster");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "LocationMaster");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "LocationMaster");

            migrationBuilder.DropColumn(
                name: "VendorID",
                table: "LocationMaster");

            migrationBuilder.AlterColumn<int>(
                name: "ModuleMasterID",
                table: "PageMaster",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_PageMaster_ModuleMaster_ModuleMasterID",
                table: "PageMaster",
                column: "ModuleMasterID",
                principalTable: "ModuleMaster",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
