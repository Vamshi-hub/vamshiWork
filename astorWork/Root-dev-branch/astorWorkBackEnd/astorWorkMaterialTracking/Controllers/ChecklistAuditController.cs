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

        // GET: api/<controller>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        [HttpGet]
        public List<MaterialChecklist> GetMaterialChecklists([FromQuery]int material_id, [FromQuery]int route_to_id)
        {
            // Return a list of structural checklist for the respective material and routed to the respective RTO
            List<MaterialChecklist> materialChecklists = _context.ChecklistAudit.Where(ca => ca.Checklist.MaterialStageID != null
                                                                                          && ca.MaterialStageAudit.MaterialMasterID == material_id
                                                                                          && ca.Status == QCStatus.QC_routed_to_RTO
                                                                                          && ca.RouteTo.ID == route_to_id
                                                                                      )
                                                                                //.GroupBy(ca => ca.MaterialStageAudit.MaterialMasterID)
                                                                                //.SelectMany(ca => ca.Where(b => ca.Max(s => s.MaterialStageAudit.Stage.Order) == stageID))
                                                                                .Select(ca => CreateMaterialChecklist(ca))
                                                                                .ToList();



            if (materialChecklists == null || materialChecklists.Count == 0)
            {
                int currentMaterialStageID = _context.MaterialStageAudit.Where(msa => msa.MaterialMasterID == material_id).OrderByDescending(msa => msa.CreatedDate).FirstOrDefault().StageID;
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

        // POST api/<controller>
        [HttpPost]
        public void Post([FromBody]string value)
        {
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

        private MaterialChecklist CreateMaterialChecklist(ChecklistAudit checklistAudit)
        {
            return new MaterialChecklist
            {
                ChecklistID = checklistAudit.ChecklistID,
                MaterialID = checklistAudit.MaterialStageAudit.MaterialMasterID,
                MarkingNo = checklistAudit.MaterialStageAudit.MaterialMaster.MarkingNo,
                MaterialType = checklistAudit.MaterialStageAudit.MaterialMaster.MaterialType.Name,
                Block = checklistAudit.MaterialStageAudit.MaterialMaster.Block,
                Level = checklistAudit.MaterialStageAudit.MaterialMaster.Level,
                Zone = checklistAudit.MaterialStageAudit.MaterialMaster.Zone,
                SubConID = checklistAudit.MaterialStageAudit.MaterialMaster.Organisation.ID,
                SubConName = checklistAudit.MaterialStageAudit.MaterialMaster.Organisation.Name,
                ProjectID = checklistAudit.MaterialStageAudit.MaterialMaster.ProjectID,
                ProjectName = checklistAudit.MaterialStageAudit.MaterialMaster.Project.Name,
                Status = checklistAudit.Status.ToString(),
                StatusCode = (int)checklistAudit.Status
            };
        }
    }
}
