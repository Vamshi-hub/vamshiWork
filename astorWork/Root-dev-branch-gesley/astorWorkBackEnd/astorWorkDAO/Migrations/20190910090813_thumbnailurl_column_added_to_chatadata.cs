using Microsoft.EntityFrameworkCore.Migrations;

namespace astorWorkDAO.Migrations
{
    public partial class thumbnailurl_column_added_to_chatadata : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OriginalPhotoUrl",
                table: "ChatData",
                newName: "OriginalAttachmentUrl");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OriginalAttachmentUrl",
                table: "ChatData",
                newName: "OriginalPhotoUrl");
        }
    }
}
