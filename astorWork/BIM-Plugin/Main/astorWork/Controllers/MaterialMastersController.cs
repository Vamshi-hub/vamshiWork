using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using astorWork.Models;

namespace astorWork.Controllers
{
    public class MaterialMastersController : Controller
    {
        private astorWorkEntities db = new astorWorkEntities();

        // GET: MaterialMasters
        public async Task<ActionResult> Index()
        {
            return View(await db.MaterialMasters.ToListAsync());
        }

        public async Task<ActionResult> BIMHist()
        {
            return View(await db.BIMSyncHists.ToListAsync());
        }


        // GET: MaterialMasters/Details/5
        public async Task<ActionResult> Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MaterialMaster materialMaster = await db.MaterialMasters.FindAsync(id);
            if (materialMaster == null)
            {
                return HttpNotFound();
            }
            return View(materialMaster);
        }

        // GET: MaterialMasters/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: MaterialMasters/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "MarkingNo,DrawingNo,MaterialType,MaterialSize,EstimatedLength,LocationID,CreatedBy,CreatedDate,UpdatedBy,UpdatedDate,Status,Contractor,ActualLength,DrawingRevisionNo,Officer,Project,Remarks,RFIDTagID,BeaconID,MRFNo,DrawingIssueDate,Block,Level,Zone,MaterialNo,DeliveryDate,DeliveryRemarks,CastingDate,LotNo")] MaterialMaster materialMaster)
        {
            if (ModelState.IsValid)
            {
                db.MaterialMasters.Add(materialMaster);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(materialMaster);
        }

        // GET: MaterialMasters/Edit/5
        public async Task<ActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MaterialMaster materialMaster = await db.MaterialMasters.FindAsync(id);
            if (materialMaster == null)
            {
                return HttpNotFound();
            }
            return View(materialMaster);
        }

        // POST: MaterialMasters/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "MarkingNo,DrawingNo,MaterialType,MaterialSize,EstimatedLength,LocationID,CreatedBy,CreatedDate,UpdatedBy,UpdatedDate,Status,Contractor,ActualLength,DrawingRevisionNo,Officer,Project,Remarks,RFIDTagID,BeaconID,MRFNo,DrawingIssueDate,Block,Level,Zone,MaterialNo,DeliveryDate,DeliveryRemarks,CastingDate,LotNo")] MaterialMaster materialMaster)
        {
            if (ModelState.IsValid)
            {
                db.Entry(materialMaster).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(materialMaster);
        }

        // GET: MaterialMasters/Delete/5
        public async Task<ActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MaterialMaster materialMaster = await db.MaterialMasters.FindAsync(id);
            if (materialMaster == null)
            {
                return HttpNotFound();
            }
            return View(materialMaster);
        }

        // POST: MaterialMasters/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(long id)
        {
            MaterialMaster materialMaster = await db.MaterialMasters.FindAsync(id);
            db.MaterialMasters.Remove(materialMaster);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
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
