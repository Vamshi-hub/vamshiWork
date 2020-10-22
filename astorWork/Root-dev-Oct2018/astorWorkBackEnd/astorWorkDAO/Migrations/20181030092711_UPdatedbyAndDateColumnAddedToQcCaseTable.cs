using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace astorWorkDAO.Migrations
{
    public partial class UPdatedbyAndDateColumnAddedToQcCaseTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UpdatedById",
                table: "MaterialQCCase",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedDate",
                table: "MaterialQCCase",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MaterialQCCase_UpdatedById",
                table: "MaterialQCCase",
                column: "UpdatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_MaterialQCCase_UserMaster_UpdatedById",
                table: "MaterialQCCase",
                column: "UpdatedById",
                principalTable: "UserMaster",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MaterialQCCase_UserMaster_UpdatedById",
                table: "MaterialQCCase");

            migrationBuilder.DropIndex(
                name: "IX_MaterialQCCase_UpdatedById",
                table: "MaterialQCCase");

            migrationBuilder.DropColumn(
                name: "UpdatedById",
                table: "MaterialQCCase");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "MaterialQCCase");
        }
    }
}
