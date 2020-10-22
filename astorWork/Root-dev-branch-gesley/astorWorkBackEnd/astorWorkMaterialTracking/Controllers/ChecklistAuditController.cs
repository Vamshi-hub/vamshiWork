using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using astorWorkDAO;
using astorWorkMaterialTracking.Models;
using Microsoft.AspNetCore.Mvc;
using static astorWorkShared.Utilities.Enums;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace astorWorkMaterialTracking.Controllers
{
    [Produces("application/json")]
    [Route("checklist-audit")]
    [ApiController]
    public class ChecklistAuditController : Controller
    {
        protected astorWorkDbContext _context;

        public ChecklistAuditController(astorWorkDbContext context)
        {
            _context = context;
        }

        // GET api/<controller>/5
        [HttpGet]
        public List<MaterialChecklist> GetMaterialChecklists([FromQuery]int material_id, [FromQuery]int route_to_id)
        {
            // Return a list of structural checklist for the respective material and routed to the respective RTO
            List<MaterialChecklist> materialChecklists = _context.ChecklistAudit.Where(ca => ca.Checklist.MaterialStageID != null
                                                                                          && ca.MaterialStageAudit.Material.ID == material_id
                                                                                          && ca.Status == QCStatus.QC_routed_to_RTO
                                                                                          && ca.RouteTo.ID == route_to_id
                                                                                      )
                                                                                .Select(ca => CreateMaterialChecklist(ca))
                                                                                .ToList();



            if (materialChecklists == null || materialChecklists.Count == 0)
            {
                int currentMaterialStageID = _context.MaterialStageAudit.Where(msa => msa.Material.ID == material_id)
                                                                        .OrderByDescending(msa => msa.CreatedDate)
                                                                        .FirstOrDefault()
                                                                        .StageID;
                int producedStageID = 1;
                materialChecklists = _context.ChecklistMaster.Where(cm => currentMaterialStageID == producedStageID)
                                        .Select(cm => new MaterialChecklist
                                        {
                                            MaterialID = material_id,
                                            ChecklistID = cm.ID,
                                            Status = QCStatus.QC_routed_to_RTO.ToString(),
                                            StatusCode = (int)QCStatus.QC_routed_to_RTO
                                        }).ToList();
            }

            return materialChecklists;
        }

        private MaterialChecklist CreateMaterialChecklist(ChecklistAudit checklistAudit)
        {
            MaterialStageAudit materialStageAudit = checklistAudit.MaterialStageAudit;
            MaterialMaster material = materialStageAudit.Material;
            OrganisationMaster organisation = material.Organisation;
            ProjectMaster project = material.Project;
            QCStatus qcStatus= checklistAudit.Status;

            return new MaterialChecklist
            {
                ChecklistID = checklistAudit.ChecklistID,
                MaterialID = materialStageAudit.MaterialID,
                MarkingNo = material.MarkingNo,
                MaterialType = material.MaterialType.Name,
                Block = material.Block,
                Level = material.Level,
                Zone = material.Zone,
                SubConID = organisation.ID,
                SubConName = organisation.Name,
                ProjectID = project.ID,
                ProjectName = project.Name,
                Status = qcStatus.ToString(),
                StatusCode = (int)qcStatus
            };
        }
    }
}
