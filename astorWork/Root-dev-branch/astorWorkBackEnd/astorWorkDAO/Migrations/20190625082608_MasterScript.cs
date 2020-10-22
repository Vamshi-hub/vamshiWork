using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace astorWorkDAO.Migrations
{
    public partial class MasterScript : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BIMForgeModel",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BucketKey = table.Column<string>(nullable: true),
                    ObjectKey = table.Column<string>(nullable: true),
                    ObjectID = table.Column<string>(nullable: true),
                    Sha1 = table.Column<string>(nullable: true),
                    Size = table.Column<int>(nullable: false),
                    Location = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BIMForgeModel", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ChecklistItemMaster",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    TimeFrame = table.Column<string>(nullable: true),
                    Type = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChecklistItemMaster", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "MaterialDrawingAudit",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DrawingNo = table.Column<string>(nullable: true),
                    RevisionNo = table.Column<int>(nullable: false),
                    DrawingIssueDate = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialDrawingAudit", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "MaterialStageMaster",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 30, nullable: false),
                    Order = table.Column<int>(nullable: false),
                    Colour = table.Column<string>(nullable: false),
                    MilestoneId = table.Column<int>(nullable: false),
                    IsEditable = table.Column<bool>(nullable: false),
                    MaterialTypes = table.Column<string>(nullable: true),
                    CanIgnoreQC = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialStageMaster", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "MaterialTypeMaster",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    RouteTo = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialTypeMaster", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ModuleMaster",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false),
                    UrlPrefix = table.Column<string>(nullable: false),
                    ParentModuleID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModuleMaster", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ModuleMaster_ModuleMaster_ParentModuleID",
                        column: x => x.ParentModuleID,
                        principalTable: "ModuleMaster",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrganisationMaster",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false),
                    CycleDays = table.Column<int>(nullable: false, defaultValue: 7),
                    OrganisationType = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganisationMaster", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ProjectMaster",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false),
                    Country = table.Column<string>(nullable: true),
                    TimeZoneOffset = table.Column<int>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    EstimatedStartDate = table.Column<DateTimeOffset>(nullable: true),
                    EstimatedEndDate = table.Column<DateTimeOffset>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectMaster", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "SystemHealthMaster",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Type = table.Column<int>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    Message = table.Column<string>(nullable: true),
                    Reference = table.Column<string>(nullable: true),
                    LastUpdated = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemHealthMaster", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "TradeMaster",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    RouteTo = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TradeMaster", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "PageMaster",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false),
                    UrlPath = table.Column<string>(nullable: false),
                    ModuleMasterID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PageMaster", x => x.ID);
                    table.ForeignKey(
                        name: "FK_PageMaster_ModuleMaster_ModuleMasterID",
                        column: x => x.ModuleMasterID,
                        principalTable: "ModuleMaster",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SiteMaster",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Country = table.Column<string>(nullable: true),
                    TimeZoneOffset = table.Column<int>(nullable: false),
                    OrganisationID = table.Column<int>(nullable: true),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiteMaster", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SiteMaster_OrganisationMaster_OrganisationID",
                        column: x => x.OrganisationID,
                        principalTable: "OrganisationMaster",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ChecklistMaster",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    TradeID = table.Column<int>(nullable: true),
                    Sequence = table.Column<int>(nullable: false),
                    MaterialStageID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChecklistMaster", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ChecklistMaster_MaterialStageMaster_MaterialStageID",
                        column: x => x.MaterialStageID,
                        principalTable: "MaterialStageMaster",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChecklistMaster_TradeMaster_TradeID",
                        column: x => x.TradeID,
                        principalTable: "TradeMaster",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TradeMaterialTypeAssociation",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    MaterialTypeID = table.Column<int>(nullable: false),
                    TradeID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TradeMaterialTypeAssociation", x => x.ID);
                    table.ForeignKey(
                        name: "FK_TradeMaterialTypeAssociation_MaterialTypeMaster_MaterialTypeID",
                        column: x => x.MaterialTypeID,
                        principalTable: "MaterialTypeMaster",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TradeMaterialTypeAssociation_TradeMaster_TradeID",
                        column: x => x.TradeID,
                        principalTable: "TradeMaster",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoleMaster",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    PlatformCode = table.Column<string>(nullable: true),
                    DefaultPageID = table.Column<int>(nullable: true),
                    MobileEntryPoint = table.Column<int>(nullable: false),
                    IsEditable = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleMaster", x => x.ID);
                    table.ForeignKey(
                        name: "FK_RoleMaster_PageMaster_DefaultPageID",
                        column: x => x.DefaultPageID,
                        principalTable: "PageMaster",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LocationMaster",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Type = table.Column<int>(nullable: false),
                    OrganisationID = table.Column<int>(nullable: true),
                    SiteID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocationMaster", x => x.ID);
                    table.ForeignKey(
                        name: "FK_LocationMaster_OrganisationMaster_OrganisationID",
                        column: x => x.OrganisationID,
                        principalTable: "OrganisationMaster",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LocationMaster_SiteMaster_SiteID",
                        column: x => x.SiteID,
                        principalTable: "SiteMaster",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NotificationTimerMaster",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<int>(nullable: false),
                    TriggerTime = table.Column<int>(nullable: false),
                    Enabled = table.Column<bool>(nullable: false),
                    UpdateRequired = table.Column<bool>(nullable: false),
                    SiteID = table.Column<int>(nullable: true),
                    ProjectID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationTimerMaster", x => x.ID);
                    table.ForeignKey(
                        name: "FK_NotificationTimerMaster_ProjectMaster_ProjectID",
                        column: x => x.ProjectID,
                        principalTable: "ProjectMaster",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NotificationTimerMaster_SiteMaster_SiteID",
                        column: x => x.SiteID,
                        principalTable: "SiteMaster",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ChecklistItemAssociation",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ChecklistItemID = table.Column<int>(nullable: false),
                    ChecklistID = table.Column<int>(nullable: false),
                    ChecklistItemSequence = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChecklistItemAssociation", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ChecklistItemAssociation_ChecklistMaster_ChecklistID",
                        column: x => x.ChecklistID,
                        principalTable: "ChecklistMaster",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChecklistItemAssociation_ChecklistItemMaster_ChecklistItemID",
                        column: x => x.ChecklistItemID,
                        principalTable: "ChecklistItemMaster",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RolePageAssociation",
                columns: table => new
                {
                    RoleId = table.Column<int>(nullable: false),
                    PageId = table.Column<int>(nullable: false),
                    AccessLevel = table.Column<int>(nullable: false)
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
                        name: "FK_RolePageAssociation_RoleMaster_RoleId",
                        column: x => x.RoleId,
                        principalTable: "RoleMaster",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RolePowerBIReport",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RoleId = table.Column<int>(nullable: false),
                    PowerBIReportId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePowerBIReport", x => x.ID);
                    table.ForeignKey(
                        name: "FK_RolePowerBIReport_RoleMaster_RoleId",
                        column: x => x.RoleId,
                        principalTable: "RoleMaster",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserMaster",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserName = table.Column<string>(nullable: false),
                    Password = table.Column<string>(nullable: true),
                    Salt = table.Column<string>(nullable: true),
                    PersonName = table.Column<string>(nullable: false),
                    Email = table.Column<string>(nullable: true),
                    RoleID = table.Column<int>(nullable: false),
                    OrganisationID = table.Column<int>(nullable: true),
                    LastLogin = table.Column<DateTimeOffset>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    SiteID = table.Column<int>(nullable: true),
                    ProjectID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserMaster", x => x.ID);
                    table.ForeignKey(
                        name: "FK_UserMaster_OrganisationMaster_OrganisationID",
                        column: x => x.OrganisationID,
                        principalTable: "OrganisationMaster",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserMaster_ProjectMaster_ProjectID",
                        column: x => x.ProjectID,
                        principalTable: "ProjectMaster",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserMaster_RoleMaster_RoleID",
                        column: x => x.RoleID,
                        principalTable: "RoleMaster",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserMaster_SiteMaster_SiteID",
                        column: x => x.SiteID,
                        principalTable: "SiteMaster",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NotificationAudit",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Type = table.Column<int>(nullable: false),
                    Code = table.Column<int>(nullable: false),
                    Reference = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(nullable: false),
                    ProcessedDate = table.Column<DateTimeOffset>(nullable: true),
                    NotificationTimerID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationAudit", x => x.ID);
                    table.ForeignKey(
                        name: "FK_NotificationAudit_NotificationTimerMaster_NotificationTimerID",
                        column: x => x.NotificationTimerID,
                        principalTable: "NotificationTimerMaster",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AttachmentMaster",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    URL = table.Column<string>(nullable: true),
                    FileName = table.Column<string>(nullable: true),
                    FileSize = table.Column<int>(nullable: false),
                    CreatedByID = table.Column<int>(nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(nullable: false),
                    UpdatedByID = table.Column<int>(nullable: true),
                    UpdatedDate = table.Column<DateTimeOffset>(nullable: true),
                    Type = table.Column<int>(nullable: false),
                    Reference = table.Column<string>(nullable: true),
                    Remarks = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttachmentMaster", x => x.ID);
                    table.ForeignKey(
                        name: "FK_AttachmentMaster_UserMaster_CreatedByID",
                        column: x => x.CreatedByID,
                        principalTable: "UserMaster",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AttachmentMaster_UserMaster_UpdatedByID",
                        column: x => x.UpdatedByID,
                        principalTable: "UserMaster",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BIMSyncAudit",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ProjectID = table.Column<int>(nullable: false),
                    BIMModelId = table.Column<string>(nullable: false),
                    BIMVideoUrl = table.Column<string>(nullable: true),
                    SyncedMaterialIds = table.Column<string>(nullable: true),
                    UnsyncedMaterialIds = table.Column<string>(nullable: true),
                    SyncedByID = table.Column<int>(nullable: false),
                    SyncTime = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BIMSyncAudit", x => x.ID);
                    table.ForeignKey(
                        name: "FK_BIMSyncAudit_ProjectMaster_ProjectID",
                        column: x => x.ProjectID,
                        principalTable: "ProjectMaster",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BIMSyncAudit_UserMaster_SyncedByID",
                        column: x => x.SyncedByID,
                        principalTable: "UserMaster",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MRFMaster",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    MRFNo = table.Column<string>(nullable: true),
                    OrderDate = table.Column<DateTimeOffset>(nullable: false),
                    PlannedCastingDate = table.Column<DateTimeOffset>(nullable: true),
                    ExpectedDeliveryDate = table.Column<DateTimeOffset>(nullable: false),
                    Remarks = table.Column<string>(nullable: true),
                    MRFCompletion = table.Column<double>(nullable: false),
                    CreatedByID = table.Column<int>(nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(nullable: false),
                    UpdatedByID = table.Column<int>(nullable: true),
                    UpdatedDate = table.Column<DateTimeOffset>(nullable: false)
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
                name: "UserRequestAudit",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserID = table.Column<int>(nullable: false),
                    RequestType = table.Column<int>(nullable: false),
                    RequestTime = table.Column<DateTimeOffset>(nullable: false),
                    RequestSuccess = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRequestAudit", x => x.ID);
                    table.ForeignKey(
                        name: "FK_UserRequestAudit_UserMaster_UserID",
                        column: x => x.UserID,
                        principalTable: "UserMaster",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserSessionAudit",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AccessToken = table.Column<string>(nullable: false),
                    RefreshToken = table.Column<string>(nullable: false),
                    ExpireIn = table.Column<int>(nullable: false),
                    CreatedTime = table.Column<DateTimeOffset>(nullable: false),
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

            migrationBuilder.CreateTable(
                name: "UserNotificationAssociation",
                columns: table => new
                {
                    UserID = table.Column<int>(nullable: false),
                    NotificationID = table.Column<int>(nullable: false),
                    ID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserNotificationAssociation", x => new { x.UserID, x.NotificationID });
                    table.ForeignKey(
                        name: "FK_UserNotificationAssociation_NotificationAudit_NotificationID",
                        column: x => x.NotificationID,
                        principalTable: "NotificationAudit",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserNotificationAssociation_UserMaster_UserID",
                        column: x => x.UserID,
                        principalTable: "UserMaster",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MaterialMaster",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    MarkingNo = table.Column<string>(nullable: false),
                    ProjectID = table.Column<int>(nullable: false),
                    Block = table.Column<string>(nullable: false),
                    Level = table.Column<string>(nullable: false),
                    Zone = table.Column<string>(nullable: false),
                    MaterialTypeID = table.Column<int>(nullable: false),
                    OrganisationID = table.Column<int>(nullable: false),
                    MRFID = table.Column<int>(nullable: true),
                    SN = table.Column<int>(nullable: false),
                    CastingDate = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialMaster", x => x.ID);
                    table.ForeignKey(
                        name: "FK_MaterialMaster_MRFMaster_MRFID",
                        column: x => x.MRFID,
                        principalTable: "MRFMaster",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MaterialMaster_MaterialTypeMaster_MaterialTypeID",
                        column: x => x.MaterialTypeID,
                        principalTable: "MaterialTypeMaster",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MaterialMaster_OrganisationMaster_OrganisationID",
                        column: x => x.OrganisationID,
                        principalTable: "OrganisationMaster",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MaterialMaster_ProjectMaster_ProjectID",
                        column: x => x.ProjectID,
                        principalTable: "ProjectMaster",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
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
                name: "BIMForgeElement",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DbID = table.Column<int>(nullable: false),
                    ForgeModelID = table.Column<int>(nullable: false),
                    MaterialMasterID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BIMForgeElement", x => x.ID);
                    table.ForeignKey(
                        name: "FK_BIMForgeElement_BIMForgeModel_ForgeModelID",
                        column: x => x.ForgeModelID,
                        principalTable: "BIMForgeModel",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BIMForgeElement_MaterialMaster_MaterialMasterID",
                        column: x => x.MaterialMasterID,
                        principalTable: "MaterialMaster",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JobSchedule",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    MaterialID = table.Column<int>(nullable: false),
                    TradeID = table.Column<int>(nullable: false),
                    SubconID = table.Column<int>(nullable: true),
                    PlannedStartDate = table.Column<DateTimeOffset>(nullable: true),
                    PlannedEndDate = table.Column<DateTimeOffset>(nullable: true),
                    ActualStartDate = table.Column<DateTimeOffset>(nullable: true),
                    ActualEndDate = table.Column<DateTimeOffset>(nullable: true),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobSchedule", x => x.ID);
                    table.ForeignKey(
                        name: "FK_JobSchedule_MaterialMaster_MaterialID",
                        column: x => x.MaterialID,
                        principalTable: "MaterialMaster",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JobSchedule_OrganisationMaster_SubconID",
                        column: x => x.SubconID,
                        principalTable: "OrganisationMaster",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobSchedule_TradeMaster_TradeID",
                        column: x => x.TradeID,
                        principalTable: "TradeMaster",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
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
                    ExpectedDeliveryDate = table.Column<DateTimeOffset>(nullable: false),
                    Remarks = table.Column<string>(nullable: true),
                    MaterialID = table.Column<int>(nullable: true),
                    CreatedByID = table.Column<int>(nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(nullable: false),
                    UpdatedByID = table.Column<int>(nullable: true),
                    UpdatedDate = table.Column<DateTimeOffset>(nullable: false)
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
                name: "MaterialQCCase",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CaseName = table.Column<string>(nullable: true),
                    CreatedById = table.Column<int>(nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(nullable: false),
                    UpdatedById = table.Column<int>(nullable: true),
                    UpdatedDate = table.Column<DateTimeOffset>(nullable: true),
                    MaterialMasterId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialQCCase", x => x.ID);
                    table.ForeignKey(
                        name: "FK_MaterialQCCase_UserMaster_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "UserMaster",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MaterialQCCase_MaterialMaster_MaterialMasterId",
                        column: x => x.MaterialMasterId,
                        principalTable: "MaterialMaster",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MaterialQCCase_UserMaster_UpdatedById",
                        column: x => x.UpdatedById,
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
                    MaterialMasterID = table.Column<int>(nullable: false),
                    StageID = table.Column<int>(nullable: false),
                    LocationID = table.Column<int>(nullable: false),
                    Remarks = table.Column<string>(nullable: true),
                    QCStatus = table.Column<int>(nullable: false),
                    CreatedByID = table.Column<int>(nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialStageAudit", x => x.ID);
                    table.ForeignKey(
                        name: "FK_MaterialStageAudit_UserMaster_CreatedByID",
                        column: x => x.CreatedByID,
                        principalTable: "UserMaster",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MaterialStageAudit_LocationMaster_LocationID",
                        column: x => x.LocationID,
                        principalTable: "LocationMaster",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MaterialStageAudit_MaterialMaster_MaterialMasterID",
                        column: x => x.MaterialMasterID,
                        principalTable: "MaterialMaster",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MaterialStageAudit_MaterialStageMaster_StageID",
                        column: x => x.StageID,
                        principalTable: "MaterialStageMaster",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TrackerMaster",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Tag = table.Column<string>(nullable: false),
                    Type = table.Column<string>(nullable: false),
                    Label = table.Column<string>(nullable: true),
                    BatchNumber = table.Column<int>(nullable: false),
                    MaterialID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrackerMaster", x => x.ID);
                    table.ForeignKey(
                        name: "FK_TrackerMaster_MaterialMaster_MaterialID",
                        column: x => x.MaterialID,
                        principalTable: "MaterialMaster",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "JobAudit",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    JobScheduleID = table.Column<int>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedByID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobAudit", x => x.ID);
                    table.ForeignKey(
                        name: "FK_JobAudit_UserMaster_CreatedByID",
                        column: x => x.CreatedByID,
                        principalTable: "UserMaster",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobAudit_JobSchedule_JobScheduleID",
                        column: x => x.JobScheduleID,
                        principalTable: "JobSchedule",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MaterialQCDefect",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Status = table.Column<int>(nullable: false),
                    OrganisationID = table.Column<int>(nullable: true),
                    CreatedByID = table.Column<int>(nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(nullable: false),
                    UpdatedByID = table.Column<int>(nullable: true),
                    UpdatedDate = table.Column<DateTimeOffset>(nullable: true),
                    QCCaseID = table.Column<int>(nullable: false),
                    Remarks = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialQCDefect", x => x.ID);
                    table.ForeignKey(
                        name: "FK_MaterialQCDefect_UserMaster_CreatedByID",
                        column: x => x.CreatedByID,
                        principalTable: "UserMaster",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MaterialQCDefect_OrganisationMaster_OrganisationID",
                        column: x => x.OrganisationID,
                        principalTable: "OrganisationMaster",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MaterialQCDefect_MaterialQCCase_QCCaseID",
                        column: x => x.QCCaseID,
                        principalTable: "MaterialQCCase",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MaterialQCDefect_UserMaster_UpdatedByID",
                        column: x => x.UpdatedByID,
                        principalTable: "UserMaster",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ChatData",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Message = table.Column<string>(nullable: true),
                    Attachment = table.Column<string>(nullable: true),
                    UserID = table.Column<int>(nullable: false),
                    MaterialStageAuditID = table.Column<int>(nullable: true),
                    JobScheduleID = table.Column<int>(nullable: true),
                    ChecklistID = table.Column<int>(nullable: true),
                    ChecklistItemID = table.Column<int>(nullable: true),
                    IsSystem = table.Column<bool>(nullable: false),
                    HasAttachment = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatData", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ChatData_ChecklistMaster_ChecklistID",
                        column: x => x.ChecklistID,
                        principalTable: "ChecklistMaster",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChatData_ChecklistItemMaster_ChecklistItemID",
                        column: x => x.ChecklistItemID,
                        principalTable: "ChecklistItemMaster",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChatData_JobSchedule_JobScheduleID",
                        column: x => x.JobScheduleID,
                        principalTable: "JobSchedule",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChatData_MaterialStageAudit_MaterialStageAuditID",
                        column: x => x.MaterialStageAuditID,
                        principalTable: "MaterialStageAudit",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChatData_UserMaster_UserID",
                        column: x => x.UserID,
                        principalTable: "UserMaster",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChecklistAudit",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    JobScheduleID = table.Column<int>(nullable: true),
                    MaterialStageAuditID = table.Column<int>(nullable: true),
                    ChecklistID = table.Column<int>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    RouteToID = table.Column<int>(nullable: true),
                    SignatureURL = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(nullable: true),
                    CreatedByID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChecklistAudit", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ChecklistAudit_ChecklistMaster_ChecklistID",
                        column: x => x.ChecklistID,
                        principalTable: "ChecklistMaster",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChecklistAudit_UserMaster_CreatedByID",
                        column: x => x.CreatedByID,
                        principalTable: "UserMaster",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChecklistAudit_JobSchedule_JobScheduleID",
                        column: x => x.JobScheduleID,
                        principalTable: "JobSchedule",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChecklistAudit_MaterialStageAudit_MaterialStageAuditID",
                        column: x => x.MaterialStageAuditID,
                        principalTable: "MaterialStageAudit",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChecklistAudit_UserMaster_RouteToID",
                        column: x => x.RouteToID,
                        principalTable: "UserMaster",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InventoryAudit",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    MarkingNo = table.Column<string>(nullable: true),
                    SN = table.Column<int>(nullable: false),
                    CastingDate = table.Column<DateTimeOffset>(nullable: true),
                    TrackerID = table.Column<int>(nullable: true),
                    ProjectID = table.Column<int>(nullable: true),
                    OrganisationID = table.Column<int>(nullable: true),
                    CreatedByID = table.Column<int>(nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(nullable: true)
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
                        name: "FK_InventoryAudit_OrganisationMaster_OrganisationID",
                        column: x => x.OrganisationID,
                        principalTable: "OrganisationMaster",
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
                });

            migrationBuilder.CreateTable(
                name: "MaterialQCPhotos",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    URL = table.Column<string>(nullable: true),
                    Remarks = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    CreatedById = table.Column<int>(nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(nullable: false),
                    MaterialQCDefectID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialQCPhotos", x => x.ID);
                    table.ForeignKey(
                        name: "FK_MaterialQCPhotos_UserMaster_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "UserMaster",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MaterialQCPhotos_MaterialQCDefect_MaterialQCDefectID",
                        column: x => x.MaterialQCDefectID,
                        principalTable: "MaterialQCDefect",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ChatUserAssociation",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ChatID = table.Column<int>(nullable: true),
                    UserMasterID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatUserAssociation", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ChatUserAssociation_ChatData_ChatID",
                        column: x => x.ChatID,
                        principalTable: "ChatData",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChatUserAssociation_UserMaster_UserMasterID",
                        column: x => x.UserMasterID,
                        principalTable: "UserMaster",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ChecklistItemAudit",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ChecklistAuditID = table.Column<int>(nullable: false),
                    ChecklistItemID = table.Column<int>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(nullable: false),
                    CreatedByID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChecklistItemAudit", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ChecklistItemAudit_ChecklistAudit_ChecklistAuditID",
                        column: x => x.ChecklistAuditID,
                        principalTable: "ChecklistAudit",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChecklistItemAudit_ChecklistItemMaster_ChecklistItemID",
                        column: x => x.ChecklistItemID,
                        principalTable: "ChecklistItemMaster",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChecklistItemAudit_UserMaster_CreatedByID",
                        column: x => x.CreatedByID,
                        principalTable: "UserMaster",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AttachmentMaster_CreatedByID",
                table: "AttachmentMaster",
                column: "CreatedByID");

            migrationBuilder.CreateIndex(
                name: "IX_AttachmentMaster_UpdatedByID",
                table: "AttachmentMaster",
                column: "UpdatedByID");

            migrationBuilder.CreateIndex(
                name: "IX_BIMForgeElement_ForgeModelID",
                table: "BIMForgeElement",
                column: "ForgeModelID");

            migrationBuilder.CreateIndex(
                name: "IX_BIMForgeElement_MaterialMasterID",
                table: "BIMForgeElement",
                column: "MaterialMasterID");

            migrationBuilder.CreateIndex(
                name: "IX_BIMSyncAudit_ProjectID",
                table: "BIMSyncAudit",
                column: "ProjectID");

            migrationBuilder.CreateIndex(
                name: "IX_BIMSyncAudit_SyncedByID",
                table: "BIMSyncAudit",
                column: "SyncedByID");

            migrationBuilder.CreateIndex(
                name: "IX_ChatData_ChecklistID",
                table: "ChatData",
                column: "ChecklistID");

            migrationBuilder.CreateIndex(
                name: "IX_ChatData_ChecklistItemID",
                table: "ChatData",
                column: "ChecklistItemID");

            migrationBuilder.CreateIndex(
                name: "IX_ChatData_JobScheduleID",
                table: "ChatData",
                column: "JobScheduleID");

            migrationBuilder.CreateIndex(
                name: "IX_ChatData_MaterialStageAuditID",
                table: "ChatData",
                column: "MaterialStageAuditID");

            migrationBuilder.CreateIndex(
                name: "IX_ChatData_UserID",
                table: "ChatData",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_ChatUserAssociation_ChatID",
                table: "ChatUserAssociation",
                column: "ChatID");

            migrationBuilder.CreateIndex(
                name: "IX_ChatUserAssociation_UserMasterID",
                table: "ChatUserAssociation",
                column: "UserMasterID");

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistAudit_ChecklistID",
                table: "ChecklistAudit",
                column: "ChecklistID");

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistAudit_CreatedByID",
                table: "ChecklistAudit",
                column: "CreatedByID");

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistAudit_JobScheduleID",
                table: "ChecklistAudit",
                column: "JobScheduleID");

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistAudit_MaterialStageAuditID",
                table: "ChecklistAudit",
                column: "MaterialStageAuditID");

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistAudit_RouteToID",
                table: "ChecklistAudit",
                column: "RouteToID");

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistItemAssociation_ChecklistID",
                table: "ChecklistItemAssociation",
                column: "ChecklistID");

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistItemAssociation_ChecklistItemID",
                table: "ChecklistItemAssociation",
                column: "ChecklistItemID");

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistItemAudit_ChecklistAuditID",
                table: "ChecklistItemAudit",
                column: "ChecklistAuditID");

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistItemAudit_ChecklistItemID",
                table: "ChecklistItemAudit",
                column: "ChecklistItemID");

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistItemAudit_CreatedByID",
                table: "ChecklistItemAudit",
                column: "CreatedByID");

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistMaster_MaterialStageID",
                table: "ChecklistMaster",
                column: "MaterialStageID");

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistMaster_TradeID",
                table: "ChecklistMaster",
                column: "TradeID");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryAudit_CreatedByID",
                table: "InventoryAudit",
                column: "CreatedByID");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryAudit_OrganisationID",
                table: "InventoryAudit",
                column: "OrganisationID");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryAudit_ProjectID",
                table: "InventoryAudit",
                column: "ProjectID");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryAudit_TrackerID",
                table: "InventoryAudit",
                column: "TrackerID");

            migrationBuilder.CreateIndex(
                name: "IX_JobAudit_CreatedByID",
                table: "JobAudit",
                column: "CreatedByID");

            migrationBuilder.CreateIndex(
                name: "IX_JobAudit_JobScheduleID",
                table: "JobAudit",
                column: "JobScheduleID");

            migrationBuilder.CreateIndex(
                name: "IX_JobSchedule_MaterialID",
                table: "JobSchedule",
                column: "MaterialID");

            migrationBuilder.CreateIndex(
                name: "IX_JobSchedule_SubconID",
                table: "JobSchedule",
                column: "SubconID");

            migrationBuilder.CreateIndex(
                name: "IX_JobSchedule_TradeID",
                table: "JobSchedule",
                column: "TradeID");

            migrationBuilder.CreateIndex(
                name: "IX_LocationMaster_Name",
                table: "LocationMaster",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LocationMaster_OrganisationID",
                table: "LocationMaster",
                column: "OrganisationID");

            migrationBuilder.CreateIndex(
                name: "IX_LocationMaster_SiteID",
                table: "LocationMaster",
                column: "SiteID");

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
                name: "IX_MaterialMaster_MRFID",
                table: "MaterialMaster",
                column: "MRFID");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialMaster_MaterialTypeID",
                table: "MaterialMaster",
                column: "MaterialTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialMaster_OrganisationID",
                table: "MaterialMaster",
                column: "OrganisationID");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialMaster_ProjectID_MarkingNo_Block_Level_Zone",
                table: "MaterialMaster",
                columns: new[] { "ProjectID", "MarkingNo", "Block", "Level", "Zone" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MaterialQCCase_CreatedById",
                table: "MaterialQCCase",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialQCCase_MaterialMasterId",
                table: "MaterialQCCase",
                column: "MaterialMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialQCCase_UpdatedById",
                table: "MaterialQCCase",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialQCDefect_CreatedByID",
                table: "MaterialQCDefect",
                column: "CreatedByID");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialQCDefect_OrganisationID",
                table: "MaterialQCDefect",
                column: "OrganisationID");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialQCDefect_QCCaseID",
                table: "MaterialQCDefect",
                column: "QCCaseID");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialQCDefect_UpdatedByID",
                table: "MaterialQCDefect",
                column: "UpdatedByID");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialQCPhotos_CreatedById",
                table: "MaterialQCPhotos",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialQCPhotos_MaterialQCDefectID",
                table: "MaterialQCPhotos",
                column: "MaterialQCDefectID");

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
                name: "IX_MaterialStageMaster_Name",
                table: "MaterialStageMaster",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ModuleMaster_ParentModuleID",
                table: "ModuleMaster",
                column: "ParentModuleID");

            migrationBuilder.CreateIndex(
                name: "IX_ModuleMaster_UrlPrefix",
                table: "ModuleMaster",
                column: "UrlPrefix",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MRFMaster_CreatedByID",
                table: "MRFMaster",
                column: "CreatedByID");

            migrationBuilder.CreateIndex(
                name: "IX_MRFMaster_UpdatedByID",
                table: "MRFMaster",
                column: "UpdatedByID");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationAudit_NotificationTimerID",
                table: "NotificationAudit",
                column: "NotificationTimerID");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationTimerMaster_ProjectID",
                table: "NotificationTimerMaster",
                column: "ProjectID");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationTimerMaster_SiteID",
                table: "NotificationTimerMaster",
                column: "SiteID");

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationMaster_Name",
                table: "OrganisationMaster",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PageMaster_ModuleMasterID",
                table: "PageMaster",
                column: "ModuleMasterID");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectMaster_Name",
                table: "ProjectMaster",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RoleMaster_DefaultPageID",
                table: "RoleMaster",
                column: "DefaultPageID");

            migrationBuilder.CreateIndex(
                name: "IX_RoleMaster_Name",
                table: "RoleMaster",
                column: "Name",
                unique: true,
                filter: "[Name] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_RolePageAssociation_PageId",
                table: "RolePageAssociation",
                column: "PageId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePowerBIReport_RoleId",
                table: "RolePowerBIReport",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_SiteMaster_Name",
                table: "SiteMaster",
                column: "Name",
                unique: true,
                filter: "[Name] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_SiteMaster_OrganisationID",
                table: "SiteMaster",
                column: "OrganisationID");

            migrationBuilder.CreateIndex(
                name: "IX_TrackerMaster_MaterialID",
                table: "TrackerMaster",
                column: "MaterialID");

            migrationBuilder.CreateIndex(
                name: "IX_TrackerMaster_Tag",
                table: "TrackerMaster",
                column: "Tag",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TradeMaterialTypeAssociation_MaterialTypeID",
                table: "TradeMaterialTypeAssociation",
                column: "MaterialTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_TradeMaterialTypeAssociation_TradeID",
                table: "TradeMaterialTypeAssociation",
                column: "TradeID");

            migrationBuilder.CreateIndex(
                name: "IX_UserMaster_OrganisationID",
                table: "UserMaster",
                column: "OrganisationID");

            migrationBuilder.CreateIndex(
                name: "IX_UserMaster_ProjectID",
                table: "UserMaster",
                column: "ProjectID");

            migrationBuilder.CreateIndex(
                name: "IX_UserMaster_RoleID",
                table: "UserMaster",
                column: "RoleID");

            migrationBuilder.CreateIndex(
                name: "IX_UserMaster_SiteID",
                table: "UserMaster",
                column: "SiteID");

            migrationBuilder.CreateIndex(
                name: "IX_UserMaster_UserName",
                table: "UserMaster",
                column: "UserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserMRFAssociation_MRFID",
                table: "UserMRFAssociation",
                column: "MRFID");

            migrationBuilder.CreateIndex(
                name: "IX_UserNotificationAssociation_NotificationID",
                table: "UserNotificationAssociation",
                column: "NotificationID");

            migrationBuilder.CreateIndex(
                name: "IX_UserRequestAudit_UserID",
                table: "UserRequestAudit",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_UserSessionAudit_UserMasterID",
                table: "UserSessionAudit",
                column: "UserMasterID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AttachmentMaster");

            migrationBuilder.DropTable(
                name: "BIMForgeElement");

            migrationBuilder.DropTable(
                name: "BIMSyncAudit");

            migrationBuilder.DropTable(
                name: "ChatUserAssociation");

            migrationBuilder.DropTable(
                name: "ChecklistItemAssociation");

            migrationBuilder.DropTable(
                name: "ChecklistItemAudit");

            migrationBuilder.DropTable(
                name: "InventoryAudit");

            migrationBuilder.DropTable(
                name: "JobAudit");

            migrationBuilder.DropTable(
                name: "MaterialDrawingAssociation");

            migrationBuilder.DropTable(
                name: "MaterialInfoAudit");

            migrationBuilder.DropTable(
                name: "MaterialQCPhotos");

            migrationBuilder.DropTable(
                name: "RolePageAssociation");

            migrationBuilder.DropTable(
                name: "RolePowerBIReport");

            migrationBuilder.DropTable(
                name: "SystemHealthMaster");

            migrationBuilder.DropTable(
                name: "TradeMaterialTypeAssociation");

            migrationBuilder.DropTable(
                name: "UserMRFAssociation");

            migrationBuilder.DropTable(
                name: "UserNotificationAssociation");

            migrationBuilder.DropTable(
                name: "UserRequestAudit");

            migrationBuilder.DropTable(
                name: "UserSessionAudit");

            migrationBuilder.DropTable(
                name: "BIMForgeModel");

            migrationBuilder.DropTable(
                name: "ChatData");

            migrationBuilder.DropTable(
                name: "ChecklistAudit");

            migrationBuilder.DropTable(
                name: "TrackerMaster");

            migrationBuilder.DropTable(
                name: "MaterialDrawingAudit");

            migrationBuilder.DropTable(
                name: "MaterialQCDefect");

            migrationBuilder.DropTable(
                name: "NotificationAudit");

            migrationBuilder.DropTable(
                name: "ChecklistItemMaster");

            migrationBuilder.DropTable(
                name: "ChecklistMaster");

            migrationBuilder.DropTable(
                name: "JobSchedule");

            migrationBuilder.DropTable(
                name: "MaterialStageAudit");

            migrationBuilder.DropTable(
                name: "MaterialQCCase");

            migrationBuilder.DropTable(
                name: "NotificationTimerMaster");

            migrationBuilder.DropTable(
                name: "TradeMaster");

            migrationBuilder.DropTable(
                name: "LocationMaster");

            migrationBuilder.DropTable(
                name: "MaterialStageMaster");

            migrationBuilder.DropTable(
                name: "MaterialMaster");

            migrationBuilder.DropTable(
                name: "MRFMaster");

            migrationBuilder.DropTable(
                name: "MaterialTypeMaster");

            migrationBuilder.DropTable(
                name: "UserMaster");

            migrationBuilder.DropTable(
                name: "ProjectMaster");

            migrationBuilder.DropTable(
                name: "RoleMaster");

            migrationBuilder.DropTable(
                name: "SiteMaster");

            migrationBuilder.DropTable(
                name: "PageMaster");

            migrationBuilder.DropTable(
                name: "OrganisationMaster");

            migrationBuilder.DropTable(
                name: "ModuleMaster");
        }
    }
}
