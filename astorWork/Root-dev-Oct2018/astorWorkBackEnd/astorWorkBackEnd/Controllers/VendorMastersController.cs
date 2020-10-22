using astorWorkBackEnd.Common;
using astorWorkBackEnd.Models;
using astorWorkDAO;
using astorWorkShared.GlobalResponse;
using astorWorkShared.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace astorWorkBackEnd.Controllers
{
    [Produces("application/json")]
    [Route("vendors")]
    public class VendorMastersController : CommonController
    {
        public VendorMastersController(astorWorkDbContext context) : base(context)
        {
        }

        // GET: vendors
        [HttpGet]
        public APIResponse ListVendors()
        {
            List<Vendor> vendors = new List<Vendor>();

            IEnumerable<VendorMaster> vendorMasters = _context.VendorMaster;

            foreach (VendorMaster v in vendorMasters)
            {
                Vendor vendor = new Vendor();
                vendor.ID = v.ID;
                vendor.Name = v.Name;
                vendor.CycleDays = v.CycleDays;

                IEnumerable<UserMaster> contactPeople = _context.UserMaster.Where(u => u.Vendor.ID == v.ID && u.IsActive && u.RoleID == Convert.ToInt32(Enums.RoleType.VendorProjectManager));

                if (contactPeople.Count() > 0)
                {
                    vendor.ContactPeople = new List<ContactPerson>();
                }

                foreach (UserMaster u in contactPeople)
                {
                    ContactPerson contactPerson = new ContactPerson();
                    contactPerson.ID = u.ID;
                    contactPerson.Name = u.PersonName;
                    vendor.ContactPeople.Add(contactPerson);
                }

                vendors.Add(vendor);
            }

            return new APIResponse(0, vendors);
        }

        // GET: vendors/5
        [HttpGet("{id}")]
        public async Task<APIResponse> GetVendorDetails([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return new APIBadRequest();
            }

            var vendor = await _context.VendorMaster.FindAsync(id);

            if (vendor == null)
            {
                return new DbRecordNotFound("Vendor", "Vendor ID", id.ToString());
            }

            var contactPeople = _context.UserMaster.Where(um => um.Vendor == vendor)
                .Select(um => new { um.ID, um.PersonName });

            var locations = _context.LocationMaster.Where(lm => lm.Vendor == vendor)
                .Select(lm => new { lm.ID, lm.Name });

            var result = new
            {
                vendor.ID,
                vendor.Name,
                vendor.CycleDays,
                contactPeople,
                locations
            };

            return new APIResponse
            {
                Status = 0,
                Data = result
            };
        }

        // POST: vendors
        [HttpPost]
        public async Task<APIResponse> CreateVendor([FromBody] VendorMaster vendorMaster)
        {
            if (!ModelState.IsValid)
            {
                return new APIBadRequest();
            }

            try
            {
                if (VendorMasterExists(vendorMaster.Name))
                {
                    return new DbDuplicateRecord("Vendor", "Name", vendorMaster.Name);
                }
                else
                {
                    _context.VendorMaster.Add(vendorMaster);
                    await _context.SaveChangesAsync();
                }

            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                return new APIBadRequest();
            }


            return new APIResponse
            {
                Status = 0,
                Data = vendorMaster
            };
        }

        // PUT: vendors/5
        [HttpPut("{id}")]
        public async Task<APIResponse> PutVendorMaster([FromRoute] int id, [FromBody] VendorMaster vendor)
        {
            if (!ModelState.IsValid)
            {
                return new APIBadRequest();
            }

            vendor.ID = id;
            _context.Entry(vendor).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException exc)
            {
                return new DbConcurrentUpdate(exc.Message);
            }

            return new APIResponse
            {
                Status = 0,
                Data = vendor
            };
        }

        // DELETE: api/VendorMasters/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVendorMaster([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var vendorMaster = await _context.VendorMaster.SingleOrDefaultAsync(m => m.ID == id);
            if (vendorMaster == null)
            {
                return NotFound();
            }

            _context.VendorMaster.Remove(vendorMaster);
            await _context.SaveChangesAsync();

            return Ok(vendorMaster);
        }

        private bool VendorMasterExists(int id)
        {
            return _context.VendorMaster.Any(e => e.ID == id);
        }

        private bool VendorMasterExists(string vendorName)
        {
            return _context.VendorMaster.Any(v => v.Name == vendorName);
        }
    }
}