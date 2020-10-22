using Microsoft.EntityFrameworkCore.Migrations;

namespace astorWorkDAO.Migrations
{
    public partial class add_user_notification_association : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserMaster_NotificationAudit_NotificationAuditID",
                table: "UserMaster");

            migrationBuilder.DropIndex(
                name: "IX_UserMaster_NotificationAuditID",
                table: "UserMaster");

            migrationBuilder.DropColumn(
                name: "NotificationAuditID",
                table: "UserMaster");

            migrationBuilder.CreateTable(
                name: "UserNotificationAssociation",
                columns: table => new
                {
                    MyProperty = table.Column<int>(nullable: false),
                    UserID = table.Column<int>(nullable: false),
                    NotificationID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserNotificationAssociation", x => new { x.UserID, x.NotificationID });
                    table.ForeignKey(
                        name: "FK_UserNotificationAssociation_NotificationAudit_NotificationID",
                        column: x => x.NotificationID,
                        principalTable: "NotificationAudit",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserNotificationAssociation_UserMaster_UserID",
                        column: x => x.UserID,
                        principalTable: "UserMaster",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserNotificationAssociation_NotificationID",
                table: "UserNotificationAssociation",
                column: "NotificationID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserNotificationAssociation");

            migrationBuilder.AddColumn<int>(
                name: "NotificationAuditID",
                table: "UserMaster",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserMaster_NotificationAuditID",
                table: "UserMaster",
                column: "NotificationAuditID");

            migrationBuilder.AddForeignKey(
                name: "FK_UserMaster_NotificationAudit_NotificationAuditID",
                table: "UserMaster",
                column: "NotificationAuditID",
                principalTable: "NotificationAudit",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
