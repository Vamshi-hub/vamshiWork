using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using astorWorkDAO;
using astorWorkShared.GlobalExceptions;
using astorWorkShared.GlobalResponse;
using static astorWorkShared.Utilities.Enums;
using astorWorkMaterialTracking.Models;
using astorWorkShared.Services;
using astorWorkMaterialTracking.Common;

namespace astorWorkMaterialTracking.Controllers
{
    [Produces("application/json")]
    [Route("checklist-item-audit")]
    [ApiController]
    public class ChecklistItemAuditsController : CommonController
    {
        public ChecklistItemAuditsController(astorWorkDbContext context, IAstorWorkBlobStorage blobStorage) : base(context)
        {
            _context = context;
            _blobStorage = blobStorage;
        }

        // GET: api/ChecklistItemAudits
        [HttpGet]
        public async Task<SignedChecklist> GetChecklistItemAudits([FromQuery] int material_id, [FromQuery] int checklist_id)
        {
            if (!ModelState.IsValid)
                throw new GenericException(ErrorMessages.BadRequest, ErrorMessages.BadRequestMsg());

            try
            {
                int producedStageID = 1;

                List<ChecklistAudit> checklistAudits = await _context.ChecklistAudit.Include(ca => ca.CreatedBy)
                                                                                    .ThenInclude(cb => cb.Role)
                                                                                    .Where(ca => ca.MaterialStageAudit.MaterialMasterID == material_id
                                                                                              && ca.ChecklistID == checklist_id
                                                                                              && ca.MaterialStageAudit.StageID == producedStageID
                                                                                          )
                                                                                    .ToListAsync();
                                                                                    

                List<ChecklistItem> checklistItems = new List<ChecklistItem>();

                if (checklistAudits != null && checklistAudits.Count > 0)
                {
                    UserMaster user = await _context.GetUserFromHttpContext(HttpContext);

                    List<ChecklistItemAudit> checklistItemAudits = await GetLatestChecklist(checklistAudits);

                    if (checklistItemAudits != null && checklistItemAudits.Count > 0) 
                        checklistItems = CreateChecklist(checklistItemAudits);  // Get checklist item audit records from db if they exist
                    else
                        checklistItems = await CreateChecklist(checklist_id);   // Create checklist items if no checklist item audits exist in db
                }
                else
                    checklistItems = await CreateChecklist(checklist_id);   // Create a checklist items if no checklist audits exist in db

                return new SignedChecklist
                {
                    ChecklistItems = checklistItems,
                    Signatures = GetSignatures(checklistAudits)
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // GET: api/ChecklistItemAudits/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ChecklistItemAudit>> GetChecklistItemAudit(int id)
        {
            var checklistItemAudit = await _context.ChecklistItemAudit.FindAsync(id);

            if (checklistItemAudit == null)
            {
                return NotFound();
            }

            return checklistItemAudit;
        }

        // PUT: api/ChecklistItemAudits/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutChecklistItemAudit(int id, ChecklistItemAudit checklistItemAudit)
        {
            if (id != checklistItemAudit.ID)
            {
                return BadRequest();
            }

            _context.Entry(checklistItemAudit).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ChecklistItemAuditExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/ChecklistItemAudits
        [HttpPost]
        public async Task<ActionResult<ChecklistItemAudit>> PostChecklistItemAudit(ChecklistItemAudit checklistItemAudit)
        {
            _context.ChecklistItemAudit.Add(checklistItemAudit);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetChecklistItemAudit", new { id = checklistItemAudit.ID }, checklistItemAudit);
        }

        // DELETE: api/ChecklistItemAudits/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ChecklistItemAudit>> DeleteChecklistItemAudit(int id)
        {
            var checklistItemAudit = await _context.ChecklistItemAudit.FindAsync(id);
            if (checklistItemAudit == null)
            {
                return NotFound();
            }

            _context.ChecklistItemAudit.Remove(checklistItemAudit);
            await _context.SaveChangesAsync();

            return checklistItemAudit;
        }

        private bool ChecklistItemAuditExists(int id)
        {
            return _context.ChecklistItemAudit.Any(e => e.ID == id);
        }
    }
}
