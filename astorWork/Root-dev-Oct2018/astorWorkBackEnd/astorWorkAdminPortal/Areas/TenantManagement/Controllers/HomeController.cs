using astorWorkDAO;
using astorWorkDAO.Data;
using astorWorkShared.Models;
using astorWorkShared.MultiTenancy;
using astorWorkShared.Services;
using astorWorkShared.Utilities;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace astorWorkAdminPortal.Areas.TenantManagement.Controllers
{
    [Area("TenantManagement")]
    public class HomeController : Controller
    {
        IAstorWorkTableStorage _tableStorage;
        IAstorWorkBlobStorage _blobStorage;
        IAstorWorkForge _forge;

        public HomeController(IAstorWorkTableStorage tableStorage, IAstorWorkBlobStorage blobStorage, IAstorWorkForge forge)
        {
            _tableStorage = tableStorage;
            _blobStorage = blobStorage;
            _forge = forge;
        }

        public async Task<IActionResult> Index()
        {
            var tenants = await _tableStorage.ListTenants();
            return View(tenants);
        }

        public async Task<IActionResult> Operations(string id)
        {
            var tenant = await _tableStorage.GetTenantInfo(id);
            return View(tenant);
        }

        public async Task<IActionResult> Edit(TenantInfo tenant)
        {
            tenant.Timestamp = DateTimeOffset.Now;
            var success = await _tableStorage.AddOrUpdateEntity(AppConfiguration.GetTenantTableName(), tenant);
            if (!success)
                ViewBag.Message = "Fail to update tenant";
            return View("Operations", tenant);
        }

        [HttpPost]
        public async Task<IActionResult> Database(string rowKey, string Op)
        {
            var message = string.Empty;
            var tenant = await _tableStorage.GetTenantInfo(rowKey);
            if (tenant == null)
                message = "No tenant specified";
            else
            {
                string dbUser = Environment.GetEnvironmentVariable("SQL_SERVER_USER");
                string dbPassword = Environment.GetEnvironmentVariable("SQL_SERVER_PASSWORD");

                using (var context =
                    new astorWorkDbContext(tenant.DBServer, tenant.DBName, dbUser, dbPassword))
                {
                    try
                    {
                        switch (Op)
                        {
                            case "Drop":
                                context.DropDB();
                                message = "Drop DB successfully";
                                break;
                            case "Migrate":
                                context.MigrateDB();
                                message = "Migrate DB successfully";
                                break;
                            case "Add login":
                                context.GrantDBOwner(tenant.DBUserID);
                                message = $"Grant DBO role to {tenant.DBUserID} successfully";
                                break;
                            case "Populate basic data":
                                //var context2 = new astorWorkDAO.astorWorkDbContext(tenant);
                                DbInitializer.PopulateBasicData(context);
                                message = $"Populate basic data successfully";
                                break;
                            case "Populate demo setup":
                                DbInitializer.PopulateDemoSetup(context);
                                message = $"Populate demo setup successfully";
                                break;
                            case "Populate demo materials":
                                DbInitializer.PopulateDemoMaterials(context);
                                message = $"Populate demo materials successfully";
                                break;
                            case "Populate demo progress":
                                DbInitializer.PopulateDemoProgress(context);
                                message = $"Populate demo progress successfully";
                                break;
                            case "Populate demo QC":
                                string imgUrlPrefix = $"{Environment.GetEnvironmentVariable("BLOB_STORAGE_HOST")}/{Environment.GetEnvironmentVariable("IMAGE_CONTAINER")}/";
                                DbInitializer.PopulateDemoQC(context, imgUrlPrefix);
                                message = $"Populate demo QC successfully";
                                break;
                            case "Populate demo BIM association":
                                message = $"Under construction";
                                break;
                            default:
                                message = "Unkown DB operation";
                                break;
                        }
                    }
                    catch (Exception exc)
                    {
                        message = $"Exception happended: {exc.Message}";
                        if (exc.InnerException != null)
                        {
                            message += Environment.NewLine;
                            message += exc.InnerException.Message;
                        }
                        else
                        {
                            message += Environment.NewLine;
                            message += exc.StackTrace;
                        }
                    }
                }
            }
            ViewBag.Message = message;

            return View("Operations", tenant);
        }

        [HttpPost]
        public async Task<IActionResult> AssociateForge(string rowKey, string bucketKey)
        {
            var message = string.Empty;
            var tenant = await _tableStorage.GetTenantInfo(rowKey);
            if (tenant == null)
                message = "No tenant specified";
            else if (string.IsNullOrEmpty(bucketKey))
                message = "No Forge bucket specified";
            else
            {
                try
                {
                    var listObjects = await _forge.GetObjects(bucketKey);
                    if (listObjects == null)
                        message = "No Forge objects found";
                    else
                    {
                        using (var context = new astorWorkDbContext(tenant.DBServer, tenant.DBName,
                            tenant.DBUserID, tenant.DBPassword, 120))
                        {
                            foreach (var forgeObject in listObjects)
                            {
                                var forgeModel = JsonConvert.DeserializeObject<BIMForgeModel>(JsonConvert.SerializeObject(forgeObject));

                                var modelData = await _forge.GetMetadata(forgeObject.ObjectId);
                                var modelDataJson = modelData.ToJson();
                                var metadata = modelDataJson["data"]["metadata"][0].ToObject<ForgeMetadata>();
                                var viewData = await _forge.GetViewMetadata(forgeObject.ObjectId, metadata.Guid);
                                var viewDataJson = viewData.ToJson();
                                var rootElement = viewDataJson["data"]["objects"][0].ToObject<ForgeElement>();
                                var dbIds = rootElement.Flatten().Distinct().ToArray();

                                var listMM = context.MaterialMaster.Where(mm => dbIds.Contains(mm.ID)).ToList();
                                forgeModel.Elements = listMM.Select(mm => new BIMForgeElement
                                {
                                    DbId = mm.ID,
                                    MaterialMasterId = mm.ID
                                }).ToList();
                                await context.BIMForgeModel.AddAsync(forgeModel);
                                message += $"Added {forgeModel.ObjectId} with {forgeModel.Elements.Count} elements";
                                message += Environment.NewLine;
                            }
                            await context.SaveChangesAsync();
                        }
                    }
                }
                catch (Exception exc)
                {
                    message = $"Exception happended: {exc.Message}";
                    if (exc.InnerException != null)
                    {
                        message += Environment.NewLine;
                        message += exc.InnerException.Message;
                    }
                    else
                    {
                        message += Environment.NewLine;
                        message += exc.StackTrace;
                    }
                }
            }
            ViewBag.Message = message;

            return View("Operations", tenant);
        }
    }
}