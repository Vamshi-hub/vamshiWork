using astorWorkDAO;
using astorWorkDAO.Data;
using astorWorkShared.MultiTenancy;
using astorWorkShared.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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

            try
            {
                foreach (var tenant in tenantInfos)
                {
                    Console.WriteLine("Make sure db is migrated for Tenant: " + tenant.RowKey);
                    Console.WriteLine(tenant.ConnectionString);
                    using (var _astorWorkDbContext = new astorWorkDbContext(tenant))
                    {
                        await _astorWorkDbContext.Database.MigrateAsync();
                        Console.WriteLine("DB Migration Done");
                        DbInitializer.Initialize(_astorWorkDbContext);

                        var roles = await _astorWorkDbContext.RoleMaster.ToListAsync();

                        if (! await _astorWorkDbContext.PageMaster.AnyAsync(pm => pm.UrlPath == "qc-cases/:id"))
                        {
                            var page = new PageMaster
                            {
                                Name = "List QC Cases",
                                UrlPath = "qc-cases/:id",
                                ModuleMasterID = 1
                            };
                            await _astorWorkDbContext.PageMaster.AddAsync(page);
                            // GenerateFullAccessRights(_astorWorkDbContext, pageMaster);

                            foreach(var role in roles.FindAll(r => r.ID == 2 || r.ID == 3 || r.ID == 4))
                            {
                                var rolePageAssociation = new RolePageAssociation
                                {
                                    Page = page,
                                    Role = role,
                                    AccessLevel = 3
                                };
                                await _astorWorkDbContext.RolePageAssociation.AddAsync(rolePageAssociation);
                            }
                        }

                        if (!await _astorWorkDbContext.PageMaster.AnyAsync(pm => pm.UrlPath == "qc-defects/:id"))
                        {
                            var page = new PageMaster
                            {
                                Name = "List QC Defects",
                                UrlPath = "qc-defects/:id",
                                ModuleMasterID = 1
                            };
                            await _astorWorkDbContext.PageMaster.AddAsync(page);
                            //GenerateFullAccessRights(_astorWorkDbContext, pageMaster);

                            foreach (var role in roles.FindAll(r => r.ID == 2 || r.ID == 3 || r.ID == 4))
                            {
                                var rolePageAssociation = new RolePageAssociation
                                {
                                    Page = page,
                                    Role = role,
                                    AccessLevel = 3
                                };
                                await _astorWorkDbContext.RolePageAssociation.AddAsync(rolePageAssociation);
                            }
                        }

                        if (!await _astorWorkDbContext.PageMaster.AnyAsync(pm => pm.UrlPath == "import-material"))
                        {
                            var page = new PageMaster
                            {
                                Name = "Import Materials",
                                UrlPath = "import-material",
                                ModuleMasterID = 2
                            };
                            await _astorWorkDbContext.PageMaster.AddAsync(page);
                            // GenerateFullAccessRights(_astorWorkDbContext, pageMaster);

                            foreach (var role in roles.FindAll(r => r.ID == 1 || r.ID == 2))
                            {
                                var rolePageAssociation = new RolePageAssociation
                                {
                                    Page = page,
                                    Role = role,
                                    AccessLevel = 3
                                };
                                await _astorWorkDbContext.RolePageAssociation.AddAsync(rolePageAssociation);
                            }
                        }

                        if (!await _astorWorkDbContext.PageMaster.AnyAsync(pm => pm.UrlPath == "notification-config"))
                        {
                            var page = new PageMaster
                            {
                                Name = "Notification Config",
                                UrlPath = "notification-config",
                                ModuleMasterID = 2
                            };
                            await _astorWorkDbContext.PageMaster.AddAsync(page);
                            // GenerateFullAccessRights(_astorWorkDbContext, page);
                            foreach (var role in roles.FindAll(r => r.ID == 1 || r.ID == 2))
                            {
                                var rolePageAssociation = new RolePageAssociation
                                {
                                    Page = page,
                                    Role = role,
                                    AccessLevel = 3
                                };
                                await _astorWorkDbContext.RolePageAssociation.AddAsync(rolePageAssociation);
                            }
                        }

                        if (!await _astorWorkDbContext.PageMaster.AnyAsync(pm => pm.UrlPath == "list-reports"))
                        {
                            var page = new PageMaster
                            {
                                Name = "List Reports",
                                UrlPath = "list-reports",
                                ModuleMasterID = 1
                            };
                            await _astorWorkDbContext.PageMaster.AddAsync(page);
                            GenerateFullAccessRights(_astorWorkDbContext, page);
                        }

                        if (!await _astorWorkDbContext.PageMaster.AnyAsync(pm => pm.UrlPath == "powerbi-viewer"))
                        {
                            var page = new PageMaster
                            {
                                Name = "View Report",
                                UrlPath = "powerbi-viewer/:guid",
                                ModuleMasterID = 1
                            };
                            await _astorWorkDbContext.PageMaster.AddAsync(page);
                            GenerateFullAccessRights(_astorWorkDbContext, page);
                        }

                        await _astorWorkDbContext.SaveChangesAsync();
                        // PopulateDefaultData(_astorWorkDbContext);
                        // Console.WriteLine("Data populating" + tenant.TenantName);
                        //_astorWorkDbContext.Database.CommitTransaction();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
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
            foreach (var role in _astorWorkDbContext.RoleMaster)
            {
                var rolePageAssociation = new RolePageAssociation
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
                foreach (var m in modules)
                {
                    var module = astorWorkDbContext.ModuleMaster.Include(p => p.Pages).FirstOrDefaultAsync(p => p.Name == m.Name).Result;
                    if (module == null)
                        astorWorkDbContext.ModuleMaster.Add(m);
                    else
                    {
                        module.Pages = m.Pages;
                        astorWorkDbContext.Entry(module).State = EntityState.Modified;
                    }

                }

                RoleMaster[] roles = DbInitializer.InsertRoles(modules);
                foreach (var r in roles)
                {
                    var role = astorWorkDbContext.RoleMaster.FirstOrDefaultAsync(p => p.Name == r.Name).Result;
                    if (role == null)
                        astorWorkDbContext.RoleMaster.Add(r);
                }

                UserMaster[] users = DbInitializer.InsertUserMasters(roles);
                foreach (var u in users)
                {
                    var user = astorWorkDbContext.UserMaster.FirstOrDefaultAsync(p => p.UserName == u.UserName).Result;
                    if (user == null)
                        astorWorkDbContext.UserMaster.AddAsync(u);
                }

                var stages = DbInitializer.InsertMaterialStages();
                foreach (var s in stages)
                {
                    var user = astorWorkDbContext.MaterialStageMaster.FirstOrDefaultAsync(p => p.Name == s.Name).Result;
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
