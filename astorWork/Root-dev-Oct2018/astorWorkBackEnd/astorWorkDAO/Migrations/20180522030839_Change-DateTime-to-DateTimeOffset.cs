using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace astorWorkDAO.Migrations
{
    public partial class ChangeDateTimetoDateTimeOffset : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LocationMaster_UserMaster_CreatedByID",
                table: "LocationMaster");

            migrationBuilder.DropForeignKey(
                name: "FK_LocationMaster_UserMaster_UpdatedByID",
                table: "LocationMaster");

            migrationBuilder.DropForeignKey(
                name: "FK_MaterialMaster_UserMaster_CreatedByID",
                table: "MaterialMaster");

            migrationBuilder.DropForeignKey(
                name: "FK_MaterialMaster_UserMaster_UpdatedByID",
                table: "MaterialMaster");

            migrationBuilder.DropForeignKey(
                name: "FK_MaterialStageAudit_UserMaster_UpdatedByID",
                table: "MaterialStageAudit");

            migrationBuilder.DropForeignKey(
                name: "FK_MaterialStageMaster_UserMaster_CreatedByID",
                table: "MaterialStageMaster");

            migrationBuilder.DropForeignKey(
                name: "FK_MaterialStageMaster_UserMaster_UpdatedByID",
                table: "MaterialStageMaster");

            migrationBuilder.DropForeignKey(
                name: "FK_TrackerMaster_UserMaster_CreatedByID",
                table: "TrackerMaster");

            migrationBuilder.DropForeignKey(
                name: "FK_TrackerMaster_UserMaster_UpdatedByID",
                table: "TrackerMaster");

            migrationBuilder.DropIndex(
                name: "IX_TrackerMaster_CreatedByID",
                table: "TrackerMaster");

            migrationBuilder.DropIndex(
                name: "IX_TrackerMaster_UpdatedByID",
                table: "TrackerMaster");

            migrationBuilder.DropIndex(
                name: "IX_MaterialStageMaster_CreatedByID",
                table: "MaterialStageMaster");

            migrationBuilder.DropIndex(
                name: "IX_MaterialStageMaster_UpdatedByID",
                table: "MaterialStageMaster");

            migrationBuilder.DropIndex(
                name: "IX_MaterialStageAudit_UpdatedByID",
                table: "MaterialStageAudit");

            migrationBuilder.DropIndex(
                name: "IX_MaterialMaster_CreatedByID",
                table: "MaterialMaster");

            migrationBuilder.DropIndex(
                name: "IX_MaterialMaster_UpdatedByID",
                table: "MaterialMaster");

            migrationBuilder.DropIndex(
                name: "IX_LocationMaster_CreatedByID",
                table: "LocationMaster");

            migrationBuilder.DropIndex(
                name: "IX_LocationMaster_UpdatedByID",
                table: "LocationMaster");

            migrationBuilder.DropColumn(
                name: "CreatedByID",
                table: "TrackerMaster");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "TrackerMaster");

            migrationBuilder.DropColumn(
                name: "UpdatedByID",
                table: "TrackerMaster");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "TrackerMaster");

            migrationBuilder.DropColumn(
                name: "CreatedByID",
                table: "MaterialStageMaster");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "MaterialStageMaster");

            migrationBuilder.DropColumn(
                name: "UpdatedByID",
                table: "MaterialStageMaster");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "MaterialStageMaster");

            migrationBuilder.DropColumn(
                name: "UpdatedByID",
                table: "MaterialStageAudit");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "MaterialStageAudit");

            migrationBuilder.DropColumn(
                name: "CreatedByID",
                table: "MaterialMaster");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "MaterialMaster");

            migrationBuilder.DropColumn(
                name: "UpdatedByID",
                table: "MaterialMaster");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "MaterialMaster");

            migrationBuilder.DropColumn(
                name: "CreatedByID",
                table: "LocationMaster");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "LocationMaster");

            migrationBuilder.DropColumn(
                name: "UpdatedByID",
                table: "LocationMaster");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "LocationMaster");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedTime",
                table: "UserSessionAudit",
                nullable: false,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "LastLogin",
                table: "UserMaster",
                nullable: false,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedDate",
                table: "MRFMaster",
                nullable: false,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "PlannedCastingDate",
                table: "MRFMaster",
                nullable: false,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "OrderDate",
                table: "MRFMaster",
                nullable: false,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "ExpectedDeliveryDate",
                table: "MRFMaster",
                nullable: false,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedDate",
                table: "MRFMaster",
                nullable: false,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedDate",
                table: "MaterialStageAudit",
                nullable: false,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CastingDate",
                table: "MaterialMaster",
                nullable: false,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedDate",
                table: "MaterialInfoAudit",
                nullable: false,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "ExpectedDeliveryDate",
                table: "MaterialInfoAudit",
                nullable: false,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedDate",
                table: "MaterialInfoAudit",
                nullable: false,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DrawingIssueDate",
                table: "MaterialDrawingAudit",
                nullable: false,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedDate",
                table: "InventoryAudit",
                nullable: false,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedDate",
                table: "InventoryAudit",
                nullable: false,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CastingDate",
                table: "InventoryAudit",
                nullable: false,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "SyncTime",
                table: "BIMSyncAudit",
                nullable: false,
                oldClrType: typeof(DateTime));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedTime",
                table: "UserSessionAudit",
                nullable: false,
                oldClrType: typeof(DateTimeOffset));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastLogin",
                table: "UserMaster",
                nullable: false,
                oldClrType: typeof(DateTimeOffset));

            migrationBuilder.AddColumn<int>(
                name: "CreatedByID",
                table: "TrackerMaster",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "TrackerMaster",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "UpdatedByID",
                table: "TrackerMaster",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "TrackerMaster",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedDate",
                table: "MRFMaster",
                nullable: false,
                oldClrType: typeof(DateTimeOffset));

            migrationBuilder.AlterColumn<DateTime>(
                name: "PlannedCastingDate",
                table: "MRFMaster",
                nullable: false,
                oldClrType: typeof(DateTimeOffset));

            migrationBuilder.AlterColumn<DateTime>(
                name: "OrderDate",
                table: "MRFMaster",
                nullable: false,
                oldClrType: typeof(DateTimeOffset));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ExpectedDeliveryDate",
                table: "MRFMaster",
                nullable: false,
                oldClrType: typeof(DateTimeOffset));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "MRFMaster",
                nullable: false,
                oldClrType: typeof(DateTimeOffset));

            migrationBuilder.AddColumn<int>(
                name: "CreatedByID",
                table: "MaterialStageMaster",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "MaterialStageMaster",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "UpdatedByID",
                table: "MaterialStageMaster",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "MaterialStageMaster",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "MaterialStageAudit",
                nullable: false,
                oldClrType: typeof(DateTimeOffset));

            migrationBuilder.AddColumn<int>(
                name: "UpdatedByID",
                table: "MaterialStageAudit",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "MaterialStageAudit",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CastingDate",
                table: "MaterialMaster",
                nullable: false,
                oldClrType: typeof(DateTimeOffset));

            migrationBuilder.AddColumn<int>(
                name: "CreatedByID",
                table: "MaterialMaster",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "MaterialMaster",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "UpdatedByID",
                table: "MaterialMaster",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "MaterialMaster",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedDate",
                table: "MaterialInfoAudit",
                nullable: false,
                oldClrType: typeof(DateTimeOffset));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ExpectedDeliveryDate",
                table: "MaterialInfoAudit",
                nullable: false,
                oldClrType: typeof(DateTimeOffset));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "MaterialInfoAudit",
                nullable: false,
                oldClrType: typeof(DateTimeOffset));

            migrationBuilder.AlterColumn<DateTime>(
                name: "DrawingIssueDate",
                table: "MaterialDrawingAudit",
                nullable: false,
                oldClrType: typeof(DateTimeOffset));

            migrationBuilder.AddColumn<int>(
                name: "CreatedByID",
                table: "LocationMaster",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "LocationMaster",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "UpdatedByID",
                table: "LocationMaster",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "LocationMaster",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedDate",
                table: "InventoryAudit",
                nullable: false,
                oldClrType: typeof(DateTimeOffset));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "InventoryAudit",
                nullable: false,
                oldClrType: typeof(DateTimeOffset));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CastingDate",
                table: "InventoryAudit",
                nullable: false,
                oldClrType: typeof(DateTimeOffset));

            migrationBuilder.AlterColumn<DateTime>(
                name: "SyncTime",
                table: "BIMSyncAudit",
                nullable: false,
                oldClrType: typeof(DateTimeOffset));

            migrationBuilder.CreateIndex(
                name: "IX_TrackerMaster_CreatedByID",
                table: "TrackerMaster",
                column: "CreatedByID");

            migrationBuilder.CreateIndex(
                name: "IX_TrackerMaster_UpdatedByID",
                table: "TrackerMaster",
                column: "UpdatedByID");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialStageMaster_CreatedByID",
                table: "MaterialStageMaster",
                column: "CreatedByID");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialStageMaster_UpdatedByID",
                table: "MaterialStageMaster",
                column: "UpdatedByID");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialStageAudit_UpdatedByID",
                table: "MaterialStageAudit",
                column: "UpdatedByID");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialMaster_CreatedByID",
                table: "MaterialMaster",
                column: "CreatedByID");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialMaster_UpdatedByID",
                table: "MaterialMaster",
                column: "UpdatedByID");

            migrationBuilder.CreateIndex(
                name: "IX_LocationMaster_CreatedByID",
                table: "LocationMaster",
                column: "CreatedByID");

            migrationBuilder.CreateIndex(
                name: "IX_LocationMaster_UpdatedByID",
                table: "LocationMaster",
                column: "UpdatedByID");

            migrationBuilder.AddForeignKey(
                name: "FK_LocationMaster_UserMaster_CreatedByID",
                table: "LocationMaster",
                column: "CreatedByID",
                principalTable: "UserMaster",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LocationMaster_UserMaster_UpdatedByID",
                table: "LocationMaster",
                column: "UpdatedByID",
                principalTable: "UserMaster",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MaterialMaster_UserMaster_CreatedByID",
                table: "MaterialMaster",
                column: "CreatedByID",
                principalTable: "UserMaster",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MaterialMaster_UserMaster_UpdatedByID",
                table: "MaterialMaster",
                column: "UpdatedByID",
                principalTable: "UserMaster",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MaterialStageAudit_UserMaster_UpdatedByID",
                table: "MaterialStageAudit",
                column: "UpdatedByID",
                principalTable: "UserMaster",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MaterialStageMaster_UserMaster_CreatedByID",
                table: "MaterialStageMaster",
                column: "CreatedByID",
                principalTable: "UserMaster",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MaterialStageMaster_UserMaster_UpdatedByID",
                table: "MaterialStageMaster",
                column: "UpdatedByID",
                principalTable: "UserMaster",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TrackerMaster_UserMaster_CreatedByID",
                table: "TrackerMaster",
                column: "CreatedByID",
                principalTable: "UserMaster",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TrackerMaster_UserMaster_UpdatedByID",
                table: "TrackerMaster",
                column: "UpdatedByID",
                principalTable: "UserMaster",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
