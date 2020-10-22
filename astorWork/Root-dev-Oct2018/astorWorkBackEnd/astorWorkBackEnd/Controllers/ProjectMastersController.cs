using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using astorWorkDAO;
using Microsoft.AspNetCore.Authorization;
using astorWorkShared.GlobalResponse;
using astorWorkBackEnd.Common;
using astorWorkBackEnd.Models;
using System.Security.Claims;

namespace astorWorkBackEnd.Controllers
{
    [Produces("application/json")]
    [Route("projects")]
    public class ProjectMastersController : CommonController
    {
        private static readonly string Module = "Project";

        public ProjectMastersController(astorWorkDbContext context) : base(context)
        {
        }

        // GET /projects/{id}
        [HttpGet("{id}")]
        public async Task<APIResponse> GetProjectInformation([FromRoute] int id)
        {
            if (!ModelState.IsValid)
                return new APIBadRequest();

            var projectMaster = await _context.ProjectMaster.SingleOrDefaultAsync(m => m.ID == id);

            if (projectMaster == null)
                return new DbRecordNotFound("Project", "Id", id.ToString());

            var materials = _context.MaterialMaster.Where(m => m.Project.Equals(projectMaster)).Include(m => m.MRF).ToList();

            if (materials == null)
                return new DbRecordNotFound("Materials", "Project ID", projectMaster.ID.ToString());

            var projectManager = await _context.UserMaster.Where(um => um.RoleID == 4 && um.Project == projectMaster).FirstOrDefaultAsync();
            
            var blocks = materials.Where(m => m.Block != null).OrderBy(m => m.Block.Substring(0).PadLeft(10)).Select(m => m.Block).Distinct();
            var materialTypes = materials.Where(m => m.MaterialType != null).Select(m => m.MaterialType).Distinct();
            var mrfs = materials.Where(m => m.MRF != null).Select(m => m.MRF.MRFNo).Distinct();
            var result = new
            {
                id = projectMaster.ID,
                name = projectMaster.Name,
                description = projectMaster.Description,
                startDate = projectMaster.EstimatedStartDate,
                endDate = projectMaster.EstimatedEndDate,
                country = projectMaster.Country,
                timeZoneOffset = projectMaster.TimeZoneOffset,
                projectManagerID = projectManager == null ? 0 : projectManager.ID,
                projectManagerName = projectManager == null ?
                string.Empty : projectManager.PersonName,
                materialTypes,
                blocks,
                mrfs
            };

            return new APIResponse(0, result);
        }

        // GET: /projects
        [HttpGet]
        public APIResponse ListProjects()
        {
            if (_context.ProjectMaster == null || !_context.ProjectMaster.Any())
                return new APIResponse(1, null, "No projects found");

            var user = _context.GetUserFromHttpContext(HttpContext);

            var projects = _context.ProjectMaster.ToList();
            if (user != null)
            {
                if (user.RoleID == 4 || user.RoleID == 5)
                    projects = projects.Where(p => p == user.Project).ToList();
                else if ((user.RoleID == 7 || user.RoleID == 8) && user.Vendor != null)
                {
                    projects = _context.MaterialMaster.Include(mm => mm.Project).Where(mm => mm.Vendor == user.Vendor).Select(mm => mm.Project).Distinct().ToList();
                }
            }

            var projectManagers = _context.UserMaster.Include(um => um.Project)
                .Where(um => um.RoleID == 4);
            var countries = Country.GetCountryList();
            var listProjectDetails = projects.Select(
                P => new
                {
                    P.ID,
                    P.Name,
                    StartDate = P.EstimatedStartDate,
                    EndDate = P.EstimatedEndDate,
                    P.Description,
                    Country = !string.IsNullOrEmpty(P.Country) ? countries.Where(C => C.CountryCode == P.Country)
                    .Select(C => C.CountryName).FirstOrDefault().ToString() : "",
                    P.TimeZoneOffset,
                    ProjectManagerID = projectManagers.Where(pm => pm.Project == P).FirstOrDefault() == null ? 0 : projectManagers.Where(pm => pm.Project == P).FirstOrDefault().ID,
                    ProjectManagerName = projectManagers.Where(pm => pm.Project == P).FirstOrDefault() == null ? string.Empty : projectManagers.Where(pm => pm.Project == P).FirstOrDefault().PersonName
                }).ToList();
            return new APIResponse(0, listProjectDetails);
        }

        // GET: projects/{project_id}/materials/types
        [HttpGet("{id}/material-types")]
        public APIResponse ListMaterialTypes([FromRoute] int id)
        {
            if (!ModelState.IsValid)
                return new APIBadRequest();

            // Retrieve materials types
            IEnumerable<string> materialTypes = _context.MaterialMaster.Select(m => m.MaterialType).Distinct();

            if (materialTypes == null)
                return new DbRecordNotFound("", "", "", "No Material Types found");

            return new APIResponse(0, materialTypes);
        }

        // POST: /projects
        [HttpPost]
        public async Task<APIResponse> CreateProject([FromBody] ProjectDetails projectMaster)
        {
            if (!ModelState.IsValid)
            {
                return new APIBadRequest();
            }
            if (_context.ProjectMaster.Any(P => P.Name == projectMaster.Name))
            {
                return new DbDuplicateRecord("Project", "project name ", projectMaster.Name);
            }
            ProjectMaster objProjectMaster = new ProjectMaster();
            objProjectMaster.Name = projectMaster.Name;
            objProjectMaster.Description = projectMaster.Description;
            objProjectMaster.EstimatedStartDate = projectMaster.StartDate;
            objProjectMaster.EstimatedEndDate = projectMaster.EndDate;
            objProjectMaster.Country = projectMaster.country;
            objProjectMaster.TimeZoneOffset = projectMaster.TimeZoneOffset;

            _context.ProjectMaster.Add(objProjectMaster);
            await _context.SaveChangesAsync();

            return new APIResponse(0, null);
        }

        // PUT: /projects
        [HttpPut("{id}")]
        public async Task<APIResponse> EditProject([FromRoute] int id, [FromBody] ProjectDetails projectMaster)
        {
            if (!ModelState.IsValid)
            {
                return new APIBadRequest();
            }

            if (id != projectMaster.ID)
            {
                return new APIBadRequest();
            }
            ProjectMaster objProjectMaster = _context.ProjectMaster.Where(P => P.ID == id).FirstOrDefault();
            objProjectMaster.Description = projectMaster.Description;
            objProjectMaster.EstimatedEndDate = projectMaster.EndDate;
            objProjectMaster.Country = projectMaster.country;
            objProjectMaster.TimeZoneOffset = projectMaster.TimeZoneOffset;

            _context.Entry(objProjectMaster).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException exc)
            {
                if (!ProjectMasterExists(id))
                {
                    return new DbConcurrentUpdate(exc.Message);
                }
                else
                {
                    throw;
                }
            }
            return new APIResponse(0, null);
        }

        // DELETE: /projects
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProjectMaster([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var projectMaster = await _context.ProjectMaster.SingleOrDefaultAsync(m => m.ID == id);
            if (projectMaster == null)
            {
                return NotFound();
            }

            _context.ProjectMaster.Remove(projectMaster);
            await _context.SaveChangesAsync();

            return Ok(projectMaster);
        }

        protected bool ProjectMasterExists(int id)
        {
            return _context.ProjectMaster.Any(e => e.ID == id);
        }
    }
}