using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using astorWorkDAO;
using astorWorkMaterialTracking.Models;
using Microsoft.EntityFrameworkCore;
using astorWorkShared.Services;
using static astorWorkShared.Utilities.Enums;

namespace astorWorkMaterialTracking.Common
{
    public class CommonController : Controller
    {
        protected int startDeliveryStageID = 0;
        protected int deliveredStageID = 0;
        protected int installedStageID = 0;
        protected int producedStageID = 0;
        protected astorWorkDbContext _context;
        protected IAstorWorkBlobStorage _blobStorage;
        private astorWorkDbContext context;

        public CommonController(astorWorkDbContext context, IAstorWorkBlobStorage blobStorage)
        {
            _context = context;
            _blobStorage = blobStorage;
        }

        public CommonController(astorWorkDbContext context)
        {
            _context = context;
        }

        protected async Task SetMaterialStagesID() {
            List<MaterialStageMaster> stages = await _context.MaterialStageMaster.ToListAsync();
            startDeliveryStageID = stages.Where(s => s.Order == 2).FirstOrDefault().ID;
            deliveredStageID = stages.Where(s => s.Order == 3).FirstOrDefault().ID;
            installedStageID = stages.OrderBy(s => s.Order).LastOrDefault().ID;
            producedStageID = stages.Where(s => s.Order == 1).FirstOrDefault().ID;
        }

        protected UserMaster getCurrentUser()
        {
            return _context.UserMaster.Where(u => u.ID == 3).FirstOrDefault();
        }

        protected MaterialStageMaster getMaxStage()
        {
            return _context.MaterialStageMaster.OrderByDescending(s => s.Order).FirstOrDefault();
        }

        protected async Task<double> GetProgress(int mrfID)
        {
            return await GetNumberOfInstalledMaterials(mrfID) / await GetNumberOfMaterials(mrfID);
        }

        protected async Task<double> GetNumberOfInstalledMaterials(int mRFID)
        {
            await SetMaterialStagesID();
            List<MaterialMaster> materials = await _context.MaterialMaster
                                                           .Include(m => m.StageAudits)
                                                           .Where(m => m.MRF.ID == mRFID &&
                                                                       m.StageAudits.OrderByDescending(s => s.CreatedDate)
                                                                                    .FirstOrDefault()
                                                                                    .Stage.ID == installedStageID)
                                                           .ToListAsync();
            return materials.Count();
        }

        protected async Task<double> GetNumberOfMaterials(int mRFID)
        {
            return await _context.MaterialMaster
                         .Where(m => m.MRF.ID == mRFID)
                         .CountAsync();
        }

        protected async Task UpdateNotificationAudit(IEnumerable<UserMaster> recipients, 
            int code, int type, string referenceID)
        {
            if (recipients.Count() > 0)
            {
                NotificationAudit notification = new NotificationAudit
                {
                    Code = code,
                    Type = type,
                    Reference = referenceID,
                    CreatedDate = DateTimeOffset.Now,
                    ProcessedDate = null
                };
                IEnumerable<UserNotificationAssociation> userNotificationAssociation = recipients.Select(
                    receipient => new UserNotificationAssociation
                    {
                        Receipient = receipient,
                        Notification = notification
                    });

                //await _context.NotificationAudit.AddAsync(notification);
                await _context.UserNotificationAssociation.AddRangeAsync(userNotificationAssociation);

                _context.SaveChanges();
            }
        }

        protected async Task<MaterialStageMaster> GetLastStage() {
            return await _context.MaterialStageMaster.OrderBy(msm => msm.Order).LastAsync();
        }

        protected async Task<MaterialStageMaster> GetDeliveredStage() {
            MaterialStageMaster[] materialStages =  await _context.MaterialStageMaster.OrderBy(msm => msm.Order).ToArrayAsync();

            return materialStages[2];
        }

        protected async Task<List<MaterialStage>> GetMaterialStages(List<MaterialStageMaster> stages) {
            return stages.OrderBy(s => s.Order).Select(s => new MaterialStage
            {
                Order = s.Order,
                Name = s.Name,
                Colour = s.Colour
            }).ToList();
        }

        protected List<Signature> GetSignatures(List<ChecklistAudit> checklistAudits)
        {
            List<Signature> signatures = new List<Signature>();
            try
            {


                List<ChecklistAudit> checklistAudit = checklistAudits.Where(ca => !string.IsNullOrEmpty(ca.SignatureURL))
                                                               .OrderByDescending(ca => ca.CreatedDate).ToList();
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

        protected string GetSignatureURL(ChecklistAudit checklistAudit)
        {
            string signatureURL = checklistAudit.SignatureURL;
            string token = _blobStorage.GetContainerAccessToken();
            return signatureURL + token;
        }

        protected async Task<List<ChecklistItemAudit>> GetLatestChecklist(List<ChecklistAudit> checklistAudits)
        {
            ChecklistAudit checklistAudit = checklistAudits.OrderByDescending(ca => ca.CreatedDate)
                                                                   .FirstOrDefault();

            return await _context.ChecklistItemAudit
                                 .Include(chk => chk.ChecklistItem)
                                 .Where(cia => cia.ChecklistAuditID == checklistAudit.ID)
                                 .ToListAsync();
        }

        protected List<ChecklistItem> CreateChecklist(List<ChecklistItemAudit> checklistItemAudits)
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

        protected async Task<List<ChecklistItem>> CreateChecklist(int checklistID)
        {
            List<ChecklistItem> checklistItems = new List<ChecklistItem>();
            List<ChecklistItemAssociation> checklistItemAssociations = await _context.ChecklistItemAssociation
                                                                                         .Include(cia => cia.ChecklistItem)
                                                                                         .Where(cia => cia.ChecklistID == checklistID)
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
    }
}