using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace astorWorkDAO.Migrations
{
    public partial class remove_update_date_inventory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventoryAudit_UserMaster_CreatedByID",
                table: "InventoryAudit");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryAudit_ProjectMaster_ProjectID",
                table: "InventoryAudit");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryAudit_TrackerMaster_TrackerID",
                table: "InventoryAudit");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryAudit_UserMaster_UpdatedByID",
                table: "InventoryAudit");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryAudit_VendorMaster_VendorID",
                table: "InventoryAudit");

            migrationBuilder.DropForeignKey(
                name: "FK_MaterialQCCase_UserMaster_CreatedByID",
                table: "MaterialQCCase");

            migrationBuilder.DropForeignKey(
                name: "FK_MaterialQCDefect_UserMaster_CreatedByID",
                table: "MaterialQCDefect");

            migrationBuilder.DropForeignKey(
                name: "FK_MaterialQCDefect_UserMaster_UpdatedByID",
                table: "MaterialQCDefect");

            migrationBuilder.DropForeignKey(
                name: "FK_MaterialQCPhotos_UserMaster_CreatedByID",
                table: "MaterialQCPhotos");

            migrationBuilder.DropForeignKey(
                name: "FK_MaterialStageAudit_UserMaster_CreatedByID",
                table: "MaterialStageAudit");

            migrationBuilder.DropForeignKey(
                name: "FK_MaterialStageAudit_LocationMaster_LocationID",
                table: "MaterialStageAudit");

            migrationBuilder.DropForeignKey(
                name: "FK_MaterialStageAudit_MaterialMaster_MaterialMasterID",
                table: "MaterialStageAudit");

            migrationBuilder.DropIndex(
                name: "IX_InventoryAudit_UpdatedByID",
                table: "InventoryAudit");

            migrationBuilder.DropColumn(
                name: "UpdatedByID",
                table: "InventoryAudit");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "InventoryAudit");

            migrationBuilder.RenameColumn(
                name: "LocationID",
                table: "MaterialStageAudit",
                newName: "LocationId");

            migrationBuilder.RenameIndex(
                name: "IX_MaterialStageAudit_LocationID",
                table: "MaterialStageAudit",
                newName: "IX_MaterialStageAudit_LocationId");

            migrationBuilder.RenameColumn(
                name: "CreatedByID",
                table: "MaterialQCPhotos",
                newName: "CreatedById");

            migrationBuilder.RenameIndex(
                name: "IX_MaterialQCPhotos_CreatedByID",
                table: "MaterialQCPhotos",
                newName: "IX_MaterialQCPhotos_CreatedById");

            migrationBuilder.RenameColumn(
                name: "UpdatedByID",
                table: "MaterialQCDefect",
                newName: "UpdatedById");

            migrationBuilder.RenameColumn(
                name: "CreatedByID",
                table: "MaterialQCDefect",
                newName: "CreatedById");

            migrationBuilder.RenameIndex(
                name: "IX_MaterialQCDefect_UpdatedByID",
                table: "MaterialQCDefect",
                newName: "IX_MaterialQCDefect_UpdatedById");

            migrationBuilder.RenameIndex(
                name: "IX_MaterialQCDefect_CreatedByID",
                table: "MaterialQCDefect",
                newName: "IX_MaterialQCDefect_CreatedById");

            migrationBuilder.RenameColumn(
                name: "CreatedByID",
                table: "MaterialQCCase",
                newName: "CreatedById");

            migrationBuilder.RenameIndex(
                name: "IX_MaterialQCCase_CreatedByID",
                table: "MaterialQCCase",
                newName: "IX_MaterialQCCase_CreatedById");

            migrationBuilder.AlterColumn<int>(
                name: "MaterialMasterID",
                table: "MaterialStageAudit",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "LocationId",
                table: "MaterialStageAudit",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CreatedByID",
                table: "MaterialStageAudit",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "VendorID",
                table: "InventoryAudit",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "TrackerID",
                table: "InventoryAudit",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ProjectID",
                table: "InventoryAudit",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedDate",
                table: "InventoryAudit",
                nullable: true,
                oldClrType: typeof(DateTimeOffset));

            migrationBuilder.AlterColumn<int>(
                name: "CreatedByID",
                table: "InventoryAudit",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryAudit_UserMaster_CreatedByID",
                table: "InventoryAudit",
                column: "CreatedByID",
                principalTable: "UserMaster",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryAudit_ProjectMaster_ProjectID",
                table: "InventoryAudit",
                column: "ProjectID",
                principalTable: "ProjectMaster",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryAudit_TrackerMaster_TrackerID",
                table: "InventoryAudit",
                column: "TrackerID",
                principalTable: "TrackerMaster",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryAudit_VendorMaster_VendorID",
                table: "InventoryAudit",
                column: "VendorID",
                principalTable: "VendorMaster",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MaterialQCCase_UserMaster_CreatedById",
                table: "MaterialQCCase",
                column: "CreatedById",
                principalTable: "UserMaster",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MaterialQCDefect_UserMaster_CreatedById",
                table: "MaterialQCDefect",
                column: "CreatedById",
                principalTable: "UserMaster",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MaterialQCDefect_UserMaster_UpdatedById",
                table: "MaterialQCDefect",
                column: "UpdatedById",
                principalTable: "UserMaster",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MaterialQCPhotos_UserMaster_CreatedById",
                table: "MaterialQCPhotos",
                column: "CreatedById",
                principalTable: "UserMaster",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MaterialStageAudit_UserMaster_CreatedByID",
                table: "MaterialStageAudit",
                column: "CreatedByID",
                principalTable: "UserMaster",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MaterialStageAudit_LocationMaster_LocationId",
                table: "MaterialStageAudit",
                column: "LocationId",
                principalTable: "LocationMaster",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MaterialStageAudit_MaterialMaster_MaterialMasterID",
                table: "MaterialStageAudit",
                column: "MaterialMasterID",
                principalTable: "MaterialMaster",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventoryAudit_UserMaster_CreatedByID",
                table: "InventoryAudit");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryAudit_ProjectMaster_ProjectID",
                table: "InventoryAudit");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryAudit_TrackerMaster_TrackerID",
                table: "InventoryAudit");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryAudit_VendorMaster_VendorID",
                table: "InventoryAudit");

            migrationBuilder.DropForeignKey(
                name: "FK_MaterialQCCase_UserMaster_CreatedById",
                table: "MaterialQCCase");

            migrationBuilder.DropForeignKey(
                name: "FK_MaterialQCDefect_UserMaster_CreatedById",
                table: "MaterialQCDefect");

            migrationBuilder.DropForeignKey(
                name: "FK_MaterialQCDefect_UserMaster_UpdatedById",
                table: "MaterialQCDefect");

            migrationBuilder.DropForeignKey(
                name: "FK_MaterialQCPhotos_UserMaster_CreatedById",
                table: "MaterialQCPhotos");

            migrationBuilder.DropForeignKey(
                name: "FK_MaterialStageAudit_UserMaster_CreatedByID",
                table: "MaterialStageAudit");

            migrationBuilder.DropForeignKey(
                name: "FK_MaterialStageAudit_LocationMaster_LocationId",
                table: "MaterialStageAudit");

            migrationBuilder.DropForeignKey(
                name: "FK_MaterialStageAudit_MaterialMaster_MaterialMasterID",
                table: "MaterialStageAudit");

            migrationBuilder.RenameColumn(
                name: "LocationId",
                table: "MaterialStageAudit",
                newName: "LocationID");

            migrationBuilder.RenameIndex(
                name: "IX_MaterialStageAudit_LocationId",
                table: "MaterialStageAudit",
                newName: "IX_MaterialStageAudit_LocationID");

            migrationBuilder.RenameColumn(
                name: "CreatedById",
                table: "MaterialQCPhotos",
                newName: "CreatedByID");

            migrationBuilder.RenameIndex(
                name: "IX_MaterialQCPhotos_CreatedById",
                table: "MaterialQCPhotos",
                newName: "IX_MaterialQCPhotos_CreatedByID");

            migrationBuilder.RenameColumn(
                name: "UpdatedById",
                table: "MaterialQCDefect",
                newName: "UpdatedByID");

            migrationBuilder.RenameColumn(
                name: "CreatedById",
                table: "MaterialQCDefect",
                newName: "CreatedByID");

            migrationBuilder.RenameIndex(
                name: "IX_MaterialQCDefect_UpdatedById",
                table: "MaterialQCDefect",
                newName: "IX_MaterialQCDefect_UpdatedByID");

            migrationBuilder.RenameIndex(
                name: "IX_MaterialQCDefect_CreatedById",
                table: "MaterialQCDefect",
                newName: "IX_MaterialQCDefect_CreatedByID");

            migrationBuilder.RenameColumn(
                name: "CreatedById",
                table: "MaterialQCCase",
                newName: "CreatedByID");

            migrationBuilder.RenameIndex(
                name: "IX_MaterialQCCase_CreatedById",
                table: "MaterialQCCase",
                newName: "IX_MaterialQCCase_CreatedByID");

            migrationBuilder.AlterColumn<int>(
                name: "MaterialMasterID",
                table: "MaterialStageAudit",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "LocationID",
                table: "MaterialStageAudit",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "CreatedByID",
                table: "MaterialStageAudit",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "VendorID",
                table: "InventoryAudit",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "TrackerID",
                table: "InventoryAudit",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "ProjectID",
                table: "InventoryAudit",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedDate",
                table: "InventoryAudit",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CreatedByID",
                table: "InventoryAudit",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<int>(
                name: "UpdatedByID",
                table: "InventoryAudit",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedDate",
                table: "InventoryAudit",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.CreateIndex(
                name: "IX_InventoryAudit_UpdatedByID",
                table: "InventoryAudit",
                column: "UpdatedByID");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryAudit_UserMaster_CreatedByID",
                table: "InventoryAudit",
                column: "CreatedByID",
                principalTable: "UserMaster",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryAudit_ProjectMaster_ProjectID",
                table: "InventoryAudit",
                column: "ProjectID",
                principalTable: "ProjectMaster",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryAudit_TrackerMaster_TrackerID",
                table: "InventoryAudit",
                column: "TrackerID",
                principalTable: "TrackerMaster",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryAudit_UserMaster_UpdatedByID",
                table: "InventoryAudit",
                column: "UpdatedByID",
                principalTable: "UserMaster",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryAudit_VendorMaster_VendorID",
                table: "InventoryAudit",
                column: "VendorID",
                principalTable: "VendorMaster",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MaterialQCCase_UserMaster_CreatedByID",
                table: "MaterialQCCase",
                column: "CreatedByID",
                principalTable: "UserMaster",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MaterialQCDefect_UserMaster_CreatedByID",
                table: "MaterialQCDefect",
                column: "CreatedByID",
                principalTable: "UserMaster",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MaterialQCDefect_UserMaster_UpdatedByID",
                table: "MaterialQCDefect",
                column: "UpdatedByID",
                principalTable: "UserMaster",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MaterialQCPhotos_UserMaster_CreatedByID",
                table: "MaterialQCPhotos",
                column: "CreatedByID",
                principalTable: "UserMaster",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MaterialStageAudit_UserMaster_CreatedByID",
                table: "MaterialStageAudit",
                column: "CreatedByID",
                principalTable: "UserMaster",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MaterialStageAudit_LocationMaster_LocationID",
                table: "MaterialStageAudit",
                column: "LocationID",
                principalTable: "LocationMaster",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MaterialStageAudit_MaterialMaster_MaterialMasterID",
                table: "MaterialStageAudit",
                column: "MaterialMasterID",
                principalTable: "MaterialMaster",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
