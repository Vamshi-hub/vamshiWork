using astorWorkMaterialTracking.Common;
using astorWorkMaterialTracking.Models;
using astorWorkDAO;
using astorWorkShared.GlobalExceptions;
using astorWorkShared.GlobalResponse;
using astorWorkShared.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace astorWorkMaterialTracking.Controllers
{
    [Produces("application/json")]
    [Route("organisations")]
    public class VendorMastersController : CommonVendorMasterController
    {
        public VendorMastersController(astorWorkDbContext context) : base(context)
        {
        }

        // GET: vendors
        [HttpGet]
        public async Task<List<Organisation>> ListOrganisations()
        {
            List<OrganisationMaster> organisations = await _context.OrganisationMaster.ToListAsync();
            return await CreateOrganisations(organisations);
        }

        // GET: vendors/5
        [HttpGet("{id}")]
        public async Task<Organisation> GetOrganisationDetails([FromRoute] int id)
        {
            if (!ModelState.IsValid)
                throw new GenericException(ErrorMessages.BadRequest, ErrorMessages.BadRequestMsg());

            OrganisationMaster vendor = await _context.OrganisationMaster.FindAsync(id);

            if (vendor == null)
                throw new GenericException(ErrorMessages.DbRecordNotFound, "Vendor not found!");

            List<ContactPerson> contactPeople = await GetContacts(vendor);
            List<Location> locations = await GetLocations(vendor);

            return new Organisation
            {
                ID = vendor.ID,
                Name = vendor.Name,
                CycleDays = vendor.CycleDays,
                OrganisationType = Convert.ToInt32(vendor.OrganisationType),
                OrganisationTypeName = vendor.OrganisationType.ToString(),
                ContactPeople = contactPeople,
                Locations = locations
            };
        }

        // POST: vendors
        [HttpPost]
        public async Task<OrganisationMaster> CreateNewVendor([FromBody] Organisation organisation)
        {
            OrganisationMaster organisationMaster = new OrganisationMaster();
            if (!ModelState.IsValid)
                throw new GenericException(ErrorMessages.BadRequest, ErrorMessages.BadRequestMsg());

            try
            {
                if (VendorMasterExists(organisation.ID))
                    throw new GenericException(ErrorMessages.DbDuplicateRecord, ErrorMessages.DbDuplicateRecordMsg("Vendor", "Name", organisation.Name));
                
                organisationMaster.Name = organisation.Name;
                organisationMaster.OrganisationType = (Enums.OrganisationType)organisation.OrganisationType; 
                organisationMaster.CycleDays = organisation.CycleDays;
                _context.OrganisationMaster.Add(organisationMaster);
                await _context.SaveChangesAsync();


            }
            catch (Exception ex)
            {
                throw new GenericException(ErrorMessages.BadRequest, ex.Message);
            }


            return organisationMaster;
        }

        // PUT: vendors/5
        [HttpPut("{id}")]
        public async Task<OrganisationMaster> PutVendorMaster([FromRoute] int id, [FromBody] OrganisationMaster vendor)
        {
            if (!ModelState.IsValid)
                throw new GenericException(ErrorMessages.BadRequest, ErrorMessages.BadRequestMsg());

            vendor.ID = id;
            _context.Entry(vendor).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new GenericException(ErrorMessages.DbConcurrentUpdate, ex.Message);
            }

            return vendor;
        }

        // DELETE: api/VendorMasters/5
        [HttpDelete("{id}")]
        public async Task<OrganisationMaster> DeleteVendorMaster([FromRoute] int id)
        {
            if (!ModelState.IsValid)
                throw new GenericException(ErrorMessages.BadRequest, ErrorMessages.BadRequestMsg());

            OrganisationMaster vendor = await _context.OrganisationMaster.SingleOrDefaultAsync(m => m.ID == id);
            if (vendor == null)
                throw new GenericException(ErrorMessages.DbRecordNotFound, "Vendor not found!");

            _context.OrganisationMaster.Remove(vendor);
            await _context.SaveChangesAsync();

            return vendor;
        }
    }
}