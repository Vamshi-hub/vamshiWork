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
    public class CommonVendorMasterController:CommonController
    {
        protected IAstorWorkEmail _emailService;
        protected TenantInfo _tenant;
        protected IConverter _converter;
        protected IAstorWorkBlobStorage _blobStorage;

        public CommonVendorMasterController(astorWorkDbContext context) : base(context)
        {
        }

        protected async Task<Organisation> SetContacts(Organisation vendor)
        {
            List<UserMaster> contactPeople = await _context.UserMaster
                                                           .Where(u => u.Organisation.ID == vendor.ID 
                                                                    && u.IsActive 
                                                                    && u.RoleID == Convert.ToInt32(Enums.RoleType.VendorProjectManager))
                                                           .ToListAsync();

            if (contactPeople.Count() > 0)
                vendor.ContactPeople = new List<ContactPerson>();

            foreach (UserMaster user in contactPeople)
            {
                ContactPerson contactPerson = new ContactPerson();
                contactPerson.ID = user.ID;
                contactPerson.Name = user.PersonName;
                vendor.ContactPeople.Add(contactPerson);
            }

            return vendor;
        }

        protected async Task<List<Organisation>> CreateOrganisations(List<OrganisationMaster> vendorMasters) {
            List<Organisation> organisations = new List<Organisation>();

            foreach (OrganisationMaster vendorMaster in vendorMasters)
                organisations.Add(await CreateOrganisation(vendorMaster));

            return organisations;
        }

        protected async Task<Organisation> CreateOrganisation(OrganisationMaster organisationMaster) {
            Organisation organisation = new Organisation();
            organisation.ID = organisationMaster.ID;
            organisation.Name = organisationMaster.Name;
            organisation.CycleDays = organisationMaster.CycleDays;
            organisation.OrganisationType = Convert.ToInt32(organisationMaster.OrganisationType);
            organisation.OrganisationTypeName = organisationMaster.OrganisationType.ToString();
            return await SetContacts(organisation);
        }

        protected async Task<List<ContactPerson>> GetContacts(OrganisationMaster vendor) {
            return await _context.UserMaster.Where(um => um.Organisation == vendor)
                .Select(um => new ContactPerson { ID = um.ID, Name = um.PersonName }).ToListAsync();
        }

        protected async Task<List<Location>> GetLocations(OrganisationMaster vendor) {
            return await _context.LocationMaster.Where(lm => lm.Organisation == vendor)
                .Select(lm => new Location { ID = lm.ID, Name = lm.Name }).ToListAsync();
        }

        //protected bool VendorMasterExists(int id)
        //{
        //    return _context.OrganisationMaster.Any(e => e.ID == id);
        //}

        protected bool VendorMasterExists(string name)
        {
            return _context.OrganisationMaster.Any(v => v.Name == name);
        }
    }
}