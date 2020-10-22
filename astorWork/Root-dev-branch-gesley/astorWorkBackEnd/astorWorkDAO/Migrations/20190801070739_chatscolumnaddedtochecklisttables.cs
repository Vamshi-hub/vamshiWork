using Microsoft.EntityFrameworkCore.Migrations;

namespace astorWorkDAO.Migrations
{
    public partial class chatscolumnaddedtochecklisttables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ChecklistAuditID",
                table: "ChatData",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ChecklistItemAuditID",
                table: "ChatData",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChatData_ChecklistAuditID",
                table: "ChatData",
                column: "ChecklistAuditID");

            migrationBuilder.CreateIndex(
                name: "IX_ChatData_ChecklistItemAuditID",
                table: "ChatData",
                column: "ChecklistItemAuditID");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatData_ChecklistAudit_ChecklistAuditID",
                table: "ChatData",
                column: "ChecklistAuditID",
                principalTable: "ChecklistAudit",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ChatData_ChecklistItemAudit_ChecklistItemAuditID",
                table: "ChatData",
                column: "ChecklistItemAuditID",
                principalTable: "ChecklistItemAudit",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatData_ChecklistAudit_ChecklistAuditID",
                table: "ChatData");

            migrationBuilder.DropForeignKey(
                name: "FK_ChatData_ChecklistItemAudit_ChecklistItemAuditID",
                table: "ChatData");

            migrationBuilder.DropIndex(
                name: "IX_ChatData_ChecklistAuditID",
                table: "ChatData");

            migrationBuilder.DropIndex(
                name: "IX_ChatData_ChecklistItemAuditID",
                table: "ChatData");

            migrationBuilder.DropColumn(
                name: "ChecklistAuditID",
                table: "ChatData");

            migrationBuilder.DropColumn(
                name: "ChecklistItemAuditID",
                table: "ChatData");
        }
    }
}
