using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace astorWorkDAO.Migrations
{
    public partial class AddedLoginClasses : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PageAccessRight_PageMaster_PageID",
                table: "PageAccessRight");

            migrationBuilder.DropColumn(
                name: "URL",
                table: "PageMaster");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "UserMaster",
                newName: "UserName");

            migrationBuilder.RenameColumn(
                name: "PageID",
                table: "PageAccessRight",
                newName: "PageMasterID");

            migrationBuilder.RenameIndex(
                name: "IX_PageAccessRight_PageID",
                table: "PageAccessRight",
                newName: "IX_PageAccessRight_PageMasterID");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "UserMaster",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastLogin",
                table: "UserMaster",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "UserMaster",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PersonName",
                table: "UserMaster",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Salt",
                table: "UserMaster",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DefaultPageID",
                table: "RoleMaster",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "PageMaster",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UrlPath",
                table: "PageMaster",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "UserSessionAudit",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AccessToken = table.Column<string>(nullable: false),
                    CreatedTime = table.Column<DateTime>(nullable: false),
                    ExpireIn = table.Column<int>(nullable: false),
                    UserMasterID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSessionAudit", x => x.ID);
                    table.ForeignKey(
                        name: "FK_UserSessionAudit_UserMaster_UserMasterID",
                        column: x => x.UserMasterID,
                        principalTable: "UserMaster",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RoleMaster_DefaultPageID",
                table: "RoleMaster",
                column: "DefaultPageID");

            migrationBuilder.CreateIndex(
                name: "IX_UserSessionAudit_UserMasterID",
                table: "UserSessionAudit",
                column: "UserMasterID");

            migrationBuilder.AddForeignKey(
                name: "FK_PageAccessRight_PageMaster_PageMasterID",
                table: "PageAccessRight",
                column: "PageMasterID",
                principalTable: "PageMaster",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RoleMaster_PageMaster_DefaultPageID",
                table: "RoleMaster",
                column: "DefaultPageID",
                principalTable: "PageMaster",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PageAccessRight_PageMaster_PageMasterID",
                table: "PageAccessRight");

            migrationBuilder.DropForeignKey(
                name: "FK_RoleMaster_PageMaster_DefaultPageID",
                table: "RoleMaster");

            migrationBuilder.DropTable(
                name: "UserSessionAudit");

            migrationBuilder.DropIndex(
                name: "IX_RoleMaster_DefaultPageID",
                table: "RoleMaster");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "UserMaster");

            migrationBuilder.DropColumn(
                name: "LastLogin",
                table: "UserMaster");

            migrationBuilder.DropColumn(
                name: "Password",
                table: "UserMaster");

            migrationBuilder.DropColumn(
                name: "PersonName",
                table: "UserMaster");

            migrationBuilder.DropColumn(
                name: "Salt",
                table: "UserMaster");

            migrationBuilder.DropColumn(
                name: "DefaultPageID",
                table: "RoleMaster");

            migrationBuilder.DropColumn(
                name: "UrlPath",
                table: "PageMaster");

            migrationBuilder.RenameColumn(
                name: "UserName",
                table: "UserMaster",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "PageMasterID",
                table: "PageAccessRight",
                newName: "PageID");

            migrationBuilder.RenameIndex(
                name: "IX_PageAccessRight_PageMasterID",
                table: "PageAccessRight",
                newName: "IX_PageAccessRight_PageID");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "PageMaster",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AddColumn<string>(
                name: "URL",
                table: "PageMaster",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PageAccessRight_PageMaster_PageID",
                table: "PageAccessRight",
                column: "PageID",
                principalTable: "PageMaster",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
