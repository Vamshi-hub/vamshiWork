using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using astorWorkDAO;
using astorWorkShared.GlobalResponse;
using astorWorkBackEnd.Models;

namespace astorWorkBackEnd.Controllers
{
    [Produces("application/json")]
    [Route("roles")]
    public class RoleMastersController : Controller
    {
        #region Declarations

        private readonly astorWorkDbContext _context;
        private readonly string Module = "Role";

        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context"></param>
        public RoleMastersController(astorWorkDbContext context)
        {
            _context = context;
        }

        #endregion

        #region API methods


        /// <summary>

        ///  returs you list of roles
        /// </summary>

        [HttpGet]
        public APIResponse ListRoles()
        {
            return new APIResponse(0, _context.RoleMaster.Select(R => new { R.ID, R.Name, DefaultPage = R.DefaultPage.Name }));
        }



        /// <summary>
        /// <URL>GET: /pages</URL>
        ///  returs you list of roles
        /// </summary>
        [HttpGet("pages")]
        public APIResponse ListPages()
        {
            var pages = _context.ModuleMaster.Where(M => M.Name != "User Account").Select(m => new { moduleId = m.ID, moduleName = m.Name, listofpages = m.Pages.Select(p => new { pageId = p.ID, pageName = p.Name, accessLevel = "0" }) });
            return new APIResponse(0, pages);
        }



        /// <summary>
        /// <URL>GET: /defaultPages</URL>
        ///  returs you list of defaultPages
        /// </summary>
        [HttpGet("default-pages")]
        public APIResponse ListDefaultPages()
        {
            string[] lstpages = new string[]
             {
                "List Materials","Dashboard","List BIM Sync Sessions","List MRFs"
             };
            var pages = _context.PageMaster.Where(p => lstpages.Contains(p.Name)).Select(m => new { pageId = m.ID, pageName = m.Name });
            return new APIResponse(0, pages);
        }



        /// <summary>
        /// <URL>GET: /5</URL>
        ///  returs you Role Details
        ///<param name="roleID"></param>
        /// </summary>
        [HttpGet("{id}")]
        public APIResponse GetRoleMaster([FromRoute] int id)
        {
            if (!ModelState.IsValid)
                return new APIBadRequest();

            var roleMaster = _context.RoleMaster.Where(r => r.ID == id)
                .Select(r => new {
                    roleID = r.ID,
                    roleName = r.Name,
                    defaultPageId = r.DefaultPage.ID,
                    listofPages = r.RolePageAssociations.Where(rp => rp.Page.Module.Name != "User Account")
                        .Select(i => new {
                            moduleName = i.Page.Module.Name,
                            moduleId = i.Page.Module.ID,
                            pageId = i.PageId,
                            pageName = i.Page.Name,
                            accessLevel = i.AccessLevel.ToString()
                        })
                });

            if (roleMaster == null)
            {
                return new DbRecordNotFound(Module, "Role", id.ToString());
            }

            return new APIResponse(0, roleMaster);
        }



        /// <summary>
        /// <URL>PUT: </URL>
        ///  Updates the Role Details
        ///<param name="roleID"></param>
        /// <param name="roleMaster"></param>
        /// </summary>

        [HttpPut("{id}")]
        public async Task<APIResponse> PutRoleMaster([FromRoute] int id, [FromBody] RoleDetails roleDetails)
        {
            if (!ModelState.IsValid)
            {
                return new APIBadRequest();
            }

            if (id != roleDetails.RoleID)
            {
                return new APIBadRequest();
            }
            RoleMaster objRoleMaster = _context.RoleMaster.Where(R => R.ID == id).FirstOrDefault();
            objRoleMaster.DefaultPage = _context.PageMaster.Where(P => P.ID == roleDetails.DefaultPageID).FirstOrDefault();
            List<RolePageAssociation> objRolePageAssociation = new List<RolePageAssociation>();
            //delete existing records from RolePageAssociation for the respectiv roleid
            _context.RolePageAssociation.RemoveRange(_context.RolePageAssociation.Where(RP => RP.RoleId == id));
            _context.SaveChanges();
            
            objRolePageAssociation = roleDetails.ListofPages.Select(RP => new RolePageAssociation
            {
                RoleId = roleDetails.RoleID,
                Role = _context.RoleMaster.Where(R => R.ID == roleDetails.RoleID).FirstOrDefault(),
                PageId = RP.PageId,
                Page = _context.PageMaster.Where(P => P.ID == RP.PageId).FirstOrDefault(),
                AccessLevel = _context.PageMaster.Where(P => P.ID == RP.PageId).FirstOrDefault() == objRoleMaster.DefaultPage?3:Convert.ToInt32(RP.AccessLevel)
            }).ToList();
            objRoleMaster.RolePageAssociations = objRolePageAssociation;

            _context.Entry(objRoleMaster).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException exc)
            {
                if (!RoleMasterExists(id))
                    return new DbConcurrentUpdate(exc.Message);
                else
                    throw;
            }
            return new APIResponse(0, null);
        }



        /// <summary>
        /// <URL>POST:</URL>
        ///  Updates the Role Details
        /// <param name="roleMaster"></param>
        /// </summary>
        [HttpPost]
        public async Task<APIResponse> PostRoleMaster([FromBody] RoleDetails roleDetails)
        {
            if (!ModelState.IsValid)
            {
                return new APIBadRequest();
            }
            if(_context.RoleMaster.Any(R => R.Name == roleDetails.RoleName))
            {
                return new DbDuplicateRecord("Role", "Rolename ", roleDetails.RoleName);
            }
            RoleMaster objRoleMaster = new RoleMaster();
            objRoleMaster.Name = roleDetails.RoleName;
            objRoleMaster.DefaultPage = _context.PageMaster.Where(P => P.ID == roleDetails.DefaultPageID).FirstOrDefault();
            objRoleMaster.MobileEntryPoint = 0;
            objRoleMaster.PlatformCode = "01";
            List<RolePageAssociation> objRolePageAssociation = new List<RolePageAssociation>();
            objRolePageAssociation = roleDetails.ListofPages.Select(RP => new RolePageAssociation
            {
                RoleId = roleDetails.RoleID,
                Role = _context.RoleMaster.Where(R => R.ID == roleDetails.RoleID).FirstOrDefault(),
                PageId = RP.PageId,
                Page = _context.PageMaster.Where(P => P.ID == RP.PageId).FirstOrDefault(),
                AccessLevel = Convert.ToInt32(RP.AccessLevel)
            }).ToList();

            objRoleMaster.RolePageAssociations = objRolePageAssociation;

            _context.RoleMaster.Add(objRoleMaster);
            await _context.SaveChangesAsync();

            return new APIResponse(0, null);
        }

        // DELETE: api/RoleMasters/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoleMaster([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var roleMaster = await _context.RoleMaster.SingleOrDefaultAsync(m => m.ID == id);
            if (roleMaster == null)
            {
                return NotFound();
            }

            _context.RoleMaster.Remove(roleMaster);
            await _context.SaveChangesAsync();

            return Ok(roleMaster);
        }

        private bool RoleMasterExists(int id)
        {
            return _context.RoleMaster.Any(e => e.ID == id);
        }
        #endregion

    }
}