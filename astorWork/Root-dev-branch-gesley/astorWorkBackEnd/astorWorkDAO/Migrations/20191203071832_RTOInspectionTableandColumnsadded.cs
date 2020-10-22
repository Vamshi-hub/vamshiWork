using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace astorWorkDAO.Migrations
{
    public partial class RTOInspectionTableandColumnsadded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RTOInspection",
                table: "TradeMaster",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "TradeMaster",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "ChecklistMaster",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "InspectionInfoID",
                table: "ChecklistAudit",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "InspectionSignatureAssociation",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ChecklistAuditID = table.Column<int>(nullable: true),
                    UserID = table.Column<int>(nullable: true),
                    Signature = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InspectionSignatureAssociation", x => x.ID);
                    table.ForeignKey(
                        name: "FK_InspectionSignatureAssociation_ChecklistAudit_ChecklistAuditID",
                        column: x => x.ChecklistAuditID,
                        principalTable: "ChecklistAudit",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InspectionSignatureAssociation_UserMaster_UserID",
                        column: x => x.UserID,
                        principalTable: "UserMaster",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InspectionAudit",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RoomType = table.Column<string>(nullable: true),
                    ReferenceNo = table.Column<string>(nullable: true),
                    InspectionDate = table.Column<DateTime>(nullable: true),
                    InspectionTime = table.Column<TimeSpan>(nullable: true),
                    SignaturesID = table.Column<int>(nullable: true),
                    Remarks = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InspectionAudit", x => x.ID);
                    table.ForeignKey(
                        name: "FK_InspectionAudit_InspectionSignatureAssociation_SignaturesID",
                        column: x => x.SignaturesID,
                        principalTable: "InspectionSignatureAssociation",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistAudit_InspectionInfoID",
                table: "ChecklistAudit",
                column: "InspectionInfoID");

            migrationBuilder.CreateIndex(
                name: "IX_InspectionAudit_SignaturesID",
                table: "InspectionAudit",
                column: "SignaturesID");

            migrationBuilder.CreateIndex(
                name: "IX_InspectionSignatureAssociation_ChecklistAuditID",
                table: "InspectionSignatureAssociation",
                column: "ChecklistAuditID");

            migrationBuilder.CreateIndex(
                name: "IX_InspectionSignatureAssociation_UserID",
                table: "InspectionSignatureAssociation",
                column: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_ChecklistAudit_InspectionAudit_InspectionInfoID",
                table: "ChecklistAudit",
                column: "InspectionInfoID",
                principalTable: "InspectionAudit",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChecklistAudit_InspectionAudit_InspectionInfoID",
                table: "ChecklistAudit");

            migrationBuilder.DropTable(
                name: "InspectionAudit");

            migrationBuilder.DropTable(
                name: "InspectionSignatureAssociation");

            migrationBuilder.DropIndex(
                name: "IX_ChecklistAudit_InspectionInfoID",
                table: "ChecklistAudit");

            migrationBuilder.DropColumn(
                name: "RTOInspection",
                table: "TradeMaster");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "TradeMaster");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "ChecklistMaster");

            migrationBuilder.DropColumn(
                name: "InspectionInfoID",
                table: "ChecklistAudit");
        }
    }
}
