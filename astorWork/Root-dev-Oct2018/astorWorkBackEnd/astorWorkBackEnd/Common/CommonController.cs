using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using astorWorkDAO;
using astorWorkBackEnd.Models;
using Microsoft.EntityFrameworkCore;

namespace astorWorkBackEnd.Common
{
    [Produces("application/json")]
    [Route("api/Base")]
    public class CommonController : Controller
    {
        protected int startDeliveryStageId = 0;
        protected int deliveredStageId = 0;
        protected int installedStageId = 0;

        public CommonController(astorWorkDbContext context)
        {
            _context = context;
            var stages = _context.MaterialStageMaster.ToList();
            startDeliveryStageId = stages.Where(s => s.Order == 2).FirstOrDefault().ID;
            deliveredStageId = stages.Where(s => s.Order == 3).FirstOrDefault().ID;
            installedStageId = stages.OrderBy(s => s.Order).LastOrDefault().ID;
        }

        protected astorWorkDbContext _context;

        protected UserMaster getCurrentUser()
        {
            return _context.UserMaster.Where(u => u.ID == 3).FirstOrDefault();
        }

        protected MaterialStageMaster getMaxStage()
        {
            return _context.MaterialStageMaster.OrderByDescending(s => s.Order).FirstOrDefault();
        }

        protected double GetProgress(int mrfID)
        {
            return GetNumberOfInstalledMaterials(mrfID) / GetNumberOfMaterials(mrfID);
        }

        protected double GetNumberOfInstalledMaterials(int mRFID)
        {

            var mm = _context.MaterialMaster
                     .Include(m => m.StageAudits)
                     .Where(m => m.MRF.ID == mRFID &&
                          m.StageAudits
                           .OrderByDescending(s => s.CreatedDate)
                           .FirstOrDefault().Stage.ID == installedStageId &&
                          m.StageAudits
                           .OrderByDescending(s => s.CreatedDate)
                           .FirstOrDefault().StagePassed);
            return mm.Count();
        }

        protected double GetNumberOfMaterials(int mRFID)
        {
            return _context.MaterialMaster
                   .Where(m => m.MRF.ID == mRFID)
                   .Count();
        }

        public async Task UpdateNotificationAudit(List<UserMaster> recipients, 
            int code, int type, string referenceID)
        {
            if (recipients.Count > 0)
            {
                var notification = new NotificationAudit
                {
                    Code = code,
                    Type = type,
                    Reference = referenceID,
                    CreatedDate = DateTimeOffset.Now,
                    ProcessedDate = null
                };
                var userNotificationAssociation = recipients.Select(
                    receipient => new UserNotificationAssociation
                    {
                        Receipient = receipient,
                        Notification = notification
                    });

                //_context.NotificationAudit.AddAsync(notification);
                await _context.UserNotificationAssociation.AddRangeAsync(userNotificationAssociation);
            }
        }
    }
}