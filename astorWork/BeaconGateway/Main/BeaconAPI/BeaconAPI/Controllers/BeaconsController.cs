using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using BeaconAPI.Models;

namespace BeaconAPI.Controllers
{
    public class BeaconsController : ApiController
    {
        private astorTrack_GEEntities db = new astorTrack_GEEntities();

        // GET: api/Beacons
        public IQueryable<Beacon> GetBeacons()
        {
            return db.Beacons;
        }

        // GET: api/Beacons/5
        [ResponseType(typeof(Beacon))]
        public async Task<IHttpActionResult> GetBeacon(string id)
        {
            Beacon beacon = await db.Beacons.FindAsync(id);
            if (beacon == null)
            {
                return NotFound();
            }

            return Ok(beacon);
        }

        // PUT: api/Beacons/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutBeacon(string id, Beacon beacon)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != beacon.BeaconID)
            {
                return BadRequest();
            }

            db.Entry(beacon).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BeaconExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Beacons
        [ResponseType(typeof(Beacon))]
        public async Task<IHttpActionResult> PostBeacon(Beacon beacon)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Beacons.Add(beacon);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (BeaconExists(beacon.BeaconID))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = beacon.BeaconID }, beacon);
        }

        // DELETE: api/Beacons/5
        [ResponseType(typeof(Beacon))]
        public async Task<IHttpActionResult> DeleteBeacon(string id)
        {
            Beacon beacon = await db.Beacons.FindAsync(id);
            if (beacon == null)
            {
                return NotFound();
            }

            db.Beacons.Remove(beacon);
            await db.SaveChangesAsync();

            return Ok(beacon);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool BeaconExists(string id)
        {
            return db.Beacons.Count(e => e.BeaconID == id) > 0;
        }
    }
}