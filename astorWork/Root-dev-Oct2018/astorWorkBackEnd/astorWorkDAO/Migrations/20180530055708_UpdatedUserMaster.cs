using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace astorWorkDAO.Migrations
{
    public partial class UpdatedUserMaster : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserMaster_VendorMaster_VendorMasterID",
                table: "UserMaster");

            migrationBuilder.RenameColumn(
                name: "VendorMasterID",
                table: "UserMaster",
                newName: "VendorID");

            migrationBuilder.RenameIndex(
                name: "IX_UserMaster_VendorMasterID",
                table: "UserMaster",
                newName: "IX_UserMaster_VendorID");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "UserMaster",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_UserMaster_VendorMaster_VendorID",
                table: "UserMaster",
                column: "VendorID",
                principalTable: "VendorMaster",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserMaster_VendorMaster_VendorID",
                table: "UserMaster");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "UserMaster");

            migrationBuilder.RenameColumn(
                name: "VendorID",
                table: "UserMaster",
                newName: "VendorMasterID");

            migrationBuilder.RenameIndex(
                name: "IX_UserMaster_VendorID",
                table: "UserMaster",
                newName: "IX_UserMaster_VendorMasterID");

            migrationBuilder.AddForeignKey(
                name: "FK_UserMaster_VendorMaster_VendorMasterID",
                table: "UserMaster",
                column: "VendorMasterID",
                principalTable: "VendorMaster",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
