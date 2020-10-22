using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace astorWorkDAO.Migrations
{
    public partial class Add_unique_constraints : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "VendorMaster",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "UserName",
                table: "UserMaster",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "RoleMaster",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ProjectMaster",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "EstimatedStartDate",
                table: "ProjectMaster",
                nullable: true,
                oldClrType: typeof(DateTimeOffset));

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "EstimatedEndDate",
                table: "ProjectMaster",
                nullable: true,
                oldClrType: typeof(DateTimeOffset));

            migrationBuilder.AlterColumn<string>(
                name: "UrlPrefix",
                table: "ModuleMaster",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "MaterialStageMaster",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "LocationMaster",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.CreateIndex(
                name: "IX_VendorMaster_Name",
                table: "VendorMaster",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserMaster_UserName",
                table: "UserMaster",
                column: "UserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RoleMaster_Name",
                table: "RoleMaster",
                column: "Name",
                unique: true,
                filter: "[Name] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectMaster_Name",
                table: "ProjectMaster",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ModuleMaster_UrlPrefix",
                table: "ModuleMaster",
                column: "UrlPrefix",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MaterialStageMaster_Name",
                table: "MaterialStageMaster",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LocationMaster_Name",
                table: "LocationMaster",
                column: "Name",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_VendorMaster_Name",
                table: "VendorMaster");

            migrationBuilder.DropIndex(
                name: "IX_UserMaster_UserName",
                table: "UserMaster");

            migrationBuilder.DropIndex(
                name: "IX_RoleMaster_Name",
                table: "RoleMaster");

            migrationBuilder.DropIndex(
                name: "IX_ProjectMaster_Name",
                table: "ProjectMaster");

            migrationBuilder.DropIndex(
                name: "IX_ModuleMaster_UrlPrefix",
                table: "ModuleMaster");

            migrationBuilder.DropIndex(
                name: "IX_MaterialStageMaster_Name",
                table: "MaterialStageMaster");

            migrationBuilder.DropIndex(
                name: "IX_LocationMaster_Name",
                table: "LocationMaster");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "VendorMaster",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "UserName",
                table: "UserMaster",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "RoleMaster",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ProjectMaster",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "EstimatedStartDate",
                table: "ProjectMaster",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "EstimatedEndDate",
                table: "ProjectMaster",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UrlPrefix",
                table: "ModuleMaster",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "MaterialStageMaster",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "LocationMaster",
                nullable: false,
                oldClrType: typeof(string));
        }
    }
}
