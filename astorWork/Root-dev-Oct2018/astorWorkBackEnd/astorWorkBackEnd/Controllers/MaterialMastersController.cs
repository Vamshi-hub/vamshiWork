using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using astorWorkDAO;
using astorWorkBackEnd.Models;
using System;
using astorWorkShared.GlobalResponse;
using astorWorkBackEnd.Common;
using astorWorkShared.Services;
using System.IO;
using System.Collections.Generic;

namespace astorWorkBackEnd.Controllers
{
    [Produces("application/json")]
    [Route("projects/{project_id}/materials")]
    public class MaterialMastersController : CommonMaterialMastersController
    {
        private readonly string Module = "Material";
        private IAstorWorkBlobStorage _blobStorage;

        public MaterialMastersController(astorWorkDbContext context, IAstorWorkBlobStorage blobStorage): base(context)
        {
            _blobStorage = blobStorage;
        }

        // GET: projects/{project_id}/materials?Block={0}
        [HttpGet()]
        public APIResponse ListMaterials([FromRoute] int project_id, [FromQuery] string block)
        {
            if (!ModelState.IsValid || string.IsNullOrEmpty(block))
                return new APIBadRequest();

            // Retrieve materials based on Project and Block
            var materials = _context.MaterialMaster
                        .Include(m => m.MRF)
                        .Include(m => m.StageAudits)
                        .ThenInclude(sa => sa.Stage)
                        .Include(m => m.Elements)
                        .ThenInclude(elm => elm.ForgeModel)
                        .Include(m => m.MaterialInfoAudits)
                        .Where(m => m.ProjectId == project_id && (block == "All" ? true : m.Block == block)).ToList();

            var user = _context.GetUserFromHttpContext(HttpContext);

            if ((user.RoleID == 7 || user.RoleID == 8) && user.Vendor != null)
            {
                materials = materials.Where(m => m.Vendor == user.Vendor).ToList();
            }

            var deliveredStage = _context.MaterialStageMaster.OrderBy(msm => msm.Order).ToArray()[2];

            return new APIResponse(0, materials.Select(m => new
            {
                m.ID,
                m.MarkingNo,
                m.Block,
                m.Level,
                m.Zone,
                m.MaterialType,
                MRFNo = m.MRF == null ? null : m.MRF.MRFNo,
                expectedDeliveryDate = m.MaterialInfoAudits.OrderByDescending(mi => mi.CreatedDate).FirstOrDefault() == null ? (m.MRF == null ? null : m.MRF?.ExpectedDeliveryDate) :
                m.MaterialInfoAudits.OrderByDescending(mi => mi.CreatedDate).FirstOrDefault().ExpectedDeliveryDate,
                stageOrder = m.StageAudits.OrderByDescending(s => s.CreatedDate).FirstOrDefault() == null ? 0 : m.StageAudits.OrderByDescending(s => s.CreatedDate).FirstOrDefault().Stage.Order,                       
                StageName = m.StageAudits.OrderByDescending(s => s.CreatedDate).FirstOrDefault() == null ? null : m.StageAudits.OrderByDescending(s => s.CreatedDate).FirstOrDefault().Stage.Name,
                StageColour = m.StageAudits.OrderByDescending(s => s.CreatedDate).FirstOrDefault() == null ? null : m.StageAudits.OrderByDescending(s => s.CreatedDate).FirstOrDefault().Stage.Colour,
                ModelUrn = m.Elements.FirstOrDefault()?.ForgeModel.ObjectId,
                ElementId = m.Elements.FirstOrDefault()?.DbId,
                DeliveryStageOrder = deliveredStage.Order
            }));
        }

        // GET: projects/{projectID}/Materials/5
        [Route("{id}")]
        public APIResponse GetMaterialDetail([FromRoute] int project_id, [FromRoute] int id)
        {
            if (!ModelState.IsValid)
                return new APIBadRequest();

            // Retrieve material detail based on ID
            var materials = CreateMaterialDetail(id);

            if (materials == null)
                return new DbRecordNotFound(Module, "ID", id.ToString());

            return new APIResponse(0, materials);
        }

        // PUT /projects/{project_id}/materials/edit/{id}
        [HttpPut("{id}")]
        public async Task<APIResponse> PutMaterialMaster([FromRoute] int id, [FromBody] MaterialDetail materialDetail)
        {
            if (!ModelState.IsValid)
                return new APIBadRequest();

            var materialMaster = await _context.MaterialMaster.SingleOrDefaultAsync(m => m.ID == id);

            if (id != materialMaster.ID)
                return new DbRecordNotFound(Module, "ID", id.ToString());

            MaterialInfoAudit materialInfoAudit = await _context.MaterialInfoAudit.SingleOrDefaultAsync(m => m.Material.ID == id);

            bool editedBefore = true;
            if (materialInfoAudit == null)
            {
                editedBefore = false;
                materialInfoAudit = new MaterialInfoAudit();
            }

            // check to see if other fields get edited
            materialInfoAudit.Remarks = materialDetail.Remarks;
            materialInfoAudit.ExpectedDeliveryDate = materialDetail.ExpectedDeliveryDate == null ? Convert.ToDateTime(materialDetail.ExpectedDeliveryDate) : DateTimeOffset.Parse(materialDetail.ExpectedDeliveryDate.ToString());
            materialInfoAudit.CreatedBy = _context.UserMaster.Where(u => u.UserName == "admin").FirstOrDefault();
            materialInfoAudit.CreatedDate = DateTime.Now;
            materialInfoAudit.Material = materialMaster;

            if (editedBefore)
                _context.Entry(materialInfoAudit).State = EntityState.Modified;
            else
                _context.Add(materialInfoAudit);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException exc)
            {
                if (!MaterialMasterExists(id))
                    return new DbRecordNotFound(Module, "ID", id.ToString());
                else
                    return new DbConcurrentUpdate(exc.Message);
            }

            return new APIResponse(0, null);
        }

        // POST: api/MaterialMasters
        [HttpPost]
        public async Task<IActionResult> PostMaterialMaster([FromRoute] int projectId, [FromBody] MaterialMaster materialMaster)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.MaterialMaster.Add(materialMaster);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMaterialMaster", new { id = materialMaster.ID }, materialMaster);
        }

        // DELETE: api/MaterialMasters/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMaterialMaster([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var materialMaster = await _context.MaterialMaster.SingleOrDefaultAsync(m => m.ID == id);
            if (materialMaster == null)
            {
                return NotFound();
            }

            _context.MaterialMaster.Remove(materialMaster);
            await _context.SaveChangesAsync();

            return Ok(materialMaster);
        }

        [HttpPost("delete-template-test")]
        public APIResponse DeleteTestFromTemplate([FromRoute] int project_id,
            ImportMaterialMasterTemplate importTemplate)
        {
            if (!ModelState.IsValid)
            {
                return new APIBadRequest();
            }

            var mmToBeDeleted = _context.MaterialMaster.Where(mm => 
            mm.ProjectId == project_id && 
            mm.VendorId == importTemplate.VendorId && 
            mm.MaterialType == importTemplate.MaterialType && 
            mm.Block == importTemplate.Block).ToList();

            if (mmToBeDeleted.Count() > 0)
            {
                _context.MaterialMaster.RemoveRange(mmToBeDeleted);
                _context.SaveChanges();
            }

            return new APIResponse(0, mmToBeDeleted);
        }

        [HttpPost("import-template")]
        public async Task<APIResponse> ImportFromTemplate([FromRoute] int project_id,
            ImportMaterialMasterTemplate importTemplate)
        {
            if (!ModelState.IsValid)
            {
                return new APIBadRequest();
            }

            using (var fileReader = new StreamReader(importTemplate.TemplateFile.OpenReadStream()))
            {
                var materialMasters = new List<MaterialMaster>();
                if(fileReader.Peek() > 0)
                    await fileReader.ReadLineAsync();

                var rowsNotUploaded = new List<string>();
                while (!fileReader.EndOfStream)
                {
                    var row = await fileReader.ReadLineAsync();
                    var content = row.Split(',');
                    if (content.Length >= 3)    //&& !content.Any(c => string.IsNullOrWhiteSpace(c))
                    {
                        foreach (var markingNo in content.Skip(2))
                        {
                            if (markingNo.Length > 0)
                            {
                                materialMasters.Add(new MaterialMaster
                                {
                                    ProjectId = project_id,
                                    VendorId = importTemplate.VendorId,
                                    Block = importTemplate.Block,
                                    MaterialType = importTemplate.MaterialType,
                                    Level = content[0],
                                    Zone = content[1],
                                    MarkingNo = markingNo
                                });
                            }
                        }
                    }
                    else
                    {
                        rowsNotUploaded.Add(row);
                    }
                }

                if (materialMasters.Count > 0)
                {
                    try
                    {
                        await _context.MaterialMaster.AddRangeAsync(materialMasters);
                        _context.MaterialStageMaster.UpdateRange(AddNewMaterialToAllStages(importTemplate.MaterialType));
                        await _context.SaveChangesAsync();
                        return new APIResponse(0, new
                        {
                            CountUploaded = materialMasters.Count,
                            rowsNotUploaded
                        });
                    }
                    catch(DbUpdateException dbExc)
                    {
                        return new APIResponse(ErrorMessages.DbDuplicateRecord, null, 
                            dbExc.InnerException == null ? dbExc.Message : dbExc.InnerException.Message);
                    }
                    catch (Exception exc)
                    {
                        return new APIResponse(ErrorMessages.UnkownError, null, exc.Message);
                    }
                }
                else
                {
                    return new APIResponse(ErrorMessages.FileInvalid, null, "No materials to import");
                }
            }
        }
    }
}