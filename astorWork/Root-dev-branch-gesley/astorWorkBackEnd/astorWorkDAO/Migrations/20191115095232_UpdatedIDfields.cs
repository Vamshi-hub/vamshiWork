using Microsoft.EntityFrameworkCore.Migrations;

namespace astorWorkDAO.Migrations
{
    public partial class UpdatedIDfields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MaterialStageAudit_MaterialMaster_MaterialMasterID",
                table: "MaterialStageAudit");

            migrationBuilder.RenameColumn(
                name: "MilestoneId",
                table: "MaterialStageMaster",
                newName: "MilestoneID");

            migrationBuilder.RenameColumn(
                name: "MaterialMasterID",
                table: "MaterialStageAudit",
                newName: "MaterialID");

            migrationBuilder.RenameIndex(
                name: "IX_MaterialStageAudit_MaterialMasterID",
                table: "MaterialStageAudit",
                newName: "IX_MaterialStageAudit_MaterialID");

            migrationBuilder.AddForeignKey(
                name: "FK_MaterialStageAudit_MaterialMaster_MaterialID",
                table: "MaterialStageAudit",
                column: "MaterialID",
                principalTable: "MaterialMaster",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MaterialStageAudit_MaterialMaster_MaterialID",
                table: "MaterialStageAudit");

            migrationBuilder.RenameColumn(
                name: "MilestoneID",
                table: "MaterialStageMaster",
                newName: "MilestoneId");

            migrationBuilder.RenameColumn(
                name: "MaterialID",
                table: "MaterialStageAudit",
                newName: "MaterialMasterID");

            migrationBuilder.RenameIndex(
                name: "IX_MaterialStageAudit_MaterialID",
                table: "MaterialStageAudit",
                newName: "IX_MaterialStageAudit_MaterialMasterID");

            migrationBuilder.AddForeignKey(
                name: "FK_MaterialStageAudit_MaterialMaster_MaterialMasterID",
                table: "MaterialStageAudit",
                column: "MaterialMasterID",
                principalTable: "MaterialMaster",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
