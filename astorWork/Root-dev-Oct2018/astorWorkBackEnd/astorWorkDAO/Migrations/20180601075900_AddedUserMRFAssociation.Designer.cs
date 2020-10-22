﻿// <auto-generated />
using astorWorkDAO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using System;

namespace astorWorkDAO.Migrations
{
    [DbContext(typeof(astorWorkDbContext))]
    [Migration("20180601075900_AddedUserMRFAssociation")]
    partial class AddedUserMRFAssociation
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.3-rtm-10026")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("astorWorkDAO.BIMSyncAudit", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

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
                        .ValueGeneratedOnAdd();

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
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("ID");

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
                        .ValueGeneratedOnAdd();

                    b.Property<DateTimeOffset>("DrawingIssueDate");

                    b.Property<string>("DrawingNo");

                    b.Property<int>("RevisionNo");

                    b.HasKey("ID");

                    b.ToTable("MaterialDrawingAudit");
                });

            modelBuilder.Entity("astorWorkDAO.MaterialInfoAudit", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

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
                        .ValueGeneratedOnAdd();

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

                    b.Property<int>("ProjectID");

                    b.Property<int>("SN");

                    b.Property<int?>("TrackerID");

                    b.Property<int?>("VendorID");

                    b.Property<string>("Zone")
                        .IsRequired();

                    b.HasKey("ID");

                    b.HasIndex("MRFID");

                    b.HasIndex("ProjectID");

                    b.HasIndex("TrackerID");

                    b.HasIndex("VendorID");

                    b.HasIndex("MarkingNo", "Block", "Level", "Zone")
                        .IsUnique();

                    b.ToTable("MaterialMaster");
                });

            modelBuilder.Entity("astorWorkDAO.MaterialStageAudit", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

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
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Colour")
                        .IsRequired();

                    b.Property<bool>("IsQCStage");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<int>("Order");

                    b.HasKey("ID");

                    b.ToTable("MaterialStageMaster");
                });

            modelBuilder.Entity("astorWorkDAO.ModuleMaster", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<string>("UrlPrefix")
                        .IsRequired();

                    b.HasKey("ID");

                    b.ToTable("ModuleMaster");
                });

            modelBuilder.Entity("astorWorkDAO.MRFMaster", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

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

            modelBuilder.Entity("astorWorkDAO.PageMaster", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("ModuleMasterID");

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
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("ID");

                    b.ToTable("ProjectMaster");
                });

            modelBuilder.Entity("astorWorkDAO.RoleMaster", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("DefaultPageID");

                    b.Property<int>("MobileEntryPoint");

                    b.Property<string>("Name");

                    b.Property<string>("PlatformCode");

                    b.HasKey("ID");

                    b.HasIndex("DefaultPageID");

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

            modelBuilder.Entity("astorWorkDAO.TrackerMaster", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

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
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Email");

                    b.Property<bool>("IsActive");

                    b.Property<DateTimeOffset?>("LastLogin");

                    b.Property<string>("Password");

                    b.Property<string>("PersonName")
                        .IsRequired();

                    b.Property<int?>("RoleID");

                    b.Property<string>("Salt");

                    b.Property<string>("UserName")
                        .IsRequired();

                    b.Property<int?>("VendorID");

                    b.HasKey("ID");

                    b.HasIndex("RoleID");

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

            modelBuilder.Entity("astorWorkDAO.UserSessionAudit", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AccessToken")
                        .IsRequired();

                    b.Property<DateTimeOffset>("CreatedTime");

                    b.Property<int>("ExpireIn");

                    b.Property<int?>("UserMasterID");

                    b.HasKey("ID");

                    b.HasIndex("UserMasterID");

                    b.ToTable("UserSessionAudit");
                });

            modelBuilder.Entity("astorWorkDAO.VendorMaster", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("CycleDays")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(7);

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("ID");

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
                        .HasForeignKey("ProjectID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("astorWorkDAO.TrackerMaster", "Tracker")
                        .WithMany()
                        .HasForeignKey("TrackerID");

                    b.HasOne("astorWorkDAO.VendorMaster", "Vendor")
                        .WithMany()
                        .HasForeignKey("VendorID");
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
                    b.HasOne("astorWorkDAO.ModuleMaster")
                        .WithMany("Pages")
                        .HasForeignKey("ModuleMasterID");
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

            modelBuilder.Entity("astorWorkDAO.UserMaster", b =>
                {
                    b.HasOne("astorWorkDAO.RoleMaster", "Role")
                        .WithMany()
                        .HasForeignKey("RoleID");

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
