using Microsoft.EntityFrameworkCore.Migrations;

namespace astorWorkDAO.Migrations
{
    public partial class UpdatedRoleMgtTbls : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ParentModuleID",
                table: "ModuleMaster",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ModuleMaster_ParentModuleID",
                table: "ModuleMaster",
                column: "ParentModuleID");

            migrationBuilder.AddForeignKey(
                name: "FK_ModuleMaster_ModuleMaster_ParentModuleID",
                table: "ModuleMaster",
                column: "ParentModuleID",
                principalTable: "ModuleMaster",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ModuleMaster_ModuleMaster_ParentModuleID",
                table: "ModuleMaster");

            migrationBuilder.DropIndex(
                name: "IX_ModuleMaster_ParentModuleID",
                table: "ModuleMaster");

            migrationBuilder.DropColumn(
                name: "ParentModuleID",
                table: "ModuleMaster");
        }
    }
}
