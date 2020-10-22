using Microsoft.EntityFrameworkCore.Migrations;

namespace astorWorkDAO.Migrations
{
    public partial class signeturesColumnModifiedinInspectionAuditTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InspectionAudit_InspectionSignatureAssociation_SignaturesID",
                table: "InspectionAudit");

            migrationBuilder.DropIndex(
                name: "IX_InspectionAudit_SignaturesID",
                table: "InspectionAudit");

            migrationBuilder.DropColumn(
                name: "SignaturesID",
                table: "InspectionAudit");

            migrationBuilder.AddColumn<int>(
                name: "InspectionAuditID",
                table: "InspectionSignatureAssociation",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_InspectionSignatureAssociation_InspectionAuditID",
                table: "InspectionSignatureAssociation",
                column: "InspectionAuditID");

            migrationBuilder.AddForeignKey(
                name: "FK_InspectionSignatureAssociation_InspectionAudit_InspectionAuditID",
                table: "InspectionSignatureAssociation",
                column: "InspectionAuditID",
                principalTable: "InspectionAudit",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InspectionSignatureAssociation_InspectionAudit_InspectionAuditID",
                table: "InspectionSignatureAssociation");

            migrationBuilder.DropIndex(
                name: "IX_InspectionSignatureAssociation_InspectionAuditID",
                table: "InspectionSignatureAssociation");

            migrationBuilder.DropColumn(
                name: "InspectionAuditID",
                table: "InspectionSignatureAssociation");

            migrationBuilder.AddColumn<int>(
                name: "SignaturesID",
                table: "InspectionAudit",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_InspectionAudit_SignaturesID",
                table: "InspectionAudit",
                column: "SignaturesID");

            migrationBuilder.AddForeignKey(
                name: "FK_InspectionAudit_InspectionSignatureAssociation_SignaturesID",
                table: "InspectionAudit",
                column: "SignaturesID",
                principalTable: "InspectionSignatureAssociation",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
