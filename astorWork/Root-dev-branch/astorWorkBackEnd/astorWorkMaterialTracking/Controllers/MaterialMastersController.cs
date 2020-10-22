using astorWorkDAO;
using astorWorkJobTracking.Models;
using astorWorkMaterialTracking.Models;
using astorWorkShared.GlobalExceptions;
using astorWorkShared.GlobalResponse;
using astorWorkShared.Services;
using astorWorkShared.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace astorWorkMaterialTracking.Controllers
{
    [Produces("application/json")]
    [Route("projects/{project_id}/materials")]
    public class MaterialMastersController : Controller
    {
        private readonly string module = "Material";

        private astorWorkDbContext _context;
        private IAstorWorkBlobStorage _blobStorage;

        public MaterialMastersController(astorWorkDbContext context, IAstorWorkBlobStorage blobStorage)
        {
            _context = context;
            _blobStorage = blobStorage;
        }

        // GET: projects/{project_id}/materials?Block={0}
        [HttpGet()]
        public async Task<List<MaterialDetail>> ListMaterials([FromRoute] int project_id, [FromQuery] string block)
        {
            if (string.IsNullOrEmpty(block))
                block = "All";

            if (!ModelState.IsValid || string.IsNullOrEmpty(block))
                throw new GenericException(ErrorMessages.BadRequest, ErrorMessages.BadRequestMsg());

            // Retrieve materials based on Project and Block
            List<MaterialMaster> materials = await GetMaterials(project_id, block);
            if (materials == null || materials.Count == 0)
                throw new GenericException(ErrorMessages.DbRecordNotFound, $"Materials not available");
            UserMaster user = await _context.GetUserFromHttpContext(HttpContext);

            if ((user.RoleID == 7 || user.RoleID == 8) && user.Organisation != null)
                materials = GetVendorMaterials(materials, user.Organisation);

            return await CreateMaterialList(materials);
        }

        // GET: projects/{projectID}/Materials/5
        [HttpGet("{id}")]
        public MaterialDetail GetMaterial([FromRoute] int project_id, [FromRoute] int id)
        {
            if (!ModelState.IsValid)
                throw new GenericException(ErrorMessages.BadRequest, ErrorMessages.BadRequestMsg());

            // Retrieve material detail based on ID
            MaterialDetail material = CreateMaterialDetail(id);

            if (material == null)
                throw new GenericException(ErrorMessages.DbRecordNotFound, ErrorMessages.DBRecordNotFoundMsg(module, "ID", id.ToString()));

            return material;
        }

        // PUT /projects/{project_id}/materials/{id}
        [HttpPut("{id}")]
        public async Task PutMaterialMaster([FromRoute] int id, [FromBody] MaterialDetail materialDetail)
        {
            if (!ModelState.IsValid)
                throw new GenericException(ErrorMessages.BadRequest, ErrorMessages.BadRequestMsg());

            MaterialMaster materialMaster = await _context.MaterialMaster.SingleOrDefaultAsync(m => m.ID == id);

            if (id != materialMaster.ID)
                throw new GenericException(ErrorMessages.DbRecordNotFound, ErrorMessages.DBRecordNotFoundMsg(module, "ID", id.ToString()));

            MaterialInfoAudit materialInfoAudit = await _context.MaterialInfoAudit.SingleOrDefaultAsync(m => m.Material.ID == id);

            bool editedBefore = true;
            if (materialInfoAudit == null)
            {
                editedBefore = false;
                materialInfoAudit = new MaterialInfoAudit();
            }

            materialInfoAudit = await UpdateMaterialInfoAudit(materialInfoAudit, materialDetail, materialMaster);

            if (editedBefore)
                _context.Entry(materialInfoAudit).State = EntityState.Modified;
            else
                _context.Add(materialInfoAudit);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MaterialMasterExists(id))
                    throw new GenericException(ErrorMessages.DbRecordNotFound, ErrorMessages.DBRecordNotFoundMsg(module, "ID", id.ToString()));
                else
                    throw new GenericException(ErrorMessages.DbConcurrentUpdate, "Database concurrent update error!");
            }
        }

        // POST: api/MaterialMasters
        [HttpPost]
        public async Task<MaterialMaster> CreateMaterial([FromRoute] int project_id, [FromBody] MaterialMaster materialMaster)
        {
            if (!ModelState.IsValid)
                throw new GenericException(ErrorMessages.BadRequest, ErrorMessages.BadRequestMsg());

            _context.MaterialMaster.Add(materialMaster);
            await _context.SaveChangesAsync();

            return materialMaster;
        }

        // DELETE: api/MaterialMasters/5
        [HttpDelete("{id}")]
        public async Task<MaterialMaster> DeleteMaterialMaster([FromRoute] int id)
        {
            if (!ModelState.IsValid)
                throw new GenericException(ErrorMessages.BadRequest, ErrorMessages.BadRequestMsg());

            MaterialMaster material = await _context.MaterialMaster.SingleOrDefaultAsync(m => m.ID == id);
            if (material == null)
                throw new GenericException(ErrorMessages.DbRecordNotFound, ErrorMessages.DBRecordNotFoundMsg(module, "ID", id.ToString()));

            _context.MaterialMaster.Remove(material);
            await _context.SaveChangesAsync();

            return material;
        }

        [HttpPost("delete-template-test")]
        public async Task<List<MaterialMaster>> DeleteTestFromTemplate([FromRoute] int project_id,
            ImportMaterialMasterTemplate importTemplate)
        {
            if (!ModelState.IsValid)
                throw new GenericException(ErrorMessages.BadRequest, ErrorMessages.BadRequestMsg());

            List<MaterialMaster> mmToBeDeleted = await _context.MaterialMaster.Where(mm => mm.ProjectID == project_id &&
                                                                                     mm.OrganisationID == importTemplate.OrganisationID &&
                                                                                     mm.MaterialType.Name == importTemplate.MaterialType &&
                                                                                     mm.Block == importTemplate.Block).ToListAsync();

            if (mmToBeDeleted.Count() > 0)
            {
                _context.MaterialMaster.RemoveRange(mmToBeDeleted);
                _context.SaveChanges();
            }

            return mmToBeDeleted;
        }

        [HttpPost("import-template")]
        public async Task<ImportMaterialMasterStats> ImportFromTemplate([FromRoute] int project_id,
            ImportMaterialMasterTemplate importTemplate)
        {
            string[] columnsInRow = null;
            if (!ModelState.IsValid)
                throw new GenericException(ErrorMessages.BadRequest, ErrorMessages.BadRequestMsg());

            using (StreamReader fileReader = new StreamReader(importTemplate.TemplateFile.OpenReadStream()))
            {
                List<MaterialMaster> materials = new List<MaterialMaster>();
                if (fileReader.Peek() > 0)
                    await fileReader.ReadLineAsync();

                List<string> rowsNotUploaded = new List<string>();
                List<MaterialDrawingAudit> materialDrawingAudits = new List<MaterialDrawingAudit>();
                while (!fileReader.EndOfStream)
                {
                    string row = await fileReader.ReadLineAsync();
                    columnsInRow = row.Split(',');
                    if (columnsInRow.Length >= 3)    //&& !content.Any(c => string.IsNullOrWhiteSpace(c))
                        materials = await AddMaterials(materials, columnsInRow, importTemplate, project_id, materialDrawingAudits);
                    else
                        rowsNotUploaded.Add(row);
                }

                if (materials.Count > 0)
                {
                    try
                    {
                        await _context.MaterialMaster.AddRangeAsync(materials);
                        await _context.MaterialDrawingAudit.AddRangeAsync(materialDrawingAudits);

                        if (await _context.MaterialStageMaster.AnyAsync(msm => msm.MaterialTypes.Contains(importTemplate.MaterialType)))
                            _context.MaterialStageMaster.UpdateRange(AddNewMaterialToAllStages(importTemplate.MaterialType));

                        await _context.SaveChangesAsync();
                        List<MaterialDrawingAssociation> listDrawingAssociations = new List<MaterialDrawingAssociation>();

                        foreach (var drwaings in materials.Zip(materialDrawingAudits, Tuple.Create))
                        {
                            MaterialDrawingAssociation drawingAssociation = new MaterialDrawingAssociation();
                            drawingAssociation.DrawingID = drwaings.Item2.ID;
                            drawingAssociation.MaterialID = drwaings.Item1.ID;
                            listDrawingAssociations.Add(drawingAssociation);
                        }
                        await _context.MaterialDrawingAssociation.AddRangeAsync(listDrawingAssociations);
                        await _context.SaveChangesAsync();
                        return new ImportMaterialMasterStats
                        {
                            CountUploaded = materials.Count,
                            RowsNotUploaded = rowsNotUploaded.Count
                        };
                    }
                    catch (DbUpdateException ex)
                    {
                        string duplicateRecord = ex.InnerException.Message.Split('(')[1];
                        duplicateRecord = duplicateRecord.Split(')')[0];
                        throw new GenericException(ErrorMessages.DbDuplicateRecord, ErrorMessages.DbDuplicateRecordMsg("Material", "Marking No., Block, Level, Zone", duplicateRecord));
                    }
                    catch (Exception ex)
                    {
                        throw new GenericException(ErrorMessages.UnkownError, ex.Message);
                    }
                }
                else
                {
                    throw new GenericException(ErrorMessages.FileInvalid, "No materials to import");
                }
            }
        }

        // GET: projects/{project_id}/materials/ordered-produced?vendor_id={0}
        [HttpGet("ordered-produced")]
        public async Task<List<TrackerAssociation>> ListOrderedAndProduced([FromRoute] int project_id, [FromQuery] int vendor_id)
        {
            if (!ModelState.IsValid)
                throw new GenericException(ErrorMessages.BadRequest, ErrorMessages.BadRequestMsg());

            List<MaterialMaster> materials = await GetOrderedOrProducedMaterials(project_id, vendor_id);

            if (materials.Count() == 0)
                throw new GenericException(ErrorMessages.DbRecordNotFound, "No ordered or produced materials found");

            return GetInventoryList(materials,"");
        }

        // GET: projects/{project_id}/materials/rto
        [HttpGet("rto")]
        public async Task<List<TrackerAssociation>> ListRTO([FromRoute] int project_id)
        {
            try
            {
                if (!ModelState.IsValid)
                    throw new GenericException(ErrorMessages.BadRequest, ErrorMessages.BadRequestMsg());

                List<MaterialMaster> materials = await GetRTOMaterials(project_id);

                if (materials.Count() == 0)
                    throw new GenericException(ErrorMessages.DbRecordNotFound, "No materials found");

                return GetInventoryList(materials,"");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet("produced")]
        public async Task<List<TrackerAssociation>> GetMainConProducedMaterials([FromRoute]int project_id,[FromQuery] string tenant_name)
        {
            if (!ModelState.IsValid)
                throw new GenericException(ErrorMessages.BadRequest, ErrorMessages.BadRequestMsg());
            List<MaterialMaster> producedMaterials;
            try
            {
                UserMaster user = await _context.GetUserFromHttpContext(HttpContext);
                producedMaterials = await _context.MaterialMaster
                              .Include(mm => mm.DrawingAssociations)
                              .ThenInclude(ds => ds.Drawing)
                              .Include(mm => mm.Project)
                              .Include(mm => mm.Trackers)
                              .Include(mm => mm.MRF)
                              .Include(mm => mm.MaterialType)
                              .Include(mm => mm.Organisation)
                              .Include(mm => mm.QCCases)
                              .ThenInclude(qc => qc.Defects)
                              .Include(mm => mm.StageAudits)
                              .ThenInclude(sa => sa.Location)
                              .Include(mm => mm.StageAudits)
                              .ThenInclude(mm => mm.Stage)
                              .ThenInclude(st => st.checklists)
                              .Include(mm => mm.Elements)
                              .ThenInclude(elm => elm.ForgeModel)
                              .Where(mm => mm.ProjectID == user.ProjectID &&
                                           mm.MRF != null && mm.StageAudits != null
                                           && mm.StageAudits.Count() > 0).ToListAsync();

                       producedMaterials = producedMaterials.Where(mm => mm.StageAudits.OrderByDescending(s => s.CreatedDate).FirstOrDefault().Stage.checklists != null && mm.StageAudits.OrderByDescending(s => s.CreatedDate).FirstOrDefault().Stage.checklists.Count() > 0).ToList();
                //&& mm.StageAudits.Select(pp => pp.Stage.MilestoneId == 1).Count() > 0
                if (producedMaterials.Count == 0)
                {
                    throw new GenericException(ErrorMessages.DbRecordNotFound, "No  produced materials found");
                }

                if (producedMaterials.Count() == 0)
                    throw new GenericException(ErrorMessages.DbRecordNotFound, "No ordered or produced materials found");

                return GetInventoryList(producedMaterials, tenant_name);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet("produced-by-vendor")]
        public async Task<List<TrackerAssociation>> GetProducedMaterials([FromRoute]int project_id)
        {
            if (!ModelState.IsValid)
                throw new GenericException(ErrorMessages.BadRequest, ErrorMessages.BadRequestMsg());
            List<MaterialMaster> producedMaterials;
            try
            {

                List<MaterialStageAudit> StageAudits;
                UserMaster user = await _context.GetUserFromHttpContext(HttpContext);

                StageAudits = await _context.MaterialStageAudit
                                    .Where(ma => ma.CreatedByID == user.ID && ma.QCStatus == Enums.JobStatus.QC_failed_by_Maincon)
                                    .ToListAsync();

                producedMaterials = await _context.MaterialMaster
                              .Include(mm => mm.DrawingAssociations)
                              .ThenInclude(ds => ds.Drawing)
                              .Include(mm => mm.Project)
                              .Include(mm => mm.Trackers)
                              .Include(mm => mm.MRF)
                              .Include(mm => mm.MaterialType)
                              .Include(mm => mm.Organisation)
                              .Include(mm => mm.QCCases)
                              .ThenInclude(qc => qc.Defects)
                              .Include(mm => mm.StageAudits)
                              .ThenInclude(sa => sa.Location)
                              .Include(mm => mm.StageAudits)
                              .ThenInclude(mm => mm.Stage)
                               .ThenInclude(st => st.checklists)
                              .Include(mm => mm.Elements)
                              .ThenInclude(elm => elm.ForgeModel)
                              .Where(mm => mm.ProjectID == user.ProjectID &&
                                           mm.MRF != null &&
                                           mm.StageAudits.Count == 1
                                         )
                              .ToListAsync();
                if (producedMaterials.Count == 0)
                    throw new GenericException(ErrorMessages.DbRecordNotFound, "No  produced materials found");

                if (producedMaterials.Count() == 0)
                    throw new GenericException(ErrorMessages.DbRecordNotFound, "No ordered or produced materials found");

                return GetInventoryList(producedMaterials,"");
            }
            catch (Exception)
            {
                throw;
            }
        }
        protected TrackingHistory CreateStageTrackingHistory(MaterialStageAudit materialStageAudit)
        {
            return new TrackingHistory()
            {
                ID = materialStageAudit.ID,
                StageName = materialStageAudit.Stage.Name,
                StageStatus = 1,
                Location = materialStageAudit.Location.Name,
                CreatedBy = materialStageAudit.CreatedBy.PersonName,
                CreatedDate = materialStageAudit.CreatedDate,
                Remarks = materialStageAudit.Remarks,
                IsQCStage = false,
            };
        }

        protected IEnumerable<TrackingHistory> CreateStageTrackingHistories(MaterialMaster materialMaster)
        {
            return (from materialStageAudit in materialMaster.StageAudits
                    select CreateStageTrackingHistory(materialStageAudit))
                                                      .OrderBy(sa => sa.CreatedDate);
        }

        protected TrackingHistory CreateCaseTrackingHistory(MaterialMaster materialMaster, IEnumerable<MaterialQCCase> materialQCCases,List<ChecklistAudit> checklists,TrackingHistory materialstageaudit)
        {
            TrackingHistory trackingHistory = new TrackingHistory();
            trackingHistory.ID = materialMaster.ID;
            trackingHistory.StageName = string.Empty;
            trackingHistory.IsQCStage = true;
            trackingHistory.Location = string.Empty;

            if (materialQCCases != null && materialQCCases.Count() > 0)
            {
                trackingHistory.StageStatus = materialQCCases.Where(d => d.Defects.Any(q => q.Status == Enums.QCStatus.QC_failed_by_Maincon || q.Status == Enums.QCStatus.QC_rectified_by_Subcon)).Count() > 0 ? 2 : 1;
                trackingHistory.CreatedDate = materialQCCases.LastOrDefault()?.CreatedDate;
                trackingHistory.CreatedBy = materialQCCases.LastOrDefault()?.CreatedBy.PersonName;
                trackingHistory.Remarks = materialQCCases.LastOrDefault()?.Defects.LastOrDefault()?.Remarks;
                trackingHistory.OpenQCCaseIds = string.Join(",", materialQCCases.Select(q => q.ID).ToList());
                trackingHistory.CountQCCase = materialQCCases.Count();
                trackingHistory.CountQCDefects = materialQCCases.Select(q => q.Defects).Count();
                trackingHistory.CountClosedDefect = materialQCCases.Select(q => q.Defects.Any(d => d.Status == Enums.QCStatus.QC_passed_by_Maincon)).Count();
                trackingHistory.CountRectifiedDefect = materialQCCases.Select(q => q.Defects.Any(d => d.Status == Enums.QCStatus.QC_rectified_by_Subcon)).Count();
                trackingHistory.CountOpenDefect = materialQCCases.Select(q => q.Defects.Any(d => d.Status == Enums.QCStatus.QC_failed_by_Maincon)).Count();
            }
            if(checklists != null && checklists.Count > 0)
            {
                var chk = checklists.GroupBy(ch => new { ch.ChecklistID, ch.MaterialStageAuditID, ch.JobScheduleID }).Select(c => new JobQCDetails
                {
                    ChecklistID = c.Key.ChecklistID,
                    ID  = Convert.ToInt32(c.Key.MaterialStageAuditID),
                    TradeID = c.Key.JobScheduleID != null ? Convert.ToInt32(c.Key.JobScheduleID): 0,

                }).ToList();
                trackingHistory.StageName = materialstageaudit.StageName;
                trackingHistory.StageStatus = trackingHistory.StageStatus == 2 ? 2 : materialMaster.StageAudits.Where(m => m.ID == materialstageaudit.ID).FirstOrDefault().QCStatus == Enums.JobStatus.All_QC_passed ? 1 
                    : materialMaster.StageAudits.Where(m => m.ID == materialstageaudit.ID).FirstOrDefault().QCStatus == Enums.JobStatus.QC_failed_by_Maincon 
                    || materialMaster.StageAudits.Where(m => m.ID == materialstageaudit.ID).FirstOrDefault().QCStatus == Enums.JobStatus.QC_rejected_by_RTO ? 2: 3;
                trackingHistory.StructQCLastUpdatedBy = materialMaster.StageAudits.Where(m => m.ID == materialstageaudit.ID).FirstOrDefault().CreatedBy.PersonName;
                trackingHistory.StructQCLastUpdatedDate = materialMaster.StageAudits.Where(m => m.ID == materialstageaudit.ID).FirstOrDefault().CreatedDate;
                trackingHistory.TotalStructChecklistCount = chk.Where(c => c.ID == materialstageaudit.ID && c.TradeID == 0).Count();
                trackingHistory.TotalStructPassCount = materialMaster.StageAudits.Where(m => m.ID == materialstageaudit.ID).FirstOrDefault().QCStatus == Enums.JobStatus.All_QC_passed ? trackingHistory.TotalStructChecklistCount : trackingHistory.TotalStructChecklistCount - 1;
                trackingHistory.TotalArchiChecklistCount = chk.Where(c => c.ID == materialstageaudit.ID && c.TradeID > 0).Count();
                trackingHistory.TotalArchiPassCount = chk.Where(c => c.ID == materialstageaudit.ID && c.TradeID > 0).Count(); // have to wirte code for it to find arch qc pass count
                trackingHistory.CreatedDate = trackingHistory.CreatedDate == null ? trackingHistory.StructQCLastUpdatedDate == null ? trackingHistory.ArchiQCLastUpdatedDate : trackingHistory.StructQCLastUpdatedDate : trackingHistory.CreatedDate;
            }
            return trackingHistory;
        }

        protected List<TrackingHistory> AddCaseTrackingHistories(MaterialMaster materialMaster, List<TrackingHistory> trackingHistories, List<ChecklistAudit> checklistAudits)
        {
            IEnumerable<MaterialQCCase> qcCases = materialMaster.QCCases.OrderBy(qc => qc.CreatedDate);

            int trackingHistoriesStagesCount = trackingHistories.Where(t => !t.IsQCStage).Count();
            for (int i = 0; i < trackingHistoriesStagesCount; i++)
            {
                List<MaterialQCCase> materialQCCases = new List<MaterialQCCase>();
                if (i < trackingHistoriesStagesCount - 1)
                    materialQCCases = qcCases.Where(qc => qc.CreatedDate >= trackingHistories.ElementAt(i).CreatedDate
                                                       && qc.CreatedDate < trackingHistories.ElementAt(i + 1).CreatedDate).OrderBy(qc => qc.CreatedDate).ToList();
                else
                    materialQCCases = qcCases.Where(qc => qc.CreatedDate >= trackingHistories.ElementAt(i).CreatedDate).OrderBy(qc => qc.CreatedDate).ToList();

                var stageChecklistAudit = checklistAudits.Where(chk => chk.MaterialStageAuditID == trackingHistories.ElementAt(i).ID).ToList();
                if ((materialQCCases != null && materialQCCases.Count > 0) || (stageChecklistAudit != null && stageChecklistAudit.Count > 0))
                    trackingHistories.Add(CreateCaseTrackingHistory(materialMaster, materialQCCases, stageChecklistAudit, trackingHistories.ElementAt(i)));

            }

            return trackingHistories.OrderBy(t => t.CreatedDate).ToList();
        }

        protected List<TrackingHistory> AddSubsequentStages(MaterialMaster materialMaster, List<TrackingHistory> trackingHistories)
        {
            int currentOrder = trackingHistories.Count > 0 ? materialMaster.StageAudits.Max(s => s.Stage.Order) : 0;

            IEnumerable<MaterialStageMaster> materialStageMasters = _context.MaterialStageMaster.Where(msm => msm.Order > currentOrder && msm.MaterialTypes.Contains(materialMaster.MaterialType.Name)).OrderBy(msm => msm.Order);

            foreach (MaterialStageMaster materialStageMaster in materialStageMasters)
            {
                TrackingHistory materialTrackingHistory = new TrackingHistory
                {
                    ID = materialStageMaster.Order,
                    StageName = materialStageMaster.Name,
                    StageStatus = 0
                };
                trackingHistories.Add(materialTrackingHistory);
            }
            return trackingHistories;
        }

        protected List<TrackingHistory> CreateMaterialTrackingHistory(MaterialMaster materialMaster, List<ChecklistAudit> checklistAudits)
        {
            List<TrackingHistory> trackingHistories = CreateStageTrackingHistories(materialMaster).ToList();

            //MaterialQCCase operQCCase = materialMaster.QCCases.Where(q => q.Defects.Any(d => d.IsOpen)).FirstOrDefault();
            trackingHistories = AddCaseTrackingHistories(materialMaster, trackingHistories, checklistAudits);

            return AddSubsequentStages(materialMaster, trackingHistories);
        }

        protected MaterialDetail CreateMaterialDetail(int materialID)
        {
            MaterialMaster materialMaster = _context.MaterialMaster
                .Include(m => m.Organisation)
                .Include(m => m.MaterialType)
                .Include(m => m.Trackers)
                .Include(m => m.StageAudits)
                .ThenInclude(sa => sa.Location)
                .Include(m => m.QCCases)
                .ThenInclude(d => d.Defects)
                .Include(m => m.QCCases)
                .ThenInclude(u => u.CreatedBy)
                .Include(m => m.StageAudits)
                .ThenInclude(sa => sa.Stage)
                .ThenInclude(chk => chk.checklists)
                .Include(m => m.StageAudits)
                .ThenInclude(sa => sa.CreatedBy)
                .Include(m => m.MRF)
                .Include(m => m.DrawingAssociations)
                .ThenInclude(da => da.Drawing)
                .Include(m => m.MaterialType)
                .FirstOrDefault(m => m.ID == materialID);

            List<ChecklistAudit> checklistAudits = _context.ChecklistAudit
                                                    .Include(chk => chk.MaterialStageAudit)
                                                    .Where(chk => materialMaster.StageAudits!=null && materialMaster.StageAudits.Count>0 &&chk.MaterialStageAudit!=null && materialMaster.StageAudits.Contains(chk.MaterialStageAudit)).ToList();

            MaterialDetail materialDetail = CreateMaterialInfo(materialID, materialMaster);
            materialDetail = CreateMaterialRemarksAndExpectedDeliveryDate(materialDetail, materialMaster);
            materialDetail.TrackingHistory = CreateMaterialTrackingHistory(materialMaster, checklistAudits);

            return materialDetail;
        }

        protected MaterialDetail CreateMaterialInfo(int materialID, MaterialMaster materialMaster)
        {
            MaterialDetail materialDetail = new MaterialDetail();

            materialDetail.ID = materialMaster.ID;
            materialDetail.MarkingNo = materialMaster.MarkingNo;
            materialDetail.Block = materialMaster.Block;
            materialDetail.Level = materialMaster.Level;
            materialDetail.Zone = materialMaster.Zone;
            materialDetail.MaterialType = materialMaster.MaterialType.Name;
            materialDetail.OrganisationName = materialMaster.Organisation?.Name;
            materialDetail.TrackerType = materialMaster.Trackers?.FirstOrDefault()?.Type;
            materialDetail.TrackerTag = materialMaster.Trackers?.FirstOrDefault()?.Tag;
            materialDetail.TrackerLabel = materialMaster.Trackers?.FirstOrDefault()?.Label;
            materialDetail.AssemblyLocation = materialMaster.AssemblyLocation;

            if (materialMaster.MRF != null)
            {
                materialDetail.CastingDate = materialMaster.MRF.PlannedCastingDate;
                materialDetail.OrderDate = materialMaster.MRF.OrderDate;
            }

            var drawingAssociation = materialMaster.DrawingAssociations
                                        .OrderByDescending(a => a.Drawing.DrawingIssueDate)
                                        .FirstOrDefault();
            if (drawingAssociation != null)
            {
                materialDetail.Drawing = drawingAssociation.Drawing;
            }
            materialDetail.Area = materialMaster.Area;
            materialDetail.Length = materialMaster.Length;
            materialDetail.Dimensions = materialMaster.Dimensions;

            return materialDetail;
        }

        protected MaterialDetail CreateMaterialRemarksAndExpectedDeliveryDate(MaterialDetail materialDetail, MaterialMaster materialMaster)
        {
            if (materialMaster.MRF != null)
            {
                materialDetail.ExpectedDeliveryDate = materialMaster.MRF.ExpectedDeliveryDate;
            }

            MaterialInfoAudit materialInfoAudit = _context.MaterialInfoAudit.FirstOrDefault(m => m.Material.ID == materialMaster.ID);

            if (materialInfoAudit != null)
            {
                materialDetail.Remarks = materialInfoAudit.Remarks;
                materialDetail.ExpectedDeliveryDate = materialInfoAudit.ExpectedDeliveryDate;
            }

            return materialDetail;
        }

        protected IEnumerable<MaterialStageMaster> AddNewMaterialToAllStages(string materialType)
        {
            IEnumerable<MaterialStageMaster> stages = GetAllStages();

            foreach (MaterialStageMaster stage in stages)
            {
                stage.MaterialTypes += "," + materialType;
            }

            return stages;
        }

        protected bool MaterialTypeExists(string materialType)
        {
            IEnumerable<MaterialMaster> materialMasters = _context.MaterialMaster.Where(m => m.MaterialType.Name.Contains(materialType));

            if (materialMasters == null)
                return false;

            return (materialMasters.Count() > 0);
        }

        protected IEnumerable<MaterialStageMaster> GetAllStages()
        {
            return _context.MaterialStageMaster;
        }

        protected async Task<List<MaterialMaster>> GetMaterials(int projectID, string block)
        {
            return await _context.MaterialMaster
                                 .Include(mm => mm.DrawingAssociations)
                                 .ThenInclude(ds => ds.Drawing)
                                 .Include(m => m.MRF)
                                 .Include(m => m.Trackers)
                                 .Include(m => m.StageAudits)
                                 .ThenInclude(sa => sa.Stage)
                                 .ThenInclude(chk => chk.checklists)
                                 .Include(m => m.Elements)
                                 .ThenInclude(elm => elm.ForgeModel)
                                 .Include(m => m.MaterialInfoAudits)
                                 .Include(m => m.QCCases)
                                 .ThenInclude(d => d.Defects)
                                 .Include(m => m.MaterialType)
                                 .Where(m => m.ProjectID == projectID && (block == "All" ? true : m.Block == block))
                                 .ToListAsync();
        }

        protected async Task<List<MaterialDetail>> CreateMaterialList(List<MaterialMaster> materials)
        {
            MaterialStageMaster deliveredStage = await GetDeliveredStage();
            List<MaterialDetail> lstmaterials = new List<MaterialDetail>();

            lstmaterials = materials.Select(m => new MaterialDetail
            {
                ID = m.ID,
                MarkingNo = m.MarkingNo,
                Block = m.Block,
                Level = m.Level,
                Zone = m.Zone,
                TrackerTag = m.Trackers?.Where(t => t.Type == Enums.TagType.QR_Code.ToString().Replace('_', ' ')).FirstOrDefault()?.Tag,
                TrackerLabel = m.Trackers?.Where(t => t.Type == Enums.TagType.QR_Code.ToString().Replace('_', ' ')).FirstOrDefault()?.Label,
                TrackerType = m.Trackers?.Where(t => t.Type == Enums.TagType.QR_Code.ToString().Replace('_', ' ')).FirstOrDefault()?.Type,
                MaterialType = m.MaterialType.Name,
                OpenQCCaseID = m.QCCases == null || m.QCCases.Count == 0 || m.QCCases.Where(d => d.Defects.Any(q => q.Status != Enums.QCStatus.QC_passed_by_Maincon)).Count() == 0 ? 0 : m.QCCases.Where(d => d.Defects.Any(q => q.Status != Enums.QCStatus.QC_passed_by_Maincon)).FirstOrDefault().ID,
                MRFNo = m.MRF == null ? null : m.MRF.MRFNo,
                ExpectedDeliveryDate = m.MaterialInfoAudits.OrderByDescending(mi => mi.CreatedDate).FirstOrDefault() == null ? (m.MRF == null ? null : m.MRF?.ExpectedDeliveryDate) :
                                       m.MaterialInfoAudits.OrderByDescending(mi => mi.CreatedDate).FirstOrDefault().ExpectedDeliveryDate,
                StageOrder = m.StageAudits.OrderByDescending(s => s.CreatedDate).FirstOrDefault() == null ? 0 : m.StageAudits.OrderByDescending(s => s.CreatedDate).FirstOrDefault().Stage.Order,
                StageName = m.StageAudits.OrderByDescending(s => s.CreatedDate).FirstOrDefault() == null ? null : m.StageAudits.OrderByDescending(s => s.CreatedDate).FirstOrDefault().Stage.Name,
                StageColour = m.StageAudits.OrderByDescending(s => s.CreatedDate).FirstOrDefault() == null ? null : m.StageAudits.OrderByDescending(s => s.CreatedDate).FirstOrDefault().Stage.Colour,
                ForgeModelURN = m.Elements.FirstOrDefault()?.ForgeModel.ObjectID,
                ForgeElementId = m.Elements.FirstOrDefault()?.DbID,
                DeliveryStageOrder = deliveredStage.Order,
                Length = m.Length,
                Area = m.Area,
                DrawingNo = m.DrawingAssociations?.FirstOrDefault()?.Drawing?.DrawingNo,
                QcStatusCode = (m.QCCases != null && m.QCCases.Count > 0
                            && m.QCCases.Where(d => d.Defects.Any(q => q.Status != Enums.QCStatus.QC_passed_by_Maincon)).Count() > 0) ? 1
                            : m.StageAudits == null || m.StageAudits.Count == 0
                            || m.StageAudits.OrderByDescending(s => s.CreatedDate).FirstOrDefault().Stage.checklists == null
                            || m.StageAudits.OrderByDescending(s => s.CreatedDate).FirstOrDefault().Stage.checklists.Count == 0 ? -1
                            : m.StageAudits.OrderByDescending(s => s.CreatedDate).FirstOrDefault().QCStatus == Enums.JobStatus.Job_not_assigned ? 0
                            : m.StageAudits.OrderByDescending(s => s.CreatedDate).FirstOrDefault().QCStatus == Enums.JobStatus.All_QC_passed ? 2 
                            : m.StageAudits.OrderByDescending(s => s.CreatedDate).FirstOrDefault().QCStatus == Enums.JobStatus.QC_failed_by_Maincon
                            || m.StageAudits.OrderByDescending(s => s.CreatedDate).FirstOrDefault().QCStatus == Enums.JobStatus.QC_rejected_by_RTO ? 1 : 3,
                QcStatus = (m.QCCases != null && m.QCCases.Count > 0
                            && m.QCCases.Where(d => d.Defects.Any(q => q.Status != Enums.QCStatus.QC_passed_by_Maincon)).Count() > 0) ? "Failed"
                            :m.StageAudits == null || m.StageAudits.Count == 0 
                            || m.StageAudits.OrderByDescending(s => s.CreatedDate).FirstOrDefault().Stage.checklists == null
                            || m.StageAudits.OrderByDescending(s => s.CreatedDate).FirstOrDefault().Stage.checklists.Count == 0 ? "NA"
                            : m.StageAudits.OrderByDescending(s => s.CreatedDate).FirstOrDefault().QCStatus == Enums.JobStatus.Job_not_assigned ? "Qc Pending"
                            : m.StageAudits.OrderByDescending(s => s.CreatedDate).FirstOrDefault().QCStatus.ToString().Replace('_',' ')
            }).ToList();
            return lstmaterials;
        }

        protected List<MaterialMaster> GetVendorMaterials(List<MaterialMaster> materials, OrganisationMaster organisation)
        {
            return materials.Where(m => m.Organisation == organisation).ToList();
        }

        protected async Task<MaterialInfoAudit> UpdateMaterialInfoAudit(MaterialInfoAudit materialInfoAudit, MaterialDetail materialDetail, MaterialMaster materialMaster)
        {
            // check to see if other fields get edited
            materialInfoAudit.Remarks = materialDetail.Remarks;
            materialInfoAudit.ExpectedDeliveryDate = materialDetail.ExpectedDeliveryDate == null ? Convert.ToDateTime(materialDetail.ExpectedDeliveryDate) : DateTimeOffset.Parse(materialDetail.ExpectedDeliveryDate.ToString());
            materialInfoAudit.CreatedBy = await _context.UserMaster.Where(u => u.UserName == "admin").FirstOrDefaultAsync();
            materialInfoAudit.CreatedDate = DateTime.Now;
            materialInfoAudit.Material = materialMaster;

            return materialInfoAudit;
        }

        protected async Task<List<MaterialMaster>> AddMaterials(List<MaterialMaster> materials, string[] columnsInRow, ImportMaterialMasterTemplate importTemplate, int projectID, List<MaterialDrawingAudit> materialDrawingAudits)
        {
            string materialTypeName = importTemplate.MaterialType;
            ProjectMaster project = await _context.ProjectMaster.Where(p => p.ID == projectID).FirstOrDefaultAsync();
            MaterialTypeMaster materialType = await _context.MaterialTypeMaster.Where(m => m.Name == materialTypeName).FirstOrDefaultAsync();

            if (materialType == null)
            {
                materialType = new MaterialTypeMaster
                {
                    Name = importTemplate.MaterialType,
                };

                await _context.MaterialTypeMaster.AddAsync(materialType);
                _context.SaveChanges();

                materialType = await _context.MaterialTypeMaster.Where(m => m.Name == materialTypeName).FirstOrDefaultAsync();
            }

            foreach (string markingNo in columnsInRow.Skip(7))
            {
                if (markingNo.Length > 0)
                {
                    materials.Add(new MaterialMaster
                    {
                        ProjectID = projectID,
                        Project = project,
                        OrganisationID = importTemplate.OrganisationID,
                        Block = importTemplate.Block,
                        MaterialType = materialType,
                        Level = columnsInRow[0],
                        Zone = columnsInRow[1],
                        AssemblyLocation = columnsInRow[2],
                        Area = float.Parse(columnsInRow[3] == "" ? "0" : columnsInRow[3].ToString()),
                        Dimensions = columnsInRow[4],
                        Length = float.Parse(columnsInRow[5] == "" ? "0" : columnsInRow[5].ToString()),
                        MarkingNo = markingNo,
                    });
                    MaterialDrawingAudit materialDrawing = new MaterialDrawingAudit();
                    if (columnsInRow[6] != null)
                        materialDrawing.DrawingNo = columnsInRow[6];
                    materialDrawing.RevisionNo = 1;
                    materialDrawing.DrawingIssueDate = DateTimeOffset.UtcNow;
                    materialDrawingAudits.Add(materialDrawing);
                }
            }

            return materials;
        }

        protected async Task<List<MaterialMaster>> GetOrderedOrProducedMaterials(int projectID, int organisationID)
        {
            return await _context.MaterialMaster
                                 .Include(mm => mm.DrawingAssociations)
                                 .ThenInclude(ds => ds.Drawing)
                                 .Include(mm => mm.Project)
                                 .Include(mm => mm.Trackers)
                                 .Include(mm => mm.MRF)
                                 .Include(mm => mm.MaterialType)
                                 .Include(mm => mm.Organisation)
                                 .Include(mm => mm.QCCases)
                                 .ThenInclude(qc => qc.Defects)
                                 .Include(mm => mm.StageAudits)
                                 .ThenInclude(sa => sa.Location)
                                 .Include(mm => mm.StageAudits)
                                 .ThenInclude(sa => sa.Stage)
                                  .ThenInclude(st => st.checklists)
                                 .Include(mm => mm.Elements)
                                 .ThenInclude(elm => elm.ForgeModel)
                                 .Where(mm => mm.OrganisationID == organisationID &&
                                              mm.ProjectID == projectID &&
                                              mm.MRF != null).ToListAsync();  //mm.StageAudits.Count <= 1  commented to bind qc failed  materials in in qc notification tab in vendorscreen irrespective of stage.

        }

        protected async Task<List<MaterialMaster>> GetRTOMaterials(int projectID)
        {
            try
            {
                UserMaster rto = await _context.GetUserFromHttpContext(HttpContext);

                // Get the material IDs of structural checklists routed to rto
                List<int> materialIDs = await _context.ChecklistAudit.Include(r => r.RouteTo)
                    .Include(r => r.Checklist)
                    .ThenInclude(s => s.MaterialStage)
                    .Include(r => r.MaterialStageAudit)
                    .ThenInclude(m => m.MaterialMaster)
                    .ThenInclude(mm => mm.DrawingAssociations)
                    .ThenInclude(ds => ds.Drawing)
                    .Where(ca => ca.RouteTo == rto && ca.Checklist.MaterialStage != null
                                                   && ca.MaterialStageAudit.MaterialMaster.ProjectID == projectID)
                    .Select(ca => ca.MaterialStageAudit.MaterialMaster.ID)
                    .ToListAsync();

                if (materialIDs != null && materialIDs.Count > 0)
                {

                    return await _context.MaterialMaster
                                         .Include(mm => mm.DrawingAssociations)
                                         .ThenInclude(ds => ds.Drawing)
                                         .Include(mm => mm.Project)
                                         .Include(mm => mm.Trackers)
                                         .Include(mm => mm.MRF)
                                         .Include(mm => mm.MaterialType)
                                         .Include(mm => mm.Organisation)
                                         .Include(mm => mm.QCCases)
                                         .ThenInclude(qc => qc.Defects)
                                         .Include(mm => mm.StageAudits)
                                         .ThenInclude(sa => sa.Location)
                                         .Include(mm => mm.StageAudits)
                                         .ThenInclude(sa => sa.Stage)
                                         .ThenInclude(st => st.checklists)
                                         .Include(mm => mm.Elements)
                                         .ThenInclude(elm => elm.ForgeModel)
                                         .Where(mm => materialIDs.Contains(mm.ID))
                                         .ToListAsync();
                }
                else
                {
                    throw new GenericException(ErrorMessages.DbRecordNotFound, "No materials avilable");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected List<TrackerAssociation> GetInventoryList(IEnumerable<MaterialMaster> materials,string tenant_name)
        {
            List<TrackerAssociation> lstTrackerAssociation = new List<TrackerAssociation>();
            try
            { 
                lstTrackerAssociation = materials.Select(mm => new TrackerAssociation
                {
                    Tracker = mm.Trackers.Select(t => new MobileTracker
                    {
                        ID = t.ID,
                        Tag = t.Tag,
                        Label = t.Label,
                        Type = t.Type
                    }).ToList(),
                    Material = new MaterialMobile
                    {
                        ID = mm.ID,
                        ProjectID = mm.ProjectID,
                        Block = mm.Block,
                        Level = mm.Level,
                        RouteTo = tenant_name.ToLower()=="tenant2"|| tenant_name.ToLower() == "alec" ?mm.StageAudits!=null &&  mm.StageAudits.Count()>1 ?"RTO":mm.MaterialType.RouteTo:mm.MaterialType.RouteTo,
                        Zone = mm.Zone,
                        MarkingNo = mm.MarkingNo,
                        MaterialType = mm.MaterialType.Name,
                        OrganisationID = mm.OrganisationID,
                        CastingDate = mm.CastingDate,
                        ExpectedDeliveryDate = mm.MRF.ExpectedDeliveryDate,
                        OrderDate = mm.MRF.OrderDate,
                        CurrentStage = mm.StageAudits.OrderBy(sa => sa.CreatedDate).Select(s => new MaterialStage
                        {
                            ID = s.Stage.ID,
                            CanIgnoreQC = s.Stage.CanIgnoreQC,
                            Colour = s.Stage.Colour,
                            IsEditable = s.Stage.IsEditable,
                            MilestoneId = s.Stage.MilestoneId,
                            Name = s.Stage.Name,
                        }
                                                    ).LastOrDefault(),
                        CanIgnoreQC = mm.StageAudits.OrderBy(sa => sa.CreatedDate).LastOrDefault()?.Stage != null ? mm.StageAudits.OrderBy(sa => sa.CreatedDate).LastOrDefault().Stage.CanIgnoreQC : true,
                        CountQCCase = mm.QCCases.Where(qc => qc.Defects.Any(d => d.Status != Enums.QCStatus.QC_passed_by_Maincon)).Count(),
                        MRFNo = mm.MRF.MRFNo,
                        SN = mm.SN,
                        CurrentLocation = mm.StageAudits.OrderBy(sa => sa.CreatedDate).Select(
                                                        l => new MaterialLocation
                                                        {
                                                            Id = l.Location.ID,
                                                            Name = l.Location.Name
                                                        }).LastOrDefault(),
                        ForgeElementID = mm.Elements.FirstOrDefault()?.DbID,
                        ForgeModelURN = mm.Elements.FirstOrDefault()?.ForgeModel.ObjectID,
                        QCStatus = mm.StageAudits.OrderByDescending(sa => sa.CreatedDate).FirstOrDefault()?.QCStatus.ToString(),
                        QCStatusCode = mm.StageAudits.OrderByDescending(sa => sa.CreatedDate).FirstOrDefault() == null ? -0 : (int)mm.StageAudits.OrderByDescending(sa => sa.CreatedDate).FirstOrDefault().QCStatus,
                        Length = mm.Length,
                        Area = mm.Area,
                        DrawingNo = mm.DrawingAssociations.FirstOrDefault()?.Drawing?.DrawingNo,
                        IsChecklist = mm.StageAudits.OrderBy(s => s.CreatedDate).LastOrDefault()?.Stage?.checklists != null && mm.StageAudits.OrderBy(s => s.CreatedDate).LastOrDefault()?.Stage?.checklists?.Count() > 0


                    }
                }).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return lstTrackerAssociation;
        }

        protected bool MaterialMasterExists(int id)
        {
            return _context.MaterialMaster.Any(e => e.ID == id);
        }

        protected async Task<MaterialStageMaster> GetDeliveredStage()
        {
            return await _context.MaterialStageMaster.FirstOrDefaultAsync(mm => mm.MilestoneId == 2);
        }
    }
}