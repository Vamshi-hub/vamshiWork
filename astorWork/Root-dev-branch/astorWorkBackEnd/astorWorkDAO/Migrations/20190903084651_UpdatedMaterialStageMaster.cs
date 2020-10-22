using Microsoft.EntityFrameworkCore.Migrations;

namespace astorWorkDAO.Migrations
{
    public partial class UpdatedMaterialStageMaster : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Attachment",
                table: "ChatData",
                newName: "ThumbnailUrl");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "MaterialStageMaster",
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 30);

            migrationBuilder.AddColumn<string>(
                name: "OriginalPhotoUrl",
                table: "ChatData",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OriginalPhotoUrl",
                table: "ChatData");

            migrationBuilder.RenameColumn(
                name: "ThumbnailUrl",
                table: "ChatData",
                newName: "Attachment");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "MaterialStageMaster",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(string));
        }
    }
}
