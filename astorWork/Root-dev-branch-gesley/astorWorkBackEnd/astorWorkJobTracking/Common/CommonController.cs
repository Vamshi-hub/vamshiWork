using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using astorWorkDAO;
using astorWorkJobTracking.Models;
using Microsoft.EntityFrameworkCore;
using astorWorkShared.Services;
using static astorWorkShared.Utilities.Enums;

namespace astorWorkJobTracking.Common
{
    public class CommonController : Controller
    {
        protected int startDeliveryStageID = 0;
        protected int deliveredStageID = 0;
        protected int installedStageID = 0;
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

        public CommonController() { }

        protected async Task UpdateNotificationAudit(IEnumerable<UserMaster> recipients,
            int code, int type, string referenceID)
        {
            try
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
                                                                                                            }
                                                                                                      );

                    await _context.UserNotificationAssociation.AddRangeAsync(userNotificationAssociation);

                    _context.SaveChanges();
                }
            }
            catch(Exception ex)
            { }
           
        }

        protected async Task<List<UserMaster>> GetMainContractor(JobSchedule jobSchedule)
        {
            return await _context.UserMaster
                 .Include(u => u.Organisation)
                 .Include(u => u.Role)
                 .Where(u => u.Organisation!=null && u.Organisation.ID == jobSchedule.SubconID
                 && u.RoleID == (int)RoleType.SiteOfficer).Distinct().ToListAsync();
        }

        protected async Task<List<UserMaster>> GetMainContractorQC(ChecklistAudit checklistAudit)
        {
            return await _context.UserMaster
                 .Include(u => u.Role)
                 .Where(u => u.ProjectID==checklistAudit.MaterialStageAudit.Material.ProjectID
                 && u.RoleID == (int)RoleType.MainConQC).Distinct().ToListAsync();
        }

        protected async Task<List<UserMaster>> GetSubContractor(JobSchedule jobSchedule)
        {
            return await _context.UserMaster
                .Include(u => u.Organisation)
                .Include(u => u.Role)
                .Where(u =>u.Organisation!=null && u.Organisation.ID == jobSchedule.SubconID
                && u.RoleID == (int)RoleType.PPVCSubcontractor).ToListAsync();
        }

        protected async Task<List<UserMaster>> GetRTO(ChecklistAudit checklistAudit)
        {
            return await _context.UserMaster.Where(u => u.ID == checklistAudit.RouteTo.ID).Distinct().ToListAsync();
        }
    }
}