using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using astorWorkMVC.Models;
using System.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.File;
using astorWorkMVC.Utilities;

namespace astorWorkMVC.Controllers
{
    public class BIMSyncHistsController : Controller
    {
        private astorWorkEntities db = new astorWorkMVC.Models.astorWorkEntities();

        // GET: BIMSyncHists
        public async Task<ActionResult> Index()
        {
            var bIMSyncHists = db.BIMSyncHists.Include(b => b.BIMUserInfo);
            return View(await bIMSyncHists.ToListAsync());
        }

        // GET: BIMSyncHists/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BIMSyncHist bIMSyncHist = await db.BIMSyncHists.FindAsync(id);
            if (bIMSyncHist == null)
            {
                return HttpNotFound();
            }

            var syncedIds = Array.ConvertAll(bIMSyncHist.SyncedMaterialIds.Split(','), s => long.Parse(s));
            var missingIds = Array.ConvertAll(bIMSyncHist.MissingMaterialIds.Split(','), s => long.Parse(s));
            ViewBag.SyncSession = bIMSyncHist;
            if(syncedIds != null)
                ViewBag.SyncedMaterials = db.MaterialMasters.Where(mm => syncedIds.Contains(mm.MaterialID));
            else
                ViewBag.SyncedMaterials = null;
            if (missingIds != null)
                ViewBag.MissingMaterials = db.MaterialMasters.Where(mm => missingIds.Contains(mm.MaterialID));
            else
                ViewBag.MissingMaterials = null;
            
            //ViewBag.VisualizeUrl = await AzureHelper.GetBIMVisualizationUrl(bIMSyncHist.UserID, bIMSyncHist.VisualizeURL); 
            return View();
        }
        
        // GET: BIMSyncHists/Visualize/5
        public async Task<ActionResult> Visualize(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BIMSyncHist bIMSyncHist = await db.BIMSyncHists.FindAsync(id);
            if (bIMSyncHist == null)
            {
                return HttpNotFound();
            }
            if (string.IsNullOrEmpty(bIMSyncHist.VisualizeURL))
                return HttpNotFound();
            else
            {
                var uri = await AzureHelper.GetBIMVisualizationUrl(bIMSyncHist.UserID, bIMSyncHist.VisualizeURL);
                return View("Visualize", uri);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
