using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using astorWorkDAO;
using astorWorkShared.GlobalResponse;
using astorWorkMaterialTracking.Models;
using astorWorkMaterialTracking.Common;
using astorWorkShared.GlobalExceptions;

namespace astorWorkMaterialTracking.Controllers
{
    [Produces("application/json")]
    [Route("roles")]
    public class RoleMastersController : CommonRoleMasterController
    {
        #region Declarations

        private readonly astorWorkDbContext _context;
        private readonly string module = "Role";

        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context"></param>
        public RoleMastersController(astorWorkDbContext context) : base(context)
        {
            _context = context;
        }

        #endregion

        #region API methods


        /// <summary>

        ///  returs you list of roles
        /// </summary>

        [HttpGet]
        public async Task<List<Role>> ListRoles()
        {
            return await _context.RoleMaster
                                 .Select(r => new Role { ID = r.ID, Name = r.Name, DefaultPageName = r.DefaultPage.Name })
                                 .ToListAsync();
        }

        /// <summary>
        /// <URL>GET: /pages</URL>
        ///  returs you list of roles
        /// </summary>
        [HttpGet("pages")]
        public async Task<List<Module>> ListPages()
        {
            return await _context.ModuleMaster.Where(M => M.Name != "User Account")
                                              .Select(m => CreateModule(m.ID, m.Name, m.Pages))
                                              .ToListAsync();
        }

        /// <summary>
        /// <URL>GET: /defaultPages</URL>
        ///  returns you list of defaultPages
        /// </summary>
        [HttpGet("default-pages")]
        public List<Page> ListDefaultPages()
        {
            string[] defaultPages = new string[]{"List Materials","Dashboard","List BIM Sync Sessions","List MRFs", "Stage Master"};
            return _context.PageMaster.Where(p => defaultPages.Contains(p.Name))
                                      .Select(m => new Page{ ID = m.ID, Name = m.Name })
                                      .ToList();
        }

        /// <summary>
        /// <URL>GET: /5</URL>
        ///  returs you Role Details
        ///<param name="roleID"></param>
        /// </summary>
        [HttpGet("{id}")]
        public async Task<Role> GetRole([FromRoute] int id)
        {
            if (!ModelState.IsValid)
                throw new GenericException(ErrorMessages.BadRequest, ErrorMessages.BadRequestMsg());

            Role role = await CreateRole(id);

            if (role == null)
                throw new GenericException(ErrorMessages.DbRecordNotFound, ErrorMessages.DBRecordNotFoundMsg(module, "Role", id.ToString()));

            return role;
        }

        /// <summary>
        /// <URL>PUT: </URL>
        ///  Updates the Role Details
        ///<param name="roleID"></param>
        /// <param name="roleMaster"></param>
        /// </summary>
        
        [HttpPut("{id}")]
        public async Task PutRoleMaster([FromRoute] int id, [FromBody] Role role)
        {
            if (!ModelState.IsValid)
                throw new GenericException(ErrorMessages.BadRequest, ErrorMessages.BadRequestMsg());

            if (id != role.ID)
                throw new GenericException(ErrorMessages.DbRecordNotFound, ErrorMessages.DBRecordNotFoundMsg("Role", "ID", id.ToString()));

            RoleMaster objRoleMaster = _context.RoleMaster.Where(R => R.ID == id).FirstOrDefault();
            objRoleMaster.DefaultPage = _context.PageMaster.Where(P => P.ID == role.DefaultPageID).FirstOrDefault();

            //delete existing records from RolePageAssociation for the respective roleid
            _context.RolePageAssociation.RemoveRange(_context.RolePageAssociation.Where(RP => RP.RoleId == id));
            _context.SaveChanges();

            objRoleMaster.RolePageAssociations = GetRolePageAssociations(role, objRoleMaster.DefaultPage);
            _context.Entry(objRoleMaster).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!RoleMasterExists(id))
                    throw new GenericException(ErrorMessages.DbConcurrentUpdate, ex.Message);
                else
                    throw;
            }
        }



        /// <summary>
        /// <URL>POST:</URL>
        ///  Updates the Role Details
        /// <param name="roleMaster"></param>
        /// </summary>
        [HttpPost]
        public async Task PostRoleMaster([FromBody] Role roleDetails)
        {
            if (!ModelState.IsValid)
                throw new GenericException(ErrorMessages.BadRequest, ErrorMessages.BadRequestMsg());

            if (_context.RoleMaster.Any(R => R.Name == roleDetails.Name))
                throw new GenericException(ErrorMessages.DbDuplicateRecord, ErrorMessages.DbDuplicateRecordMsg("Role", "Rolename ", roleDetails.Name));

            RoleMaster role = CreateRoleMaster(roleDetails);

            _context.RoleMaster.Add(role);
            await _context.SaveChangesAsync();

        }

        // DELETE: api/RoleMasters/5
        [HttpDelete("{id}")]
        public async Task<RoleMaster> DeleteRoleMaster([FromRoute] int id)
        {
            if (!ModelState.IsValid)
                throw new GenericException(ErrorMessages.BadRequest, ErrorMessages.BadRequestMsg());

            RoleMaster role = await _context.RoleMaster.SingleOrDefaultAsync(m => m.ID == id);

            if (role == null)
                throw new GenericException(ErrorMessages.DbRecordNotFound, ErrorMessages.DBRecordNotFoundMsg("Role", "ID", id.ToString()));

            _context.RoleMaster.Remove(role);
            await _context.SaveChangesAsync();

            return role;
        }

        private bool RoleMasterExists(int id)
        {
            return _context.RoleMaster.Any(e => e.ID == id);
        }
        #endregion

    }
}