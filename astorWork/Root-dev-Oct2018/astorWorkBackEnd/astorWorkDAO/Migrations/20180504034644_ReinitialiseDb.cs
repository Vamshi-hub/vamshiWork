using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace astorWorkDAO.Migrations
{
    public partial class ReinitialiseDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MaterialDrawingAudit",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DrawingIssueDate = table.Column<DateTime>(nullable: false),
                    DrawingNo = table.Column<string>(nullable: true),
                    RevisionNo = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialDrawingAudit", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "PageMaster",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    URL = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PageMaster", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "RoleMaster",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    RoleTypeCode = table.Column<string>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleMaster", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "VendorMaster",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CycleDays = table.Column<int>(nullable: false, defaultValue: 7),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VendorMaster", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "PageAccessRight",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AccessLevel = table.Column<int>(nullable: false),
                    PageID = table.Column<int>(nullable: true),
                    RoleMasterID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PageAccessRight", x => x.ID);
                    table.ForeignKey(
                        name: "FK_PageAccessRight_PageMaster_PageID",
                        column: x => x.PageID,
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

            migrationBuilder.CreateTable(
                name: "UserMaster",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    RoleID = table.Column<int>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    VendorMasterID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserMaster", x => x.ID);
                    table.ForeignKey(
                        name: "FK_UserMaster_RoleMaster_RoleID",
                        column: x => x.RoleID,
                        principalTable: "RoleMaster",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserMaster_VendorMaster_VendorMasterID",
                        column: x => x.VendorMasterID,
                        principalTable: "VendorMaster",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LocationMaster",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedByID = table.Column<int>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    UpdatedByID = table.Column<int>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocationMaster", x => x.ID);
                    table.ForeignKey(
                        name: "FK_LocationMaster_UserMaster_CreatedByID",
                        column: x => x.CreatedByID,
                        principalTable: "UserMaster",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LocationMaster_UserMaster_UpdatedByID",
                        column: x => x.UpdatedByID,
                        principalTable: "UserMaster",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MaterialStageMaster",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Colour = table.Column<string>(nullable: false),
                    CreatedByID = table.Column<int>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    IsQCStage = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Order = table.Column<int>(nullable: false),
                    UpdatedByID = table.Column<int>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialStageMaster", x => x.ID);
                    table.ForeignKey(
                        name: "FK_MaterialStageMaster_UserMaster_CreatedByID",
                        column: x => x.CreatedByID,
                        principalTable: "UserMaster",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MaterialStageMaster_UserMaster_UpdatedByID",
                        column: x => x.UpdatedByID,
                        principalTable: "UserMaster",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MRFMaster",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedByID = table.Column<int>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ExpectedDeliveryDate = table.Column<DateTime>(nullable: false),
                    MRFNo = table.Column<string>(nullable: true),
                    OrderDate = table.Column<DateTime>(nullable: false),
                    PlannedCastingDate = table.Column<DateTime>(nullable: false),
                    Remarks = table.Column<string>(nullable: true),
                    UpdatedByID = table.Column<int>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MRFMaster", x => x.ID);
                    table.ForeignKey(
                        name: "FK_MRFMaster_UserMaster_CreatedByID",
                        column: x => x.CreatedByID,
                        principalTable: "UserMaster",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MRFMaster_UserMaster_UpdatedByID",
                        column: x => x.UpdatedByID,
                        principalTable: "UserMaster",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProjectMaster",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedByID = table.Column<int>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    UpdatedByID = table.Column<int>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectMaster", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ProjectMaster_UserMaster_CreatedByID",
                        column: x => x.CreatedByID,
                        principalTable: "UserMaster",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjectMaster_UserMaster_UpdatedByID",
                        column: x => x.UpdatedByID,
                        principalTable: "UserMaster",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TrackerMaster",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedByID = table.Column<int>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    Label = table.Column<string>(nullable: true),
                    Tag = table.Column<string>(nullable: false),
                    Type = table.Column<string>(nullable: false),
                    UpdatedByID = table.Column<int>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrackerMaster", x => x.ID);
                    table.ForeignKey(
                        name: "FK_TrackerMaster_UserMaster_CreatedByID",
                        column: x => x.CreatedByID,
                        principalTable: "UserMaster",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TrackerMaster_UserMaster_UpdatedByID",
                        column: x => x.UpdatedByID,
                        principalTable: "UserMaster",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserMRFAssociation",
                columns: table => new
                {
                    UserID = table.Column<int>(nullable: false),
                    MRFID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserMRFAssociation", x => new { x.UserID, x.MRFID });
                    table.ForeignKey(
                        name: "FK_UserMRFAssociation_MRFMaster_MRFID",
                        column: x => x.MRFID,
                        principalTable: "MRFMaster",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserMRFAssociation_UserMaster_UserID",
                        column: x => x.UserID,
                        principalTable: "UserMaster",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InventoryAudit",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CastingDate = table.Column<DateTime>(nullable: false),
                    CreatedByID = table.Column<int>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    MarkingNo = table.Column<string>(nullable: true),
                    ProjectID = table.Column<int>(nullable: true),
                    SN = table.Column<int>(nullable: false),
                    TrackerID = table.Column<int>(nullable: true),
                    UpdatedByID = table.Column<int>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    VendorID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryAudit", x => x.ID);
                    table.ForeignKey(
                        name: "FK_InventoryAudit_UserMaster_CreatedByID",
                        column: x => x.CreatedByID,
                        principalTable: "UserMaster",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InventoryAudit_ProjectMaster_ProjectID",
                        column: x => x.ProjectID,
                        principalTable: "ProjectMaster",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InventoryAudit_TrackerMaster_TrackerID",
                        column: x => x.TrackerID,
                        principalTable: "TrackerMaster",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InventoryAudit_UserMaster_UpdatedByID",
                        column: x => x.UpdatedByID,
                        principalTable: "UserMaster",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InventoryAudit_VendorMaster_VendorID",
                        column: x => x.VendorID,
                        principalTable: "VendorMaster",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MaterialMaster",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Block = table.Column<string>(nullable: false),
                    CastingDate = table.Column<DateTime>(nullable: false),
                    CreatedByID = table.Column<int>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    Level = table.Column<string>(nullable: false),
                    MRFID = table.Column<int>(nullable: true),
                    MarkingNo = table.Column<string>(nullable: false),
                    MaterialType = table.Column<string>(nullable: false),
                    ProjectID = table.Column<int>(nullable: false),
                    SN = table.Column<int>(nullable: false),
                    TrackerID = table.Column<int>(nullable: true),
                    UpdatedByID = table.Column<int>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    VendorID = table.Column<int>(nullable: true),
                    Zone = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialMaster", x => x.ID);
                    table.ForeignKey(
                        name: "FK_MaterialMaster_UserMaster_CreatedByID",
                        column: x => x.CreatedByID,
                        principalTable: "UserMaster",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MaterialMaster_MRFMaster_MRFID",
                        column: x => x.MRFID,
                        principalTable: "MRFMaster",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MaterialMaster_ProjectMaster_ProjectID",
                        column: x => x.ProjectID,
                        principalTable: "ProjectMaster",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MaterialMaster_TrackerMaster_TrackerID",
                        column: x => x.TrackerID,
                        principalTable: "TrackerMaster",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MaterialMaster_UserMaster_UpdatedByID",
                        column: x => x.UpdatedByID,
                        principalTable: "UserMaster",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MaterialMaster_VendorMaster_VendorID",
                        column: x => x.VendorID,
                        principalTable: "VendorMaster",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MaterialDrawingAssociation",
                columns: table => new
                {
                    MaterialID = table.Column<int>(nullable: false),
                    DrawingID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialDrawingAssociation", x => new { x.MaterialID, x.DrawingID });
                    table.ForeignKey(
                        name: "FK_MaterialDrawingAssociation_MaterialDrawingAudit_DrawingID",
                        column: x => x.DrawingID,
                        principalTable: "MaterialDrawingAudit",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MaterialDrawingAssociation_MaterialMaster_MaterialID",
                        column: x => x.MaterialID,
                        principalTable: "MaterialMaster",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MaterialInfoAudit",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedByID = table.Column<int>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ExpectedDeliveryDate = table.Column<DateTime>(nullable: false),
                    MaterialID = table.Column<int>(nullable: true),
                    Remarks = table.Column<string>(nullable: true),
                    UpdatedByID = table.Column<int>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialInfoAudit", x => x.ID);
                    table.ForeignKey(
                        name: "FK_MaterialInfoAudit_UserMaster_CreatedByID",
                        column: x => x.CreatedByID,
                        principalTable: "UserMaster",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MaterialInfoAudit_MaterialMaster_MaterialID",
                        column: x => x.MaterialID,
                        principalTable: "MaterialMaster",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MaterialInfoAudit_UserMaster_UpdatedByID",
                        column: x => x.UpdatedByID,
                        principalTable: "UserMaster",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MaterialStageAudit",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedByID = table.Column<int>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    LocationID = table.Column<int>(nullable: true),
                    MaterialMasterID = table.Column<int>(nullable: true),
                    Remarks = table.Column<string>(nullable: true),
                    StageID = table.Column<int>(nullable: false),
                    StagePassed = table.Column<bool>(nullable: false),
                    UpdatedByID = table.Column<int>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialStageAudit", x => x.ID);
                    table.ForeignKey(
                        name: "FK_MaterialStageAudit_UserMaster_CreatedByID",
                        column: x => x.CreatedByID,
                        principalTable: "UserMaster",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MaterialStageAudit_LocationMaster_LocationID",
                        column: x => x.LocationID,
                        principalTable: "LocationMaster",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MaterialStageAudit_MaterialMaster_MaterialMasterID",
                        column: x => x.MaterialMasterID,
                        principalTable: "MaterialMaster",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MaterialStageAudit_MaterialStageMaster_StageID",
                        column: x => x.StageID,
                        principalTable: "MaterialStageMaster",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MaterialStageAudit_UserMaster_UpdatedByID",
                        column: x => x.UpdatedByID,
                        principalTable: "UserMaster",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InventoryAudit_CreatedByID",
                table: "InventoryAudit",
                column: "CreatedByID");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryAudit_ProjectID",
                table: "InventoryAudit",
                column: "ProjectID");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryAudit_TrackerID",
                table: "InventoryAudit",
                column: "TrackerID");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryAudit_UpdatedByID",
                table: "InventoryAudit",
                column: "UpdatedByID");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryAudit_VendorID",
                table: "InventoryAudit",
                column: "VendorID");

            migrationBuilder.CreateIndex(
                name: "IX_LocationMaster_CreatedByID",
                table: "LocationMaster",
                column: "CreatedByID");

            migrationBuilder.CreateIndex(
                name: "IX_LocationMaster_UpdatedByID",
                table: "LocationMaster",
                column: "UpdatedByID");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialDrawingAssociation_DrawingID",
                table: "MaterialDrawingAssociation",
                column: "DrawingID");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialInfoAudit_CreatedByID",
                table: "MaterialInfoAudit",
                column: "CreatedByID");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialInfoAudit_MaterialID",
                table: "MaterialInfoAudit",
                column: "MaterialID");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialInfoAudit_UpdatedByID",
                table: "MaterialInfoAudit",
                column: "UpdatedByID");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialMaster_CreatedByID",
                table: "MaterialMaster",
                column: "CreatedByID");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialMaster_MRFID",
                table: "MaterialMaster",
                column: "MRFID");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialMaster_ProjectID",
                table: "MaterialMaster",
                column: "ProjectID");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialMaster_TrackerID",
                table: "MaterialMaster",
                column: "TrackerID");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialMaster_UpdatedByID",
                table: "MaterialMaster",
                column: "UpdatedByID");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialMaster_VendorID",
                table: "MaterialMaster",
                column: "VendorID");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialMaster_MarkingNo_Block_Level_Zone",
                table: "MaterialMaster",
                columns: new[] { "MarkingNo", "Block", "Level", "Zone" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MaterialStageAudit_CreatedByID",
                table: "MaterialStageAudit",
                column: "CreatedByID");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialStageAudit_LocationID",
                table: "MaterialStageAudit",
                column: "LocationID");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialStageAudit_MaterialMasterID",
                table: "MaterialStageAudit",
                column: "MaterialMasterID");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialStageAudit_StageID",
                table: "MaterialStageAudit",
                column: "StageID");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialStageAudit_UpdatedByID",
                table: "MaterialStageAudit",
                column: "UpdatedByID");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialStageMaster_CreatedByID",
                table: "MaterialStageMaster",
                column: "CreatedByID");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialStageMaster_UpdatedByID",
                table: "MaterialStageMaster",
                column: "UpdatedByID");

            migrationBuilder.CreateIndex(
                name: "IX_MRFMaster_CreatedByID",
                table: "MRFMaster",
                column: "CreatedByID");

            migrationBuilder.CreateIndex(
                name: "IX_MRFMaster_UpdatedByID",
                table: "MRFMaster",
                column: "UpdatedByID");

            migrationBuilder.CreateIndex(
                name: "IX_PageAccessRight_PageID",
                table: "PageAccessRight",
                column: "PageID");

            migrationBuilder.CreateIndex(
                name: "IX_PageAccessRight_RoleMasterID",
                table: "PageAccessRight",
                column: "RoleMasterID");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectMaster_CreatedByID",
                table: "ProjectMaster",
                column: "CreatedByID");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectMaster_UpdatedByID",
                table: "ProjectMaster",
                column: "UpdatedByID");

            migrationBuilder.CreateIndex(
                name: "IX_TrackerMaster_CreatedByID",
                table: "TrackerMaster",
                column: "CreatedByID");

            migrationBuilder.CreateIndex(
                name: "IX_TrackerMaster_UpdatedByID",
                table: "TrackerMaster",
                column: "UpdatedByID");

            migrationBuilder.CreateIndex(
                name: "IX_UserMaster_RoleID",
                table: "UserMaster",
                column: "RoleID");

            migrationBuilder.CreateIndex(
                name: "IX_UserMaster_VendorMasterID",
                table: "UserMaster",
                column: "VendorMasterID");

            migrationBuilder.CreateIndex(
                name: "IX_UserMRFAssociation_MRFID",
                table: "UserMRFAssociation",
                column: "MRFID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InventoryAudit");

            migrationBuilder.DropTable(
                name: "MaterialDrawingAssociation");

            migrationBuilder.DropTable(
                name: "MaterialInfoAudit");

            migrationBuilder.DropTable(
                name: "MaterialStageAudit");

            migrationBuilder.DropTable(
                name: "PageAccessRight");

            migrationBuilder.DropTable(
                name: "UserMRFAssociation");

            migrationBuilder.DropTable(
                name: "MaterialDrawingAudit");

            migrationBuilder.DropTable(
                name: "LocationMaster");

            migrationBuilder.DropTable(
                name: "MaterialMaster");

            migrationBuilder.DropTable(
                name: "MaterialStageMaster");

            migrationBuilder.DropTable(
                name: "PageMaster");

            migrationBuilder.DropTable(
                name: "MRFMaster");

            migrationBuilder.DropTable(
                name: "ProjectMaster");

            migrationBuilder.DropTable(
                name: "TrackerMaster");

            migrationBuilder.DropTable(
                name: "UserMaster");

            migrationBuilder.DropTable(
                name: "RoleMaster");

            migrationBuilder.DropTable(
                name: "VendorMaster");
        }
    }
}
