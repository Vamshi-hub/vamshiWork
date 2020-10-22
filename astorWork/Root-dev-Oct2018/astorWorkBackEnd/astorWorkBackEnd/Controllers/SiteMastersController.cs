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
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace astorWorkBackEnd.Controllers
{
    [Produces("application/json")]
    [Route("sites")]
    [ApiController]
    public class SiteMastersController : ControllerBase
    {
        private readonly astorWorkDbContext _context;

        public SiteMastersController(astorWorkDbContext context)
        {
            _context = context;
        }

        // GET: sites
        [HttpGet]
        public APIResponse ListSites()
        {
            /*
            CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.AllCultures & ~CultureTypes.SpecificCultures);
            var countries = cultures.Where(C => C.LCID != 127 & !C.IsNeutralCulture)
                .GroupBy(C => new { new RegionInfo(C.LCID).EnglishName, new RegionInfo(C.LCID).Name })
                .Select(C => new { countryCode = C.Key.Name, countryName = C.Key.EnglishName }).ToList();
                */
            var countries = Country.GetCountryList();
            var siteMaster = _context.SiteMaster.Include(S => S.Vendor).Select(S => new
            {
                S.ID,
                S.Name,
                VendorId = S.Vendor != null ? S.Vendor.ID : 0,
                VendorName = S.Vendor != null ? S.Vendor.Name : "",
                Country = !string.IsNullOrEmpty(S.Country) ? countries.Where(C => C.CountryCode == S.Country)
                .Select(C => C.CountryName).FirstOrDefault().ToString() : "",
                S.Description
            }).ToList();
            return new APIResponse(0, siteMaster);
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
        public APIResponse GetSiteDetails([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return new APIBadRequest();
            }

            var siteMaster = _context.SiteMaster.Include(S => S.Vendor).Where(S => S.ID == id)
                .Select(S => new
                {
                    S.ID,
                    S.Name,
                    VendorId = S.Vendor != null ? S.Vendor.ID : 0,
                    VendorName = S.Vendor != null ? S.Vendor.Name : "",
                    S.Country,
                    TimeZone = S.TimeZoneOffset,
                    S.Description
                }).FirstOrDefault();

            if (siteMaster == null)
            {
                return new DbRecordNotFound("Site", "Site ID", id.ToString());
            }

            return new APIResponse(0, siteMaster);
        }

        //GET: sites/countries
        [HttpGet("countries")]
        public APIResponse GetCountries()
        {

            //CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.AllCultures & ~CultureTypes.SpecificCultures);
            //var countries = cultures.Where(C => C.LCID != 127 & !C.IsNeutralCulture)
            //    .GroupBy(C => new { new RegionInfo(C.LCID).EnglishName, new RegionInfo(C.LCID).Name })
            //    .Select(C => new { countryCode = C.Key.Name, countryName = C.Key.EnglishName })
            //    .OrderBy(C => C.countryName);
            //List<TimeZoneInfo> tzList = TimeZoneInfo.GetSystemTimeZones().ToList();


            var countries = Country.GetCountryList();
            return new APIResponse(0, countries
                .Select(c => new {
                     c.CountryCode
                    ,c.CountryName
                    ,c.Offset
                    ,OffsetInMinutes = c.Offset.Contains("+") ? 
                    Math.Abs(TimeSpan.Parse(c.Offset.Replace("+", "")).TotalMinutes) 
                    : -Math.Abs(TimeSpan.Parse(c.Offset.Replace("-","")).TotalMinutes)
                }));
        }

        //private List<Country> GetCountryList()
        //{
        //    var jsonString = System.IO.File.ReadAllText("countries.json");
        //    var json = JsonConvert.DeserializeObject<List<Country>>(jsonString);

        //    return json;
        //}


        // PUT: sites/5
        [HttpPut("{id}")]
        public async Task<APIResponse> EditSite([FromRoute] int id, [FromBody] SiteDetails site)
        {
            if (!ModelState.IsValid)
            {
                return new APIBadRequest();
            }
            SiteMaster siteMaster = await _context.SiteMaster.Include(V => V.Vendor).Include(L => L.Locations).Where(S => S.ID == id).FirstOrDefaultAsync();
            if (siteMaster != null)
            {
                siteMaster.Name = site.Name;
                siteMaster.Country = site.Country;
                siteMaster.Description = site.Description;
                siteMaster.TimeZoneOffset = site.TimeZone;
                if (site.VendorID != 0)
                    siteMaster.Vendor = await _context.VendorMaster.FindAsync(site.VendorID);
                else
                    siteMaster.Vendor = null;


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
                return new DbRecordNotFound("No Site Found!");

            return new APIResponse(0, site);
        }

        // POST: sites
        [HttpPost]
        public async Task<APIResponse> CreateSite([FromBody] SiteDetails site)
        {
            if (!ModelState.IsValid)
            {
                return new APIBadRequest();
            }
            if (_context.SiteMaster.Any(S => S.Name == site.Name))
                return new DbDuplicateRecord("Site", "Name", site.Name);
            SiteMaster siteMaster = new SiteMaster();
            siteMaster.Name = site.Name;
            siteMaster.Description = site.Description;
            siteMaster.Country = site.Country;
            siteMaster.TimeZoneOffset = site.TimeZone;
            siteMaster.Vendor = _context.VendorMaster.Where(V => V.ID == site.VendorID).FirstOrDefault();
            

            _context.SiteMaster.Add(siteMaster);
            await _context.SaveChangesAsync();

            return new APIResponse(0, siteMaster);
        }

        // DELETE: sites/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSiteMaster([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var siteMaster = await _context.SiteMaster.FindAsync(id);
            if (siteMaster == null)
            {
                return NotFound();
            }

            _context.SiteMaster.Remove(siteMaster);
            await _context.SaveChangesAsync();

            return Ok(siteMaster);
        }

        private bool SiteMasterExists(int id)
        {
            return _context.SiteMaster.Any(e => e.ID == id);
        }
    }
}