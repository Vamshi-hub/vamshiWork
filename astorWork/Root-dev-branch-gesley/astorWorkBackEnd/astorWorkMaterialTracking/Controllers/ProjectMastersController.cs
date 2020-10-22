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
using astorWorkMaterialTracking.Common;
using astorWorkMaterialTracking.Models;
using System.Security.Claims;
using astorWorkShared.Utilities;
using astorWorkShared.GlobalExceptions;

namespace astorWorkMaterialTracking.Controllers
{
    [Produces("application/json")]
    [Route("projects")]
    public class ProjectMastersController : CommonProjectMasterController
    {
        private static readonly string module = "Project";

        public ProjectMastersController(astorWorkDbContext context) : base(context)
        {
        }

        /// <summary>
        ///     Used in the following page filters: List Materials, MRF and Job Schedule
        ///     Used in the following dropdown selection: Create MRF
        /// </summary>
        // GET /projects/{id}&filter={filter}
        [HttpGet("{id}")]
        public async Task<Project> GetProject([FromRoute] int id, [FromQuery] string filter = "")
        {
            if (!ModelState.IsValid)
                throw new GenericException(ErrorMessages.BadRequest, ErrorMessages.BadRequestMsg());

            ProjectMaster projectMaster = await _context.ProjectMaster.SingleOrDefaultAsync(m => m.ID == id);

            if (projectMaster == null)
                throw new GenericException(ErrorMessages.DbRecordNotFound, ErrorMessages.DBRecordNotFoundMsg(module, "ID", id.ToString()));

            List<MaterialMaster> materials = await GetMaterials(projectMaster, filter);

            if (materials == null)
                throw new GenericException(ErrorMessages.DbRecordNotFound, ErrorMessages.DBRecordNotFoundMsg("Materials", "Project ID", projectMaster.ID.ToString()));

            UserMaster projectManager = await GetProjectManager(projectMaster);

            return CreateProject(projectMaster, projectManager, materials); ;
        }

        protected async Task<List<MaterialMaster>> GetMaterials(ProjectMaster project, string filter)
        {
            try
            {
                return await _context.MaterialMaster.Where(m => m.Project.Equals(project) && ((filter == "mrf")?(m.MRF == null):true)).Include(m => m.MRF).ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        // GET: /projects
        [HttpGet]
        public async Task<List<Project>> ListProjects()
        {
            if (_context.ProjectMaster == null || !_context.ProjectMaster.Any())
                throw new GenericException(ErrorMessages.DbRecordNotFound, "No projects found");

            UserMaster user = await _context.GetUserFromHttpContext(HttpContext);
            List<ProjectMaster> projectMasters = await GetProjects(user);
            List<UserMaster> projectManagers = await GetProjectManagers();
            List<Country> countries = Country.GetCountries();

            return await GetProjects(projectMasters, countries, projectManagers);
        }

        // GET: projects/{project_id}/materials/types
        [HttpGet("{id}/material-types")]
        public async Task<List<string>> ListMaterialTypes([FromRoute] int id)
        {
            if (!ModelState.IsValid)
                throw new GenericException(ErrorMessages.BadRequest, ErrorMessages.BadRequestMsg());

            List<string> materialTypes = await _context.MaterialTypeMaster.Select(m => m.Name).Distinct().ToListAsync();

            if (materialTypes == null)
                throw new GenericException(ErrorMessages.DbRecordNotFound, "No projects found");

            return materialTypes;
        }

        // POST: /projects
        [HttpPost]
        public async Task CreateProject([FromBody] Project project)
        {
            if (!ModelState.IsValid)
                throw new GenericException(ErrorMessages.BadRequest, ErrorMessages.BadRequestMsg());

            if (_context.ProjectMaster.Any(P => P.Name == project.Name))
                throw new GenericException(ErrorMessages.DbDuplicateRecord, ErrorMessages.DbDuplicateRecordMsg("Project", "project name ", project.Name));

            _context.ProjectMaster.Add(CreateProjectMaster(project));
            await _context.SaveChangesAsync();
        }

        // PUT: /projects
        [HttpPut("{id}")]
        public async Task EditProject([FromRoute] int id, [FromBody] Project project)
        {
            if (!ModelState.IsValid)
                throw new GenericException(ErrorMessages.BadRequest, ErrorMessages.BadRequestMsg());

            if (id != project.ID)
                throw new GenericException(ErrorMessages.DbRecordNotFound, "No project found!");

            UpdateProject(id, project);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!ProjectMasterExists(id))
                    throw new GenericException(ErrorMessages.DbConcurrentUpdate, ex.Message);
                else
                    throw;
            }
        }

        // DELETE: /projects
        [HttpDelete("{id}")]
        public async Task<ProjectMaster> DeleteProjectMaster([FromRoute] int id)
        {
            if (!ModelState.IsValid)
                throw new GenericException(ErrorMessages.BadRequest, ErrorMessages.BadRequestMsg());

            ProjectMaster projectMaster = await _context.ProjectMaster.SingleOrDefaultAsync(m => m.ID == id);
            if (projectMaster == null)
                throw new GenericException(ErrorMessages.DbRecordNotFound, "No project found!");

            _context.ProjectMaster.Remove(projectMaster);
            await _context.SaveChangesAsync();

            return projectMaster;
        }
    }
}