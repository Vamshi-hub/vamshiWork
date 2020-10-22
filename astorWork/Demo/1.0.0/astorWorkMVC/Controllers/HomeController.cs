using astorWorkMVC.Models;
using System.Data.Entity;
using astorWorkMVC.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Net;

namespace astorWorkMVC.Controllers
{
    public class HomeController : Controller
    {
        private astorWorkEntities db = new astorWorkEntities();

        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View();
        }

        public async Task<ActionResult> Echo()
        {
            ViewBag.Title = "Echo";

            return View(await db.BeaconGateWayDatas.ToListAsync());
        }

        public async Task<ActionResult> EchoDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var beaconData = await db.BeaconGateWayDatas.FindAsync(id);
            if (beaconData == null)
            {
                return HttpNotFound();
            }
            return View(beaconData);
        }
    }
}
