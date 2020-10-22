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
    [Route("locations")]
    public class LocationMastersController : Controller
    {
        private astorWorkDbContext _context;
        private readonly string module = "Location";

        public LocationMastersController(astorWorkDbContext context)
        {
            _context = context;
        }

        // GET: locations
        [HttpGet]
        public async Task<IEnumerable<Location>> ListLocations([FromQuery] int? user_id)
        {
            if (user_id.HasValue)
                return await GetLocations(user_id.Value);
            else
                return GetLocations();
        }

        // GET: locations/5
        [HttpGet("{id}")]
        public async Task<Location> GetLocationDetails([FromRoute] int id)
        {
            if (!ModelState.IsValid)
                throw new GenericException(ErrorMessages.BadRequest, ErrorMessages.BadRequestMsg());

            LocationMaster locationMaster = await GetLocation(id);

            if (locationMaster == null)
                throw new GenericException(ErrorMessages.DbRecordNotFound, ErrorMessages.DBRecordNotFoundMsg(module, "ID", id.ToString()));

            return CreateLocation(locationMaster);
        }

        // PUT: api/LocationMasters/5
        [HttpPut("{id}")]
        public async Task<Location> EditLocation([FromRoute] int id, [FromBody] Location location)
        {
            if (!ModelState.IsValid)
                throw new GenericException(ErrorMessages.BadRequest, ErrorMessages.BadRequestMsg());

            //LocationMaster locationMaster = CreateLocationMaster(location, id);
            //_context.Entry(locationMaster).State = EntityState.Modified;

            LocationMaster locationMaster = await _context.LocationMaster.Include(l => l.Site).Include(l => l.Organisation).Where(l => l.ID == id).FirstOrDefaultAsync();

            if (LocationMasterExists(id, location.Name))
                    throw new GenericException(ErrorMessages.DbDuplicateRecord, ErrorMessages.DbDuplicateRecordMsg(module, "name", location.Name.ToString()));

            if (locationMaster != null)
            {
                locationMaster = await UpdateLocationMaster(locationMaster, location);

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (Exception)
                {
                    throw new GenericException(ErrorMessages.DbConcurrentUpdate, "Database concurrent update error!");
                }
            }
            else
                throw new GenericException(ErrorMessages.DbRecordNotFound, ErrorMessages.DBRecordNotFoundMsg(module, "ID", id.ToString()));

            return location;
        }

        // POST: locations
        [HttpPost]
        public async Task<Location> CreateLocation([FromBody] Location location)
        {
            if (!ModelState.IsValid)
                throw new GenericException(ErrorMessages.BadRequest, ErrorMessages.BadRequestMsg());

            if (LocationMasterExists(location.Name))
                throw new GenericException(ErrorMessages.DbDuplicateRecord, ErrorMessages.DbDuplicateRecordMsg(module, "name", location.Name.ToString()));

            LocationMaster locationMaster = await CreateLocationMaster(location);
            
            _context.LocationMaster.Add(locationMaster);
            await _context.SaveChangesAsync();

            return location;
        }

        // DELETE: locations/5
        [HttpDelete("{id}")]
        public async Task<LocationMaster> DeleteLocation([FromRoute] int id)
        {
            if (!ModelState.IsValid)
                throw new GenericException(ErrorMessages.BadRequest, ErrorMessages.BadRequestMsg());

            LocationMaster location = await _context.LocationMaster.FindAsync(id);
            if (location == null)
                throw new GenericException(ErrorMessages.DbRecordNotFound, ErrorMessages.DBRecordNotFoundMsg(module, "ID", id.ToString()));

            _context.LocationMaster.Remove(location);
            await _context.SaveChangesAsync();

            return location;
        }

        protected IEnumerable<Location> GetLocations()
        {
            return _context.LocationMaster.Select(l => new Location
            {
                ID = l.ID,
                Name = l.Name,
                Description = l.Description,
                Type = l.Type,
                SiteName = l.Site.Name
            });
        }


        protected async Task<List<Location>> GetLocations(int userID)
        {
            UserMaster user = await GetUser(userID);

            List<LocationMaster> locations = GetLocations(user);

            //if (locations.Count() == 0)
            //    return new DbRecordNotFound("No Location Found !!");

            return locations.Select(l => new Location
            {
                ID = l.ID,
                Name = l.Name,
                Description = l.Description,
                Type = l.Type,
                SiteName = l.Site.Name
            }).ToList();
        }

        protected async Task<UserMaster> GetUser(int userID)
        {
            return await _context.UserMaster.Include(um => um.Role)
                .Include(um => um.Site).ThenInclude(s => s.Locations)
                .Where(U => U.ID == userID).FirstOrDefaultAsync();
        }

        protected async Task<LocationMaster> GetLocation(int id)
        {
            return await _context.LocationMaster
                           .Include(L => L.Site)
                           .Where(l => l.ID == id)
                           .FirstOrDefaultAsync();
        }

        protected List<LocationMaster> GetLocations(UserMaster user)
        {
            List<LocationMaster> locations = new List<LocationMaster>();
            if (user != null)
                // Only Site officer or Production officer has location for mobile app
                if (user.Role.ID == 5 || user.Role.ID == 8)
                    locations = user.Site.Locations;

            return locations;
        }

        protected Location CreateLocation(LocationMaster locationMaster)
        {
            return new Location
            {
                ID = locationMaster.ID,
                Name = locationMaster.Name,
                Description = locationMaster.Description,
                Type = locationMaster.Type,
                SiteID = locationMaster.Site == null ? 0 : locationMaster.Site.ID
            };
        }

        protected async Task<LocationMaster> CreateLocationMaster(Location location, int id = 0)
        {
            LocationMaster locationMaster = null;
            if (id > 0)
                locationMaster = _context.LocationMaster.Find(id);
            else
                locationMaster = new LocationMaster();

            if (locationMaster != null)
                await UpdateLocationMaster(locationMaster, location);

            return locationMaster;
        }

        protected async Task<LocationMaster> UpdateLocationMaster(LocationMaster locationMaster, Location location)
        {
            locationMaster.Name = location.Name;
            locationMaster.Type = location.Type;
            locationMaster.Description = location.Description;
            locationMaster.Site = _context.SiteMaster.Include(l => l.Organisation).Where(S => S.ID == location.SiteID).FirstOrDefault();
            locationMaster.Organisation = (location.Type != 0) ? null : await GetVendor(locationMaster.Site.Organisation.ID);

            return locationMaster;
        }

        protected async Task<OrganisationMaster> GetVendor(int id)
        {
            return await _context.OrganisationMaster.FindAsync(id);
        }

        protected bool LocationMasterExists(int id)
        {
            return _context.LocationMaster.Any(e => e.ID == id);
        }

        protected bool LocationMasterExists(string name)
        {
            return _context.LocationMaster.Any(l => l.Name == name);
        }

        protected bool LocationMasterExists(int id, string name)
        {
            return _context.LocationMaster.Any(l => l.ID != id && l.Name == name);
        }
    }
}