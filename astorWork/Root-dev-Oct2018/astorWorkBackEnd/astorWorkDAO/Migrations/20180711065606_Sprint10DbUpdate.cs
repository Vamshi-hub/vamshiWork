using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace astorWorkDAO.Migrations
{
    public partial class Sprint10DbUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SiteID",
                table: "UserMaster",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MaterialTypes",
                table: "MaterialStageMaster",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SiteID",
                table: "LocationMaster",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MaterialQCPhotos",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    URL = table.Column<string>(nullable: true),
                    Remarks = table.Column<string>(nullable: true),
                    MaterialStageAuditID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialQCPhotos", x => x.ID);
                    table.ForeignKey(
                        name: "FK_MaterialQCPhotos_MaterialStageAudit_MaterialStageAuditID",
                        column: x => x.MaterialStageAuditID,
                        principalTable: "MaterialStageAudit",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SiteMaster",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Country = table.Column<string>(nullable: true),
                    VendorID = table.Column<int>(nullable: true),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiteMaster", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SiteMaster_VendorMaster_VendorID",
                        column: x => x.VendorID,
                        principalTable: "VendorMaster",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserMaster_SiteID",
                table: "UserMaster",
                column: "SiteID");

            migrationBuilder.CreateIndex(
                name: "IX_LocationMaster_SiteID",
                table: "LocationMaster",
                column: "SiteID");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialQCPhotos_MaterialStageAuditID",
                table: "MaterialQCPhotos",
                column: "MaterialStageAuditID");

            migrationBuilder.CreateIndex(
                name: "IX_SiteMaster_VendorID",
                table: "SiteMaster",
                column: "VendorID");

            migrationBuilder.AddForeignKey(
                name: "FK_LocationMaster_SiteMaster_SiteID",
                table: "LocationMaster",
                column: "SiteID",
                principalTable: "SiteMaster",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserMaster_SiteMaster_SiteID",
                table: "UserMaster",
                column: "SiteID",
                principalTable: "SiteMaster",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LocationMaster_SiteMaster_SiteID",
                table: "LocationMaster");

            migrationBuilder.DropForeignKey(
                name: "FK_UserMaster_SiteMaster_SiteID",
                table: "UserMaster");

            migrationBuilder.DropTable(
                name: "MaterialQCPhotos");

            migrationBuilder.DropTable(
                name: "SiteMaster");

            migrationBuilder.DropIndex(
                name: "IX_UserMaster_SiteID",
                table: "UserMaster");

            migrationBuilder.DropIndex(
                name: "IX_LocationMaster_SiteID",
                table: "LocationMaster");

            migrationBuilder.DropColumn(
                name: "SiteID",
                table: "UserMaster");

            migrationBuilder.DropColumn(
                name: "MaterialTypes",
                table: "MaterialStageMaster");

            migrationBuilder.DropColumn(
                name: "SiteID",
                table: "LocationMaster");
        }
    }
}
