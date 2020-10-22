using astorWorkDAO.Data;
using astorWorkShared.GlobalModels;
using astorWorkShared.Services;
using astorWorkShared.Utilities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace astorWorkDAO.DBSchemaSync
{
    public class DBSchemaSync
    {
        public static async Task MigrateAll()
        {
            AstorWorkTableStorage tableStorage = new AstorWorkTableStorage();
            List<TenantInfo> tenantInfos = await tableStorage.ListTenants();

            foreach (TenantInfo tenant in tenantInfos)
            {
                try
                {
                    Console.WriteLine("Make sure db is migrated for Tenant: " + tenant.RowKey);
                    Console.WriteLine(tenant.ConnectionString);
                    using (var context = new astorWorkDbContext(tenant))
                    {
                        await context.Database.MigrateAsync();
                        Console.WriteLine("DB Migration Done, start initializing data");
                        string error = DbInitializer.Initialize(context);
                        if (!string.IsNullOrEmpty(error))
                            Console.WriteLine(error);
                    }
                }
                catch (Exception ex)
                {
                    Console.Write(ExceptionUtility.GetExceptionDetails(ex));
                }
            }
            //tableStorage.GetTenantInfo("test");
            //Console.WriteLine("Data populating Completed");
            //TenantInfo tenant = tableStorage.GetTenantInfo("test").Result;
            //tenant.ConnectionString = GetConnection(tenant);
            //astorWorkDbContext astorWorkDbContext = new astorWorkDbContext(tenant);
            //astorWorkDbContext.Database.Migrate();
            //Console.WriteLine("Make sure db is migrated" + tenant.TenantName);
            //PopulateDefaultData(astorWorkDbContext);
            //Console.WriteLine("Data populating" + tenant.TenantName);
        }

        private async static void GenerateFullAccessRights(astorWorkDbContext _astorWorkDbContext, PageMaster page)
        {
            foreach (RoleMaster role in _astorWorkDbContext.RoleMaster)
            {
                RolePageAssociation rolePageAssociation = new RolePageAssociation
                {
                    Page = page,
                    Role = role,
                    AccessLevel = 3
                };
                await _astorWorkDbContext.RolePageAssociation.AddAsync(rolePageAssociation);
            }
        }

        public string GetConnection(TenantInfo _appTenant)
        {
            SqlConnectionStringBuilder _sqlConBuilder = new SqlConnectionStringBuilder();
            _sqlConBuilder.DataSource = _appTenant.DBServer;
            _sqlConBuilder.InitialCatalog = _appTenant.DBName;
            _sqlConBuilder.UserID = _appTenant.DBUserID;
            _sqlConBuilder.Password = _appTenant.DBPassword;
            _sqlConBuilder.ConnectTimeout = 300;
            return _sqlConBuilder.ConnectionString.ToString();
        }

        public void PopulateDefaultData(astorWorkDbContext astorWorkDbContext)
        {
            try
            {

                ModuleMaster[] modules = DbInitializer.InserModuleMaster();
                foreach (ModuleMaster m in modules)
                {
                    ModuleMaster module = astorWorkDbContext.ModuleMaster.Include(p => p.Pages).FirstOrDefaultAsync(p => p.Name == m.Name).Result;
                    if (module == null)
                        astorWorkDbContext.ModuleMaster.Add(m);
                    else
                    {
                        module.Pages = m.Pages;
                        astorWorkDbContext.Entry(module).State = EntityState.Modified;
                    }

                }

                RoleMaster[] roles = DbInitializer.InsertRoles(modules);
                foreach (RoleMaster r in roles)
                {
                    RoleMaster role = astorWorkDbContext.RoleMaster.FirstOrDefaultAsync(p => p.Name == r.Name).Result;
                    if (role == null)
                        astorWorkDbContext.RoleMaster.Add(r);
                }

                UserMaster[] users = DbInitializer.InsertUserMasters(roles);
                foreach (UserMaster u in users)
                {
                    UserMaster user = astorWorkDbContext.UserMaster.FirstOrDefaultAsync(p => p.UserName == u.UserName).Result;
                    if (user == null)
                        astorWorkDbContext.UserMaster.AddAsync(u);
                }

                MaterialStageMaster[] stages = DbInitializer.InsertMaterialStages();
                foreach (MaterialStageMaster s in stages)
                {
                    MaterialStageMaster user = astorWorkDbContext.MaterialStageMaster.FirstOrDefaultAsync(p => p.Name == s.Name).Result;
                    if (user == null)
                        astorWorkDbContext.MaterialStageMaster.Add(s);
                }
                astorWorkDbContext.SaveChangesAsync().Wait();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
