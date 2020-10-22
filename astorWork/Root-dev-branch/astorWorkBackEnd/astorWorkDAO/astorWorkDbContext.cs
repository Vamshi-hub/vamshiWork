using astorWorkShared.GlobalModels;
using astorWorkShared.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace astorWorkDAO
{
    public class astorWorkDbContext : DbContext
    {
        private readonly TenantInfo _tenant;
        private readonly string _dbServer;
        private readonly string _dbName;
        private readonly string _dbUser;
        private readonly string _dbPassword;
        private readonly string _connectionString;

        public astorWorkDbContext(TenantInfo tenant)
        {
            _tenant = tenant;
        }

        public astorWorkDbContext(string dbServer, string dbName, string dbUser, string dbPassword, int commandTimeout = 30)
        {
            _dbServer = dbServer;
            _dbName = dbName;
            _dbUser = dbUser;
            _dbPassword = dbPassword;

            try
            {
                SqlConnectionStringBuilder _sqlConBuilder = new SqlConnectionStringBuilder();
                _sqlConBuilder.DataSource = _dbServer;
                _sqlConBuilder.InitialCatalog = _dbName;
                _sqlConBuilder.UserID = _dbUser;
                _sqlConBuilder.Password = _dbPassword;
                _sqlConBuilder.ConnectTimeout = 180;
                _connectionString = _sqlConBuilder.ConnectionString.ToString();

                Database.SetCommandTimeout(TimeSpan.FromSeconds(commandTimeout));

            }
            catch (Exception exc)
            {
                Debug.WriteLine(exc);
            }
        }

        public astorWorkDbContext()
        {

        }

        public DbSet<InventoryAudit> InventoryAudit { get; set; }
        public DbSet<MaterialMaster> MaterialMaster { get; set; }
        public DbSet<ProjectMaster> ProjectMaster { get; set; }
        public DbSet<MRFMaster> MRFMaster { get; set; }
        public DbSet<UserMRFAssociation> UserMRFAssociation { get; set; }
        public DbSet<MaterialStageMaster> MaterialStageMaster { get; set; }
        public DbSet<LocationMaster> LocationMaster { get; set; }
        public DbSet<ModuleMaster> ModuleMaster { get; set; }
        public DbSet<PageMaster> PageMaster { get; set; }
        public DbSet<RoleMaster> RoleMaster { get; set; }
        public DbSet<RolePowerBIReport> RolePowerBIReport { get; set; }
        public DbSet<UserMaster> UserMaster { get; set; }
        public DbSet<TrackerMaster> TrackerMaster { get; set; }
        public DbSet<MaterialStageAudit> MaterialStageAudit { get; set; }
        public DbSet<MaterialInfoAudit> MaterialInfoAudit { get; set; }
        public DbSet<MaterialDrawingAudit> MaterialDrawingAudit { get; set; }
        public DbSet<MaterialDrawingAssociation> MaterialDrawingAssociation { get; set; }
        public DbSet<RolePageAssociation> RolePageAssociation { get; set; }
        public DbSet<UserSessionAudit> UserSessionAudit { get; set; }
        public DbSet<BIMSyncAudit> BIMSyncAudit { get; set; }
        public DbSet<UserRequestAudit> UserRequestAudit { get; set; }
        public DbSet<MaterialQCPhotos> MaterialQCPhotos { get; set; }
        public DbSet<SiteMaster> SiteMaster { get; set; }

        public DbSet<MaterialQCCase> MaterialQCCase { get; set; }
        public DbSet<MaterialQCDefect> MaterialQCDefect { get; set; }

        public DbSet<NotificationAudit> NotificationAudit { get; set; }
        public DbSet<UserNotificationAssociation> UserNotificationAssociation { get; set; }
        public DbSet<SystemHealthMaster> SystemHealthMaster { get; set; }
        public DbSet<AttachmentMaster> AttachmentMaster { get; set; }
        public DbSet<NotificationTimerMaster> NotificationTimerMaster { get; set; }
        public DbSet<BIMForgeModel> BIMForgeModel { get; set; }
        public DbSet<BIMForgeElement> BIMForgeElement { get; set; }

        public DbSet<MaterialTypeMaster> MaterialTypeMaster { get; set; }
        public DbSet<TradeMaster> TradeMaster { get; set; }
        public DbSet<OrganisationMaster> OrganisationMaster { get; set; }
        public DbSet<ChecklistItemMaster> ChecklistItemMaster { get; set; }
        public DbSet<TradeMaterialTypeAssociation> TradeMaterialTypeAssociation { get; set; }
        public DbSet<ChecklistItemAssociation> ChecklistItemAssociation { get; set; }
        public DbSet<JobSchedule> JobSchedule { get; set; }
        public DbSet<JobAudit> JobAudit { get; set; }
        public DbSet<ChecklistMaster> ChecklistMaster { get; set; }
        public DbSet<ChecklistAudit> ChecklistAudit { get; set; }
        public DbSet<ChecklistItemAudit> ChecklistItemAudit { get; set; }

        public DbSet<ChatData> ChatData { get; set; }
        public DbSet<ChatUserAssociation> ChatUserAssociation { get; set; }
        public DbSet<NotificationSeenBy> NotificationSeenBy { get; set; }
        public DbSet<ConfigurationMaster> ConfigurationMaster { get; set; }
        public async Task<UserMaster> GetUserFromHttpContext(HttpContext httpContext)
        {
            if (httpContext != null)
            {
                var userClaim = httpContext.User.Claims.FirstOrDefault(cl => cl.Type.Equals(ClaimTypes.NameIdentifier));

                if (userClaim != null)
                    return await UserMaster.Include(um => um.Organisation)
                        .Include(um => um.Project)
                        .Include(um => um.Role)
                        .FirstOrDefaultAsync(um => um.ID.ToString() == userClaim.Value);
            }

            return null;
        }
       

        public string GetConnectionString()
        {
            if (_tenant != null)
                return _tenant.ConnectionString;
            else
                return _connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connection = GetConnectionString();
            if (string.IsNullOrEmpty(connection))
                optionsBuilder.UseSqlServer(AppConfiguration.GetDefaultSQLConn());
            else
                optionsBuilder.UseSqlServer(connection);

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<OrganisationMaster>()
                .Property(v => v.CycleDays)
                .HasDefaultValue(7);

            CreateUserMRFAssociation(modelBuilder);
            CreateMaterialDrawingAssociation(modelBuilder);
            CreateRolePageAssociation(modelBuilder);
            CreateUserNotificationAssociation(modelBuilder);

            modelBuilder.Entity<MaterialMaster>()
                .HasIndex(mm => new { mm.ProjectID, mm.MarkingNo, mm.Block, mm.Level, mm.Zone })
                .IsUnique();

            modelBuilder.Entity<UserMaster>()
                .HasIndex(um => um.UserName)
                .IsUnique();

            modelBuilder.Entity<SiteMaster>()
                .HasIndex(sm => sm.Name)
                .IsUnique();

            modelBuilder.Entity<LocationMaster>()
                .HasIndex(lm => lm.Name)
                .IsUnique();

            modelBuilder.Entity<RoleMaster>()
                .HasIndex(rm => rm.Name)
                .IsUnique();

            modelBuilder.Entity<OrganisationMaster>()
                .HasIndex(vm => vm.Name)
                .IsUnique();

            modelBuilder.Entity<ModuleMaster>()
                .HasIndex(mm => mm.UrlPrefix)
                .IsUnique();

            modelBuilder.Entity<ProjectMaster>()
                .HasIndex(pm => pm.Name)
                .IsUnique();

            modelBuilder.Entity<MaterialStageMaster>()
                .HasIndex(pm => pm.Name)
                .IsUnique();

            modelBuilder.Entity<TrackerMaster>()
                .HasIndex(tm => tm.Tag)
                .IsUnique();

        }

        private static void CreateUserMRFAssociation(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserMRFAssociation>()
                .HasKey(um => new { um.UserID, um.MRFID });

            modelBuilder.Entity<UserMRFAssociation>()
                .HasOne(um => um.MRF)
                .WithMany(u => u.UserMRFAssociations)
                .HasForeignKey(um => um.MRFID);

            modelBuilder.Entity<UserMRFAssociation>()
                .HasOne(um => um.User)
                .WithMany(m => m.UserMRFAssociations)
                .HasForeignKey(um => um.UserID);
        }

        private static void CreateMaterialDrawingAssociation(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MaterialDrawingAssociation>()
                .HasKey(md => new { md.MaterialID, md.DrawingID });

            modelBuilder.Entity<MaterialDrawingAssociation>()
                .HasOne(md => md.Material)
                .WithMany(md => md.DrawingAssociations)
                .HasForeignKey(md => md.MaterialID);

            modelBuilder.Entity<MaterialDrawingAssociation>()
                .HasOne(md => md.Drawing)
                .WithMany()
                .HasForeignKey(md => md.DrawingID);
        }

        private static void CreateRolePageAssociation(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RolePageAssociation>()
                .HasKey(rp => new { rp.RoleId, rp.PageId });

            modelBuilder.Entity<RolePageAssociation>()
                .HasOne(rp => rp.Role)
                .WithMany(rp => rp.RolePageAssociations)
                .HasForeignKey(rp => rp.RoleId);

            modelBuilder.Entity<RolePageAssociation>()
                .HasOne(rp => rp.Page)
                .WithMany()
                .HasForeignKey(rp => rp.PageId);
        }

        private static void CreateUserNotificationAssociation(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserNotificationAssociation>()
                .HasKey(rp => new { rp.UserID, rp.NotificationID });

            modelBuilder.Entity<UserNotificationAssociation>()
                .HasOne(una => una.Receipient)
                .WithMany(r => r.UserNotificationAssociation)
                .HasForeignKey(una => una.UserID);

            modelBuilder.Entity<UserNotificationAssociation>()
                .HasOne(una => una.Notification)
                .WithMany(n => n.UserNotificationAssociation)
                .HasForeignKey(una => una.NotificationID);
        }

        public List<AttachmentMaster> GetShopDrawings(MRFMaster mrf)
        {
            List<AttachmentMaster> result = new List<AttachmentMaster>();
            if (mrf != null)
            {
                result = AttachmentMaster.Where(am => am.Type == (int)Enums.AttachmentType.MRFShopDrawing &&
                am.Reference == mrf.ID.ToString()).ToList();
            }

            return result;
        }

        public string GetPageFullUrl(PageMaster page)
        {
            var result = string.Empty;
            if (page != null)
            {
                var urlBuilder = new StringBuilder(page.UrlPath);
                var module = ModuleMaster.Find(page.ModuleMasterID);
                while (module != null)
                {
                    urlBuilder.Insert(0, "/");
                    urlBuilder.Insert(0, module.UrlPrefix);
                    urlBuilder.Insert(0, "/");
                    module = ModuleMaster.Find(module.ParentModuleID);
                }
                result = urlBuilder.ToString();
            }
            return result;
        }

        public void OpenConn()
        {
            Database.OpenConnection();
        }

        public void CloseConn()
        {
            Database.CloseConnection();
        }

        public void DropDB()
        {
            Database.EnsureDeleted();
        }

        public void MigrateDB()
        {

            Database.Migrate();
        }

        public void GrantDBOwner(string loginName)
        {
            if (!string.IsNullOrEmpty(loginName))
            {
                var command1 =
                    $"CREATE USER [{loginName}] FROM LOGIN [{loginName}]";
                Database.ExecuteSqlCommand(command1);

                var command2 = $"sp_addrolemember 'db_owner', {loginName}";
                Database.ExecuteSqlCommand(command2);
            }
        }
    }
}
