using astorWorkShared.Utilities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using static astorWorkShared.Utilities.Enums;

namespace astorWorkDAO.Data
{
    public static class DbInitializer
    {
        private static readonly string[] LIST_MATERIAL_TYPES =
            { "Facade", "Rebar", "Slab", "Beam", "Plank", "Wall Panel", "PPVC-LivingRoom", "PPVC-Bedroom", "PPVC-Kitchen", "PPVC-Kitchen-PBU", "PPVC-Bedroom-PBU" };

        public static void PopulateBasicData(astorWorkDbContext context)
        {
            Console.WriteLine("Start populating basic data");
            ModuleMaster[] modules = InserModuleMaster();
            context.ModuleMaster.AddRange(modules);

            MaterialStageMaster[] stages = InsertMaterialStages();
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

        private static string CreateHashedPassword(Guid salt)
        {
            string pwdHashed = string.Empty;

            using (var algorithm = SHA256.Create())
            {
                string pwd = "abc12345";
                IEnumerable<byte> pwdSalted = Encoding.UTF8.GetBytes(pwd).Concat(salt.ToByteArray());
                byte[] hash = algorithm.ComputeHash(pwdSalted.ToArray());
                return Convert.ToBase64String(hash);
            }
        }

        private static OrganisationMaster[] InsertOrganisations()
        {
            // Generate organisations
            OrganisationMaster[] organisations = new OrganisationMaster[]
            {
                new OrganisationMaster { Name = "Reinforcing Asia", CycleDays=5,
                OrganisationType = OrganisationType.Vendor },
                new OrganisationMaster { Name = "Pre-cast Technology", CycleDays=7,
                OrganisationType = OrganisationType.Vendor },
                new OrganisationMaster {Name = "WaterPro Pte Ltd",
                    OrganisationType = OrganisationType.Subcon },
                new OrganisationMaster {Name = "EZ Flooring Inc",
                    OrganisationType = OrganisationType.Subcon },
                new OrganisationMaster {Name = "Everything Can Do LLP",
                    OrganisationType = OrganisationType.Subcon }
            };

            return organisations;
        }

        private static ProjectMaster[] InsertProjects()
        {
            // Generate projects
            return new ProjectMaster[]
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
        }

        private static SiteMaster[] InsertSites(OrganisationMaster[] organisations)
        {
            // Generate sites
            return new SiteMaster[]
            {
                new SiteMaster
                {
                    Country = "MY",
                    TimeZoneOffset = 480,
                    Description = "Reinforcing Asia's Headquarter",
                    Name = "RA Malaysia",
                    Organisation = organisations[0],
                    Locations = new List<LocationMaster>
                    {
                        new LocationMaster
                        {
                            Name = "RA Factory",
                            Type = 0,
                            Description = "Reinforcing Asia's factory in Malaysia",
                            Organisation = organisations[0]
                        }
                    }
                },
                new SiteMaster
                {
                    Country = "IT",
                    TimeZoneOffset = 120,
                    Description = "Pre-cast Technology's Headquarter",
                    Name = "PT Italy",
                    Organisation = organisations[1],
                    Locations = new List<LocationMaster>
                    {
                        new LocationMaster
                        {
                            Name = "PT Factory",
                            Type = 0,
                            Description = "Pre-cast Technology's factory in Italy",
                            Organisation = organisations[1]
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
        }

        private static TrackerMaster[] InsertTrackers()
        {
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

            return trackers;
        }

        private static TrackerMaster[] InsertTrackers2()
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

        private static UserMaster[] InsertUsers(string pwdHashed, Guid salt, ProjectMaster[] projects, SiteMaster[] sites, OrganisationMaster[] organisations)
        {
            // Generate users
            return new UserMaster[]
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
                // Organisation users
                new UserMaster{ UserName = "amelia", Email = "amelia@ra.com", PersonName = "Amelia Taylor", RoleID = 7, Password = pwdHashed, Salt = salt.ToString(),IsActive=true,
                    Organisation = organisations[0], Site = sites[0] },
                new UserMaster{ UserName = "oliver", Email = "oliver@ra.com", PersonName = "Oliver Jones", RoleID = 8, Password = pwdHashed, Salt = salt.ToString(),IsActive=true,
                    Organisation = organisations[0], Site = sites[0] },
                new UserMaster{ UserName = "jacob", Email = "jacob@ra.com", PersonName = "Jacob Jones", RoleID = 8, Password = pwdHashed, Salt = salt.ToString(),IsActive=true,
                    Organisation = organisations[0], Site = sites[0] },
                new UserMaster{ UserName = "jessica", Email = "jessica@pt.com", PersonName = "Jessica Wilson", RoleID = 7, Password = pwdHashed, Salt = salt.ToString(),IsActive=true,
                    Organisation = organisations[1], Site = sites[1] },
                new UserMaster{ UserName = "james", Email = "james@pt.com", PersonName = "James Evans", RoleID = 8, Password = pwdHashed, Salt = salt.ToString(),IsActive=true,
                    Organisation = organisations[1], Site = sites[1] },
                new UserMaster{ UserName = "jack", Email = "jack@pt.com", PersonName = "Jack Thomas", RoleID = 8,
                    Password = pwdHashed, Salt = salt.ToString(),IsActive=true,
                    Organisation = organisations[1], Site = sites[1] },
                new UserMaster{ UserName = "lucas", Email = "lucas@water.com", PersonName = "Lucas Ng", RoleID = 9, Password = pwdHashed, Salt = salt.ToString(),IsActive=true,
                    Organisation = organisations[2] },
                new UserMaster{ UserName = "emma", Email = "emma@floor.com", PersonName = "Emma Lim", RoleID = 9, Password = pwdHashed, Salt = salt.ToString(),IsActive=true,
                    Organisation = organisations[3] },
                new UserMaster{ UserName = "ian", Email = "ian@ecd.com", PersonName = "Ian Lee", RoleID = 9,
                    Password = pwdHashed, Salt = salt.ToString(),IsActive=true,
                    Organisation = organisations[4] }
            };
        }

        public static void PopulateDemoSetup(astorWorkDbContext context)
        {
            Console.WriteLine("Start populating demo setup");

            Guid salt = Guid.NewGuid();
            string pwdHashed = CreateHashedPassword(salt);

            RoleMaster[] roles = context.RoleMaster.OrderBy(r => r.ID).ToArray();

            OrganisationMaster[] organisations = InsertOrganisations();
            context.OrganisationMaster.AddRange(organisations);

            ProjectMaster[] projects = InsertProjects();
            context.ProjectMaster.AddRange(projects);

            SiteMaster[] sites = InsertSites(organisations);
            context.SiteMaster.AddRange(sites);

            TrackerMaster[] trackers = InsertTrackers();
            context.TrackerMaster.AddRange(trackers);

            UserMaster[] users = InsertUsers(pwdHashed, salt, projects, sites, organisations);
            context.UserMaster.AddRange(users);

            context.SaveChanges();
            Console.WriteLine("Done populating demo data");
        }

        public static void PopulateDemoMaterials(astorWorkDbContext context)
        {
            var materialTypes = GenerateMaterialTypes();

            context.MaterialTypeMaster.AddRange(materialTypes);

            Console.WriteLine("Start populating demo materials");

            var projects = context.ProjectMaster.ToArray();
            var organisations = context.OrganisationMaster.ToArray();
            var materials = new List<MaterialMaster>();

            var materialDrawingAudits = InsertDrawings();
            context.MaterialDrawingAudit.AddRange(materialDrawingAudits);

            //var materialTypes = GenerateMaterialTypes(projects[0]);

            //context.MaterialTypeMaster.AddRange(materialTypes);

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
                            int organisationIndex = rnd.Next(0, organisations.Length);
                            int materialTypeIndex = rnd.Next(0, 11);

                            maxSN++;

                            MaterialMaster material = GenerateMaterialMaster(materialDrawingAudits, materialTypes.ToArray(), projects[0],  organisations[organisationIndex].ID,
                                $"BLK {blkIndex}", levelIndex, zoneIndex, materialTypeIndex, materialIndex, maxSN);

                            materials.Add(material);
                        }
                    }
                }
            }

            // Populate for Uppal New Mall
            materialDrawingAudits = InsertDrawings();
            context.MaterialDrawingAudit.AddRange(materialDrawingAudits);

           // materialTypes = GenerateMaterialTypes(projects[1]);
           // context.MaterialTypeMaster.AddRange(materialTypes);

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
                            int organisationIndex = rnd.Next(0, organisations.Length);
                            int materialTypeIndex = rnd.Next(0, 6);

                            maxSN++;

                            var material = GenerateMaterialMaster(materialDrawingAudits, materialTypes.ToArray(), projects[1], organisations[organisationIndex].ID,
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

        public static void PopulateDemoJobAndChecklists(astorWorkDbContext context)
        {
            Console.WriteLine("Start populating demo job and checklists");

            var jobs = new List<TradeMaster>
            {
                new TradeMaster { Name = "Plastering" },
                new TradeMaster { Name = "Waterproofing" },
                new TradeMaster { Name = "Floor Screeeding" },
                new TradeMaster { Name = "Window Frame Installation" },
                new TradeMaster { Name = "Door Frame Installation" },
                new TradeMaster { Name = "Wardrobe Installation" },
                new TradeMaster { Name = "Sanitary Fixtures Installation" }
            };
            context.TradeMaster.AddRange(jobs);

            var checklistItems = new List<ChecklistItemMaster>
            {
                new ChecklistItemMaster { Name = "Ensure bricks are properly aligned",
                    TimeFrame = "on completion" },
                new ChecklistItemMaster { Name = "Check for squareness for corner",
                    TimeFrame = "on completion" },
                new ChecklistItemMaster { Name = "Check mix consistency",
                    TimeFrame = "before application" },
                new ChecklistItemMaster { Name = "Check for evenness, hollowness, etc.",
                    TimeFrame = "after curing" },
                new ChecklistItemMaster { Name = "Ensure defects are rectified",
                    TimeFrame = "on completion" },
                new ChecklistItemMaster { Name = "Ensure all penetrations and holes are properly sealed",
                    TimeFrame = "before plastering" },
                new ChecklistItemMaster {Name = "Ensure wall is properly cleaned", TimeFrame="before plastering"},
                new ChecklistItemMaster {Name="Ensure wire mesh in place over brick/ concrete joints", TimeFrame="before plastering"},
                new ChecklistItemMaster {Name="Ensure wall is slightly dampened before plastering", TimeFrame="before plastering"},
                new ChecklistItemMaster {Name="Ensure wire mesh added to areas where plaster is too thick", TimeFrame="during plastering"},
                new ChecklistItemMaster {Name="Ensure surface is sufficiently roughened to receive final plaster", TimeFrame="during plastering"},
                new ChecklistItemMaster {Name="Ensure base plaster is suffiently cured before final plaster proceeds", TimeFrame="during plastering"},
                new ChecklistItemMaster {Name="Ensure entire wall is done in one go to avoid cold joints", TimeFrame="during plastering"},
                new ChecklistItemMaster {Name="Ensure final plastered wall is flat and smooth", TimeFrame="during plastering"},
                new ChecklistItemMaster {Name="no hollowness"},
                new ChecklistItemMaster {Name="no cracks"},
                new ChecklistItemMaster {Name="free of chipped edging/ corners"},
                new ChecklistItemMaster {Name="even edge straightness at corners"},
                new ChecklistItemMaster {Name="even/ smooth surface, no undulation"},
                new ChecklistItemMaster {Name="free of stains, mortar, paint drips"},
                new ChecklistItemMaster {Name="wall at right angles" }
            };

            context.ChecklistItemMaster.AddRange(checklistItems);

            context.SaveChanges();
            Console.WriteLine("Done populating demo job & checklist");
        }

        public static void PopulateDemoProgress(astorWorkDbContext context)
        {
            Console.WriteLine("Start populating demo progress");
            Random rnd = new Random();
            var organisationPMs = context.UserMaster.Include(um => um.Organisation).Where(um => um.RoleID == 7).ToList();
            var organisationOfficers = context.UserMaster.Include(um => um.Organisation).Where(um => um.RoleID == 8).ToList();
            var siteOfficers = context.UserMaster.Include(um => um.Project).Include(um => um.Site).Where(um => um.RoleID == 5).ToList();
            var trackers = context.TrackerMaster.ToList();
            var stages = context.MaterialStageMaster.ToList();
            var locations = context.LocationMaster.Include(loc => loc.Organisation).Include(loc => loc.Site).ToList();

            foreach (var project in context.ProjectMaster)
            {
                var mrfs = new List<MRFMaster>();
                var inventories = new List<InventoryAudit>();
                var stageAudits = new List<MaterialStageAudit>();

                var materials = context.MaterialMaster.Include(mm => mm.Project).Include(mm => mm.Organisation)
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
                            foreach (OrganisationMaster organisation in materials3.Select(mm => mm.Organisation).Distinct())
                            {
                                var materials4 = materials3.Where(mm => mm.Organisation == organisation);
                                var mrf = new MRFMaster
                                {
                                    MRFCompletion = 1,
                                    Materials = materials4.ToList(),
                                    MRFNo = project.Name.Contains("@") ? "PP09-" : "MRF-",
                                    UserMRFAssociations = new List<UserMRFAssociation>
                                    {
                                        new UserMRFAssociation
                                        {
                                            UserID = organisationPMs.Where(vpm => vpm.Organisation.ID == organisation.ID)
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
                foreach (MRFMaster mrf in mrfs)
                {
                    mrf.MRFNo = mrf.MRFNo + mrfIndex.ToString("D4");
                    mrf.OrderDate = project.EstimatedStartDate.Value.AddDays(rnd.Next(mrfIndex, mrfIndex + 3));
                    mrf.PlannedCastingDate = mrf.OrderDate.AddDays(rnd.Next(1, 3));
                    if (mrf.PlannedCastingDate != null)
                        mrf.ExpectedDeliveryDate = mrf.PlannedCastingDate.Value.AddDays(rnd.Next(5, 8));
                    mrfIndex++;

                    foreach (var material in mrf.Materials)
                    {
                        var tracker = trackers[rnd.Next(0, trackers.Count)];
                        var organisationOfficer = organisationOfficers
                            .Where(vo => vo.Organisation.ID == material.OrganisationID).First();

                        var inventory = new InventoryAudit
                        {
                            MarkingNo = material.MarkingNo,
                            CastingDate = DateTimeOffset.UtcNow,
                            SN = sn,
                            ProjectID = project.ID,
                            TrackerID = tracker.ID,
                            OrganisationID = material.OrganisationID,
                            CreatedByID = organisationOfficer.ID,
                            CreatedDate = mrf.PlannedCastingDate
                        };
                        inventories.Add(inventory);

                        var siteOfficer = siteOfficers.Where(so => so.ProjectID == project.ID).First();
                        foreach (var stage in stages)
                        {
                            LocationMaster location;
                            int createdById;
                            if (stage.Order < 3)
                            {
                                location = locations
                                    .Where(loc => loc.Type == 0 && loc.OrganisationID == material.OrganisationID).First();
                                createdById = organisationOfficer.ID;
                            }
                            else if (stage.Order < 5)
                            {
                                location = locations
                                    .Where(loc => loc.Type == 1 && loc.SiteID == siteOfficer.SiteID).First();
                                createdById = siteOfficer.ID;
                            }
                            else
                            {
                                location = locations
                                    .Where(loc => loc.Type == 2 && loc.SiteID == siteOfficer.SiteID).First();
                                createdById = siteOfficer.ID;
                            }

                            MaterialStageAudit stageAudit = new MaterialStageAudit
                            {
                                MaterialMasterID = material.ID,
                                StageID = stage.ID,
                                LocationID = location.ID,
                                CreatedByID = createdById,
                                CreatedDate = mrf.PlannedCastingDate.Value.AddDays(stage.Order)
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
            var materials = context.MaterialMaster.ToArray();

            // Generate an open case
            var openMaterialIndex = rnd.Next(1, materials.Length / 2);
            var openMaterial = materials[openMaterialIndex];
            var openSiteOfficer = siteOfficers.Where(so => so.ProjectID == openMaterial.ProjectID).First();
            var qcCaseOpen = new MaterialQCCase
            {
                CaseName = "QC-" + new string(Enumerable.Repeat(chars, 4)
                .Select(s => s[rnd.Next(s.Length)]).ToArray()),
                MaterialMasterId = openMaterial.ID,
                CreatedById = openSiteOfficer.ID,
                CreatedDate = DateTime.Today.AddDays(-5).AddHours(1),
                Defects = new List<MaterialQCDefect>()
                {
                    new MaterialQCDefect
                    {
                        Status = QCStatus.QC_passed_by_Maincon,
                        CreatedByID = openSiteOfficer.ID,
                        CreatedDate = DateTime.Today.AddDays(-5).AddHours(1),
                        Remarks = "Defect-" + new string(Enumerable.Repeat(chars, 4)
                .Select(s => s[rnd.Next(s.Length)]).ToArray()),
                        Photos = new List<MaterialQCPhotos>
                        {
                            new MaterialQCPhotos
                            {
                                Status = QCStatus.QC_passed_by_Maincon,
                                URL = imgUrlPrefix + "crack-1.jpg",
                                CreatedById = openSiteOfficer.ID,
                                CreatedDate = DateTime.Today.AddDays(-5).AddHours(1)
                            },
                            new MaterialQCPhotos
                            {
                                Status = QCStatus.QC_passed_by_Maincon,
                                URL = imgUrlPrefix + "crack-2.jpg",
                                CreatedById = openSiteOfficer.ID,
                                CreatedDate = DateTime.Today.AddDays(-5).AddHours(1)
                            }
                        }
                    }
                }
            };

            // Generate a closed case
            var closedMaterialIndex = rnd.Next(materials.Length / 2 + 1, materials.Length);
            var closedMaterial = materials[closedMaterialIndex];
            var closeSiteOfficer = siteOfficers.Where(so => so.ProjectID == closedMaterial.ProjectID).First();
            var qcCaseClosed = new MaterialQCCase
            {
                CaseName = "QC-" + new string(Enumerable.Repeat(chars, 4)
                .Select(s => s[rnd.Next(s.Length)]).ToArray()),
                MaterialMasterId = closedMaterial.ID,
                CreatedById = closeSiteOfficer.ID,
                CreatedDate = DateTime.Today.AddDays(-11).AddHours(1),
                Defects = new List<MaterialQCDefect>()
                {
                    new MaterialQCDefect
                    {
                        Status = QCStatus.QC_failed_by_Maincon,
                        CreatedByID = closeSiteOfficer.ID,
                        CreatedDate = DateTime.Today.AddDays(-11).AddHours(1),
                        UpdatedByID = closeSiteOfficer.ID,
                        UpdatedDate = DateTime.Today.AddDays(-11).AddHours(4),
                        Remarks = "Defect-" + new string(Enumerable.Repeat(chars, 4)
                .Select(s => s[rnd.Next(s.Length)]).ToArray()),
                        Photos = new List<MaterialQCPhotos>
                        {
                            new MaterialQCPhotos
                            {
                                Status = QCStatus.QC_passed_by_Maincon,
                                URL = imgUrlPrefix + "crack-1.jpg",
                                CreatedById = closeSiteOfficer.ID,
                                CreatedDate = DateTime.Today.AddDays(-11).AddHours(1)
                            },
                            new MaterialQCPhotos
                            {
                                Status = QCStatus.QC_passed_by_Maincon,
                                URL = imgUrlPrefix + "crack-2.jpg",
                                CreatedById = closeSiteOfficer.ID,
                                CreatedDate = DateTime.Today.AddDays(-11).AddHours(2)
                            },
                            new MaterialQCPhotos
                            {
                                Status = QCStatus.QC_failed_by_Maincon,
                                URL = imgUrlPrefix + "fixed-1.jpg",
                                CreatedById = closeSiteOfficer.ID,
                                CreatedDate = DateTime.Today.AddDays(-11).AddHours(3)
                            },
                            new MaterialQCPhotos
                            {
                                Status = QCStatus.QC_failed_by_Maincon,
                                URL = imgUrlPrefix + "fixed-2.jpg",
                                CreatedById = closeSiteOfficer.ID,
                                CreatedDate = DateTime.Today.AddDays(-11).AddHours(4)
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

            context.Database.Migrate();
            // Look for any materials

            if (context.UserMaster.Any())
                error = "Data has been initialized already";
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

                    OrganisationMaster[] organisations = InsertOrganisations();
                    context.OrganisationMaster.AddRange(organisations);

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

                                        MaterialMaster material = CreateMaterialMaster(organisations, materialDrawingAudits, project, blkIndex, levelIndex, zoneIndex, materialTypeIndex, materialIndex, maxSN);
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
                catch (Exception ex)
                {
                    error = ExceptionUtility.GetExceptionDetails(ex);
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
            ModuleMaster[] modulemaster = new ModuleMaster[] {
                new ModuleMaster
                {
                    Name = "Material-Tracking",
                    UrlPrefix = "material-tracking",
                    Pages = new List<PageMaster>
                    {
                        new PageMaster{Name = "List Materials", UrlPath = "materials"},
                        new PageMaster{Name = "Material Details", UrlPath = "materials/:id"},
                        new PageMaster{Name = "List MRFs", UrlPath = "mrfs"},
                        new PageMaster{Name = "Create MRF", UrlPath = "mrfs/create"},
                        new PageMaster{Name = "List BIM Sync Sessions", UrlPath = "bim-syncs"},
                        new PageMaster{Name = "BIM Sync Session Details", UrlPath = "bim-syncs/:id"},
                        new PageMaster{Name = "Dashboard", UrlPath = "dashboard"},
                        new PageMaster{Name = "List QC Defects", UrlPath = "qc-defects"},
                        new PageMaster{Name = "List QC Cases", UrlPath = "qc-cases/:id"},
                        new PageMaster{Name = "Import Materials", UrlPath = "import-material"},
                        new PageMaster{Name = "List Reports", UrlPath = "list-reports"},
                        new PageMaster{Name = "View Report",UrlPath = "powerbi-viewer/:guid"},
                        new PageMaster{ Name = "Create Stage", UrlPath = "stage-master/create"},
                        new PageMaster{ Name = "Stage Master", UrlPath = "stage-master"},
                        new PageMaster{ Name = "Organisation Master", UrlPath = "organisation-master"},
                        new PageMaster{ Name = "Organisation Details", UrlPath = "organisation-details/:id"},
                        new PageMaster{ Name = "List of Locations", UrlPath = "location-master"},
                        new PageMaster{ Name = "Location Details", UrlPath = "location-details/:id"},
                        new PageMaster{ Name = "List of Sites", UrlPath = "site-master" },
                        new PageMaster{ Name = "Site Details", UrlPath = "site-details/:id" },
                        new PageMaster{ Name = "Notification Config", UrlPath = "notification-config" },
                        new PageMaster{Name = "Material Dashboard", UrlPath = "material-dashboard"},
                        new PageMaster{ Name = "Material QC", UrlPath = "material-qc" },
                          new PageMaster{ Name = "Material Stage Dashboard", UrlPath = "materialstagedashboard" },
                    }
                },
                new ModuleMaster
                {
                    Name = "User Account",
                    UrlPrefix = "user-account",
                    Pages = new List<PageMaster>
                    {
                        new PageMaster{Name = "Edit Profile", UrlPath = ":id"},
                        new PageMaster{Name = "Change Password", UrlPath = "change-password"}
                    }
                },
                new ModuleMaster
                {
                    Name = "Configuration",
                    UrlPrefix = "configuration",
                    Pages = new List<PageMaster>
                    {
                        new PageMaster{ Name = "List of Users", UrlPath = "user-master"},
                        new PageMaster{ Name = "User Details", UrlPath = "user-details/:id"},
                        new PageMaster{ Name = "List of Roles", UrlPath = "role-master"},
                        new PageMaster{ Name = "Role Details", UrlPath = "role-details/:id"},
                        new PageMaster{ Name = "Import RFID Tags", UrlPath = "import-rfid-tags"},
                        new PageMaster{ Name = "List Projects", UrlPath = "project-master"},
                        new PageMaster{ Name = "Project Details", UrlPath = "project-details/:id"}
                    }
                },
                new ModuleMaster
                {
                    Name = "Job Tracking",
                    UrlPrefix = "job-tracking",
                    Pages = new List<PageMaster>
                    {
                        new PageMaster{ Name = "Job Association", UrlPath = "job-association" },
                        new PageMaster{ Name = "Job Scheduling", UrlPath = "job-scheduling" },
                        new PageMaster{ Name = "Job QC", UrlPath = "job-qc" },
                        new PageMaster{ Name = "Job Tasks", UrlPath = "job-tasks" },
                        new PageMaster{Name="Import Master", UrlPath = "import-master"},
                        new PageMaster{Name="Job dashboard", UrlPath = "job-dashboard"},
                        new PageMaster{Name="Import JobSchedule",UrlPath="job-scheduling/import-jobschedule"}
                    }
                }
            };
            return modulemaster;
        }


        public static RoleMaster[] InsertRoles(ModuleMaster[] modules)
        {
            // Insert roles
            RoleMaster[] roles = new RoleMaster[]
            {
                new RoleMaster { ID = 1, Name = "Super", DefaultPage = modules[2].Pages[5],
                    PlatformCode = "0123" },
                new RoleMaster { ID = 2, Name = "Admin", DefaultPage = modules[0].Pages[6],
                    PlatformCode = "0123" },
                new RoleMaster { ID = 3, Name = "Management", DefaultPage = modules[0].Pages[6],
                    PlatformCode = "0" },
                new RoleMaster { ID = 4, Name = "Project Manager", DefaultPage = modules[0].Pages[6],
                    MobileEntryPoint = 5,PlatformCode = "0123" },
                new RoleMaster { ID = 5, Name = "Site Officer", DefaultPage = modules[0].Pages[2],
                    MobileEntryPoint = 1, PlatformCode = "01" },
                new RoleMaster { ID = 6, Name = "BIM", DefaultPage = modules[0].Pages[4],
                    PlatformCode = "02" },
                new RoleMaster { ID = 7, Name = "Vendor Project Manager", DefaultPage = modules[0].Pages[2],
                    PlatformCode = "01" },
                new RoleMaster { ID = 8, Name = "Vendor Production Officer", DefaultPage = modules[0].Pages[2],
                    MobileEntryPoint = 0, PlatformCode = "01" },
                new RoleMaster { ID = 9, Name = "PPVC Subcontractor", DefaultPage = modules[3].Pages[1],
                    MobileEntryPoint = 2, PlatformCode = "01" },
                new RoleMaster { ID = 10, Name = "RTO", DefaultPage = modules[3].Pages[1],
                    MobileEntryPoint = 3, PlatformCode = "01" },
                new RoleMaster { ID = 11, Name = "Main-Con QC", DefaultPage = modules[3].Pages[1],
                    MobileEntryPoint = 4, PlatformCode = "01" },
            };

            // Access rights for Super Role
            List<RolePageAssociation> superAccessRights = new List<RolePageAssociation>();
            foreach (var module in modules)
                foreach (var page in module.Pages)
                    superAccessRights.Add(new RolePageAssociation { PageId = page.ID, AccessLevel = 3 });
                
            roles[0].RolePageAssociations = superAccessRights;

            // Access rights for Admin Role
            List<RolePageAssociation> adminAccessRights = new List<RolePageAssociation>();
            foreach (var module in modules)
                foreach (var page in module.Pages)
                    if (page != modules[2].Pages[2] && page != modules[2].Pages[3])
                        adminAccessRights.Add(new RolePageAssociation { PageId = page.ID, AccessLevel = 3 });
                    else
                        adminAccessRights.Add(new RolePageAssociation { PageId = page.ID, AccessLevel = 1 });
            
            roles[1].RolePageAssociations = adminAccessRights;

            // Access rights for Main-con higher management & PM
            List<RolePageAssociation> mainConManageAccessRights = new List<RolePageAssociation>();
            foreach (var module in modules.Where(m => m != modules[2]))
                foreach (var page in module.Pages)
                    mainConManageAccessRights.Add(new RolePageAssociation { PageId = page.ID, AccessLevel = 3 });

            roles[2].RolePageAssociations = mainConManageAccessRights;
            roles[3].RolePageAssociations = mainConManageAccessRights;

            // Access rights for Main-con site officer & Vendor PM
            List<RolePageAssociation> mainConOfficerAccessRights = new List<RolePageAssociation>(){
                new RolePageAssociation { Page = modules[0].Pages[0], AccessLevel = 1 },
                new RolePageAssociation { Page = modules[0].Pages[1], AccessLevel = 1 },
                new RolePageAssociation { Page = modules[0].Pages[2], AccessLevel = 1 },
                new RolePageAssociation { Page = modules[0].Pages[3], AccessLevel = 1 },
                new RolePageAssociation { Page = modules[0].Pages[7], AccessLevel = 1 },
                new RolePageAssociation { Page = modules[0].Pages[8], AccessLevel = 1 },
                new RolePageAssociation { Page = modules[0].Pages[10], AccessLevel = 1 },
                new RolePageAssociation { Page = modules[0].Pages[11], AccessLevel = 1 },
                new RolePageAssociation { Page = modules[1].Pages[0], AccessLevel = 3 },
                new RolePageAssociation { Page = modules[1].Pages[1], AccessLevel = 3 },
                new RolePageAssociation { Page = modules[3].Pages[0], AccessLevel = 1 },
                new RolePageAssociation { Page = modules[3].Pages[1], AccessLevel = 1 },
                new RolePageAssociation { Page = modules[3].Pages[2], AccessLevel = 1 },
                new RolePageAssociation { Page = modules[3].Pages[3], AccessLevel = 1 },
            };
            roles[4].RolePageAssociations = mainConOfficerAccessRights;
            roles[6].RolePageAssociations = mainConOfficerAccessRights;

            // Access rights for Main-con BIM
            List<RolePageAssociation> mainConBIMAccessRights = new List<RolePageAssociation>(){
                new RolePageAssociation { Page = modules[0].Pages[4], AccessLevel = 1 },
                new RolePageAssociation { Page = modules[0].Pages[5], AccessLevel = 1 },
                new RolePageAssociation { Page = modules[1].Pages[0], AccessLevel = 3 },
                new RolePageAssociation { Page = modules[1].Pages[1], AccessLevel = 3 },
            };
            roles[5].RolePageAssociations = mainConBIMAccessRights;
            // Access rights for Vendor officer
            // Will change when digital wall chart come out
            roles[7].RolePageAssociations = mainConOfficerAccessRights;
            List<RolePageAssociation> ppvcSubconAccessRights = new List<RolePageAssociation>(){
                new RolePageAssociation { Page = modules[1].Pages[0], AccessLevel = 3 },
                new RolePageAssociation { Page = modules[1].Pages[1], AccessLevel = 3 },
                new RolePageAssociation { Page = modules[3].Pages[0], AccessLevel = 1 },
                new RolePageAssociation { Page = modules[3].Pages[1], AccessLevel = 1 },
                new RolePageAssociation { Page = modules[3].Pages[2], AccessLevel = 1 },
                new RolePageAssociation { Page = modules[3].Pages[3], AccessLevel = 1 },
            };
            roles[8].RolePageAssociations = ppvcSubconAccessRights;

            return roles;
        }

        public static UserMaster[] InsertUserMasters(RoleMaster[] roles)
        {
            string pwdHashed = string.Empty;
            Guid salt = Guid.NewGuid();

            using (SHA256 algorithm = SHA256.Create())
            {
                string pwd = "abc12345";
                IEnumerable<byte> pwdSalted = Encoding.UTF8.GetBytes(pwd).Concat(salt.ToByteArray());
                byte[] hash = algorithm.ComputeHash(pwdSalted.ToArray());
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
            UserMaster[] users = new UserMaster[]
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

        private static OrganisationMaster[] InsertVendors(UserMaster[] users)
        {
            // Insert 2 vendor
            OrganisationMaster[] vendors = new OrganisationMaster[]
            {
                new OrganisationMaster{ Name = "VendorOne", CycleDays=7},
                new OrganisationMaster{ Name = "VendorTwo", CycleDays=10},
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
            MaterialStageMaster[] stages = new MaterialStageMaster[]
            {
                new MaterialStageMaster{ Name = "Produced", Colour="#ffff99", Order = 1, MilestoneId = 1,IsEditable=true, MaterialTypes=materialTypes, CanIgnoreQC = true},
                new MaterialStageMaster{ Name = "Start Delivery", Colour="#ffff66", Order = 2, IsEditable=true, MaterialTypes=materialTypes, CanIgnoreQC = true},
                new MaterialStageMaster{ Name = "Delivered", Colour="#66ff99", Order = 3, MilestoneId = 2,IsEditable=false, MaterialTypes=materialTypes, CanIgnoreQC = true},
                new MaterialStageMaster{ Name = "Final QC", Colour="#44AAff", Order = 4, IsEditable =true, MaterialTypes=materialTypes, CanIgnoreQC = false},
                new MaterialStageMaster{ Name = "Installed", Colour="#3399ff", Order = 5, MilestoneId = 3,IsEditable=false, MaterialTypes=materialTypes, CanIgnoreQC = true}
            };
            return stages;
        }

        private static ProjectMaster InsertProject()
        {
            // Insert 2 projects
            ProjectMaster project = new ProjectMaster { Name = "Demo Project" };
            return project;
        }

        private static MaterialDrawingAudit[] InsertDrawings()
        {
            MaterialDrawingAudit[] drawing = new MaterialDrawingAudit[]
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

        private static LocationMaster[] InsertLocation(OrganisationMaster[] organisations)
        {
            // Insert 6 locations
            LocationMaster[] locations = new LocationMaster[]
            {
                new LocationMaster{ Name = "VenorOne's Factory", Type = 0, Organisation = organisations[0]},
                new LocationMaster{ Name = "VenorTwo's Factory", Type = 0, Organisation = organisations[1]},
                new LocationMaster{ Name = "Storage Area 1", Type = 1},
                new LocationMaster{ Name = "Storage Area 2", Type = 1},
                new LocationMaster{ Name = "Installation Area", Type = 2}
            };
            return locations;
        }

        private static List<MaterialTypeMaster> GenerateMaterialTypes()
        {
            List<MaterialTypeMaster> materialTypes = new List<MaterialTypeMaster>();
            foreach (string materialTypeName in LIST_MATERIAL_TYPES)
            {
                MaterialTypeMaster materialType = new MaterialTypeMaster
                {
                    Name = materialTypeName,
                };
                materialTypes.Add(materialType);
            }

            return materialTypes;
        }

        private static MaterialMaster GenerateMaterialMaster(MaterialDrawingAudit[] materialDrawingAudits, MaterialTypeMaster[] materialTypes, ProjectMaster project, int organisationID, string blk, int levelIndex, int zoneIndex, int materialTypeIndex, int materialIndex, int maxSN)
        {
            Random rnd = new Random();
            string[] precastZones = { "A", "B", "C", "D" };
            string[] ppvcZones = { "1", "3", "5", "7" };
            MaterialTypeMaster materialType = materialTypes[materialTypeIndex];
            string zone = materialType.Name.StartsWith("PPVC") ? ppvcZones[zoneIndex] : precastZones[zoneIndex];
            string markingNo = string.Format("{0}{1}{2}{3}{4}", project.Name.Substring(0, 1), zoneIndex + 1, materialType.Name[0], materialIndex + 1, (char)(materialTypeIndex + 65));
            if (materialType.Name.EndsWith("LivingRoom"))
                markingNo = "3RLD-" + (materialIndex + 1);
            if (materialType.Name.EndsWith("Bedroom"))
                markingNo = "3RB2A-" + (materialIndex + 1);
            if (materialType.Name.EndsWith("Kitchen"))
                markingNo = "3RKC-" + (materialIndex + 1);
            if (materialType.Name.EndsWith("Bedroom-PBU"))
                markingNo = "3RMB1A-PBU-" + (materialIndex + 1);
            if (materialType.Name.EndsWith("Kitchen-PBU"))
                markingNo = "3R1KC-PBU-" + (materialIndex + 1);

            MaterialMaster material = new MaterialMaster
            {
                Block = blk,
                Level = (levelIndex + 1).ToString(),
                Zone = zone,
                ProjectID = project.ID,
                OrganisationID = organisationID,
                MaterialType = materialType,
                MarkingNo = markingNo
            };

            material.DrawingAssociations = createDrawingAssociations(materialDrawingAudits, material);

            return material;
        }

        private static MaterialMaster CreateMaterialMaster(OrganisationMaster[] organisations, MaterialDrawingAudit[] materialDrawingAudits, ProjectMaster project, int blkIndex, int levelIndex, int zoneIndex, int materialTypeIndex, int materialIndex, int maxSN)
        {
            Random rnd = new Random();
            int dateDiff = rnd.Next(7, 30);
            string[] zones = { "A", "B", "C", "D" };
            List<MaterialTypeMaster> materialTypes = GenerateMaterialTypes();

            string blk = "BLK " + blkIndex;
            int organisationIndex = rnd.Next(0, 2);
            OrganisationMaster organisation = organisations[organisationIndex];
            //var trackers = context.TrackerMaster.ToList();

            MaterialMaster material = new MaterialMaster
            {
                Block = blk,
                Level = (levelIndex + 1).ToString(),
                CastingDate = DateTime.Today.AddDays(-dateDiff),
                Zone = zones[zoneIndex],
                Project = project,
                //Tracker = trackers[trackerIndex],
                Organisation = organisation,
                MaterialType = materialTypes[materialTypeIndex],
                MarkingNo = string.Format("{0}{1}{2}{3}{4}", project.Name.Substring(0, 1), zoneIndex + 1, materialTypes[materialTypeIndex].Name, materialIndex + 1, (char)(materialTypeIndex + 65)),
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
                MaterialDrawingAssociation materialDrawignAssociation = new MaterialDrawingAssociation
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
                InventoryAudit inventory = new InventoryAudit
                {
                    MarkingNo = material.MarkingNo,
                    CastingDate = DateTimeOffset.UtcNow,
                    SN = maxSN,
                    ProjectID = project.ID,
                    //Tracker = trackers[context.InventoryAudit.Count()],
                    OrganisationID = material.Organisation.ID
                };

                return inventory;
            }

            return null;
        }
    }
}
