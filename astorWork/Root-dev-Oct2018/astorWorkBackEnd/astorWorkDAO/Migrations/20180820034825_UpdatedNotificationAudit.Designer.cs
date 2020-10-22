﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using astorWorkDAO;

namespace astorWorkDAO.Migrations
{
    [DbContext(typeof(astorWorkDbContext))]
    [Migration("20180820034825_UpdatedNotificationAudit")]
    partial class UpdatedNotificationAudit
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.1-rtm-30846")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("astorWorkDAO.BIMSyncAudit", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("BIMModelId")
                        .IsRequired();

                    b.Property<string>("BIMVideoUrl");

                    b.Property<int>("ProjectID");

                    b.Property<DateTimeOffset>("SyncTime");

                    b.Property<int>("SyncedByID");

                    b.Property<string>("SyncedMaterialIds");

                    b.Property<string>("UnsyncedMaterialIds");

                    b.HasKey("ID");

                    b.HasIndex("ProjectID");

                    b.HasIndex("SyncedByID");

                    b.ToTable("BIMSyncAudit");
                });

            modelBuilder.Entity("astorWorkDAO.InventoryAudit", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTimeOffset>("CastingDate");

                    b.Property<int?>("CreatedByID");

                    b.Property<DateTimeOffset>("CreatedDate");

                    b.Property<string>("MarkingNo");

                    b.Property<int?>("ProjectID");

                    b.Property<int>("SN");

                    b.Property<int?>("TrackerID");

                    b.Property<int?>("UpdatedByID");

                    b.Property<DateTimeOffset>("UpdatedDate");

                    b.Property<int?>("VendorID");

                    b.HasKey("ID");

                    b.HasIndex("CreatedByID");

                    b.HasIndex("ProjectID");

                    b.HasIndex("TrackerID");

                    b.HasIndex("UpdatedByID");

                    b.HasIndex("VendorID");

                    b.ToTable("InventoryAudit");
                });

            modelBuilder.Entity("astorWorkDAO.LocationMaster", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Description");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<int?>("SiteID");

                    b.Property<int>("Type");

                    b.Property<int?>("VendorID");

                    b.HasKey("ID");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.HasIndex("SiteID");

                    b.HasIndex("VendorID");

                    b.ToTable("LocationMaster");
                });

            modelBuilder.Entity("astorWorkDAO.MaterialDrawingAssociation", b =>
                {
                    b.Property<int>("MaterialID");

                    b.Property<int>("DrawingID");

                    b.HasKey("MaterialID", "DrawingID");

                    b.HasIndex("DrawingID");

                    b.ToTable("MaterialDrawingAssociation");
                });

            modelBuilder.Entity("astorWorkDAO.MaterialDrawingAudit", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTimeOffset>("DrawingIssueDate");

                    b.Property<string>("DrawingNo");

                    b.Property<int>("RevisionNo");

                    b.HasKey("ID");

                    b.ToTable("MaterialDrawingAudit");
                });

            modelBuilder.Entity("astorWorkDAO.MaterialInfoAudit", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("CreatedByID");

                    b.Property<DateTimeOffset>("CreatedDate");

                    b.Property<DateTimeOffset>("ExpectedDeliveryDate");

                    b.Property<int?>("MaterialID");

                    b.Property<string>("Remarks");

                    b.Property<int?>("UpdatedByID");

                    b.Property<DateTimeOffset>("UpdatedDate");

                    b.HasKey("ID");

                    b.HasIndex("CreatedByID");

                    b.HasIndex("MaterialID");

                    b.HasIndex("UpdatedByID");

                    b.ToTable("MaterialInfoAudit");
                });

            modelBuilder.Entity("astorWorkDAO.MaterialMaster", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Block")
                        .IsRequired();

                    b.Property<DateTimeOffset>("CastingDate");

                    b.Property<string>("Level")
                        .IsRequired();

                    b.Property<int?>("MRFID");

                    b.Property<string>("MarkingNo")
                        .IsRequired();

                    b.Property<string>("MaterialType")
                        .IsRequired();

                    b.Property<int>("ProjectId");

                    b.Property<int>("SN");

                    b.Property<int?>("TrackerID");

                    b.Property<int>("VendorId");

                    b.Property<string>("Zone")
                        .IsRequired();

                    b.HasKey("ID");

                    b.HasIndex("MRFID");

                    b.HasIndex("ProjectId");

                    b.HasIndex("TrackerID");

                    b.HasIndex("VendorId");

                    b.HasIndex("MarkingNo", "Block", "Level", "Zone")
                        .IsUnique();

                    b.ToTable("MaterialMaster");
                });

            modelBuilder.Entity("astorWorkDAO.MaterialQCCase", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("CaseName");

                    b.Property<int?>("CreatedByID");

                    b.Property<DateTimeOffset>("CreatedDate");

                    b.Property<int>("StageAuditId");

                    b.HasKey("ID");

                    b.HasIndex("CreatedByID");

                    b.HasIndex("StageAuditId");

                    b.ToTable("MaterialQCCase");
                });

            modelBuilder.Entity("astorWorkDAO.MaterialQCDefect", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("CreatedByID");

                    b.Property<DateTimeOffset>("CreatedDate");

                    b.Property<bool>("IsOpen");

                    b.Property<int>("QCCaseId");

                    b.Property<string>("Remarks");

                    b.HasKey("ID");

                    b.HasIndex("CreatedByID");

                    b.HasIndex("QCCaseId");

                    b.ToTable("MaterialQCDefect");
                });

            modelBuilder.Entity("astorWorkDAO.MaterialQCPhotos", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("CreatedByID");

                    b.Property<DateTimeOffset>("CreatedDate");

                    b.Property<bool>("IsOpen");

                    b.Property<int?>("MaterialQCDefectID");

                    b.Property<string>("Remarks");

                    b.Property<string>("URL");

                    b.HasKey("ID");

                    b.HasIndex("CreatedByID");

                    b.HasIndex("MaterialQCDefectID");

                    b.ToTable("MaterialQCPhotos");
                });

            modelBuilder.Entity("astorWorkDAO.MaterialStageAudit", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("CreatedByID");

                    b.Property<DateTimeOffset>("CreatedDate");

                    b.Property<int?>("LocationID");

                    b.Property<int?>("MaterialMasterID");

                    b.Property<string>("Remarks");

                    b.Property<int>("StageID");

                    b.Property<bool>("StagePassed");

                    b.HasKey("ID");

                    b.HasIndex("CreatedByID");

                    b.HasIndex("LocationID");

                    b.HasIndex("MaterialMasterID");

                    b.HasIndex("StageID");

                    b.ToTable("MaterialStageAudit");
                });

            modelBuilder.Entity("astorWorkDAO.MaterialStageMaster", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Colour")
                        .IsRequired();

                    b.Property<bool>("IsEditable");

                    b.Property<bool>("IsQCStage");

                    b.Property<string>("MaterialTypes");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<int>("Order");

                    b.HasKey("ID");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("MaterialStageMaster");
                });

            modelBuilder.Entity("astorWorkDAO.ModuleMaster", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<int?>("ParentModuleID");

                    b.Property<string>("UrlPrefix")
                        .IsRequired();

                    b.HasKey("ID");

                    b.HasIndex("ParentModuleID");

                    b.HasIndex("UrlPrefix")
                        .IsUnique();

                    b.ToTable("ModuleMaster");
                });

            modelBuilder.Entity("astorWorkDAO.MRFMaster", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("CreatedByID");

                    b.Property<DateTimeOffset>("CreatedDate");

                    b.Property<DateTimeOffset>("ExpectedDeliveryDate");

                    b.Property<string>("MRFNo");

                    b.Property<DateTimeOffset>("OrderDate");

                    b.Property<DateTimeOffset>("PlannedCastingDate");

                    b.Property<string>("Remarks");

                    b.Property<int?>("UpdatedByID");

                    b.Property<DateTimeOffset>("UpdatedDate");

                    b.HasKey("ID");

                    b.HasIndex("CreatedByID");

                    b.HasIndex("UpdatedByID");

                    b.ToTable("MRFMaster");
                });

            modelBuilder.Entity("astorWorkDAO.NotificationAudit", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Code");

                    b.Property<DateTimeOffset>("CreatedDate");

                    b.Property<DateTimeOffset?>("ProcessedDate");

                    b.Property<string>("Reference");

                    b.Property<int>("Type");

                    b.HasKey("ID");

                    b.ToTable("NotificationAudit");
                });

            modelBuilder.Entity("astorWorkDAO.PageMaster", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("ModuleMasterID");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<string>("UrlPath")
                        .IsRequired();

                    b.HasKey("ID");

                    b.HasIndex("ModuleMasterID");

                    b.ToTable("PageMaster");
                });

            modelBuilder.Entity("astorWorkDAO.ProjectMaster", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Description");

                    b.Property<DateTimeOffset?>("EstimatedEndDate");

                    b.Property<DateTimeOffset?>("EstimatedStartDate");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("ID");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("ProjectMaster");
                });

            modelBuilder.Entity("astorWorkDAO.RoleMaster", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("DefaultPageID");

                    b.Property<bool>("IsEditable");

                    b.Property<int>("MobileEntryPoint");

                    b.Property<string>("Name");

                    b.Property<string>("PlatformCode");

                    b.HasKey("ID");

                    b.HasIndex("DefaultPageID");

                    b.HasIndex("Name")
                        .IsUnique()
                        .HasFilter("[Name] IS NOT NULL");

                    b.ToTable("RoleMaster");
                });

            modelBuilder.Entity("astorWorkDAO.RolePageAssociation", b =>
                {
                    b.Property<int>("RoleId");

                    b.Property<int>("PageId");

                    b.Property<int>("AccessLevel");

                    b.HasKey("RoleId", "PageId");

                    b.HasIndex("PageId");

                    b.ToTable("RolePageAssociation");
                });

            modelBuilder.Entity("astorWorkDAO.SiteMaster", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Country");

                    b.Property<string>("Description");

                    b.Property<string>("Name");

                    b.Property<int?>("VendorID");

                    b.HasKey("ID");

                    b.HasIndex("VendorID");

                    b.ToTable("SiteMaster");
                });

            modelBuilder.Entity("astorWorkDAO.SystemHealthMaster", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTimeOffset>("LastUpdated");

                    b.Property<string>("Message");

                    b.Property<string>("Reference");

                    b.Property<int>("Status");

                    b.Property<int>("Type");

                    b.HasKey("ID");

                    b.ToTable("SystemHealthMaster");
                });

            modelBuilder.Entity("astorWorkDAO.TrackerMaster", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("BatchNumber");

                    b.Property<string>("Label");

                    b.Property<string>("Tag")
                        .IsRequired();

                    b.Property<string>("Type")
                        .IsRequired();

                    b.HasKey("ID");

                    b.ToTable("TrackerMaster");
                });

            modelBuilder.Entity("astorWorkDAO.UserMaster", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Email");

                    b.Property<bool>("IsActive");

                    b.Property<DateTimeOffset?>("LastLogin");

                    b.Property<string>("Password");

                    b.Property<string>("PersonName")
                        .IsRequired();

                    b.Property<int?>("ProjectID");

                    b.Property<int>("RoleID");

                    b.Property<string>("Salt");

                    b.Property<int?>("SiteID");

                    b.Property<string>("UserName")
                        .IsRequired();

                    b.Property<int?>("VendorID");

                    b.HasKey("ID");

                    b.HasIndex("ProjectID");

                    b.HasIndex("RoleID");

                    b.HasIndex("SiteID");

                    b.HasIndex("UserName")
                        .IsUnique();

                    b.HasIndex("VendorID");

                    b.ToTable("UserMaster");
                });

            modelBuilder.Entity("astorWorkDAO.UserMRFAssociation", b =>
                {
                    b.Property<int>("UserID");

                    b.Property<int>("MRFID");

                    b.HasKey("UserID", "MRFID");

                    b.HasIndex("MRFID");

                    b.ToTable("UserMRFAssociation");
                });

            modelBuilder.Entity("astorWorkDAO.UserNotificationAssociation", b =>
                {
                    b.Property<int>("UserID");

                    b.Property<int>("NotificationID");

                    b.Property<int>("MyProperty");

                    b.HasKey("UserID", "NotificationID");

                    b.HasIndex("NotificationID");

                    b.ToTable("UserNotificationAssociation");
                });

            modelBuilder.Entity("astorWorkDAO.UserRequestAudit", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("RequestSuccess");

                    b.Property<DateTimeOffset>("RequestTime");

                    b.Property<int>("RequestType");

                    b.Property<int>("UserID");

                    b.HasKey("ID");

                    b.HasIndex("UserID");

                    b.ToTable("UserRequestAudit");
                });

            modelBuilder.Entity("astorWorkDAO.UserSessionAudit", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AccessToken")
                        .IsRequired();

                    b.Property<DateTimeOffset>("CreatedTime");

                    b.Property<int>("ExpireIn");

                    b.Property<string>("RefreshToken")
                        .IsRequired();

                    b.Property<int?>("UserMasterID");

                    b.HasKey("ID");

                    b.HasIndex("UserMasterID");

                    b.ToTable("UserSessionAudit");
                });

            modelBuilder.Entity("astorWorkDAO.VendorMaster", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("CycleDays")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(7);

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("ID");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("VendorMaster");
                });

            modelBuilder.Entity("astorWorkDAO.BIMSyncAudit", b =>
                {
                    b.HasOne("astorWorkDAO.ProjectMaster", "Project")
                        .WithMany()
                        .HasForeignKey("ProjectID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("astorWorkDAO.UserMaster", "SyncedBy")
                        .WithMany("UserBIMAudits")
                        .HasForeignKey("SyncedByID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("astorWorkDAO.InventoryAudit", b =>
                {
                    b.HasOne("astorWorkDAO.UserMaster", "CreatedBy")
                        .WithMany()
                        .HasForeignKey("CreatedByID");

                    b.HasOne("astorWorkDAO.ProjectMaster", "Project")
                        .WithMany()
                        .HasForeignKey("ProjectID");

                    b.HasOne("astorWorkDAO.TrackerMaster", "Tracker")
                        .WithMany()
                        .HasForeignKey("TrackerID");

                    b.HasOne("astorWorkDAO.UserMaster", "UpdatedBy")
                        .WithMany()
                        .HasForeignKey("UpdatedByID");

                    b.HasOne("astorWorkDAO.VendorMaster", "Vendor")
                        .WithMany()
                        .HasForeignKey("VendorID");
                });

            modelBuilder.Entity("astorWorkDAO.LocationMaster", b =>
                {
                    b.HasOne("astorWorkDAO.SiteMaster", "Site")
                        .WithMany("Locations")
                        .HasForeignKey("SiteID");

                    b.HasOne("astorWorkDAO.VendorMaster", "Vendor")
                        .WithMany()
                        .HasForeignKey("VendorID");
                });

            modelBuilder.Entity("astorWorkDAO.MaterialDrawingAssociation", b =>
                {
                    b.HasOne("astorWorkDAO.MaterialDrawingAudit", "Drawing")
                        .WithMany()
                        .HasForeignKey("DrawingID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("astorWorkDAO.MaterialMaster", "Material")
                        .WithMany("DrawingAssociations")
                        .HasForeignKey("MaterialID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("astorWorkDAO.MaterialInfoAudit", b =>
                {
                    b.HasOne("astorWorkDAO.UserMaster", "CreatedBy")
                        .WithMany()
                        .HasForeignKey("CreatedByID");

                    b.HasOne("astorWorkDAO.MaterialMaster", "Material")
                        .WithMany()
                        .HasForeignKey("MaterialID");

                    b.HasOne("astorWorkDAO.UserMaster", "UpdatedBy")
                        .WithMany()
                        .HasForeignKey("UpdatedByID");
                });

            modelBuilder.Entity("astorWorkDAO.MaterialMaster", b =>
                {
                    b.HasOne("astorWorkDAO.MRFMaster", "MRF")
                        .WithMany("Materials")
                        .HasForeignKey("MRFID");

                    b.HasOne("astorWorkDAO.ProjectMaster", "Project")
                        .WithMany()
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("astorWorkDAO.TrackerMaster", "Tracker")
                        .WithMany()
                        .HasForeignKey("TrackerID");

                    b.HasOne("astorWorkDAO.VendorMaster", "Vendor")
                        .WithMany()
                        .HasForeignKey("VendorId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("astorWorkDAO.MaterialQCCase", b =>
                {
                    b.HasOne("astorWorkDAO.UserMaster", "CreatedBy")
                        .WithMany()
                        .HasForeignKey("CreatedByID");

                    b.HasOne("astorWorkDAO.MaterialStageAudit", "StageAudit")
                        .WithMany("QCCases")
                        .HasForeignKey("StageAuditId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("astorWorkDAO.MaterialQCDefect", b =>
                {
                    b.HasOne("astorWorkDAO.UserMaster", "CreatedBy")
                        .WithMany()
                        .HasForeignKey("CreatedByID");

                    b.HasOne("astorWorkDAO.MaterialQCCase", "QCCase")
                        .WithMany("Defects")
                        .HasForeignKey("QCCaseId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("astorWorkDAO.MaterialQCPhotos", b =>
                {
                    b.HasOne("astorWorkDAO.UserMaster", "CreatedBy")
                        .WithMany()
                        .HasForeignKey("CreatedByID");

                    b.HasOne("astorWorkDAO.MaterialQCDefect")
                        .WithMany("Photos")
                        .HasForeignKey("MaterialQCDefectID");
                });

            modelBuilder.Entity("astorWorkDAO.MaterialStageAudit", b =>
                {
                    b.HasOne("astorWorkDAO.UserMaster", "CreatedBy")
                        .WithMany()
                        .HasForeignKey("CreatedByID");

                    b.HasOne("astorWorkDAO.LocationMaster", "Location")
                        .WithMany()
                        .HasForeignKey("LocationID");

                    b.HasOne("astorWorkDAO.MaterialMaster", "MaterialMaster")
                        .WithMany("StageAudits")
                        .HasForeignKey("MaterialMasterID");

                    b.HasOne("astorWorkDAO.MaterialStageMaster", "Stage")
                        .WithMany()
                        .HasForeignKey("StageID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("astorWorkDAO.ModuleMaster", b =>
                {
                    b.HasOne("astorWorkDAO.ModuleMaster", "ParentModule")
                        .WithMany()
                        .HasForeignKey("ParentModuleID");
                });

            modelBuilder.Entity("astorWorkDAO.MRFMaster", b =>
                {
                    b.HasOne("astorWorkDAO.UserMaster", "CreatedBy")
                        .WithMany()
                        .HasForeignKey("CreatedByID");

                    b.HasOne("astorWorkDAO.UserMaster", "UpdatedBy")
                        .WithMany()
                        .HasForeignKey("UpdatedByID");
                });

            modelBuilder.Entity("astorWorkDAO.PageMaster", b =>
                {
                    b.HasOne("astorWorkDAO.ModuleMaster", "Module")
                        .WithMany("Pages")
                        .HasForeignKey("ModuleMasterID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("astorWorkDAO.RoleMaster", b =>
                {
                    b.HasOne("astorWorkDAO.PageMaster", "DefaultPage")
                        .WithMany()
                        .HasForeignKey("DefaultPageID");
                });

            modelBuilder.Entity("astorWorkDAO.RolePageAssociation", b =>
                {
                    b.HasOne("astorWorkDAO.PageMaster", "Page")
                        .WithMany()
                        .HasForeignKey("PageId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("astorWorkDAO.RoleMaster", "Role")
                        .WithMany("RolePageAssociations")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("astorWorkDAO.SiteMaster", b =>
                {
                    b.HasOne("astorWorkDAO.VendorMaster", "Vendor")
                        .WithMany()
                        .HasForeignKey("VendorID");
                });

            modelBuilder.Entity("astorWorkDAO.UserMaster", b =>
                {
                    b.HasOne("astorWorkDAO.ProjectMaster", "Project")
                        .WithMany()
                        .HasForeignKey("ProjectID");

                    b.HasOne("astorWorkDAO.RoleMaster", "Role")
                        .WithMany()
                        .HasForeignKey("RoleID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("astorWorkDAO.SiteMaster", "Site")
                        .WithMany()
                        .HasForeignKey("SiteID");

                    b.HasOne("astorWorkDAO.VendorMaster", "Vendor")
                        .WithMany()
                        .HasForeignKey("VendorID");
                });

            modelBuilder.Entity("astorWorkDAO.UserMRFAssociation", b =>
                {
                    b.HasOne("astorWorkDAO.MRFMaster", "MRF")
                        .WithMany("UserMRFAssociations")
                        .HasForeignKey("MRFID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("astorWorkDAO.UserMaster", "User")
                        .WithMany("UserMRFAssociations")
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("astorWorkDAO.UserNotificationAssociation", b =>
                {
                    b.HasOne("astorWorkDAO.NotificationAudit", "Notification")
                        .WithMany("UserNotificationAssociation")
                        .HasForeignKey("NotificationID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("astorWorkDAO.UserMaster", "Receipient")
                        .WithMany("UserNotificationAssociation")
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("astorWorkDAO.UserRequestAudit", b =>
                {
                    b.HasOne("astorWorkDAO.UserMaster", "User")
                        .WithMany()
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("astorWorkDAO.UserSessionAudit", b =>
                {
                    b.HasOne("astorWorkDAO.UserMaster")
                        .WithMany("UserSessionAudits")
                        .HasForeignKey("UserMasterID");
                });
#pragma warning restore 612, 618
        }
    }
}
