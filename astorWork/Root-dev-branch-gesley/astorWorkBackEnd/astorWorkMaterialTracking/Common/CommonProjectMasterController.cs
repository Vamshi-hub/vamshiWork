using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using astorWorkMaterialTracking.Models;
using astorWorkDAO;
using Microsoft.EntityFrameworkCore;
using System.Data;
using astorWorkShared.Services;
using astorWorkShared.GlobalModels;
using System.Text;
using System.IO;
using DinkToPdf.Contracts;
using astorWorkShared.Utilities;

namespace astorWorkMaterialTracking.Common
{
    public class CommonProjectMasterController : CommonController
    {
        protected IAstorWorkEmail _emailService;
        protected TenantInfo _tenant;
        protected IConverter _converter;

        public CommonProjectMasterController(astorWorkDbContext context) : base(context)
        {
        }

        

        protected Project CreateProject(ProjectMaster projectMaster, UserMaster projectManager, List<MaterialMaster> materials)
        {
            return new Project
            {
                ID = projectMaster.ID,
                Name = projectMaster.Name,
                Description = projectMaster.Description,
                StartDate = projectMaster.EstimatedStartDate,
                EndDate = projectMaster.EstimatedEndDate,
                Country = projectMaster.Country,
                TimeZoneOffset = projectMaster.TimeZoneOffset,
                ProjectManagerID = projectManager == null ? 0 : projectManager.ID,
                ProjectManagerName = projectManager == null ? string.Empty : projectManager.PersonName,
                MaterialTypes = materials.Where(m => m.MaterialType != null).Select(m => m.MaterialType.Name).Distinct().ToList(),
                Blocks = materials.Where(m => m.Block != null).OrderBy(m => m.Block.Substring(0).PadLeft(10)).Select(m => m.Block).Distinct().ToList(),
                MRFs = materials.Where(m => m.MRF != null).Select(m => m.MRF.MRFNo).Distinct().ToList()
            };
        }

        protected ProjectMaster CreateProjectMaster(Project project)
        {
            return new ProjectMaster
            {
                Name = project.Name,
                Description = project.Description,
                EstimatedStartDate = project.StartDate,
                EstimatedEndDate = project.EndDate,
                Country = project.Country,
                TimeZoneOffset = project.TimeZoneOffset
            };
        }

        protected async Task<List<ProjectMaster>> GetProjects(UserMaster user)
        {
            List<ProjectMaster> projects = _context.ProjectMaster.ToList();
            if (user != null)
                if (user.RoleID == Convert.ToInt64(Enums.RoleType.ProjectManager) || user.RoleID == Convert.ToInt64(Enums.RoleType.SiteOfficer))
                    return projects.Where(p => p == user.Project).ToList();
                else if ((user.RoleID == Convert.ToInt64(Enums.RoleType.VendorProjectManager) || user.RoleID == Convert.ToInt64(Enums.RoleType.VendorProductionOfficer)) && user.Organisation != null)
                    return await _context.MaterialMaster.Include(mm => mm.Project).Where(mm => mm.Organisation == user.Organisation).Select(mm => mm.Project).Distinct().ToListAsync();

            return projects;
        }

        protected async Task<List<Project>> GetProjects(List<ProjectMaster> projectMasters, List<Country> countries, List<UserMaster> projectManagers) {
            return projectMasters.Select(
                p => new Project
                {
                    ID = p.ID,
                    Name = p.Name,
                    StartDate = p.EstimatedStartDate,
                    EndDate = p.EstimatedEndDate,
                    Description = p.Description,
                    Country = !string.IsNullOrEmpty(p.Country) ? countries.Where(c => c.CountryCode == p.Country)
                            .Select(C => C.CountryName).FirstOrDefault().ToString() : "",
                    TimeZone = TimeSpan.FromMinutes(p.TimeZoneOffset).ToString(@"hh\:mm"),
                    ProjectManagerID = projectManagers.Where(pm => pm.Project == p).FirstOrDefault() == null ? 0 : projectManagers.Where(pm => pm.Project == p).FirstOrDefault().ID,
                    ProjectManagerName = projectManagers.Where(pm => pm.Project == p).FirstOrDefault() == null ? string.Empty : projectManagers.Where(pm => pm.Project == p).FirstOrDefault().PersonName
                }
            ).ToList();
        }

        protected async Task<UserMaster> GetProjectManager(ProjectMaster project)
        {
            return await _context.UserMaster.Where(um => um.RoleID == Convert.ToInt64(Enums.RoleType.ProjectManager) && um.Project == project).FirstOrDefaultAsync();
        }

        protected async Task<List<UserMaster>> GetProjectManagers()
        {
            return await _context.UserMaster.Include(um => um.Project).Where(um => um.RoleID == 4).ToListAsync();
        }

        protected void UpdateProject(int id, Project project) {
            ProjectMaster projectMaster = _context.ProjectMaster.Where(p => p.ID == id).FirstOrDefault();
            projectMaster.Description = project.Description;
            projectMaster.EstimatedEndDate = project.EndDate;
            projectMaster.Country = project.Country;
            projectMaster.TimeZoneOffset = project.TimeZoneOffset;
            _context.Entry(projectMaster).State = EntityState.Modified;
        }

        protected bool ProjectMasterExists(int id)
        {
            return _context.ProjectMaster.Any(e => e.ID == id);
        }
    }
}