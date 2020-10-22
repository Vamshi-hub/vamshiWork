using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace astorWorkDAO.Migrations
{
    public partial class ConfigurationMasterTableAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ConfigurationMaster",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Cofiguration = table.Column<string>(nullable: true),
                    Setting = table.Column<string>(nullable: true),
                    LastUpdatedByID = table.Column<int>(nullable: false),
                    LastUpdatedDate = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfigurationMaster", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ConfigurationMaster_UserMaster_LastUpdatedByID",
                        column: x => x.LastUpdatedByID,
                        principalTable: "UserMaster",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ConfigurationMaster_LastUpdatedByID",
                table: "ConfigurationMaster",
                column: "LastUpdatedByID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConfigurationMaster");
        }
    }
}
