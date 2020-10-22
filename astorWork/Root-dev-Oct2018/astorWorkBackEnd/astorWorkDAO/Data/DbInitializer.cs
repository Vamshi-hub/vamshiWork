using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace astorWorkDAO.Data
{
    public static class DbInitializer
    {
        private static readonly string[] LIST_MATERIAL_TYPES = 
            { "Facade", "Rebar", "Slab", "Beam", "Plank", "Wall Panel" };

        public static void PopulateBasicData(astorWorkDbContext context)
        {
            Console.WriteLine("Start populating basic data");
            ModuleMaster[] modules = InserModuleMaster();
            context.ModuleMaster.AddRange(modules);

            var stages = InsertMaterialStages();
            context.MaterialStageMaster.AddRange(stages);

            RoleMaster[] roles = InsertRoles(modules);
            context.RoleMaster.AddRange(roles);

            UserMaster[] users = InsertUserMasters(roles);
            context.UserMaster.AddRange(users);

            context.Database.OpenConnection();
            try
            {
                context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [RoleMaster] ON");
                context.SaveChanges();
                context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [RoleMaster] OFF");
            }
            finally
            {
                context.Database.CloseConnection();
            }

            Console.WriteLine("Done populating basic data");
        }

        public static void PopulateDemoSetup(astorWorkDbContext context)
        {
            Console.WriteLine("Start populating demo setup");
            var pwdHashed = string.Empty;
            var salt = Guid.NewGuid();

            using (var algorithm = SHA256.Create())
            {
                var pwd = "abc12345";
                var pwdSalted = Encoding.UTF8.GetBytes(pwd).Concat(salt.ToByteArray());
                var hash = algorithm.ComputeHash(pwdSalted.ToArray());
                pwdHashed = Convert.ToBase64String(hash);
            }

            var roles = context.RoleMaster.OrderBy(r => r.ID).ToArray();

            // Generate vendors
            var vendors = new VendorMaster[]
            {
                    new VendorMaster{ Name = "Reinforcing Asia", CycleDays=5 },
                    new VendorMaster{ Name = "Pre-cast Technology", CycleDays=7 },
            };
            context.VendorMaster.AddRange(vendors);

            // Generate projects
            var projects = new ProjectMaster[]
            {
                new ProjectMaster {
                    Name = "Smart Home @ Punggol",
                    TimeZoneOffset = 480,
                    Description = "5 experimental HDB blocks that are equipped with Smart Home devices",
                    Country = "SG",
                    EstimatedStartDate = DateTimeOffset.Now.AddDays(-27),
                    EstimatedEndDate = DateTimeOffset.Now.AddDays(341)
                },
                new ProjectMaster {
                    Name = "Uppal New Mall",
                    TimeZoneOffset = 330,
                    Country = "IN",
                    EstimatedStartDate = DateTimeOffset.Now.AddDays(-107),
                    EstimatedEndDate = DateTimeOffset.Now.AddDays(441)
                }
            };
            context.ProjectMaster.AddRange(projects);

            // Generate sites
            var sites = new SiteMaster[]
            {
                new SiteMaster
                {
                    Country = "MY",
                    TimeZoneOffset = 480,
                    Description = "Reinforcing Asia's Headquarter",
                    Name = "RA Malaysia",
                    Vendor = vendors[0],
                    Locations = new List<LocationMaster>
                    {
                        new LocationMaster
                        {
                            Name = "RA Factory",
                            Type = 0,
                            Description = "Reinforcing Asia's factory in Malaysia",
                            Vendor = vendors[0]
                        }
                    }
                },
                new SiteMaster
                {
                    Country = "IT",
                    TimeZoneOffset = 120,
                    Description = "Pre-cast Technology's Headquarter",
                    Name = "PT Italy",
                    Vendor = vendors[1],
                    Locations = new List<LocationMaster>
                    {
                        new LocationMaster
                        {
                            Name = "PT Factory",
                            Type = 0,
                            Description = "Pre-cast Technology's factory in Italy",
                            Vendor = vendors[1]
                        }
                    }
                },
                new SiteMaster
                {
                    Country = "SG",
                    TimeZoneOffset = 120,
                    Description = "Job site for Smart Home @ Punggol",
                    Name = "Punggol job site",
                    Locations = new List<LocationMaster>
                    {
                        new LocationMaster
                        {
                            Name = "Punggol storage area",
                            Type = 1,
                            Description = "Storage area for the Punggol job site"
                        },
                        new LocationMaster
                        {
                            Name = "Smart Home construction area",
                            Type = 2,
                            Description = "Construction area for the Punggol job site"
                        }
                    }
                },
                new SiteMaster
                {
                    Country = "IN",
                    TimeZoneOffset = 330,
                    Description = "Site for all Hyderabad projects",
                    Name = "Hyderabad site",
                    Locations = new List<LocationMaster>
                    {
                        new LocationMaster
                        {
                            Name = "Hyderabad storage area",
                            Type = 1,
                            Description = "Storage area for the all Hyderabad projects"
                        },
                        new LocationMaster
                        {
                            Name = "Uppal construction area",
                            Type = 2,
                            Description = "Construction area for the Uppal New Mall"
                        }
                    }
                }
            };
            context.SiteMaster.AddRange(sites);

            // Generate trackers
            Random rnd = new Random();
            string[] trackerTypes = { "RFID", "QR Code", "Beacon" };
            TrackerMaster[] trackers = new TrackerMaster[120];
            for (int i = 0; i < 120; i++)
            {
                int trackerType = rnd.Next(0, 3);
                trackers[i] = new TrackerMaster
                {
                    Type = trackerTypes[trackerType],
                    Tag = Guid.NewGuid().ToString(),
                    Label = trackerTypes[trackerType].Substring(0, 1) + i.ToString("D4")
                };
            }
            context.TrackerMaster.AddRange(trackers);

            // Generate users
            var users = new UserMaster[]
            {
                // Main-con users
                new UserMaster { UserName = "admin", Email = "admin@lsb.com",
                    PersonName = "Lim Seng Builder's Admin", RoleID = 2,
                    Password = pwdHashed, Salt = salt.ToString(), IsActive=true
                },
                new UserMaster { UserName = "charlotte", Email = "charlotte@lsb.com",
                    PersonName = "Charlotte Williams", RoleID = 3,
                    Password = pwdHashed, Salt = salt.ToString(), IsActive=true
                },
                new UserMaster { UserName = "charlie", Email = "charlie@lsb.com",
                    PersonName = "Charlie Murphy", RoleID = 4, Project = projects[0],
                    Password = pwdHashed, Salt = salt.ToString(), IsActive=true
                },
                new UserMaster { UserName = "michael", Email = "michael@lsb.com",
                    PersonName = "Michael Davis", RoleID = 4, Project = projects[1],
                    Password = pwdHashed, Salt = salt.ToString(), IsActive=true
                },
                new UserMaster { UserName = "susan", Email = "susan@lsb.com",
                    PersonName = "Susan Smith", RoleID = 5, Project = projects[0], Site = sites[2],
                    Password = pwdHashed, Salt = salt.ToString(), IsActive=true
                },
                new UserMaster { UserName = "kyle", Email = "kyle@lsb.com",
                    PersonName = "Kyle Brown", RoleID = 5, Project = projects[1], Site = sites[3],
                    Password = pwdHashed, Salt = salt.ToString(), IsActive=true
                },
                new UserMaster { UserName = "mia", Email = "mia@lsb.com",
                    PersonName = "Mia Rodriguez", RoleID = 6,
                    Password = pwdHashed, Salt = salt.ToString(), IsActive=true
                },
                // Vendor users
                new UserMaster{ UserName = "amelia", Email = "amelia@ra.com", PersonName = "Amelia Taylor", RoleID = 7, Password = pwdHashed, Salt = salt.ToString(),IsActive=true,
                    Vendor = vendors[0], Site = sites[0] },
                new UserMaster{ UserName = "oliver", Email = "oliver@ra.com", PersonName = "Oliver Jones", RoleID = 8, Password = pwdHashed, Salt = salt.ToString(),IsActive=true,
                    Vendor = vendors[0], Site = sites[0] },
                new UserMaster{ UserName = "jacob", Email = "jacob@ra.com", PersonName = "Jacob Jones", RoleID = 8, Password = pwdHashed, Salt = salt.ToString(),IsActive=true,
                    Vendor = vendors[0], Site = sites[0] },
                new UserMaster{ UserName = "jessica", Email = "jessica@pt.com", PersonName = "Jessica Wilson", RoleID = 7, Password = pwdHashed, Salt = salt.ToString(),IsActive=true,
                    Vendor = vendors[1], Site = sites[1] },
                new UserMaster{ UserName = "james", Email = "james@pt.com", PersonName = "James Evans", RoleID = 8, Password = pwdHashed, Salt = salt.ToString(),IsActive=true,
                    Vendor = vendors[1], Site = sites[1] },
                new UserMaster{ UserName = "jack", Email = "jack@pt.com", PersonName = "Jack Thomas", RoleID = 8,
                    Password = pwdHashed, Salt = salt.ToString(),IsActive=true,
                    Vendor = vendors[1], Site = sites[1] }
            };
            context.UserMaster.AddRange(users);

            context.SaveChanges();
            Console.WriteLine("Done populating demo data");
        }

        public static void PopulateDemoMaterials(astorWorkDbContext context)
        {
            Console.WriteLine("Start populating demo materials");

            var projects = context.ProjectMaster.ToArray();
            var vendors = context.VendorMaster.ToArray();
            var materials = new List<MaterialMaster>();

            var materialDrawingAudits = InsertDrawings();
            context.MaterialDrawingAudit.AddRange(materialDrawingAudits);

            // Populate for Punggol HDB blocks
            int maxSN = 0;
            for (int blkIndex = 1; blkIndex <= 10; blkIndex++)
            {
                for (int levelIndex = 0; levelIndex < 30; levelIndex++)
                {
                    Random rnd = new Random();
                    int zoneCount = rnd.Next(1, 5);

                    for (int zoneIndex = 0; zoneIndex < zoneCount; zoneIndex++)
                    {
                        for (int materialIndex = 0; materialIndex < 6; materialIndex++)
                        {
                            int vendorIndex = rnd.Next(0, vendors.Length);
                            int materialTypeIndex = rnd.Next(0, 6);

                            maxSN++;

                            var material = GenerateMaterialMaster(materialDrawingAudits, projects[0].Name, projects[0].ID, vendors[vendorIndex].ID,
                                $"BLK {blkIndex}", levelIndex, zoneIndex, materialTypeIndex, materialIndex, maxSN);

                            materials.Add(material);
                        }
                    }
                }
            }

            // Populate for Uppal New Mall
            materialDrawingAudits = InsertDrawings();
            context.MaterialDrawingAudit.AddRange(materialDrawingAudits);

            maxSN = 0;
            var blkNames = new string[] { "Mall", "Office Building", "Residence" };
            for (int blkIndex = 0; blkIndex < 3; blkIndex++)
            {
                for (int levelIndex = 0; levelIndex < 6; levelIndex++)
                {
                    Random rnd = new Random();
                    int zoneCount = rnd.Next(1, 5);
                    // int trackerIndex = rnd.Next(0, trackerCount);

                    for (int zoneIndex = 0; zoneIndex < zoneCount; zoneIndex++)
                    {
                        for (int materialIndex = 0; materialIndex < 6; materialIndex++)
                        {
                            int vendorIndex = rnd.Next(0, vendors.Length);
                            int materialTypeIndex = rnd.Next(0, 6);

                            maxSN++;

                            var material = GenerateMaterialMaster(materialDrawingAudits, projects[1].Name,
                                projects[1].ID, vendors[vendorIndex].ID,
                                blkNames[blkIndex], levelIndex, zoneIndex, materialTypeIndex, materialIndex, maxSN);

                            materials.Add(material);
                        }
                    }
                }
            }
            context.MaterialMaster.AddRange(materials);

            context.SaveChanges();
            Console.WriteLine("Done populating demo data");
        }

        public static void PopulateDemoProgress(astorWorkDbContext context)
        {
            Console.WriteLine("Start populating demo progress");
            Random rnd = new Random();
            var vendorPMs = context.UserMaster.Include(um => um.Vendor).Where(um => um.RoleID == 7).ToList();
            var vendorOfficers = context.UserMaster.Include(um => um.Vendor).Where(um => um.RoleID == 8).ToList();
            var siteOfficers = context.UserMaster.Include(um => um.Project).Include(um => um.Site).Where(um => um.RoleID == 5).ToList();
            var trackers = context.TrackerMaster.ToList();
            var stages = context.MaterialStageMaster.ToList();
            var locations = context.LocationMaster.Include(loc => loc.Vendor).Include(loc => loc.Site).ToList();

            foreach (var project in context.ProjectMaster)
            {
                var mrfs = new List<MRFMaster>();
                var inventories = new List<InventoryAudit>();
                var stageAudits = new List<MaterialStageAudit>();

                var materials = context.MaterialMaster.Include(mm => mm.Project).Include(mm => mm.Vendor)
                    .Where(mm => mm.Project == project).ToList();
                // Generate MRF for first block
                foreach (var blk in materials.Select(mm => mm.Block).Distinct().Take(1))
                {
                    var materials1 = materials.Where(mm => mm.Block == blk);
                    foreach (var level in materials1.Select(mm => mm.Level).Distinct())
                    {
                        var materials2 = materials1.Where(mm => mm.Level == level);
                        foreach (var zone in materials2.Select(mm => mm.Zone).Distinct())
                        {
                            var materials3 = materials2.Where(mm => mm.Zone == zone);
                            foreach (var vendor in materials3.Select(mm => mm.Vendor).Distinct())
                            {
                                var materials4 = materials3.Where(mm => mm.Vendor == vendor);
                                var mrf = new MRFMaster
                                {
                                    MRFCompletion = 100,
                                    Materials = materials4.ToList(),
                                    MRFNo = project.Name.Contains("@") ? "PP09-" : "MRF-",
                                    UserMRFAssociations = new List<UserMRFAssociation>
                                    {
                                        new UserMRFAssociation
                                        {
                                            UserID = vendorPMs.Where(vpm => vpm.Vendor.ID == vendor.ID)
                                            .First().ID
                                        }
                                    }
                                };

                                mrfs.Add(mrf);
                            }
                        }
                    }
                }

                int mrfIndex = 1;
                int sn = 1;
                foreach (var mrf in mrfs)
                {
                    mrf.MRFNo = mrf.MRFNo + mrfIndex.ToString("D4");
                    mrf.OrderDate = project.EstimatedStartDate.Value.AddDays(rnd.Next(mrfIndex, mrfIndex + 3));
                    mrf.PlannedCastingDate = mrf.OrderDate.AddDays(rnd.Next(1, 3));
                    mrf.ExpectedDeliveryDate = mrf.PlannedCastingDate.AddDays(rnd.Next(5, 8));
                    mrfIndex++;

                    foreach (var material in mrf.Materials)
                    {
                        var tracker = trackers[rnd.Next(0, trackers.Count)];
                        var vendorOfficer = vendorOfficers
                            .Where(vo => vo.Vendor.ID == material.VendorId).First();

                        var inventory = new InventoryAudit
                        {
                            MarkingNo = material.MarkingNo,
                            CastingDate = DateTimeOffset.UtcNow,
                            SN = sn,
                            ProjectID = project.ID,
                            TrackerID = tracker.ID,
                            VendorID = material.VendorId,
                            CreatedByID = vendorOfficer.ID,
                            CreatedDate = mrf.PlannedCastingDate
                        };
                        inventories.Add(inventory);

                        var siteOfficer = siteOfficers.Where(so => so.ProjectID == project.ID).First();
                        foreach (var stage in stages)
                        {
                            LocationMaster location;
                            int createdById;
                            if (stage.Order < 3) {
                                location = locations
                                    .Where(loc => loc.Type == 0 && loc.VendorID == material.VendorId).First();
                                createdById = vendorOfficer.ID;
                            }
                            else if (stage.Order < 5) {
                                location = locations
                                    .Where(loc => loc.Type == 1 && loc.SiteID == siteOfficer.SiteID).First();
                                createdById = siteOfficer.ID;
                            }
                            else {
                                location = locations
                                    .Where(loc => loc.Type == 2 && loc.SiteID == siteOfficer.SiteID).First();
                                createdById = siteOfficer.ID;
                            }

                            var stageAudit = new MaterialStageAudit
                            {
                                MaterialMasterID = material.ID,
                                StageID = stage.ID,
                                StagePassed = true,
                                LocationId = location.ID,
                                CreatedByID = createdById,
                                CreatedDate = mrf.PlannedCastingDate.AddDays(stage.Order)
                            };
                            stageAudits.Add(stageAudit);
                        }


                    }
                }

                context.MRFMaster.AddRange(mrfs);

                // For material beyond production, no need inventory
                // context.InventoryAudit.AddRange(inventories);
                context.MaterialStageAudit.AddRange(stageAudits);
            }
            context.SaveChanges();

            Console.WriteLine("Done populating demo progress");
        }

        public static void PopulateDemoQC(astorWorkDbContext context, string imgUrlPrefix)
        {
            Console.WriteLine("Start populating demo QC");
            Random rnd = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            var siteOfficers = context.UserMaster.Include(um => um.Project).Include(um => um.Site).Where(um => um.RoleID == 5).ToList();
            var trackers = context.TrackerMaster.ToList();
            var stages = context.MaterialStageMaster.ToList();
            var stageAudits = context.MaterialStageAudit
                .Include(msa => msa.Stage)
                .Include(msa => msa.MaterialMaster)
                .Where(msa => msa.Stage.IsQCStage && msa.Stage.Order > 2)
                .ToList();

            // Generate an open case
            var openStageAudit = stageAudits.First();
            var openSiteOfficer = siteOfficers.Where(so => so.ProjectID == openStageAudit.MaterialMaster.ProjectId).First();
            var qcCaseOpen = new MaterialQCCase
            {
                CaseName = "QC-" + new string(Enumerable.Repeat(chars, 4)
                .Select(s => s[rnd.Next(s.Length)]).ToArray()),
                StageAuditId = openStageAudit.ID,
                CreatedById = openSiteOfficer.ID,
                CreatedDate = openStageAudit.CreatedDate.AddHours(1),
                Defects = new List<MaterialQCDefect>()
                {
                    new MaterialQCDefect
                    {
                        IsOpen = true,
                        CreatedById = openSiteOfficer.ID,
                        CreatedDate = openStageAudit.CreatedDate.AddHours(1),
                        Remarks = "Defect-" + new string(Enumerable.Repeat(chars, 4)
                .Select(s => s[rnd.Next(s.Length)]).ToArray()),
                        Photos = new List<MaterialQCPhotos>
                        {
                            new MaterialQCPhotos
                            {
                                IsOpen = true,
                                URL = imgUrlPrefix + "crack-1.jpg",
                                CreatedById = openSiteOfficer.ID,
                                CreatedDate = openStageAudit.CreatedDate.AddHours(1)
                            },
                            new MaterialQCPhotos
                            {
                                IsOpen = true,
                                URL = imgUrlPrefix + "crack-2.jpg",
                                CreatedById = openSiteOfficer.ID,
                                CreatedDate = openStageAudit.CreatedDate.AddHours(2)
                            }
                        }
                    }
                }
            };

            // Generate a closed case
            var closeStageAudit = stageAudits.Last();
            var closeSiteOfficer = siteOfficers.Where(so => so.ProjectID == closeStageAudit.MaterialMaster.ProjectId).First();
            var qcCaseClosed = new MaterialQCCase
            {                
                CaseName = "QC-" + new string(Enumerable.Repeat(chars, 4)
                .Select(s => s[rnd.Next(s.Length)]).ToArray()),
                StageAuditId = closeStageAudit.ID,
                CreatedById = closeSiteOfficer.ID,
                CreatedDate = closeStageAudit.CreatedDate.AddHours(1),
                Defects = new List<MaterialQCDefect>()
                {
                    new MaterialQCDefect
                    {
                        IsOpen = false,
                        CreatedById = closeSiteOfficer.ID,
                        CreatedDate = closeStageAudit.CreatedDate.AddHours(1),
                        UpdatedById = closeSiteOfficer.ID,
                        UpdatedDate = closeStageAudit.CreatedDate.AddHours(4),
                        Remarks = "Defect-" + new string(Enumerable.Repeat(chars, 4)
                .Select(s => s[rnd.Next(s.Length)]).ToArray()),
                        Photos = new List<MaterialQCPhotos>
                        {
                            new MaterialQCPhotos
                            {
                                IsOpen = true,
                                URL = imgUrlPrefix + "crack-1.jpg",
                                CreatedById = openSiteOfficer.ID,
                                CreatedDate = openStageAudit.CreatedDate.AddHours(1)
                            },
                            new MaterialQCPhotos
                            {
                                IsOpen = true,
                                URL = imgUrlPrefix + "crack-2.jpg",
                                CreatedById = openSiteOfficer.ID,
                                CreatedDate = openStageAudit.CreatedDate.AddHours(2)
                            },
                            new MaterialQCPhotos
                            {
                                IsOpen = false,
                                URL = imgUrlPrefix + "fixed-1.jpg",
                                CreatedById = openSiteOfficer.ID,
                                CreatedDate = openStageAudit.CreatedDate.AddHours(3)
                            },
                            new MaterialQCPhotos
                            {
                                IsOpen = false,
                                URL = imgUrlPrefix + "fixed-2.jpg",
                                CreatedById = openSiteOfficer.ID,
                                CreatedDate = openStageAudit.CreatedDate.AddHours(4)
                            }
                        }
                    }
                }
            };

            context.MaterialQCCase.Add(qcCaseOpen);
            context.MaterialQCCase.Add(qcCaseClosed);
            context.SaveChanges();

            Console.WriteLine("Done populating demo QC");
        }

        public static string Initialize(astorWorkDbContext context)
        {
            string error = string.Empty;
            Console.WriteLine("Make sure db is migrated");

            context.Database.Migrate();
            // Look for any materials

            if (context.UserMaster.Any())
                error = "Database has been initialized already";
            else
            {
                Console.WriteLine("Start populating dummy data");
                context.Database.OpenConnection();
                try
                {
                    ModuleMaster[] modules = InserModuleMaster();
                    context.ModuleMaster.AddRange(modules);

                    var stages = InsertMaterialStages();
                    context.MaterialStageMaster.AddRange(stages);

                    /*
                    TrackerMaster[] trackers = InsertTrackers();
                    context.TrackerMaster.AddRange(trackers);
                    */

                    RoleMaster[] roles = InsertRoles(modules);
                    context.RoleMaster.AddRange(roles);

                    UserMaster[] users = InsertUserMasters(roles);
                    context.UserMaster.AddRange(users);

                    VendorMaster[] vendors = InsertVendors(users);
                    context.VendorMaster.AddRange(vendors);

                    var projects = new ProjectMaster[] {
                        new ProjectMaster {
                            Name = "A Project",
                            EstimatedStartDate = DateTimeOffset.Now.AddDays(-187),
                            EstimatedEndDate = DateTimeOffset.Now.AddDays(341)
                        },
                        new ProjectMaster { Name = "B Project",
                            EstimatedStartDate = DateTimeOffset.Now.AddDays(-107),
                            EstimatedEndDate = DateTimeOffset.Now.AddDays(441)
                        }
                    };
                    context.ProjectMaster.AddRange(projects);

                    /*
                    LocationMaster[] locations = InsertLocation(vendors);
                    context.LocationMaster.AddRange(locations);
                    */

                    // Insert MaterialMasters
                    List<MaterialMaster> materials = new List<MaterialMaster>();

                    for (int projectIndex = 0; projectIndex < projects.Length; projectIndex++)
                    {
                        var project = projects[projectIndex];

                        MaterialDrawingAudit[] materialDrawingAudits = InsertDrawings();
                        context.MaterialDrawingAudit.AddRange(materialDrawingAudits);


                        int maxSN = 0;
                        var inventoryAudits = new List<InventoryAudit>();
                        //int trackerIndex = 0;
                        for (int blkIndex = 1; blkIndex <= 10; blkIndex++)
                        {
                            for (int levelIndex = 0; levelIndex < 30; levelIndex++)
                            {
                                Random rnd = new Random();
                                int zoneCount = rnd.Next(1, 5);
                                // int trackerIndex = rnd.Next(0, trackerCount);

                                for (int zoneIndex = 0; zoneIndex < zoneCount; zoneIndex++)
                                {
                                    for (int materialIndex = 0; materialIndex < 6; materialIndex++)
                                    {
                                        int materialTypeIndex = rnd.Next(0, 6);

                                        maxSN++;

                                        MaterialMaster material = CreateMaterialMaster(vendors, materialDrawingAudits, project, blkIndex, levelIndex, zoneIndex, materialTypeIndex, materialIndex, maxSN);
                                        materials.Add(material);
                                    }
                                }
                            }
                        }
                    }
                    //context.InventoryAudit.AddRange(inventoryAudits);
                    context.MaterialMaster.AddRange(materials);
                    Console.WriteLine("Try to insert " + materials.Count() + " materials");
                    context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.RoleMaster ON");
                    context.SaveChanges();
                    context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.RoleMaster OFF");
                    Console.WriteLine("Data populating Completed");
                }
                catch (Exception exc)
                {
                    Console.WriteLine(exc.Message);
                    Console.WriteLine(exc.StackTrace);
                    error = exc.Message;
                }
                finally
                {
                    context.Database.CloseConnection();
                }
            }

            return error;
        }

        public static ModuleMaster[] InserModuleMaster()
        {
            var modulemaster = new ModuleMaster[] {
            new ModuleMaster
            {
                Name = "Material-Tracking",
                UrlPrefix = "material-tracking",
                Pages = new List<PageMaster>(new PageMaster[]
                     {
                    new PageMaster{Name = "List Materials", UrlPath = "materials"},
                    new PageMaster{Name = "Material Details", UrlPath = "materials/:id"},
                    new PageMaster{Name = "List MRFs", UrlPath = "mrfs"},
                    new PageMaster{Name = "Create MRF", UrlPath = "mrfs/create"},
                    new PageMaster{Name = "List BIM Sync Sessions", UrlPath = "bim-syncs"},
                    new PageMaster{Name = "BIM Sync Session Details", UrlPath = "bim-syncs/:id"},
                    new PageMaster{Name = "Dashboard", UrlPath = "dashboard"},
                    new PageMaster{Name = "List QC Defects", UrlPath = "qc-defects/:id"},
                    new PageMaster{Name = "List QC Cases", UrlPath = "qc-cases/:id"},
                    new PageMaster{Name = "Import Materials", UrlPath = "import-material"},
                    new PageMaster{Name = "List Reports", UrlPath = "list-reports"}
                     }
                     )
            },
            new ModuleMaster
            {
                Name = "User Account",
                UrlPrefix = "user-account",
                Pages = new List<PageMaster>(new PageMaster[]
                {
                    new PageMaster{Name = "Edit Profile", UrlPath = ":id"},
                    new PageMaster{Name = "Change Password", UrlPath = "change-password"}
                })
            },
            new ModuleMaster
            {
                Name = "Configuration",
                UrlPrefix = "configuration",
                Pages = new List<PageMaster>(new PageMaster[]
                {
                    new PageMaster{ Name = "List of Users", UrlPath = "user-master"},
                    new PageMaster{ Name = "User Details", UrlPath = "user-details/:id"},
                    new PageMaster{ Name = "List of Roles", UrlPath = "role-master"},
                    new PageMaster{ Name = "Role Details", UrlPath = "role-details/:id"},
                    new PageMaster{ Name = "Create Stage", UrlPath = "stage-master/create"},
                    new PageMaster{ Name = "Stage Master", UrlPath = "stage-master"},
                    new PageMaster{ Name = "Import/Generate Tag", UrlPath = "generate-qr-code"},
                    new PageMaster{ Name = "Vendor Master", UrlPath = "vendor-master"},
                    new PageMaster{ Name = "Vendor Details", UrlPath = "vendor-details/:id"},
                    new PageMaster{ Name = "List Projects", UrlPath = "project-master"},
                    new PageMaster{ Name = "Project Details", UrlPath = "project-details/:id"},
                    new PageMaster{ Name = "List of Locations", UrlPath = "location-master"},
                    new PageMaster{ Name = "Location Details", UrlPath = "location-details/:id"},
                    new PageMaster{ Name = "List of Sites", UrlPath = "site-master" },
                    new PageMaster{ Name = "Site Details", UrlPath = "site-details/:id" },
                    new PageMaster{ Name = "Notification Config", UrlPath = "notification-config" }
                })
            }};
            return modulemaster;
        }


        public static RoleMaster[] InsertRoles(ModuleMaster[] modules)
        {
            // Insert roles
            var roles = new RoleMaster[]
            {
                new RoleMaster { ID = 1, Name = "Super", DefaultPage = modules[2].Pages[5],
                    PlatformCode = "0123" },
                new RoleMaster { ID = 2, Name = "Admin", DefaultPage = modules[0].Pages[6],
                    PlatformCode = "0123" },
                new RoleMaster { ID = 3, Name = "Management", DefaultPage = modules[0].Pages[6],
                    PlatformCode = "0" },
                new RoleMaster { ID = 4, Name = "Project Manager", DefaultPage = modules[0].Pages[6],
                    PlatformCode = "0123" },
                new RoleMaster { ID = 5, Name = "Site Officer", DefaultPage = modules[0].Pages[2],
                    MobileEntryPoint = 1, PlatformCode = "01" },
                new RoleMaster { ID = 6, Name = "BIM", DefaultPage = modules[0].Pages[4],
                    PlatformCode = "02" },
                new RoleMaster { ID = 7, Name = "Vendor Project Manager", DefaultPage = modules[0].Pages[2],
                    PlatformCode = "01" },
                new RoleMaster { ID = 8, Name = "Vendor Production Officer", DefaultPage = modules[0].Pages[2],
                    MobileEntryPoint = 0, PlatformCode = "01" }
            };

            // Access rights for Super Role
            var superAccessRights = new List<RolePageAssociation>();
            foreach (var module in modules)
            {
                foreach (var page in module.Pages)
                {
                    superAccessRights.Add(new RolePageAssociation { PageId = page.ID, AccessLevel = 3 });
                }
            }
            roles[0].RolePageAssociations = superAccessRights;

            // Access rights for Admin Role
            var adminAccessRights = new List<RolePageAssociation>();
            foreach (var module in modules)
            {
                foreach (var page in module.Pages)
                {
                    if (page != modules[2].Pages[2] && page != modules[2].Pages[3] && page != modules[2].Pages[5] && page != modules[2].Pages[5])
                        adminAccessRights.Add(new RolePageAssociation { PageId = page.ID, AccessLevel = 3 });
                    else
                        adminAccessRights.Add(new RolePageAssociation { PageId = page.ID, AccessLevel = 1 });
                }
            }
            roles[1].RolePageAssociations = adminAccessRights;

            // Access rights for Main-con higher management & PM
            var mainConManageAccessRights = new List<RolePageAssociation>();
            foreach (var module in modules.Where(m => m != modules[2]))
            {
                foreach (var page in module.Pages)
                {
                    mainConManageAccessRights.Add(new RolePageAssociation { PageId = page.ID, AccessLevel = 3 });
                }
            }
            roles[2].RolePageAssociations = mainConManageAccessRights;
            roles[3].RolePageAssociations = mainConManageAccessRights;

            // Access rights for Main-con site officer & Vendor PM
            var mainConOfficerAccessRights = new List<RolePageAssociation>(){
                new RolePageAssociation { Page = modules[0].Pages[0], AccessLevel = 1 },
                new RolePageAssociation { Page = modules[0].Pages[1], AccessLevel = 1 },
                new RolePageAssociation { Page = modules[0].Pages[2], AccessLevel = 1 },
                new RolePageAssociation { Page = modules[0].Pages[3], AccessLevel = 1 },
                new RolePageAssociation { Page = modules[0].Pages[7], AccessLevel = 1 },
                new RolePageAssociation { Page = modules[0].Pages[8], AccessLevel = 1 },
                new RolePageAssociation { Page = modules[0].Pages[10], AccessLevel = 1 },
                new RolePageAssociation { Page = modules[1].Pages[0], AccessLevel = 3 },
                new RolePageAssociation { Page = modules[1].Pages[1], AccessLevel = 3 },
            };
            roles[4].RolePageAssociations = mainConOfficerAccessRights;
            roles[6].RolePageAssociations = mainConOfficerAccessRights;
            // Access rights for Main-con BIM
            var mainConBIMAccessRights = new List<RolePageAssociation>(){
                new RolePageAssociation { Page = modules[0].Pages[4], AccessLevel = 1 },
                new RolePageAssociation { Page = modules[0].Pages[5], AccessLevel = 1 },
                new RolePageAssociation { Page = modules[1].Pages[0], AccessLevel = 3 },
                new RolePageAssociation { Page = modules[1].Pages[1], AccessLevel = 3 },
            };
            roles[5].RolePageAssociations = mainConBIMAccessRights;
            // Access rights for Vendor officer
            // Will change when digital wall chart come out
            roles[7].RolePageAssociations = mainConOfficerAccessRights;

            return roles;
        }

        public static UserMaster[] InsertUserMasters(RoleMaster[] roles)
        {
            var pwdHashed = string.Empty;
            var salt = Guid.NewGuid();

            using (var algorithm = SHA256.Create())
            {
                var pwd = "abc12345";
                var pwdSalted = Encoding.UTF8.GetBytes(pwd).Concat(salt.ToByteArray());
                var hash = algorithm.ComputeHash(pwdSalted.ToArray());
                pwdHashed = Convert.ToBase64String(hash);
            }
            // Insert users
            /*
            var users = new UserMaster[]
            {
                    new UserMaster{ UserName = "vendor1", Email = "officer@vendorone.com", PersonName = "VendorOne - Officer", Role = roles[0], Password = pwdHashed, Salt = salt.ToString(),IsActive=true },
                    new UserMaster{ UserName = "vendor1qc", Email = "qc@vendorone.com", PersonName = "VendorOne - QC", Role = roles[0], Password = pwdHashed, Salt = salt.ToString() ,IsActive=true},
                    new UserMaster{ UserName = "vendor2", Email = "officer@vendortwo.com", PersonName = "VendorTwo - Officer", Role = roles[0], Password = pwdHashed, Salt = salt.ToString() ,IsActive=true},
                    new UserMaster{ UserName = "vendor2qc", Email = "qc@vendortwo.com", PersonName = "VendorTwo - QC", Role = roles[0], Password = pwdHashed, Salt = salt.ToString() ,IsActive=true},
                    new UserMaster{ UserName = "supervisor", Email = "supervisor@site.com", PersonName = "Site - Supervisor", Role = roles[1], Password = pwdHashed, Salt = salt.ToString() ,IsActive=true},
                    new UserMaster{ UserName = "qc", Email = "qc@site.com", PersonName = "Site - QC", Role = roles[1], Password = pwdHashed, Salt = salt.ToString(),IsActive=true },
                    new UserMaster{ UserName = "admin", Email="admin@site.com", PersonName = "Admin", Role = roles[3], Password = pwdHashed, Salt = salt.ToString(),IsActive=true }
            };
            */
            var users = new UserMaster[]
            {
                new UserMaster { UserName = "super", Email = "support@astorwork.com",
                    PersonName = "Astoria Super User", Role = roles[0],
                    Password = pwdHashed, Salt = salt.ToString(), IsActive=true },
                /*
                new UserMaster { UserName = "admin", Email = "admin@main-con.com",
                    PersonName = "Main Contractor Admin", Role = roles[1],
                    Password = pwdHashed, Salt = salt.ToString(), IsActive=true }
                    */
            };
            return users;
        }

        private static VendorMaster[] InsertVendors(UserMaster[] users)
        {
            // Insert 2 vendor
            VendorMaster[] vendors = new VendorMaster[]
            {
                    new VendorMaster{ Name = "VendorOne", CycleDays=7},
                    new VendorMaster{ Name = "VendorTwo", CycleDays=10},
            };

            /*
            users[0].Vendor = vendors[0];
            users[1].Vendor = vendors[0];
            users[2].Vendor = vendors[1];
            users[3].Vendor = vendors[1];
            */

            return vendors;
        }

        public static MaterialStageMaster[] InsertMaterialStages()
        {
            string materialTypes = string.Join(",", LIST_MATERIAL_TYPES);
            // Insert material stages
            var stages = new MaterialStageMaster[]
            {
                new MaterialStageMaster{ Name = "Before Deliver QC", Colour="#ffff99", Order = 1, IsQCStage = true,IsEditable=true, MaterialTypes=materialTypes},
                new MaterialStageMaster{ Name = "Start Delivery", Colour="#ffff66", Order = 2, IsQCStage = false,IsEditable=false, MaterialTypes=materialTypes},
                new MaterialStageMaster{ Name = "Delivered", Colour="#66ff99", Order = 3, IsQCStage = false,IsEditable=false, MaterialTypes=materialTypes},
                new MaterialStageMaster{ Name = "After Delivered QC", Colour="#00ff00", Order = 4, IsQCStage = true,IsEditable=true, MaterialTypes=materialTypes},
                new MaterialStageMaster{ Name = "Before Install QC", Colour="#99ccff", Order = 5, IsQCStage = true,IsEditable=true, MaterialTypes=materialTypes},
                new MaterialStageMaster{ Name = "Installed", Colour="#3399ff", Order = 6, IsQCStage = false,IsEditable=false, MaterialTypes=materialTypes}
            };
            return stages;
        }

        private static TrackerMaster[] InsertTrackers()
        {
            Random rnd = new Random();

            // Insert trackers
            string[] trackerTypes = { "RFID", "QR Code", "Beacon" };
            const int trackerCount = 10000;
            var trackers = new TrackerMaster[trackerCount];
            // Add real RFID tags
            trackers[0] = new TrackerMaster
            {
                Type = trackerTypes[0],
                Tag = "065A",
                Label = "1626"
            };
            trackers[1] = new TrackerMaster
            {
                Type = trackerTypes[0],
                Tag = "065D",
                Label = "1629"
            };
            trackers[2] = new TrackerMaster
            {
                Type = trackerTypes[0],
                Tag = "2812",
                Label = "10259"
            };
            trackers[3] = new TrackerMaster
            {
                Type = trackerTypes[0],
                Tag = "06CA",
                Label = "1738"
            };
            trackers[4] = new TrackerMaster
            {
                Type = trackerTypes[0],
                Tag = "06BC",
                Label = "1724"
            };
            for (int i = 5; i < trackerCount; i++)
            {
                int trackerType = rnd.Next(0, 3);
                trackers[i] = new TrackerMaster
                {
                    Type = trackerTypes[trackerType],
                    Tag = Guid.NewGuid().ToString(),
                    Label = trackerTypes[trackerType].Substring(0, 1) + i
                };
            }

            return trackers;
        }

        private static ProjectMaster InsertProject()
        {
            // Insert 2 projects
            var project = new ProjectMaster { Name = "Demo Project" };
            return project;
        }

        private static MaterialDrawingAudit[] InsertDrawings()
        {
            var drawing = new MaterialDrawingAudit[]
            {

                new MaterialDrawingAudit{
                    DrawingIssueDate = DateTimeOffset.UtcNow, DrawingNo = "PFT-K1F-LP-01", RevisionNo = 1
                },
                new MaterialDrawingAudit{
                    DrawingIssueDate = DateTimeOffset.UtcNow, DrawingNo = "PFT-K1F-LP-02", RevisionNo = 2
                },
                new MaterialDrawingAudit{
                    DrawingIssueDate = DateTimeOffset.UtcNow, DrawingNo = "PFT-K1F-LP-03", RevisionNo = 3
                },
            };

            return drawing;
        }

        private static LocationMaster[] InsertLocation(VendorMaster[] vendors)
        {
            // Insert 6 locations
            var locations = new LocationMaster[]
            {
                new LocationMaster{ Name = "VenorOne's Factory", Type = 0, Vendor = vendors[0]},
                new LocationMaster{ Name = "VenorTwo's Factory", Type = 0, Vendor = vendors[1]},
                new LocationMaster{ Name = "Storage Area 1", Type = 1},
                new LocationMaster{ Name = "Storage Area 2", Type = 1},
                new LocationMaster{ Name = "Installation Area", Type = 2}
            };
            return locations;
        }

        private static MaterialMaster GenerateMaterialMaster(MaterialDrawingAudit[] materialDrawingAudits, string projectName, int projectId, int vendorId, string blk, int levelIndex, int zoneIndex, int materialTypeIndex, int materialIndex, int maxSN)
        {
            Random rnd = new Random();
            string[] zones = { "A", "B", "C", "D" };

            var material = new MaterialMaster
            {
                Block = blk,
                Level = (levelIndex + 1).ToString(),
                Zone = zones[zoneIndex],
                ProjectId = projectId,
                VendorId = vendorId,
                MaterialType = LIST_MATERIAL_TYPES[materialTypeIndex],
                MarkingNo = string.Format("{0}{1}{2}{3}{4}", projectName.Substring(0, 1), zoneIndex + 1, LIST_MATERIAL_TYPES[materialTypeIndex][0], materialIndex + 1, (char)(materialTypeIndex + 65))
            };

            material.DrawingAssociations = createDrawingAssociations(materialDrawingAudits, material);

            return material;
        }

        private static MaterialMaster CreateMaterialMaster(VendorMaster[] vendors, MaterialDrawingAudit[] materialDrawingAudits, ProjectMaster project, int blkIndex, int levelIndex, int zoneIndex, int materialTypeIndex, int materialIndex, int maxSN)
        {
            Random rnd = new Random();
            int dateDiff = rnd.Next(7, 30);
            string[] zones = { "A", "B", "C", "D" };
            string[] materialTypes = { "Façade", "Rebar", "Slab", "Beam", "Plank", "Wall Panel" };

            string blk = "BLK " + blkIndex;
            int vendorIndex = rnd.Next(0, 2);
            var vendor = vendors[vendorIndex];
            //var trackers = context.TrackerMaster.ToList();

            var material = new MaterialMaster
            {
                Block = blk,
                Level = (levelIndex + 1).ToString(),
                CastingDate = DateTime.Today.AddDays(-dateDiff),
                Zone = zones[zoneIndex],
                Project = project,
                //Tracker = trackers[trackerIndex],
                Vendor = vendor,
                MaterialType = materialTypes[materialTypeIndex],
                MarkingNo = string.Format("{0}{1}{2}{3}{4}", project.Name.Substring(0, 1), zoneIndex + 1, materialTypes[materialTypeIndex][0], materialIndex + 1, (char)(materialTypeIndex + 65)),
                MRF = null,
                StageAudits = new List<MaterialStageAudit>(),

            };

            material.DrawingAssociations = createDrawingAssociations(materialDrawingAudits, material);

            return material;
        }

        private static List<MaterialDrawingAssociation> createDrawingAssociations(MaterialDrawingAudit[] materialdrawingaudits, MaterialMaster materialmaster)
        {
            List<MaterialDrawingAssociation> materialDrawingAssociations = new List<MaterialDrawingAssociation>();
            foreach (MaterialDrawingAudit d in materialdrawingaudits)
            {
                var materialDrawignAssociation = new MaterialDrawingAssociation
                {
                    MaterialID = materialmaster.ID,
                    Material = materialmaster,
                    DrawingID = d.ID,
                    Drawing = d
                };

                materialDrawingAssociations.Add(materialDrawignAssociation);
            }
            return materialDrawingAssociations;
        }

        private static void CreateStageAudits()
        {

        }

        private static InventoryAudit InsertInventoryAudit(List<TrackerMaster> trackers, ProjectMaster project, MaterialMaster material, int maxSN)
        {
            Random rnd = new Random();

            // 1/10 odds that this material is in inventory
            int shouldBeInventory = rnd.Next(0, 10);
            if (shouldBeInventory == 0)
            {
                var inventory = new InventoryAudit
                {
                    MarkingNo = material.MarkingNo,
                    CastingDate = DateTimeOffset.UtcNow,
                    SN = maxSN,
                    ProjectID = project.ID,
                    //Tracker = trackers[context.InventoryAudit.Count()],
                    VendorID = material.Vendor.ID
                };

                return inventory;
            }

            return null;
        }
    }
}
