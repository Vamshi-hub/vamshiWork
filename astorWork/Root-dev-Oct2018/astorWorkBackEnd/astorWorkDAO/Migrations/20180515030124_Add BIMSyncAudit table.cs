using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace astorWorkDAO.Migrations
{
    public partial class AddBIMSyncAudittable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectMaster_UserMaster_CreatedByID",
                table: "ProjectMaster");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectMaster_UserMaster_UpdatedByID",
                table: "ProjectMaster");

            migrationBuilder.DropForeignKey(
                name: "FK_RolePageAssociation_PageMaster_PageMasterID",
                table: "RolePageAssociation");

            migrationBuilder.DropIndex(
                name: "IX_RolePageAssociation_PageMasterID",
                table: "RolePageAssociation");

            migrationBuilder.DropIndex(
                name: "IX_ProjectMaster_CreatedByID",
                table: "ProjectMaster");

            migrationBuilder.DropIndex(
                name: "IX_ProjectMaster_UpdatedByID",
                table: "ProjectMaster");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "UserMaster");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "UserMaster");

            migrationBuilder.DropColumn(
                name: "PageMasterID",
                table: "RolePageAssociation");

            migrationBuilder.DropColumn(
                name: "CreatedByID",
                table: "ProjectMaster");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "ProjectMaster");

            migrationBuilder.DropColumn(
                name: "UpdatedByID",
                table: "ProjectMaster");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "ProjectMaster");

            migrationBuilder.CreateTable(
                name: "BIMSyncAudit",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BIMModelId = table.Column<string>(nullable: false),
                    BIMVideoUrl = table.Column<string>(nullable: true),
                    ProjectID = table.Column<int>(nullable: false),
                    SyncTime = table.Column<DateTime>(nullable: false),
                    SyncedByID = table.Column<int>(nullable: false),
                    SyncedMaterialIds = table.Column<string>(nullable: true),
                    UnsyncedMaterialIds = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BIMSyncAudit", x => x.ID);
                    table.ForeignKey(
                        name: "FK_BIMSyncAudit_ProjectMaster_ProjectID",
                        column: x => x.ProjectID,
                        principalTable: "ProjectMaster",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BIMSyncAudit_UserMaster_SyncedByID",
                        column: x => x.SyncedByID,
                        principalTable: "UserMaster",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BIMSyncAudit_ProjectID",
                table: "BIMSyncAudit",
                column: "ProjectID");

            migrationBuilder.CreateIndex(
                name: "IX_BIMSyncAudit_SyncedByID",
                table: "BIMSyncAudit",
                column: "SyncedByID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BIMSyncAudit");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "UserMaster",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "UserMaster",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "PageMasterID",
                table: "RolePageAssociation",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CreatedByID",
                table: "ProjectMaster",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "ProjectMaster",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "UpdatedByID",
                table: "ProjectMaster",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "ProjectMaster",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_RolePageAssociation_PageMasterID",
                table: "RolePageAssociation",
                column: "PageMasterID");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectMaster_CreatedByID",
                table: "ProjectMaster",
                column: "CreatedByID");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectMaster_UpdatedByID",
                table: "ProjectMaster",
                column: "UpdatedByID");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectMaster_UserMaster_CreatedByID",
                table: "ProjectMaster",
                column: "CreatedByID",
                principalTable: "UserMaster",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectMaster_UserMaster_UpdatedByID",
                table: "ProjectMaster",
                column: "UpdatedByID",
                principalTable: "UserMaster",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RolePageAssociation_PageMaster_PageMasterID",
                table: "RolePageAssociation",
                column: "PageMasterID",
                principalTable: "PageMaster",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
