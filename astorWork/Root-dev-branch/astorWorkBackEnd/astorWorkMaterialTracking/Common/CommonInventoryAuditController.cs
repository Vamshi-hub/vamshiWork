using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using astorWorkDAO;
using astorWorkMaterialTracking.Models;
using Microsoft.EntityFrameworkCore;
using astorWorkShared.Utilities;

namespace astorWorkMaterialTracking.Common
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

        protected InfoForNewInventory CreateInfoForNewInventory(List<MaterialMaster> markingNos) {
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

        protected async Task<InventoryAudit> CreateInventoryInDb(Inventory inventory, int projectID, int organisationID) {

            UserMaster user = await _context.GetUserFromHttpContext(HttpContext);

            InventoryAudit inventoryAudit = new InventoryAudit
            {
                MarkingNo = inventory.MarkingNo,
                SN = inventory.SN,
                CastingDate = inventory.CastingDate,
                Tracker = _context.TrackerMaster.Where(t => t.ID == inventory.TrackerID).FirstOrDefault(),
                Project = _context.ProjectMaster.Where(p => p.ID == projectID).FirstOrDefault(),
                Organisation = _context.OrganisationMaster.Where(v => v.ID == organisationID).FirstOrDefault(),
                CreatedByID = user.ID,
                CreatedDate = DateTimeOffset.Now
            };

            _context.InventoryAudit.Add(inventoryAudit);
            await _context.SaveChangesAsync();

            return inventoryAudit;
        }

        protected async Task<bool> TrackerIsUsed(int trackerID)
        {
            List<MaterialMaster> materials = await _context.MaterialMaster
                                                           .Where(m => m.Trackers.Any(t => t.ID == trackerID))
                                                           .ToListAsync();

            if (materials.Count() > 0)
                return true;

            List<InventoryAudit> inventories = await _context.InventoryAudit
                                                     .Where(m => m.Tracker.ID == trackerID)
                                                     .ToListAsync();

            return (inventories.Count() > 0);
        }

        protected async Task<List<MaterialMaster>> GetMaterials(int projectID, int organisationID) {
            return await _context.MaterialMaster
                           .Include(mm => mm.Project)
                           .Include(mm => mm.Trackers)
                           .Include(mm => mm.MRF)
                           .Include(mm => mm.Organisation)
                           .Include(mm => mm.QCCases)
                           .ThenInclude(qc => qc.Defects)
                           .Include(mm => mm.StageAudits)
                           .ThenInclude(sa => sa.Location)
                           .Include(mm => mm.StageAudits)
                           .ThenInclude(sa => sa.Stage)
                           .Include(mm => mm.Elements)
                           .ThenInclude(elm => elm.ForgeModel)
                           .Where(mm => mm.OrganisationID == organisationID &&
                                        mm.ProjectID == projectID && mm.MRF != null &&
                                        mm.StageAudits.Count <= 1).ToListAsync();
        }

        protected List<TrackerAssociation> GetInventories(IEnumerable<MaterialMaster> materials) {
            return materials.Select(mm => new TrackerAssociation {
                                            Tracker = mm.Trackers.Select(t => new MobileTracker
                                            {
                                                ID  = t.ID,
                                                Tag = t.Tag,
                                                Label = t.Label,
                                                Type = t.Type
                                            }).ToList(),
                                            Material = new MaterialMobile {
                                                    ID = mm.ID,
                                                    Block = mm.Block,
                                                    Level = mm.Level,
                                                    Zone = mm.Zone,
                                                    MarkingNo = mm.MarkingNo,
                                                    MaterialType = mm.MaterialType.Name,
                                                OrganisationID = mm.OrganisationID,
                                                    CastingDate = mm.CastingDate,
                                                    ExpectedDeliveryDate = mm.MRF.ExpectedDeliveryDate,
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
                                                    CountQCCase = mm.QCCases.Where(qc => qc.Defects.Any(d => d.Status < Enums.QCStatus.QC_passed_by_Maincon)).Count(),
                                                    MRFNo = mm.MRF.MRFNo,
                                                    SN = mm.SN,
                                                    CurrentLocation = mm.StageAudits.OrderBy(sa => sa.CreatedDate).Select(
                                                        l => new MaterialLocation
                                                        {
                                                            Id = l.Location.ID,
                                                            Name     = l.Location.Name
                                                        }).LastOrDefault(),
                                                    ForgeElementID = mm.Elements.FirstOrDefault()?.DbID,
                                                    ForgeModelURN = mm.Elements.FirstOrDefault()?.ForgeModel.ObjectID
                                            }
                                          }
                                    ).ToList();
        }

        protected async Task<List<MaterialMaster>> GetMarkingNos(int projectID, int organisationID, string materialType) {
            return await _context.MaterialMaster
                           .Where(m => m.Project.ID == projectID
                                    && m.Organisation.ID == organisationID
                                    && m.MaterialType.Name == materialType)
                           .ToListAsync();
        }

        protected bool InventoryAuditExists(int id)
        {
            return _context.InventoryAudit.Any(e => e.ID == id);
        }
    }
}