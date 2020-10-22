using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace astorWorkDAO.Migrations
{
    public partial class add_notification_and_forge_tables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TimeZoneOffset",
                table: "SiteMaster",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TimeZoneOffset",
                table: "ProjectMaster",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "BIMForgeModel",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BucketKey = table.Column<string>(nullable: true),
                    ObjectKey = table.Column<string>(nullable: true),
                    ObjectId = table.Column<string>(nullable: true),
                    Sha1 = table.Column<string>(nullable: true),
                    Size = table.Column<int>(nullable: false),
                    Location = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BIMForgeModel", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "NotificationTimerMaster",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<int>(nullable: false),
                    TriggerTime = table.Column<int>(nullable: false),
                    Enabled = table.Column<bool>(nullable: false),
                    UpdateRequired = table.Column<bool>(nullable: false),
                    SiteID = table.Column<int>(nullable: true),
                    ProjectID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationTimerMaster", x => x.ID);
                    table.ForeignKey(
                        name: "FK_NotificationTimerMaster_ProjectMaster_ProjectID",
                        column: x => x.ProjectID,
                        principalTable: "ProjectMaster",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NotificationTimerMaster_SiteMaster_SiteID",
                        column: x => x.SiteID,
                        principalTable: "SiteMaster",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BIMForgeElement",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DbId = table.Column<int>(nullable: false),
                    ForgeModelId = table.Column<int>(nullable: false),
                    MaterialMasterId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BIMForgeElement", x => x.ID);
                    table.ForeignKey(
                        name: "FK_BIMForgeElement_BIMForgeModel_ForgeModelId",
                        column: x => x.ForgeModelId,
                        principalTable: "BIMForgeModel",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BIMForgeElement_MaterialMaster_MaterialMasterId",
                        column: x => x.MaterialMasterId,
                        principalTable: "MaterialMaster",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BIMForgeElement_ForgeModelId",
                table: "BIMForgeElement",
                column: "ForgeModelId");

            migrationBuilder.CreateIndex(
                name: "IX_BIMForgeElement_MaterialMasterId",
                table: "BIMForgeElement",
                column: "MaterialMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationTimerMaster_ProjectID",
                table: "NotificationTimerMaster",
                column: "ProjectID");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationTimerMaster_SiteID",
                table: "NotificationTimerMaster",
                column: "SiteID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BIMForgeElement");

            migrationBuilder.DropTable(
                name: "NotificationTimerMaster");

            migrationBuilder.DropTable(
                name: "BIMForgeModel");

            migrationBuilder.DropColumn(
                name: "TimeZoneOffset",
                table: "SiteMaster");

            migrationBuilder.DropColumn(
                name: "TimeZoneOffset",
                table: "ProjectMaster");
        }
    }
}
