using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace astorWorkDAO.Migrations
{
    public partial class AddRolePageAssociation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PageAccessRight");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "RoleMaster");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "RoleMaster");

            migrationBuilder.RenameColumn(
                name: "RoleTypeCode",
                table: "RoleMaster",
                newName: "PlatformCode");

            migrationBuilder.AddColumn<int>(
                name: "MobileEntryPoint",
                table: "RoleMaster",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ModuleMasterID",
                table: "PageMaster",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ModuleMaster",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false),
                    UrlPrefix = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModuleMaster", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "RolePageAssociation",
                columns: table => new
                {
                    RoleId = table.Column<int>(nullable: false),
                    PageId = table.Column<int>(nullable: false),
                    AccessLevel = table.Column<int>(nullable: false),
                    PageMasterID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePageAssociation", x => new { x.RoleId, x.PageId });
                    table.ForeignKey(
                        name: "FK_RolePageAssociation_PageMaster_PageId",
                        column: x => x.PageId,
                        principalTable: "PageMaster",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolePageAssociation_PageMaster_PageMasterID",
                        column: x => x.PageMasterID,
                        principalTable: "PageMaster",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RolePageAssociation_RoleMaster_RoleId",
                        column: x => x.RoleId,
                        principalTable: "RoleMaster",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PageMaster_ModuleMasterID",
                table: "PageMaster",
                column: "ModuleMasterID");

            migrationBuilder.CreateIndex(
                name: "IX_RolePageAssociation_PageId",
                table: "RolePageAssociation",
                column: "PageId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePageAssociation_PageMasterID",
                table: "RolePageAssociation",
                column: "PageMasterID");

            migrationBuilder.AddForeignKey(
                name: "FK_PageMaster_ModuleMaster_ModuleMasterID",
                table: "PageMaster",
                column: "ModuleMasterID",
                principalTable: "ModuleMaster",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PageMaster_ModuleMaster_ModuleMasterID",
                table: "PageMaster");

            migrationBuilder.DropTable(
                name: "ModuleMaster");

            migrationBuilder.DropTable(
                name: "RolePageAssociation");

            migrationBuilder.DropIndex(
                name: "IX_PageMaster_ModuleMasterID",
                table: "PageMaster");

            migrationBuilder.DropColumn(
                name: "MobileEntryPoint",
                table: "RoleMaster");

            migrationBuilder.DropColumn(
                name: "ModuleMasterID",
                table: "PageMaster");

            migrationBuilder.RenameColumn(
                name: "PlatformCode",
                table: "RoleMaster",
                newName: "RoleTypeCode");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "RoleMaster",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "RoleMaster",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "PageAccessRight",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AccessLevel = table.Column<int>(nullable: false),
                    PageMasterID = table.Column<int>(nullable: true),
                    RoleMasterID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PageAccessRight", x => x.ID);
                    table.ForeignKey(
                        name: "FK_PageAccessRight_PageMaster_PageMasterID",
                        column: x => x.PageMasterID,
                        principalTable: "PageMaster",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PageAccessRight_RoleMaster_RoleMasterID",
                        column: x => x.RoleMasterID,
                        principalTable: "RoleMaster",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PageAccessRight_PageMasterID",
                table: "PageAccessRight",
                column: "PageMasterID");

            migrationBuilder.CreateIndex(
                name: "IX_PageAccessRight_RoleMasterID",
                table: "PageAccessRight",
                column: "RoleMasterID");
        }
    }
}
