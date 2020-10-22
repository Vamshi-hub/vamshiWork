using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace astorWorkDAO.Migrations
{
    public partial class TimestampcolumnTypemodifiedinChatData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "TimeStamp",
                table: "ChatData",
                nullable: false,
                oldClrType: typeof(DateTime));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "TimeStamp",
                table: "ChatData",
                nullable: false,
                oldClrType: typeof(DateTimeOffset));
        }
    }
}
