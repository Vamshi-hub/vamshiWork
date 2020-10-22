using Microsoft.EntityFrameworkCore.Migrations;

namespace astorWorkDAO.Migrations
{
    public partial class add_project_for_user : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectMaster_UserMaster_ProjectManagerID",
                table: "ProjectMaster");

            migrationBuilder.DropIndex(
                name: "IX_ProjectMaster_ProjectManagerID",
                table: "ProjectMaster");

            migrationBuilder.DropColumn(
                name: "ProjectManagerID",
                table: "ProjectMaster");

            migrationBuilder.AddColumn<int>(
                name: "ProjectID",
                table: "UserMaster",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserMaster_ProjectID",
                table: "UserMaster",
                column: "ProjectID");

            migrationBuilder.AddForeignKey(
                name: "FK_UserMaster_ProjectMaster_ProjectID",
                table: "UserMaster",
                column: "ProjectID",
                principalTable: "ProjectMaster",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserMaster_ProjectMaster_ProjectID",
                table: "UserMaster");

            migrationBuilder.DropIndex(
                name: "IX_UserMaster_ProjectID",
                table: "UserMaster");

            migrationBuilder.DropColumn(
                name: "ProjectID",
                table: "UserMaster");

            migrationBuilder.AddColumn<int>(
                name: "ProjectManagerID",
                table: "ProjectMaster",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectMaster_ProjectManagerID",
                table: "ProjectMaster",
                column: "ProjectManagerID");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectMaster_UserMaster_ProjectManagerID",
                table: "ProjectMaster",
                column: "ProjectManagerID",
                principalTable: "UserMaster",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
