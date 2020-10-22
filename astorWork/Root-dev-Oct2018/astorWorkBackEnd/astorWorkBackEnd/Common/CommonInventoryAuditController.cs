using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using astorWorkDAO;
using astorWorkBackEnd.Models;

namespace astorWorkBackEnd.Common
{
    public class CommonInventoryAuditController : CommonController
    {
        public CommonInventoryAuditController(astorWorkDbContext context) : base(context)
        {
        }

        protected int GetMaxSN() {
            int materialMasterMaxSN = _context.MaterialMaster.Count() == 0 ? 0 : _context.MaterialMaster.Max(m => m.SN);
            int inventoryAuditMaxSN = _context.InventoryAudit.Count() == 0 ? 0 : _context.InventoryAudit.Max(m => m.SN);

            return materialMasterMaxSN > inventoryAuditMaxSN ? materialMasterMaxSN : inventoryAuditMaxSN;
        }

        protected InfoForNewInventory CreateInfoForNewInventory(IEnumerable<MaterialMaster> markingNos) {
            InfoForNewInventory infoForNewInventory = new InfoForNewInventory();

            infoForNewInventory.maxSN = GetMaxSN();
            foreach (string m in markingNos.Select(m => m.MarkingNo).Distinct())
            {
                if (infoForNewInventory.markingNos == null)
                    infoForNewInventory.markingNos = new List<string>();

                infoForNewInventory.markingNos.Add(m);
            }

            return infoForNewInventory;
        }

        protected async Task<InventoryAudit> CreateInventoryInDb(Inventory inventory, int projectID, int vendorID) {

            var user = _context.GetUserFromHttpContext(HttpContext);

            var inventoryAudit = new InventoryAudit
            {
                MarkingNo = inventory.MarkingNo,
                SN = inventory.SN,
                CastingDate = inventory.CastingDate,
                Tracker = _context.TrackerMaster.Where(t => t.ID == inventory.TrackerID).FirstOrDefault(),
                Project = _context.ProjectMaster.Where(p => p.ID == projectID).FirstOrDefault(),
                Vendor = _context.VendorMaster.Where(v => v.ID == vendorID).FirstOrDefault(),
                CreatedByID = user.ID,
                CreatedDate = DateTimeOffset.Now
            };

            _context.InventoryAudit.Add(inventoryAudit);
            await _context.SaveChangesAsync();

            return inventoryAudit;
        }

        protected bool TrackerIsUsed(int trackerID)
        {
            var material = _context.MaterialMaster
                .Where(m => m.Tracker.ID == trackerID);

            if (material.Count() > 0)
                return true;

            var inventory = _context.InventoryAudit
                            .Where(m => m.Tracker.ID == trackerID);

            return (inventory.Count() > 0);
        }

        protected  bool InventoryAuditExists(int id)
        {
            return _context.InventoryAudit.Any(e => e.ID == id);
        }
    }
}