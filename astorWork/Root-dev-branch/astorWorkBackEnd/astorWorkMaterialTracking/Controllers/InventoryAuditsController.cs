using astorWorkDAO;
using astorWorkMaterialTracking.Common;
using astorWorkMaterialTracking.Models;
using astorWorkShared.GlobalExceptions;
using astorWorkShared.GlobalResponse;
using astorWorkShared.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace astorWorkMaterialTracking.Controllers
{
    [Produces("application/json")]
    [Route("projects/{project_id}/vendors/{vendor_id}/inventory")]
    public class InventoryAuditsController : Controller
    {
        protected astorWorkDbContext _context;
        private readonly string module = "Inventory";

        public InventoryAuditsController(astorWorkDbContext context)
        {
            _context = context;
        }

        // GET: /projects/{project_id}/vendors/{vendor_id}/inventory
        [HttpGet()]
        public async Task<List<TrackerAssociation>> ListInventory([FromRoute] int project_id, [FromRoute] int vendor_id)
        {
            if (!ModelState.IsValid)
                throw new GenericException(ErrorMessages.BadRequest, ErrorMessages.BadRequestMsg());

            List<MaterialMaster> materials = await GetMaterials(project_id, vendor_id);

            if (materials.Count() == 0)
                throw new GenericException(ErrorMessages.DbRecordNotFound, ErrorMessages.DBRecordNotFoundMsg(module, "Project and Vendor IDs", project_id + " " + vendor_id));

            return GetInventories(materials);
        }

        // GET: /projects/{project_id}/vendors/{vendor_id}/inventory/pre-create?material_type={material_type}
        [HttpGet("pre-create")]
        public async Task<InfoForNewInventory> GetInformationForNewInventory([FromRoute] int project_id, [FromRoute] int vendor_id, [FromQuery] string material_type)
        {
            if (!ModelState.IsValid)
                throw new GenericException(ErrorMessages.BadRequest, ErrorMessages.BadRequestMsg());

            List<MaterialMaster> markingNos = await GetMarkingNos(project_id, vendor_id, material_type);

            if (markingNos == null)
                throw new GenericException(ErrorMessages.DbRecordNotFound, ErrorMessages.DBRecordNotFoundMsg("Marking numbers", "Project and Vendor IDs and Material type", string.Format("{0}, {1} and {2}", project_id, vendor_id, material_type)));

            return CreateInfoForNewInventory(markingNos);
        }

        protected int GetMaxSN()
        {

            return _context.MaterialMaster.Count() == 0 ? 0 : _context.MaterialMaster.Max(m => m.SN);
        }

        protected InfoForNewInventory CreateInfoForNewInventory(List<MaterialMaster> markingNos)
        {
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


        protected async Task<bool> TrackerIsUsed(int trackerID)
        {
            List<MaterialMaster> materials = await _context.MaterialMaster
                .Where(m => m.Trackers.Any(t => t.ID == trackerID))
                .ToListAsync();

            return materials.Count() > 0;
        }

        protected async Task<List<MaterialMaster>> GetMaterials(int projectID, int organisationID)
        {
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

        protected List<TrackerAssociation> GetInventories(IEnumerable<MaterialMaster> materials)
        {
            return materials.Select(mm => new TrackerAssociation
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
                                                            Name = l.Location.Name
                                                        }).LastOrDefault(),
                    ForgeElementID = mm.Elements.FirstOrDefault()?.DbID,
                    ForgeModelURN = mm.Elements.FirstOrDefault()?.ForgeModel.ObjectID
                }
            }
                                    ).ToList();
        }

        protected async Task<List<MaterialMaster>> GetMarkingNos(int projectID, int organisationID, string materialType)
        {
            return await _context.MaterialMaster
                           .Where(m => m.Project.ID == projectID
                                    && m.Organisation.ID == organisationID
                                    && m.MaterialType.Name == materialType)
                           .ToListAsync();
        }

    }
}