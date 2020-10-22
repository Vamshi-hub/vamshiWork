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
using astorWorkBackEnd.Common;

namespace astorWorkBackEnd.Controllers
{
    [Produces("application/json")]
    [Route("locations")]
    public class LocationMastersController : CommonLocationController
    {
        #region Declarations
        private readonly string Module = "Location";

        public LocationMastersController(astorWorkDbContext context) : base(context)
        {
        }

        #endregion

        // GET: locations
        [HttpGet]
        public APIResponse ListLocations([FromQuery] int? user_id)
        {
            if (user_id.HasValue)
            {
                APIResponse apiResponse = GetLocationsByUser(user_id.Value);
                return apiResponse;
            }
            else
            {
                var locations = _context.LocationMaster
                                .Select(l => new
                                {
                                    l.ID,
                                    l.Name,
                                    l.Description,
                                    l.Type,
                                    siteName = l.Site.Name
                                });
                return new APIResponse(0, locations);
            }
        }

        // GET: locations/5
        [HttpGet("{id}")]
        public APIResponse GetLocationDetails([FromRoute] int id)
        {
            if (!ModelState.IsValid)
                return new APIBadRequest();

            LocationMaster locationMaster = _context.LocationMaster.Include(L => L.Site)
                                 .Where(l => l.ID == id)
                                 .FirstOrDefault();

            var location = new
            {
                id = locationMaster.ID,
                name = locationMaster.Name,
                description = locationMaster.Description,
                type = locationMaster.Type,
                siteId = locationMaster.Site == null? 0:locationMaster.Site.ID
            };

            if (locationMaster == null)
                return new DbRecordNotFound("No Location Found!");

            return new APIResponse(0, location);
        }

        // PUT: api/LocationMasters/5
        [HttpPut("{id}")]
        public async Task<APIResponse> EditLocation([FromRoute] int id, [FromBody] Location location)
        {
            if (!ModelState.IsValid)
                return new APIBadRequest();

            //LocationMaster locationMaster = CreateLocationMaster(location, id);
            //_context.Entry(locationMaster).State = EntityState.Modified;

            var locationMaster = await _context.LocationMaster.Include(l => l.Site).Include(l => l.Vendor).Where(l => l.ID == id).FirstOrDefaultAsync();
            if (locationMaster != null)
            {
                locationMaster.Name = location.Name;
                locationMaster.Type = location.Type;
                locationMaster.Description = location.Description;
                locationMaster.Site = _context.SiteMaster.Include(S => S.Vendor).Where(S => S.ID == location.siteID).FirstOrDefault();
                if (location.Type != 0)
                    locationMaster.Vendor = null;
                else
                    locationMaster.Vendor = await _context.VendorMaster.FindAsync(locationMaster.Site.Vendor.ID);

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (Exception exc)
                {
                    return new DbConcurrentUpdate(exc.Message);
                }
            }
            else
                return new DbRecordNotFound("No Location Found!");

            return new APIResponse(0, location);
        }

        // POST: locations
        [HttpPost]
        public async Task<APIResponse> CreateLocation([FromBody] Location location)
        {
            if (!ModelState.IsValid)
                return new APIBadRequest();

            if (LocationMasterExists(location.Name))
                return new DbDuplicateRecord("Location", "Name", location.Name);

            LocationMaster locationMaster = CreateLocationMaster(location);
            
            _context.LocationMaster.Add(locationMaster);
            await _context.SaveChangesAsync();

            return new APIResponse(0, location);
        }

        // DELETE: locations/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLocation([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var locationMaster = await _context.LocationMaster.FindAsync(id);
            if (locationMaster == null)
            {
                return NotFound();
            }

            _context.LocationMaster.Remove(locationMaster);
            await _context.SaveChangesAsync();

            return Ok(locationMaster);
        }
    }
}