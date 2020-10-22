using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace astorWorkDAO.Migrations
{
    public partial class addednitificationseenby : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NotificationSeenBy",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    NotificationAuditID = table.Column<int>(nullable: false),
                    UserMasterID = table.Column<int>(nullable: false),
                    SeenDate = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationSeenBy", x => x.ID);
                    table.ForeignKey(
                        name: "FK_NotificationSeenBy_NotificationAudit_NotificationAuditID",
                        column: x => x.NotificationAuditID,
                        principalTable: "NotificationAudit",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NotificationSeenBy_UserMaster_UserMasterID",
                        column: x => x.UserMasterID,
                        principalTable: "UserMaster",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NotificationSeenBy_NotificationAuditID",
                table: "NotificationSeenBy",
                column: "NotificationAuditID");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationSeenBy_UserMasterID",
                table: "NotificationSeenBy",
                column: "UserMasterID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NotificationSeenBy");
        }
    }
}
