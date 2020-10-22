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
using astorWorkShared.GlobalExceptions;
using astorWorkShared.GlobalResponse;

namespace astorWorkMaterialTracking.Common
{
    public class CommonSiteMasterController : CommonController
    {
        public CommonSiteMasterController(astorWorkDbContext context) : base(context)
        {
        }

        protected async Task<List<Site>> GetSites() {
            List<Country> countries = Country.GetCountries();

            return await _context.SiteMaster.Include(S => S.Organisation).Select(s => new Site
            {
                ID = s.ID,
                Name = s.Name,
                OrganisationID = s.Organisation != null ? s.Organisation.ID : 0,
                OrganisationName = s.Organisation != null ? s.Organisation.Name : "",
                Country = GetCountry(s, countries),
                Description = s.Description
            }).ToListAsync();
        }

        protected string GetCountry(SiteMaster site, List<Country> countries) {
            return !string.IsNullOrEmpty(site.Country) ? countries.Where(c => c.CountryCode == site.Country)
                                                                      .Select(c => c.CountryName)
                                                                      .FirstOrDefault().ToString() : "";
        }

        protected async Task<Site> GetSite(int id) {
            return await _context.SiteMaster
                                 .Include(s => s.Organisation)
                                 .Where(S => S.ID == id)
                                 .Select(S => new Site {
                                                            ID = S.ID,
                                                            Name = S.Name,
                                                            OrganisationID = S.Organisation != null ? S.Organisation.ID : 0,
                                                            OrganisationName = S.Organisation != null ? S.Organisation.Name : "",
                                                            Country = S.Country,
                                                            TimeZoneOffset = S.TimeZoneOffset,
                                                            Description = S.Description
                                                       }
                                        )
                                 .FirstOrDefaultAsync();
        }

        protected List<Country> GetCountries() {
            List<Country> countries = Country.GetCountries();
            return countries.Select(c => new Country
                                         {
                                            CountryCode = c.CountryCode,
                                            CountryName = c.CountryName,
                                            Offset = c.Offset,
                                            OffsetInMinutes = c.Offset.Contains("+") ?
                                                              Math.Abs(TimeSpan.Parse(c.Offset.Replace("+", "")).TotalMinutes)
                                                              : -Math.Abs(TimeSpan.Parse(c.Offset.Replace("-", "")).TotalMinutes)
                                         }
            ).ToList();
        }

        protected async Task UpdateSite(SiteMaster siteMaster, Site site) {
            siteMaster.Name = site.Name;
            siteMaster.Country = site.Country;
            siteMaster.Description = site.Description;
            siteMaster.TimeZoneOffset = site.TimeZone;
            if (site.OrganisationID != 0)
                siteMaster.Organisation = await _context.OrganisationMaster.FindAsync(site.OrganisationID);
            else
                siteMaster.Organisation = null;
        }

        protected async Task<SiteMaster> CreateSite(Site site) {
            SiteMaster siteMaster = new SiteMaster();
            siteMaster.Name = site.Name;
            siteMaster.Description = site.Description;
            siteMaster.Country = site.Country;
            siteMaster.TimeZoneOffset = site.TimeZone;
            siteMaster.Organisation = _context.OrganisationMaster.Where(V => V.ID == site.OrganisationID).FirstOrDefault();

            _context.SiteMaster.Add(siteMaster);
            await _context.SaveChangesAsync();

            return siteMaster;
        }

        protected bool SiteMasterExists(int id)
        {
            return _context.SiteMaster.Any(e => e.ID == id);
        }
    }
}