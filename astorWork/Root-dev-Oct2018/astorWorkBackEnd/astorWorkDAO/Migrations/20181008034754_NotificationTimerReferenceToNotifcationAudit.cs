using Microsoft.EntityFrameworkCore.Migrations;

namespace astorWorkDAO.Migrations
{
    public partial class NotificationTimerReferenceToNotifcationAudit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NotificationTimerID",
                table: "NotificationAudit",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_NotificationAudit_NotificationTimerID",
                table: "NotificationAudit",
                column: "NotificationTimerID");

            migrationBuilder.AddForeignKey(
                name: "FK_NotificationAudit_NotificationTimerMaster_NotificationTimerID",
                table: "NotificationAudit",
                column: "NotificationTimerID",
                principalTable: "NotificationTimerMaster",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NotificationAudit_NotificationTimerMaster_NotificationTimerID",
                table: "NotificationAudit");

            migrationBuilder.DropIndex(
                name: "IX_NotificationAudit_NotificationTimerID",
                table: "NotificationAudit");

            migrationBuilder.DropColumn(
                name: "NotificationTimerID",
                table: "NotificationAudit");
        }
    }
}
