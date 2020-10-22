using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using astorWorkDAO;
using astorWorkBackEnd.Models;
using astorWorkShared.GlobalResponse;
using astorWorkBackEnd.Common;

namespace astorWorkBackEnd.Controllers
{
    [Produces("application/json")]
    [Route("projects/{project_id}/vendors/{vendor_id}/inventory")]
    public class InventoryAuditsController : CommonInventoryAuditController
    {
        private readonly string Module = "Inventory";

        public InventoryAuditsController(astorWorkDbContext context) : base(context)
        {
        }

        // GET: /projects/{project_id}/vendors/{vendor_id}/inventory
        [HttpGet()]
        public APIResponse ListInventory([FromRoute] int project_id, [FromRoute] int vendor_id)
        {
            if (!ModelState.IsValid)
                return new APIBadRequest();

            var inventoryAudit = _context.InventoryAudit
                                        .Where(i => i.Project.ID == project_id
                                        && i.Vendor.ID == vendor_id
                                        && i.Tracker != null)
                                        .Include(i => i.Tracker)
                                        .Select(i => new { i.ID, i.MarkingNo, i.SN, i.CastingDate, i.Tracker.Label });

            if (inventoryAudit == null)
                return new DbRecordNotFound(Module, "Vendor ID", vendor_id.ToString());

            List<Inventory> inventoryList = new List<Inventory>();

            foreach (var i in inventoryAudit)
            {
                Inventory inventory = new Inventory();
                inventory.ID = i.ID;
                inventory.MarkingNo = i.MarkingNo;
                inventory.SN = i.SN;
                inventory.CastingDate = i.CastingDate;
                inventory.TrackerLabel = i.Label;
                inventoryList.Add(inventory);
            }

            return new APIResponse(0, inventoryList);
        }

        // GET: /projects/{project_id}/vendors/{vendor_id}/inventory/pre-create?material_type={material_type}
        [HttpGet("pre-create")]
        public APIResponse GetInformationForNewInventory([FromRoute] int project_id, [FromRoute] int vendor_id, [FromQuery] string material_type)
        {
            if (!ModelState.IsValid)
                return new APIBadRequest();

            IEnumerable<MaterialMaster> markingNos = _context.MaterialMaster
                                                     .Where(m => m.Project.ID == project_id
                                                            && m.Vendor.ID == vendor_id
                                                            && m.MaterialType == material_type);

            if (markingNos == null)
                return new DbRecordNotFound(Module, "Material Type", material_type);

            return new APIResponse(0, CreateInfoForNewInventory(markingNos));
        }

        // POST: /projects/{project_id}/vendors/{vendor_id}/inventory
        [HttpPost()]
        public async Task<APIResponse> CreateInventory([FromRoute] int project_id, [FromRoute] int vendor_id, [FromBody] Inventory inventory)
        {
            if (!ModelState.IsValid)
                return new APIBadRequest();

            if (TrackerIsUsed(inventory.TrackerID))
                return new DbDuplicateRecord("Tracker", "ID", inventory.TrackerID.ToString());

            return new APIResponse(0, await CreateInventoryInDb(inventory, project_id, vendor_id));
        }

        // DELETE: /projects/{project_id}/vendors/{vendor_id}/inventory/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInventoryAudit([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var inventoryAudit = await _context.InventoryAudit.SingleOrDefaultAsync(m => m.ID == id);
            if (inventoryAudit == null)
            {
                return NotFound();
            }

            _context.InventoryAudit.Remove(inventoryAudit);
            await _context.SaveChangesAsync();

            return Ok(inventoryAudit);
        }
    }
}