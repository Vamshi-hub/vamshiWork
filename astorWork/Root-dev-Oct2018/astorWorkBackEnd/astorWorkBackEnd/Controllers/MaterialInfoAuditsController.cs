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
using astorWorkShared.Services;

namespace astorWorkBackEnd.Controllers
{
    [Produces("application/json")]
    [Route("projects/{project_id}/materials/edit")]
    public class MaterialInfoAuditsController : CommonMaterialInfoAuditsController
    {
        private readonly string Module = "Material";
        private readonly string Field = "ID";

        public MaterialInfoAuditsController(astorWorkDbContext context) : base(context)
        {
        }

        // GET: projects/{project_id}/materials/edit/11
        [HttpGet("{id}")]
        public async Task<APIResponse> GetMaterialDetail([FromRoute] int id)
        {
            if (!ModelState.IsValid)
                return new APIBadRequest();

            MaterialMaster materialMaster = await _context.MaterialMaster.Include(m => m.MRF)
                                                  .Include(m => m.Tracker)
                                                  .ThenInclude(t => t.Type)
                                                  .SingleOrDefaultAsync(m => m.ID == id);

            if (materialMaster == null)
                return new DbRecordNotFound(Module, Field, id.ToString());

            return new APIResponse(0, CreateMaterialDetail(materialMaster));
        }

        // PUT: projects/{project_id}/materials/edit/11
        [HttpPut("{id}")]
        public async Task<APIResponse> EditMaterialDetail([FromRoute] int id, [FromBody] MaterialDetail materialDetail)
        {
            if (!ModelState.IsValid)
                return new APIBadRequest();

            var materialMaster = await _context.MaterialMaster.SingleOrDefaultAsync(m => m.ID == id);

            if (id != materialMaster.ID)
                return new DbRecordNotFound(Module, Field, id.ToString());

            var materialInfoAudit = await _context.MaterialInfoAudit.SingleOrDefaultAsync(m => m.Material.ID == id);

            // check to see if other fields get edited
            materialInfoAudit.Remarks = materialDetail.Remarks;
            materialInfoAudit.ExpectedDeliveryDate = materialDetail.ExpectedDeliveryDate == null ? Convert.ToDateTime(materialDetail.ExpectedDeliveryDate) : DateTimeOffset.Parse(materialDetail.ExpectedDeliveryDate.ToString());
            _context.Entry(materialInfoAudit).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException exc)
            {
                if (!MaterialInfoAuditExists(id))
                    return new DbConcurrentUpdate(exc.Message);
                else
                    throw;
            }
            
            return new APIResponse(0, null);
        }

        private bool MaterialInfoAuditExists(int id)
        {
            return _context.MaterialInfoAudit.Any(e => e.ID == id);
        }

        // POST: api/MaterialInfoAudits
        [HttpPost]
        public async Task<IActionResult> PostMaterialInfoAudit([FromBody] MaterialInfoAudit materialInfoAudit)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.MaterialInfoAudit.Add(materialInfoAudit);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMaterialInfoAudit", new { id = materialInfoAudit.ID }, materialInfoAudit);
        }

        // DELETE: api/MaterialInfoAudits/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMaterialInfoAudit([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var materialInfoAudit = await _context.MaterialInfoAudit.SingleOrDefaultAsync(m => m.ID == id);
            if (materialInfoAudit == null)
            {
                return NotFound();
            }

            _context.MaterialInfoAudit.Remove(materialInfoAudit);
            await _context.SaveChangesAsync();

            return Ok(materialInfoAudit);
        }

        // GET: api/MaterialInfoAudits
        [HttpGet("materialInfoAudit")]
        public IEnumerable<MaterialInfoAudit> GetMaterialInfoAudit()
        {
            return _context.MaterialInfoAudit;
        }
    }
}