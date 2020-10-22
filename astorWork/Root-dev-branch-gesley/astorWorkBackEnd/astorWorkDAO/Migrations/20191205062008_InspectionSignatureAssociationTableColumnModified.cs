using Microsoft.EntityFrameworkCore.Migrations;

namespace astorWorkDAO.Migrations
{
    public partial class InspectionSignatureAssociationTableColumnModified : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InspectionSignatureAssociation_ChecklistAudit_ChecklistAuditID",
                table: "InspectionSignatureAssociation");

            migrationBuilder.DropIndex(
                name: "IX_InspectionSignatureAssociation_ChecklistAuditID",
                table: "InspectionSignatureAssociation");

            migrationBuilder.DropColumn(
                name: "ChecklistAuditID",
                table: "InspectionSignatureAssociation");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ChecklistAuditID",
                table: "InspectionSignatureAssociation",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_InspectionSignatureAssociation_ChecklistAuditID",
                table: "InspectionSignatureAssociation",
                column: "ChecklistAuditID");

            migrationBuilder.AddForeignKey(
                name: "FK_InspectionSignatureAssociation_ChecklistAudit_ChecklistAuditID",
                table: "InspectionSignatureAssociation",
                column: "ChecklistAuditID",
                principalTable: "ChecklistAudit",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
