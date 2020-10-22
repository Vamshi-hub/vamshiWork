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
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using astorWorkShared.GlobalExceptions;

namespace astorWorkMaterialTracking.Controllers
{
    [Produces("application/json")]
    [Route("sites")]
    [ApiController]
    public class SiteMastersController : CommonSiteMasterController
    {
        public SiteMastersController(astorWorkDbContext context) : base(context)
        {
            _context = context;
        }

        // GET: sites
        [HttpGet]
        public async Task<List<Site>> ListSites()
        {
            /*
            CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.AllCultures & ~CultureTypes.SpecificCultures);
            var countries = cultures.Where(C => C.LCID != 127 & !C.IsNeutralCulture)
                .GroupBy(C => new { new RegionInfo(C.LCID).EnglishName, new RegionInfo(C.LCID).Name })
                .Select(C => new { countryCode = C.Key.Name, countryName = C.Key.EnglishName }).ToList();
                */

            return await GetSites();
        }

        ////GET: sites/countries
        //[HttpGet("countries")]
        //public APIResponse ListCountries()
        //{
        //    CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.AllCultures & ~CultureTypes.SpecificCultures);
        //    var countries = cultures.Where(C => C.LCID != 127 & !C.IsNeutralCulture)
        //        .GroupBy(C => new { new RegionInfo(C.LCID).EnglishName, new RegionInfo(C.LCID).Name })
        //        .Select(C => new { countryCode = C.Key.Name, countryName = C.Key.EnglishName })
        //        .OrderBy(C => C.countryName);

        //    return new APIResponse(0, countries);
        //}

        // GET: sites/5
        [HttpGet("{id}")]
        public async Task<Site> GetSiteDetails([FromRoute] int id)
        {
            if (!ModelState.IsValid)
                throw new GenericException(ErrorMessages.BadRequest, ErrorMessages.BadRequestMsg());

            Site site = await GetSite(id);

            if (site == null)
                throw new GenericException(ErrorMessages.DbRecordNotFound, ErrorMessages.DBRecordNotFoundMsg("Site", "Site ID", id.ToString()));

            return site;
        }

        //GET: sites/countries
        [HttpGet("countries")]
        public List<Country> ListCountries()
        {
            //CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.AllCultures & ~CultureTypes.SpecificCultures);
            //var countries = cultures.Where(C => C.LCID != 127 & !C.IsNeutralCulture)
            //    .GroupBy(C => new { new RegionInfo(C.LCID).EnglishName, new RegionInfo(C.LCID).Name })
            //    .Select(C => new { countryCode = C.Key.Name, countryName = C.Key.EnglishName })
            //    .OrderBy(C => C.countryName);
            //List<TimeZoneInfo> tzList = TimeZoneInfo.GetSystemTimeZones().ToList();

            return GetCountries();
        }

        // PUT: sites/5
        [HttpPut("{id}")]
        public async Task<Site> EditSite([FromRoute] int id, [FromBody] Site site)
        {
            if (!ModelState.IsValid)
                throw new GenericException(ErrorMessages.BadRequest, ErrorMessages.BadRequestMsg());

            SiteMaster siteMaster = await _context.SiteMaster.Include(V => V.Organisation).Include(L => L.Locations).Where(S => S.ID == id).FirstOrDefaultAsync();

            if (siteMaster == null)
                throw new GenericException(ErrorMessages.DbRecordNotFound, "No site found!");

            await UpdateSite(siteMaster, site);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new GenericException(ErrorMessages.DbConcurrentUpdate, ex.Message);
            }

            return site;
        }

        // POST: sites
        [HttpPost]
        public async Task<SiteMaster> PostSite([FromBody] Site site)
        {
            if (!ModelState.IsValid)
                throw new GenericException(ErrorMessages.BadRequest, ErrorMessages.BadRequestMsg());

            if (_context.SiteMaster.Any(S => S.Name == site.Name))
                throw new GenericException(ErrorMessages.DbRecordNotFound, ErrorMessages.DbDuplicateRecordMsg("Site", "Name", site.Name));

            return await CreateSite(site);
        }

        // DELETE: sites/5
        [HttpDelete("{id}")]
        public async Task<SiteMaster> DeleteSiteMaster([FromRoute] int id)
        {
            if (!ModelState.IsValid)
                throw new GenericException(ErrorMessages.BadRequest, ErrorMessages.BadRequestMsg());

            SiteMaster siteMaster = await _context.SiteMaster.FindAsync(id);

            if (siteMaster == null)
                throw new GenericException(ErrorMessages.DbRecordNotFound, "Site not found!");

            _context.SiteMaster.Remove(siteMaster);
            await _context.SaveChangesAsync();

            return siteMaster;
        }
    }
}