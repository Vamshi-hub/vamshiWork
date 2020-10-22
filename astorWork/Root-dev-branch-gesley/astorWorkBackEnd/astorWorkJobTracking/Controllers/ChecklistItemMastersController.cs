using astorWorkDAO;
using astorWorkJobTracking.Models;
using astorWorkShared.GlobalExceptions;
using astorWorkShared.GlobalResponse;
using astorWorkShared.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static astorWorkShared.Utilities.Enums;
using astorWorkShared.Utilities;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace astorWorkJobTracking.Controllers
{
    [Produces("application/json")]
    [Route("projects/{project_id}/checklist-items")]
    public class ChecklistItemMastersController : Controller
    {
        protected astorWorkDbContext _context;
        protected IAstorWorkImport _importService;
        protected IAstorWorkBlobStorage _blobStorage;

        public int SectionSequence = 0;
        public int Sequence = 1;

        public ChecklistItemMastersController(astorWorkDbContext context, IAstorWorkImport importService, IAstorWorkBlobStorage blobStorage)
        {
            _context = context;
            _importService = importService;
            _blobStorage = blobStorage;
        }

        // GET: projects/{project_id}/checklist-items?checklist_id=1&job_schedule_id=1
        [HttpGet()]
        public async Task<SignedChecklist> ListChecklistItems([FromQuery] int checklist_id, [FromQuery] int job_schedule_id, [FromQuery] int material_id)
        {
            if (!ModelState.IsValid)
                throw new GenericException(ErrorMessages.BadRequest, ErrorMessages.BadRequestMsg());

            try
            {
                List<ChecklistItem> checklistItems = new List<ChecklistItem>();
                List<ChecklistAudit> checklistAudits = new List<ChecklistAudit>();
                if (job_schedule_id != 0)
                {
                    checklistAudits = await _context.ChecklistAudit.Include(ca => ca.CreatedBy)
                                                                                    .ThenInclude(cb => cb.Role)
                                                                                    .Where(ca => ca.ChecklistID == checklist_id
                                                                                              && ca.JobScheduleID == job_schedule_id)
                                                                                    .ToListAsync();
                    checklistItems = await GetChecklistForTrade(checklist_id, job_schedule_id, checklistAudits);
                }
                else
                {
                    MaterialStageAudit materialStageAudit = await _context.MaterialStageAudit.Where(ms => ms.MaterialID == material_id)
                                                                                             .OrderByDescending(msa => msa.CreatedDate)
                                                                                             .FirstOrDefaultAsync();

                    if (materialStageAudit != null)
                        checklistAudits = await _context.ChecklistAudit.Include(ca => ca.CreatedBy)
                                                                       .ThenInclude(cb => cb.Role)
                                                                       .Where(ca => ca.ChecklistID == checklist_id
                                                                                 && ca.MaterialStageAuditID == materialStageAudit.ID)
                                                                       .ToListAsync();

                    checklistItems = await GetChecklistForMaterialStage(checklist_id, material_id, checklistAudits);
                }

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

        private async Task<List<ChecklistItem>> GetChecklistForMaterialStage(int checklist_id, int material_id, List<ChecklistAudit> checklistAudits)
        {
            if (checklistAudits != null && checklistAudits.Count > 0)
            {
                UserMaster user = await _context.GetUserFromHttpContext(HttpContext);
                ChecklistAudit checklistAudit = checklistAudits.OrderByDescending(ca => ca.CreatedDate)
                                                               .FirstOrDefault();

                List<ChecklistItemAudit> checklistItemAudits = await _context.ChecklistItemAudit
                                                                             .Include(chk => chk.ChecklistItem)
                                                                             .Where(cia => cia.ChecklistAuditID == checklistAudit.ID
                                                                                        && cia.ChecklistAudit.MaterialStageAudit.MaterialID == material_id)
                                                                             .ToListAsync();

                if (checklistItemAudits != null && checklistItemAudits.Count > 0)
                    return CreateChecklist(checklistItemAudits);  // Get checklist item audit records from db if they exist
            }

            return await CreateChecklist(checklist_id);   // Create a checklist items if no checklist audits exist in db
        }

        private async Task<List<ChecklistItem>> GetChecklistForTrade(int checklist_id, int job_schedule_id, List<ChecklistAudit> checklistAudits)
        {
            List<ChecklistItem> checklistItems = new List<ChecklistItem>();

            if (checklistAudits != null && checklistAudits.Count > 0)
            {
                UserMaster user = await _context.GetUserFromHttpContext(HttpContext);
                ChecklistAudit checklistAudit = checklistAudits.OrderByDescending(ca => ca.CreatedDate)
                                                               .FirstOrDefault();

                List<ChecklistItemAudit> checklistItemAudits = await _context.ChecklistItemAudit
                                                                             .Include(chk => chk.ChecklistItem)
                                                                             .Where(cia => cia.ChecklistAuditID == checklistAudit.ID
                                                                                        && cia.ChecklistAudit.JobScheduleID == job_schedule_id)
                                                                             .ToListAsync();

                if (checklistItemAudits != null && checklistItemAudits.Count > 0)
                    return CreateChecklist(checklistItemAudits);  // Get checklist item audit records from db if they exist
            }

            return await CreateChecklist(checklist_id);   // Create a checklist items if no checklist audits exist in db
        }

        private List<Signature> GetSignatures(List<ChecklistAudit> checklistAudits)
        {
            List<Signature> signatures = new List<Signature>();
            try
            {
                var checklistAudit = checklistAudits.Where(ca => !string.IsNullOrEmpty(ca.SignatureURL))
                                                               .OrderByDescending(ca => ca.CreatedDate);
                //if checklist null eror solving 
                if (checklistAudit != null)
                {
                    signatures = checklistAudit.Select(ca =>
                    new Signature()
                    {
                        SignatureURL = GetSignatureURL(ca),
                        MobileEntryPoint = ca.CreatedBy.Role.MobileEntryPoint
                    }).ToList();
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return signatures;
        }

        private string GetSignatureURL(ChecklistAudit checklistAudit)
        {

            string signatureURL = checklistAudit.SignatureURL;
            string token = _blobStorage.GetContainerAccessToken();
            return signatureURL + token;
        }

        private List<ChecklistItem> CreateChecklist(List<ChecklistItemAudit> checklistItemAudits)
        {
            List<ChecklistItem> checklistItems = new List<ChecklistItem>();
            int sequence = 0;
            foreach (ChecklistItemAudit checklistItemAudit in checklistItemAudits.OrderBy(chk => chk.ID))
            {
                ChecklistItem checklistItem = new ChecklistItem()
                {
                    ID = checklistItemAudit.ChecklistItemID,
                    Name = checklistItemAudit.ChecklistItem.Name,
                    TimeFrame = checklistItemAudit.ChecklistItem.TimeFrame,
                    StatusCode = (int)checklistItemAudit.Status,
                    Status = checklistItemAudit.Status.ToString(),
                    Sequence = sequence
                };

                checklistItems.Add(checklistItem);
                sequence++;
            }

            return checklistItems;
        }

        private async Task<List<ChecklistItem>> CreateChecklist(int checklistID)
        {
            List<ChecklistItem> checklistItems = new List<ChecklistItem>();
            List<ChecklistItemAssociation> checklistItemAssociations = await _context.ChecklistItemAssociation
                                                                                         .Include(cia => cia.ChecklistItem)
                                                                                         .Where(cia => cia.ChecklistID == checklistID && cia.IsActive == true)
                                                                                         .ToListAsync();

            foreach (ChecklistItemAssociation checklistItemAssociation in checklistItemAssociations)
            {
                ChecklistItem item = new ChecklistItem()
                {
                    ID = checklistItemAssociation.ChecklistItemID,
                    Sequence = checklistItemAssociation.ChecklistItemSequence,
                    Name = checklistItemAssociation.ChecklistItem.Name,
                    TimeFrame = checklistItemAssociation.ChecklistItem.TimeFrame,
                    StatusCode = (int)ChecklistItemStatus.Pending,
                    Status = ChecklistItemStatus.Pending.ToString()
                };
                checklistItems.Add(item);
            }

            return checklistItems;
        }

        // POST api/<controller>
        [HttpPost]
        public async Task CreateTrade([FromBody]TradeMaster trade)
        {
            if (!ModelState.IsValid)
                throw new GenericException(ErrorMessages.BadRequest, ErrorMessages.BadRequestMsg());

            _context.TradeMaster.Add(trade);
            await _context.SaveChangesAsync();
        }

        // POST api/<controller>
        [HttpPost("import-checklist")]
        public async Task<List<ChecklistItemUploadStatus>> ImportChecklistItems(IFormFile file, string project_id, [FromQuery] int trade_id, [FromQuery] string checklist_type, [FromQuery] int material_stage_id)
        {
            List<ChecklistItemUploadStatus> existingChecklistItems = new List<ChecklistItemUploadStatus>();
            try
            {
                if (!ModelState.IsValid || string.IsNullOrEmpty(project_id))
                    throw new GenericException(ErrorMessages.BadRequest, ErrorMessages.BadRequestMsg());

                List<string> jobs = _importService.GetRowsFromFile(file).Distinct().ToList();

                if (trade_id != 0)
                {
                    await InactiveTradeChecklistmaster(trade_id, checklist_type);
                    await InActiveTradeChecklistMasterAssociations(trade_id, checklist_type);
                }
                    
                else
                {
                    await InActiveMaterialChecklistMaster(material_stage_id);
                    await InActiveStageChecklistAssociations(material_stage_id);
                }
                for (int i = 1; i < jobs.Count(); i++)
                {
                    ChecklistItemMaster checklistItem = await AddChecklistItem(jobs.ElementAt(i), checklist_type);
                    ChecklistMaster checklistMaster = await AddChecklistMaster(jobs.ElementAt(i), trade_id, material_stage_id,checklist_type);
                    if (checklistMaster.Name != "")
                    {
                        ChecklistMaster objChecklistMaster = null;
                        ChecklistItemMaster objCheckListItemMaster = null;
                        if (trade_id != 0)
                        {
                            //ChecklistItemAssociation Insert
                            objChecklistMaster = _context.ChecklistMaster
                             .Where(cm => cm.TradeID == trade_id && cm.Name == checklistMaster.Name
                              && cm.Sequence == checklistMaster.Sequence &&cm.IsActive==true)
                                                         .FirstOrDefault();
                            objCheckListItemMaster = _context.ChecklistItemMaster.Where(cm => cm.Name.ToLower() == checklistItem.Name.ToLower())
                           .FirstOrDefault();
                        }
                        else
                        {
                            //ChecklistItemAssociation Insert
                            objChecklistMaster = _context.ChecklistMaster
                               .Where(cm => cm.MaterialStageID == material_stage_id && cm.Sequence == checklistMaster.Sequence &&cm.Name.ToLower()==checklistMaster.Name.ToLower() &&cm.IsActive==true).FirstOrDefault();
                            objCheckListItemMaster = _context.ChecklistItemMaster.Where(cm => cm.Name.ToLower() == checklistItem.Name.ToLower()).FirstOrDefault();

                        }
                        await ChecklistItemAssociation(objChecklistMaster, objCheckListItemMaster);

                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            if (existingChecklistItems.Count == 0)
                return new List<ChecklistItemUploadStatus>();
            else
                return existingChecklistItems;
        }

        private async Task InactiveTradeChecklistmaster(int trade_id, string checklist_type)
        {
           try
            {
                List<ChecklistMaster> checklistmastertoDelete = await _context.ChecklistMaster
                     .Where(cm => cm.TradeID == trade_id && cm.Type==(ChecklistType)Enum.Parse(typeof(ChecklistType),checklist_type)).ToListAsync();
                List<ChecklistMaster> listchecklistMaster = new List<ChecklistMaster>();

                foreach (ChecklistMaster checklist in checklistmastertoDelete)
                {
                    checklist.IsActive = false;
                    listchecklistMaster.Add(checklist);
                }
                _context.ChecklistMaster.UpdateRange(listchecklistMaster);
                await _context.SaveChangesAsync();
            }
            catch(Exception ex)
            {

            }
        }

        private async Task InActiveMaterialChecklistMaster(int material_stage_id)
        {
            List<ChecklistMaster> checklistmastertoDelete = await _context.ChecklistMaster.Where(cm => cm.MaterialStageID == material_stage_id).ToListAsync();
            List<ChecklistMaster> listchecklistMaster = new List<ChecklistMaster>();

            foreach (ChecklistMaster checklist in checklistmastertoDelete)
            {
                checklist.IsActive = false;
                listchecklistMaster.Add(checklist);
            }
            _context.ChecklistMaster.UpdateRange(listchecklistMaster);
            await _context.SaveChangesAsync();
        }

        protected async Task InActiveTradeChecklistMasterAssociations(int trade_id, string checklist_type)
        {
            try
            {
                List<ChecklistItemAssociation> checklistItemAssociationsToDelete = await _context.ChecklistItemAssociation
                         .Include(ci => ci.Checklist)
                         .Where(cl => cl.Checklist.TradeID == trade_id)
                         .ToListAsync();

                List<ChecklistItemAssociation> itemAssociation = new List<ChecklistItemAssociation>(); ;

                foreach (ChecklistItemAssociation checklistItem in checklistItemAssociationsToDelete)
                {
                    checklistItem.IsActive = false;
                    itemAssociation.Add(checklistItem);
                }
                _context.ChecklistItemAssociation.UpdateRange(itemAssociation);
                await _context.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                throw ex;
            }
          
        }

        protected async Task InActiveStageChecklistAssociations(int material_stage_id)
        {
            try
            {
                List<ChecklistItemAssociation> checklistItemAssociationsToDelete = await _context.ChecklistItemAssociation
                                                                                                           .Include(ci => ci.Checklist)
                                                                                                           .Where(cl => cl.Checklist.MaterialStageID == material_stage_id)
                                                                                                           .ToListAsync();
                List<ChecklistItemAssociation> itemAssociation = new List<ChecklistItemAssociation>(); ;

                foreach (ChecklistItemAssociation checklistItem in checklistItemAssociationsToDelete)
                {
                    checklistItem.IsActive = false;
                    itemAssociation.Add(checklistItem);
                }
                _context.ChecklistItemAssociation.UpdateRange(itemAssociation);
                await _context.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                throw ex;
            }
           
        }

        private async Task<ChecklistItemMaster> AddChecklistItem(string job, string checklistType)
        {

            ChecklistItemMaster checklistItem = CreateChecklistItem(job, checklistType);
            try
            {
                if (!ChecklistItemMasterExists(checklistItem))   // Check for duplicates in the Db
                {
                    if (!string.IsNullOrEmpty(checklistItem.Name))
                        await _context.ChecklistItemMaster.AddAsync(checklistItem);

                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }


            return checklistItem;
        }
        private async Task<ChecklistMaster> AddChecklistMaster(string job, int tradeID, int materialStageID, string checklist_type)
        {
            ChecklistMaster checklistMaster = CreateCheckListMaster(job, tradeID, materialStageID, checklist_type);
            try
            {
                if (!CheckListMasterExists(checklistMaster))
                {
                    _context.ChecklistMaster.Add(checklistMaster);
                    await _context.SaveChangesAsync();
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return checklistMaster;
        }

        private async Task ChecklistItemAssociation(ChecklistMaster objChecklistMaster, ChecklistItemMaster checklistItem)
        {
            try
            {
                //if (checklistItemAssociationExist(objChecklistMaster,checklistItem))
                //{
                //    List<ChecklistItemAssociation> checklistasscocation = _context.ChecklistItemAssociation.Where(ca => ca.ChecklistID == objChecklistMaster.ID).ToList();
                //    foreach (ChecklistItemAssociation association in checklistasscocation)
                //    {
                //        ChecklistItemAssociation itemAssociations = new ChecklistItemAssociation();
                //        itemAssociations.IsActive = false;
                //        _context.Entry(checklistasscocation).State = EntityState.Modified;
                //        //_context.ChecklistItemAssociation.Add(itemAssociations);

                //        await _context.SaveChangesAsync();
                //    }


                //    //ChecklistItemAssociation checklistasscocation = _context.ChecklistItemAssociation.Where(ca => ca.ChecklistID == objChecklistMaster.ID).FirstOrDefault();
                //    //checklistasscocation.IsActive = false;
                //    //_context.Entry(checklistasscocation).State = EntityState.Modified;
                //    //await _context.SaveChangesAsync();
                //}

                ChecklistItemAssociation checklistItemAssociation = new ChecklistItemAssociation();
                checklistItemAssociation.ChecklistID = objChecklistMaster.ID;
                checklistItemAssociation.ChecklistItemID = checklistItem.ID;
                checklistItemAssociation.ChecklistItemSequence = Sequence;
                checklistItemAssociation.IsActive = true;
                Sequence++;
                _context.ChecklistItemAssociation.Add(checklistItemAssociation);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected bool checklistItemAssociationExist(ChecklistMaster objChecklistMaster, ChecklistItemMaster objChecklistItemMaster)
        {
            return _context.ChecklistItemAssociation.Any(ca => ca.ChecklistID == objChecklistMaster.ID && ca.ChecklistItemID == objChecklistItemMaster.ID);

        }

        protected bool CheckListMasterExists(ChecklistMaster checklistMaster)
        {
            if (checklistMaster.TradeID != 0 && checklistMaster.TradeID != null)
            {
                return _context.ChecklistMaster
                       .Any(cm => cm.TradeID == checklistMaster.TradeID && cm.Name.ToLower().Trim() == checklistMaster.Name.ToLower().Trim() && cm.Sequence == checklistMaster.Sequence && cm.IsActive==true);
            }
            else
            {
                return _context.ChecklistMaster
                       .Any(cm => cm.MaterialStageID == checklistMaster.MaterialStageID && cm.Name.ToLower().Trim() == checklistMaster.Name.ToLower().Trim() && cm.Sequence == checklistMaster.Sequence &&cm.IsActive==true);
            }

        }

        protected ChecklistMaster CreateCheckListMaster(string checkListItemInfo, int? tradeID, int? MaterialStageID,string checklist_type)
        {
            ChecklistMaster checklistMaster = new ChecklistMaster();
            string[] checkListAttributes = checkListItemInfo.Trim().Split(",");
            if (checkListAttributes.Count() > 1 && checkListAttributes[1].Trim().Length > 0)
                checklistMaster.Name = checkListAttributes[1].Trim();   // Timeframe
            checklistMaster.TradeID = tradeID == 0 ? null : tradeID;
            checklistMaster.Sequence = checkListAttributes[2] == "" ? 1 : int.Parse(checkListAttributes[2]);
            checklistMaster.MaterialStageID = MaterialStageID == 0 ? null : MaterialStageID;
            checklistMaster.IsActive = true;
            checklistMaster.Type = (ChecklistType)Enum.Parse(typeof(ChecklistType), checklist_type);

            return checklistMaster;
        }

        /// <summary>
        /// Get checklistItems for a scheduled job with status and sequence
        /// </summary>
        /// <param name="JobScheduleID"></param>
        /// <param name="tradeID"></param>
        /// <returns>TradeChecklistItems</returns>
        /// <url>projects/{project_id}/checklistItems/by-jobschedule ?jobschedule_id= 1 &trade_id = 2 /</url>
        [HttpGet("by-jobschedule")]
        public List<ChecklistItem> CheckListItemByScheduleID([FromQuery] int jobschedule_id, [FromQuery] int trade_id, [FromQuery] int checklist_id)
        {
            List<ChecklistItem> lstJobChecklistItem = new List<ChecklistItem>();
            try
            {
                lstJobChecklistItem = _context.ChecklistItemAssociation
                    .Include(tc => tc.ChecklistItem)
                    .Include(tc => tc.Checklist)
                    .Where(tc => tc.ChecklistID == checklist_id)
                    .Select(tc => new ChecklistItem()
                    {
                        Name = tc.ChecklistItem.Name,
                        ID = tc.ChecklistItemID,
                        StatusCode = Convert.ToInt32(ChecklistItemStatus.Pending),
                        Status = ChecklistItemStatus.Pending.ToString(),
                        Sequence = tc.ChecklistItemSequence
                    }
                    ).ToList();
                if (lstJobChecklistItem == null || lstJobChecklistItem.Count == 0)
                {
                    throw new GenericException(ErrorMessages.DbRecordNotFound, "There is no Association for this Trade");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return lstJobChecklistItem;
        }


        protected async Task CreateTradeAssociations(int checklistID, List<string> addedChecklistItems)
        {
            List<ChecklistItemAssociation> checklisttobeDelete = _context.ChecklistItemAssociation
                      .Where(tc => tc.ChecklistID == checklistID).ToList();
            _context.ChecklistItemAssociation.RemoveRange(checklisttobeDelete);
            _context.SaveChanges();
            for (int i = 0; i < addedChecklistItems.Count; i++)
            {
                ChecklistItemAssociation tradeChecklistItemAssociation = new ChecklistItemAssociation();
                var checklist = _context.ChecklistItemMaster
                     .Where(c => c.Name.ToLower() == addedChecklistItems[i])
                     .Select(c => c.ID).ToList();
                tradeChecklistItemAssociation.ChecklistItemID = checklist[0];
                tradeChecklistItemAssociation.ChecklistID = checklistID;
                tradeChecklistItemAssociation.ChecklistItemSequence = Sequence;
                _context.ChecklistItemAssociation.Add(tradeChecklistItemAssociation);

                Sequence++;
            }

            await _context.SaveChangesAsync();
        }

        private bool CheckTradAssociationExists(ChecklistItemAssociation checklistItemAssociation)
        {
            return _context.ChecklistItemAssociation.Any(t => t.ChecklistItemID == checklistItemAssociation.ChecklistItemID && t.Checklist.Trade == checklistItemAssociation.Checklist.Trade);
        }

        protected ChecklistItemMaster CreateChecklistItem(string checklistItemInfo, string checkListItemType)
        {
            ChecklistItemMaster checklistItem = new ChecklistItemMaster();
            string[] jobAttributes = checklistItemInfo.Trim().Split(",");
            checklistItem.Name = jobAttributes[0];
            checklistItem.TimeFrame = jobAttributes[1];
            if (!string.IsNullOrEmpty(checkListItemType))
            {
                checklistItem.Type = (int)(ChecklistType)Enum.Parse(typeof(ChecklistType), checkListItemType);
            }

            return checklistItem;
        }

        protected bool ChecklistItemMasterExists(ChecklistItemMaster checklistItem)
        {
            return _context.ChecklistItemMaster.Any(mt => (mt.Name.ToLower() == checklistItem.Name.ToLower()));
        }

        protected List<ChecklistItemUploadStatus> AddChecklistItemUploadStatus(List<ChecklistItemUploadStatus> existingChecklistItems, ChecklistItemMaster checklistItem, string message)
        {
            ChecklistItemUploadStatus uploadStatus = new ChecklistItemUploadStatus();

            uploadStatus.Name = checklistItem.Name;
            uploadStatus.Message = message;

            existingChecklistItems.Add(uploadStatus);

            return existingChecklistItems;
        }

        // PUT api/<controller>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
