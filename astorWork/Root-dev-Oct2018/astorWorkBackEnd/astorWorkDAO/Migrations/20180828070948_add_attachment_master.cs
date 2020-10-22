using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace astorWorkDAO.Migrations
{
    public partial class add_attachment_master : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "MaterialStageMaster",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AddColumn<int>(
                name: "UpdatedByID",
                table: "MaterialQCDefect",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedDate",
                table: "MaterialQCDefect",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.CreateTable(
                name: "AttachmentMaster",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    URL = table.Column<string>(nullable: true),
                    FileName = table.Column<string>(nullable: true),
                    FileSize = table.Column<int>(nullable: false),
                    CreatedByID = table.Column<int>(nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(nullable: false),
                    UpdatedByID = table.Column<int>(nullable: true),
                    UpdatedDate = table.Column<DateTimeOffset>(nullable: false),
                    Type = table.Column<int>(nullable: false),
                    Reference = table.Column<string>(nullable: true),
                    Remarks = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttachmentMaster", x => x.ID);
                    table.ForeignKey(
                        name: "FK_AttachmentMaster_UserMaster_CreatedByID",
                        column: x => x.CreatedByID,
                        principalTable: "UserMaster",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AttachmentMaster_UserMaster_UpdatedByID",
                        column: x => x.UpdatedByID,
                        principalTable: "UserMaster",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MaterialQCDefect_UpdatedByID",
                table: "MaterialQCDefect",
                column: "UpdatedByID");

            migrationBuilder.CreateIndex(
                name: "IX_AttachmentMaster_CreatedByID",
                table: "AttachmentMaster",
                column: "CreatedByID");

            migrationBuilder.CreateIndex(
                name: "IX_AttachmentMaster_UpdatedByID",
                table: "AttachmentMaster",
                column: "UpdatedByID");

            migrationBuilder.AddForeignKey(
                name: "FK_MaterialQCDefect_UserMaster_UpdatedByID",
                table: "MaterialQCDefect",
                column: "UpdatedByID",
                principalTable: "UserMaster",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MaterialQCDefect_UserMaster_UpdatedByID",
                table: "MaterialQCDefect");

            migrationBuilder.DropTable(
                name: "AttachmentMaster");

            migrationBuilder.DropIndex(
                name: "IX_MaterialQCDefect_UpdatedByID",
                table: "MaterialQCDefect");

            migrationBuilder.DropColumn(
                name: "UpdatedByID",
                table: "MaterialQCDefect");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "MaterialQCDefect");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "MaterialStageMaster",
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 30);
        }
    }
}
