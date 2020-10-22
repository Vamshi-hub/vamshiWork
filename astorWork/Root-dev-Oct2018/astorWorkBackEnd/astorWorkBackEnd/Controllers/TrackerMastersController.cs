using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using astorWorkDAO;
using astorWorkBackEnd.Models;
using astorWorkShared.GlobalResponse;
using astorWorkBackEnd.Common;
using OfficeOpenXml;
using System.IO;

namespace astorWorkBackEnd.Controllers
{
    [Produces("application/json")]
    [Route("trackers")]
    public class TrackerMastersController : CommonTrackerMastersController
    {
        private readonly string Module = "Tracker";

        public TrackerMastersController(astorWorkDbContext context) : base(context)
        {
        }

        // GET: api/TrackerMasters
        [HttpGet]
        public IEnumerable<TrackerMaster> GetTrackerMaster()
        {
            return _context.TrackerMaster;
        }

        // GET: trackers/association?tag={tracker_tag}&user_name={user_name}
        [HttpGet("association")]
        public APIResponse GetTrackerAssociationInfo([FromQuery] string[] tags)
        {
            if (!ModelState.IsValid)
                return new APIBadRequest();

            List<TrackerAssociation> trackerAssociations = new List<TrackerAssociation>();

            var trackers = _context.TrackerMaster.Where(tm => tags.Contains(tm.Tag)).ToList();
            int userID = _context.GetUserFromHttpContext(HttpContext).ID;
            int? userProjectID = _context.UserMaster.Where(u => u.ID == userID).FirstOrDefault().ProjectID;

            foreach (var tracker in trackers)
            {
                TrackerAssociation trackerAssociation = CreateTrackerAssociation(tracker, userProjectID);
                trackerAssociations.Add(trackerAssociation);
            }
            return new APIResponse(0, trackerAssociations);
        }

        // GET: trackers/generateQRCodes?qty={qty}
        [HttpGet("generate-qr-codes")]
        public async Task<APIResponse> GenerateQRCodes([FromQuery] int qty, [FromQuery] string label)
        {
            List<TrackerMaster> qrCodes = new List<TrackerMaster>();
            TrackerMaster qrCode;
            TrackerMaster trackerMaster = new TrackerMaster();
            string trackerType = "QR Code";
            int currentBatchNumber = GetMaxTrackerBatchNo(trackerType) + 1;

            for (int i = 0; i < qty; i++)
            {
                qrCode = GenerateQRCode(i, currentBatchNumber, trackerType, label);
                trackerMaster = CreateTracker(qrCode.Label + "," + qrCode.Tag, trackerType, currentBatchNumber);

                _context.TrackerMaster.Add(trackerMaster);
                qrCodes.Add(qrCode);
            }

            await _context.SaveChangesAsync();

            return new APIResponse(0, qrCodes);
        }

        // POST: api/TrackerMasters
        [HttpPost("import-trackers")]
        public async Task<APIResponse> ImportTrackers(IFormFile file, [FromQuery] string type)
        {
            if (!ModelState.IsValid || string.IsNullOrEmpty(type))
            {
                return new APIBadRequest();
            }

            IEnumerable<string> trackers = GetTrackersFromFile(file).Distinct();
            List<TrackerUploadStatus> existingTrackers = new List<TrackerUploadStatus>();
            List<string> addedTags = new List<string>();
            List<string> addedLabels = new List<string>();

            int currentBatchNumber = GetMaxTrackerBatchNo(type) + 1;

            for (int i = 1; i < trackers.Count(); i++)
            {
                TrackerMaster tracker = CreateTracker(trackers.ElementAt(i), type, currentBatchNumber);

                if (TrackerMasterExists(tracker, type)) // Check for duplicates in the Db
                {
                    TrackerUploadStatus trackerUploadStatus = CreateTrackerUploadStatus(tracker, "Tracker already exist in the database");
                    existingTrackers.Add(trackerUploadStatus);
                }
                else
                {
                    if (addedTags.Contains(tracker.Tag))
                    { // Check for duplicates in the CSV file
                        TrackerUploadStatus trackerUploadStatus = CreateTrackerUploadStatus(tracker, "Duplicate Tags found in CSV");
                        existingTrackers.Add(trackerUploadStatus);
                    }
                    else
                    {
                        _context.TrackerMaster.Add(tracker);

                        addedTags.Add(tracker.Tag);
                        addedLabels.Add(tracker.Label);
                    }
                }
            }

            await _context.SaveChangesAsync();

            if (existingTrackers.Count == 0)
                return new APIResponse(0, null);
            else
                return new APIResponse(0, existingTrackers);
        }

        protected TrackerUploadStatus CreateTrackerUploadStatus(TrackerMaster tracker, string message)
        {
            TrackerUploadStatus trackerUploadStatus = new TrackerUploadStatus();

            trackerUploadStatus.Label = tracker.Label;
            trackerUploadStatus.Tag = tracker.Tag;
            trackerUploadStatus.Type = tracker.Type;
            trackerUploadStatus.Message = message;

            return trackerUploadStatus;
        }

        // PUT: api/TrackerMasters/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTrackerMaster([FromRoute] int id, [FromBody] TrackerMaster trackerMaster)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != trackerMaster.ID)
            {
                return BadRequest();
            }

            _context.Entry(trackerMaster).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TrackerMasterExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/TrackerMasters/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTrackerMaster([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var trackerMaster = await _context.TrackerMaster.SingleOrDefaultAsync(m => m.ID == id);
            if (trackerMaster == null)
            {
                return NotFound();
            }

            _context.TrackerMaster.Remove(trackerMaster);
            await _context.SaveChangesAsync();

            return Ok(trackerMaster);
        }
    }
}