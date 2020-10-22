using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace astorWorkDAO.Migrations
{
    public partial class add_missing_notification_table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MaterialMaster_ProjectMaster_ProjectID",
                table: "MaterialMaster");

            migrationBuilder.DropForeignKey(
                name: "FK_MaterialMaster_VendorMaster_VendorID",
                table: "MaterialMaster");

            migrationBuilder.RenameColumn(
                name: "VendorID",
                table: "MaterialMaster",
                newName: "VendorId");

            migrationBuilder.RenameColumn(
                name: "ProjectID",
                table: "MaterialMaster",
                newName: "ProjectId");

            migrationBuilder.RenameIndex(
                name: "IX_MaterialMaster_VendorID",
                table: "MaterialMaster",
                newName: "IX_MaterialMaster_VendorId");

            migrationBuilder.RenameIndex(
                name: "IX_MaterialMaster_ProjectID",
                table: "MaterialMaster",
                newName: "IX_MaterialMaster_ProjectId");

            migrationBuilder.AddColumn<int>(
                name: "NotificationAuditID",
                table: "UserMaster",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "VendorId",
                table: "MaterialMaster",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "NotificationAudit",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Type = table.Column<int>(nullable: false),
                    Code = table.Column<int>(nullable: false),
                    Reference = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(nullable: false),
                    ProcessedDate = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationAudit", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "SystemHealthMaster",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Type = table.Column<int>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    Message = table.Column<string>(nullable: true),
                    Reference = table.Column<string>(nullable: true),
                    LastUpdated = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemHealthMaster", x => x.ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserMaster_NotificationAuditID",
                table: "UserMaster",
                column: "NotificationAuditID");

            migrationBuilder.AddForeignKey(
                name: "FK_MaterialMaster_ProjectMaster_ProjectId",
                table: "MaterialMaster",
                column: "ProjectId",
                principalTable: "ProjectMaster",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MaterialMaster_VendorMaster_VendorId",
                table: "MaterialMaster",
                column: "VendorId",
                principalTable: "VendorMaster",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserMaster_NotificationAudit_NotificationAuditID",
                table: "UserMaster",
                column: "NotificationAuditID",
                principalTable: "NotificationAudit",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MaterialMaster_ProjectMaster_ProjectId",
                table: "MaterialMaster");

            migrationBuilder.DropForeignKey(
                name: "FK_MaterialMaster_VendorMaster_VendorId",
                table: "MaterialMaster");

            migrationBuilder.DropForeignKey(
                name: "FK_UserMaster_NotificationAudit_NotificationAuditID",
                table: "UserMaster");

            migrationBuilder.DropTable(
                name: "NotificationAudit");

            migrationBuilder.DropTable(
                name: "SystemHealthMaster");

            migrationBuilder.DropIndex(
                name: "IX_UserMaster_NotificationAuditID",
                table: "UserMaster");

            migrationBuilder.DropColumn(
                name: "NotificationAuditID",
                table: "UserMaster");

            migrationBuilder.RenameColumn(
                name: "VendorId",
                table: "MaterialMaster",
                newName: "VendorID");

            migrationBuilder.RenameColumn(
                name: "ProjectId",
                table: "MaterialMaster",
                newName: "ProjectID");

            migrationBuilder.RenameIndex(
                name: "IX_MaterialMaster_VendorId",
                table: "MaterialMaster",
                newName: "IX_MaterialMaster_VendorID");

            migrationBuilder.RenameIndex(
                name: "IX_MaterialMaster_ProjectId",
                table: "MaterialMaster",
                newName: "IX_MaterialMaster_ProjectID");

            migrationBuilder.AlterColumn<int>(
                name: "VendorID",
                table: "MaterialMaster",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_MaterialMaster_ProjectMaster_ProjectID",
                table: "MaterialMaster",
                column: "ProjectID",
                principalTable: "ProjectMaster",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MaterialMaster_VendorMaster_VendorID",
                table: "MaterialMaster",
                column: "VendorID",
                principalTable: "VendorMaster",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
