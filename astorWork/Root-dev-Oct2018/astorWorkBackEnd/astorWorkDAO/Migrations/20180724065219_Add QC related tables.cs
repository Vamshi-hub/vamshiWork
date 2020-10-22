using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace astorWorkDAO.Migrations
{
    public partial class AddQCrelatedtables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MaterialQCPhotos_MaterialStageAudit_MaterialStageAuditID",
                table: "MaterialQCPhotos");

            migrationBuilder.DropForeignKey(
                name: "FK_UserMaster_RoleMaster_RoleID",
                table: "UserMaster");

            migrationBuilder.DropIndex(
                name: "IX_MaterialQCPhotos_MaterialStageAuditID",
                table: "MaterialQCPhotos");

            migrationBuilder.DropColumn(
                name: "MaterialStageAuditID",
                table: "MaterialQCPhotos");

            migrationBuilder.AddColumn<int>(
                name: "MaterialQCDefectID",
                table: "MaterialQCPhotos",
                nullable: true);

            migrationBuilder.CreateIndex(
                "IX_MaterialQCPhotos_MaterialQCDefectID",
                "MaterialQCPhotos",
                "MaterialQCDefectID");
            /*
            migrationBuilder.RenameColumn(
                name: "MaterialStageAuditID",
                table: "MaterialQCPhotos",
                newName: "MaterialQCDefectID");

            migrationBuilder.RenameIndex(
                name: "IX_MaterialQCPhotos_MaterialStageAuditID",
                table: "MaterialQCPhotos",
                newName: "IX_MaterialQCPhotos_MaterialQCDefectID");
                */

            migrationBuilder.AlterColumn<int>(
                name: "RoleID",
                table: "UserMaster",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CreatedByID",
                table: "MaterialQCPhotos",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedDate",
                table: "MaterialQCPhotos",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.CreateTable(
                name: "MaterialQCCase",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CaseName = table.Column<string>(nullable: true),
                    CreatedByID = table.Column<int>(nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(nullable: false),
                    StageAuditId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialQCCase", x => x.ID);
                    table.ForeignKey(
                        name: "FK_MaterialQCCase_UserMaster_CreatedByID",
                        column: x => x.CreatedByID,
                        principalTable: "UserMaster",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MaterialQCCase_MaterialStageAudit_StageAuditId",
                        column: x => x.StageAuditId,
                        principalTable: "MaterialStageAudit",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MaterialQCDefect",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsOpen = table.Column<bool>(nullable: false),
                    CreatedByID = table.Column<int>(nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(nullable: false),
                    QCCaseId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialQCDefect", x => x.ID);
                    table.ForeignKey(
                        name: "FK_MaterialQCDefect_UserMaster_CreatedByID",
                        column: x => x.CreatedByID,
                        principalTable: "UserMaster",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MaterialQCDefect_MaterialQCCase_QCCaseId",
                        column: x => x.QCCaseId,
                        principalTable: "MaterialQCCase",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MaterialQCPhotos_CreatedByID",
                table: "MaterialQCPhotos",
                column: "CreatedByID");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialQCCase_CreatedByID",
                table: "MaterialQCCase",
                column: "CreatedByID");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialQCCase_StageAuditId",
                table: "MaterialQCCase",
                column: "StageAuditId");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialQCDefect_CreatedByID",
                table: "MaterialQCDefect",
                column: "CreatedByID");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialQCDefect_QCCaseId",
                table: "MaterialQCDefect",
                column: "QCCaseId");

            migrationBuilder.AddForeignKey(
                name: "FK_MaterialQCPhotos_UserMaster_CreatedByID",
                table: "MaterialQCPhotos",
                column: "CreatedByID",
                principalTable: "UserMaster",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MaterialQCPhotos_MaterialQCDefect_MaterialQCDefectID",
                table: "MaterialQCPhotos",
                column: "MaterialQCDefectID",
                principalTable: "MaterialQCDefect",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserMaster_RoleMaster_RoleID",
                table: "UserMaster",
                column: "RoleID",
                principalTable: "RoleMaster",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MaterialQCPhotos_UserMaster_CreatedByID",
                table: "MaterialQCPhotos");

            migrationBuilder.DropForeignKey(
                name: "FK_MaterialQCPhotos_MaterialQCDefect_MaterialQCDefectID",
                table: "MaterialQCPhotos");

            migrationBuilder.DropForeignKey(
                name: "FK_UserMaster_RoleMaster_RoleID",
                table: "UserMaster");

            migrationBuilder.DropTable(
                name: "MaterialQCDefect");

            migrationBuilder.DropTable(
                name: "MaterialQCCase");

            migrationBuilder.DropIndex(
                name: "IX_MaterialQCPhotos_CreatedByID",
                table: "MaterialQCPhotos");

            migrationBuilder.DropColumn(
                name: "CreatedByID",
                table: "MaterialQCPhotos");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "MaterialQCPhotos");

            migrationBuilder.RenameColumn(
                name: "MaterialQCDefectID",
                table: "MaterialQCPhotos",
                newName: "MaterialStageAuditID");

            migrationBuilder.RenameIndex(
                name: "IX_MaterialQCPhotos_MaterialQCDefectID",
                table: "MaterialQCPhotos",
                newName: "IX_MaterialQCPhotos_MaterialStageAuditID");

            migrationBuilder.AlterColumn<int>(
                name: "RoleID",
                table: "UserMaster",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_MaterialQCPhotos_MaterialStageAudit_MaterialStageAuditID",
                table: "MaterialQCPhotos",
                column: "MaterialStageAuditID",
                principalTable: "MaterialStageAudit",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserMaster_RoleMaster_RoleID",
                table: "UserMaster",
                column: "RoleID",
                principalTable: "RoleMaster",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
