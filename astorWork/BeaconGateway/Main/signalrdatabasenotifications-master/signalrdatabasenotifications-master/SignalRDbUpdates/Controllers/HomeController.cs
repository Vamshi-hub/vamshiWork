using SignalRDbUpdates.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SignalRDbUpdates.Controllers
{
    public class HomeController : Controller
    {
        private astorTrack_GEEntities db = new astorTrack_GEEntities();
        private static List<MaterialMaster> producedList = new List<MaterialMaster>();
        private static DateTime lastSyncDate;

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult GetBeacons()
        {
            BeaconsRepository _beaconRepository = new BeaconsRepository();

            // Get all detected beacons
            IEnumerable<Beacon> beacons = _beaconRepository.GetAllBeacons();
            if (lastSyncDate == DateTime.MinValue)
                deleteBeacons();

            // Only continue processing if there are beacons detected
            if (beacons.Count() > 0)
                getProducedList();
            else
                return PartialView("_BeaconsList", beacons);

            // Only continue processing if there are Produced components left to process
            if (producedList.Count > 0)
            {
                // Process each detected beacon and delete from list after it has been processed
                foreach (Beacon beacon in beacons)
                {
                    //if (!processedList.Contains(beacon))
                    //{
                    processBeacon(beacon.BeaconID, beacon.CreatedDate, 5, "admin");
                    deleteBeacon(beacon.BeaconID);
                    //processedList.Add(beacon);
                    //}
                }
            }

            return PartialView("_BeaconsList", beacons);
        }

        public void processBeacon(string beaconId, Nullable<System.DateTime> createdDate, int locationID, string createdBy)
        {
            // Check against produced list to process only components that are in Produced stage
            for (int i = 0; i < producedList.Count; i++)
            {
                if (producedList[i].BeaconID == beaconId)
                {
                    // Insert into Delivered row into MaterialDetail table
                    // MaterialDetail table contains a db trigger that will update MaterialMaster status to Delivered
                    updateMaterialStatus(producedList[i], createdDate, locationID, createdBy);

                    // Remove processed beacons from the Produced list
                    producedList.Remove(producedList[i]);

                    return;
                }
            }
        }

        private void deleteBeacon(string beaconID)
        {
            List<Beacon> beacons = db.Beacons.Where(b => b.BeaconID == beaconID).ToList();

            deleteBeaconsFromDb(beacons);
        }

        private void deleteBeacons() {
            List<Beacon> beacons = db.Beacons.ToList();

            deleteBeaconsFromDb(beacons);
        }

        private void deleteBeaconsFromDb(List<Beacon> beacons) {
            foreach (Beacon beacon in beacons)
                db.Entry(beacon).State = System.Data.Entity.EntityState.Deleted;

            db.SaveChanges();
        }

        public void updateMaterialStatus(MaterialMaster materialMaster, Nullable<System.DateTime> createdDate, int locationID, string createdBy)
        {
            MaterialDetail materialDetail = new MaterialDetail();

            materialDetail.MaterialNo = materialMaster.MaterialNo;
            materialDetail.MarkingNo = materialMaster.MarkingNo;
            materialDetail.Stage = "Delivered";
            materialDetail.IsQC = true;
            materialDetail.QCStatus = "Pass";
            materialDetail.QCBy = "admin";
            materialDetail.QCDate = DateTime.Now;
            materialDetail.LocationID = locationID;
            materialDetail.BeaconID = materialMaster.BeaconID;
            materialDetail.CreatedBy = createdBy;
            materialDetail.CreatedDate = createdDate;

            //MaterialsRepository _materialRepository = new MaterialsRepository();
            updateMaterialDb(materialDetail);
            //_materialRepository.UpdateMaterialMaster(materialDetail.MaterialNo);
        }

        private void updateMaterialDb(MaterialDetail materialDetail) {
            // Insert to MaterialDetal
            db.MaterialDetails.Add(materialDetail);

            // Update MaterialMaster
            MaterialMaster mm = db.MaterialMaster.Where(m => m.MaterialNo == materialDetail.MaterialNo && m.Status == "Produced").FirstOrDefault<MaterialMaster>();
            if (mm == null)
                return;

            mm.Status = "Delivered";
            mm.UpdatedDate = DateTime.Now;
            mm.UpdatedBy = "admin";
            db.Entry(mm).State = System.Data.Entity.EntityState.Modified;

            db.SaveChanges();
        }

        public void getProducedList()
        {
            //ConfigRepository _configRepository = new ConfigRepository();
            //DateTime lastBeaconSync = _configRepository.GetLastBeaconSync();

            // Check if the current Produced list is the latest 
            // and get the latest from db if it's not latest
            //if (lastBeaconSync > lastSyncDate || lastSyncDate == null)
            //{
            //MaterialsRepository _materialsRepository = new MaterialsRepository();

            List<MaterialMaster> producedList2 = db.MaterialMaster.Where(m => m.Status == "Produced" && m.UpdatedDate > lastSyncDate).ToList();
            //_materialsRepository.GetProducedList(lastSyncDate);
            foreach (MaterialMaster mm in producedList2)
                producedList.Add(mm);

            if (producedList2.Count > 0)
                lastSyncDate = DateTime.Now; //lastBeaconSync;
            //}
        }
    }
}