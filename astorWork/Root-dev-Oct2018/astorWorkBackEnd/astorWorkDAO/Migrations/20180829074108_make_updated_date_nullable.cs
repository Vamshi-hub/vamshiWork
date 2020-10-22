using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace astorWorkDAO.Migrations
{
    public partial class make_updated_date_nullable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Tag",
                table: "TrackerMaster",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedDate",
                table: "MaterialQCDefect",
                nullable: true,
                oldClrType: typeof(DateTimeOffset));

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedDate",
                table: "AttachmentMaster",
                nullable: true,
                oldClrType: typeof(DateTimeOffset));

            migrationBuilder.CreateIndex(
                name: "IX_TrackerMaster_Tag",
                table: "TrackerMaster",
                column: "Tag",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TrackerMaster_Tag",
                table: "TrackerMaster");

            migrationBuilder.AlterColumn<string>(
                name: "Tag",
                table: "TrackerMaster",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedDate",
                table: "MaterialQCDefect",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedDate",
                table: "AttachmentMaster",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldNullable: true);
        }
    }
}
