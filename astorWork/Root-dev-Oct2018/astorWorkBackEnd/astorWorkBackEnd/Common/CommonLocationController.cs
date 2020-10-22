using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using astorWorkBackEnd.Models;
using astorWorkDAO;
using Microsoft.EntityFrameworkCore;
using astorWorkShared.GlobalResponse;

namespace astorWorkBackEnd.Common
{
    public class CommonLocationController : CommonController
    {
        public CommonLocationController(astorWorkDbContext context) : base(context)
        {
        }

        protected APIResponse GetLocationsByUser(int userId)
        {
            List<LocationMaster> locations = new List<LocationMaster>();

            var user = _context.UserMaster.Include(um => um.Role)
                .Include(um => um.Site).ThenInclude(s => s.Locations)
                .Where(U => U.ID == userId).FirstOrDefault();

            if (user != null)
            {
                // Only Site officer or Production officer has location for mobile app
                if (user.Role.ID == 5 || user.Role.ID == 8)
                {
                    locations = user.Site.Locations;
                }
            }

            if (locations.Count == 0)
                return new DbRecordNotFound("No Location Found !!");

            return new APIResponse(0,
                                locations.Select(l => new
                                {
                                    l.ID,
                                    l.Name,
                                    l.Description,
                                    l.Type,
                                    siteName = l.Site.Name
                                })
                                );
        }

        protected LocationMaster CreateLocationMaster(Location location, int id = 0)
        {
            LocationMaster locationMaster = null;
            if (id > 0)
                locationMaster = _context.LocationMaster.Find(id);
            else
                locationMaster = new LocationMaster();

            if (locationMaster != null)
            {
                locationMaster.Name = location.Name;
                locationMaster.Description = location.Description;
                locationMaster.Type = location.Type;
                locationMaster.Site = _context.SiteMaster.Include(l => l.Vendor).Where(S => S.ID == location.siteID).FirstOrDefault();
                if (location.Type != 0)
                    locationMaster.Vendor = null;
                else
                    locationMaster.Vendor = GetVendor(locationMaster.Site.Vendor.ID);
            }

            return locationMaster;
        }

        protected VendorMaster GetVendor(int id)
        {
            return _context.VendorMaster.Where(v => v.ID == id).FirstOrDefault();
        }

        protected bool LocationMasterExists(int id)
        {
            return _context.LocationMaster.Any(e => e.ID == id);
        }

        protected bool LocationMasterExists(string name)
        {
            return _context.LocationMaster.Any(l => l.Name == name);
        }
    }
}
