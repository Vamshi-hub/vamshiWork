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
    public class CommonRoleMasterController : CommonController
    {
        protected IAstorWorkEmail _emailService;
        protected TenantInfo _tenant;
        protected IConverter _converter;
        protected IAstorWorkBlobStorage _blobStorage;

        public CommonRoleMasterController(astorWorkDbContext context) : base(context)
        {
        }

        protected Module CreateModule(int id, string name, List<PageMaster> pages)
        {
            return new Module
            {
                ID = id,
                Name = name,
                Pages = pages.Select(p => CreatePage(p.ID, p.Name)).ToList()
            };
        }

        protected Page CreatePage(int id, string name)
        {
            return new Page
            {
                ID = id,
                Name = name,
                AccessLevel = "0"
            };
        }

        protected RolePageAssociation CreateRolePageAssociation(Role role, Page page, RoleMaster roleMaster)
        {
            return null;
        }

        protected async Task<Role> CreateRole(int id)
        {
            return await _context.RoleMaster.Where(r => r.ID == id)
                .Select(r => new Role
                {
                    ID = r.ID,
                    Name = r.Name,
                    DefaultPageID = r.DefaultPage.ID,
                    Pages = r.RolePageAssociations.Where(rp => rp.Page.Module.Name != "User Account")
                             .Select(i => new Page
                             {
                                 ID = i.Page.ID,
                                 Name = i.Page.Name,
                                 AccessLevel = i.AccessLevel.ToString(),
                                 ModuleID = i.Page.Module.ID,
                                 ModuleName = i.Page.Module.Name
                             }).ToList()
                }).FirstOrDefaultAsync();
        }

        protected RoleMaster CreateRoleMaster(Role roleDetails) {
            RoleMaster roleMaster = new RoleMaster();
            roleMaster.Name = roleDetails.Name;
            roleMaster.DefaultPage = _context.PageMaster.Where(P => P.ID == roleDetails.DefaultPageID).FirstOrDefault();
            roleMaster.MobileEntryPoint = 0;
            roleMaster.PlatformCode = "01";
            roleMaster.RolePageAssociations = GetRolePageAssociations(roleDetails);

            return roleMaster;
        }

        protected List<RolePageAssociation> GetRolePageAssociations(Role roleDetails) {
            return roleDetails.Pages.Select(rp => new RolePageAssociation
            {
                RoleId = roleDetails.ID,
                Role = _context.RoleMaster.Where(r => r.ID == roleDetails.ID).FirstOrDefault(),
                PageId = rp.ID,
                Page = _context.PageMaster.Where(p => p.ID == rp.ID).FirstOrDefault(),
                AccessLevel = Convert.ToInt32(rp.AccessLevel)
            }).ToList();
        }

        protected List<RolePageAssociation> GetRolePageAssociations(Role role, PageMaster defaultPage) {
            return role.Pages.Select(RP => new RolePageAssociation
            {
                RoleId = role.ID,
                Role = _context.RoleMaster.Where(R => R.ID == role.ID).FirstOrDefault(),
                PageId = RP.ID,
                Page = _context.PageMaster.Where(P => P.ID == RP.ID).FirstOrDefault(),
                AccessLevel = _context.PageMaster.Where(P => P.ID == RP.ID).FirstOrDefault() == defaultPage ? 3 : Convert.ToInt32(RP.AccessLevel)
            }).ToList();
        }
    }
}