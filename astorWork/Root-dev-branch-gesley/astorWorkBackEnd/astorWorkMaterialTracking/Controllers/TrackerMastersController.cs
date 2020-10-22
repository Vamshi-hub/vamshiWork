using astorWorkDAO;
using astorWorkMaterialTracking.Common;
using astorWorkMaterialTracking.Models;
using astorWorkShared.GlobalExceptions;
using astorWorkShared.GlobalResponse;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace astorWorkMaterialTracking.Controllers
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

        // POST: trackers/association?tag={tracker_tag}&user_name={user_name}
        [HttpPost("association")]
        public async Task UpdateTrackerAssociation([FromBody] TrackerAssociation trackerAssociation)
        {
            if (trackerAssociation == null || trackerAssociation.Trackers == null || trackerAssociation.Material == null)
                throw new GenericException(ErrorMessages.BadRequest, ErrorMessages.BadRequestMsg());

            TrackerMaster tracker = await _context.TrackerMaster.FindAsync(trackerAssociation.Trackers.FirstOrDefault().ID);
            MaterialMaster material = await _context.MaterialMaster.FindAsync(trackerAssociation.Material.ID);

            if (tracker == null || material == null)
                throw new GenericException(ErrorMessages.DbRecordNotFound, "No associated tracker found!");
            tracker.MaterialID = material.ID;
            tracker.Material = material;
            material.Trackers = new List<TrackerMaster>();
            material.Trackers.Add(tracker);

            await _context.SaveChangesAsync();
        }

        // GET: trackers/association?tags={tags}
        [HttpGet("association")]
        public async Task<List<TrackerAssociation>> GetTrackerAssociationInfo([FromQuery] string[] tags,[FromQuery] string tenant_name)
        {
            if (!ModelState.IsValid)
                throw new GenericException(ErrorMessages.BadRequest, ErrorMessages.BadRequestMsg());

            List<TrackerAssociation> trackerAssociations = new List<TrackerAssociation>();
            List<int> trackerIDs = GetTrackerIDs(tags);

            UserMaster userMaster = await _context.GetUserFromHttpContext(HttpContext);
            if (userMaster == null)
                throw new GenericException(ErrorMessages.DbRecordNotFound, "User not found!");

            List<MaterialMaster> materials = await GetMaterials(trackerIDs, 7);
            if (materials != null)
            {
                List<string> unusedTrackerTags = GetUnusedTrackerTags(tags, materials);

                trackerAssociations.AddRange(
                    await GetTrackerAssociations(unusedTrackerTags)
                );
            }

            trackerAssociations.AddRange(materials.Select(mm =>
            new TrackerAssociation
            {
                Trackers = mm.Trackers.Select(t => new Tracker
                {
                    ID = t.ID,
                    Tag = t.Tag,
                    Label = t.Label,
                    Type = t.Type
                }).ToList(),
                Material = CreateMaterialMobile(mm, userMaster,tenant_name)
            }));

            return trackerAssociations;
        }


        [HttpGet("material-by-subcon")]
        public async Task<List<TrackerAssociation>>GetMaterialsbySubCon([FromQuery] string[] tags)
        {
            
            if(!ModelState.IsValid)
                throw new GenericException(ErrorMessages.BadRequest, ErrorMessages.BadRequestMsg());
            List<TrackerAssociation> trackerAssociations = new List<TrackerAssociation>();

            UserMaster userMaster = await _context.GetUserFromHttpContext(HttpContext);

            if (userMaster == null)
                throw new GenericException(ErrorMessages.DbRecordNotFound, "User not found!");
            List<int> trackerIDs = GetTrackerIDs(tags);
           List<MaterialMaster> materials =  GetMaterialsforSubCon(trackerIDs, userMaster);

            trackerAssociations.AddRange(materials.Select(mm =>
           new TrackerAssociation
           {
               Trackers = mm.Trackers.Select(t => new Tracker
               {
                   ID = t.ID,
                   Tag = t.Tag,
                   Label = t.Label,
                   Type = t.Type
               }).ToList(),
               Material = CreateMaterialMobile(mm,userMaster,"")
           }));
            return trackerAssociations;
        }


        // GET: trackers/generateQRCodes?qty={qty}
        [HttpGet("generate-qr-codes")]
        public async Task<List<TrackerMaster>> GenerateQRCodes([FromQuery] int qty, [FromQuery] string label)
        {
            List<TrackerMaster> qrCodes = new List<TrackerMaster>();
            //TrackerMaster qrCode;
            //TrackerMaster trackerMaster = new TrackerMaster();
            //string trackerType = "QR Code";
            //int currentBatchNumber = GetMaxTrackerBatchNo(trackerType) + 1;

            //for (int i = 0; i < qty; i++)
            //{
            //    qrCode = GenerateQRCode(i, currentBatchNumber, trackerType, label);
            //    trackerMaster = CreateTracker(qrCode.Label + "," + qrCode.Tag, trackerType, currentBatchNumber);

            //    _context.TrackerMaster.Add(trackerMaster);
            //    qrCodes.Add(qrCode);
            //}

            //await _context.SaveChangesAsync();

            return qrCodes;
        }

        // POST: api/trackers?type=QR Code
        [HttpPost("import-trackers")]
        public async Task<List<TrackerUploadStatus>> ImportTrackers(IFormFile file, [FromQuery] string type)
        {
            if (!ModelState.IsValid || string.IsNullOrEmpty(type))
                throw new GenericException(ErrorMessages.BadRequest, ErrorMessages.BadRequestMsg());

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
                    if (addedTags.Contains(tracker.Tag))    // Check for duplicates in the CSV file
                    {
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
                return new List<TrackerUploadStatus>();
            else
                return existingTrackers;
        }

        // PUT: api/TrackerMasters/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTrackerMaster([FromRoute] int id, [FromBody] TrackerMaster trackerMaster)
        {
            if (!ModelState.IsValid || id != trackerMaster.ID)
                throw new GenericException(ErrorMessages.BadRequest, ErrorMessages.BadRequestMsg());

            _context.Entry(trackerMaster).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TrackerMasterExists(id))
                    throw new GenericException(ErrorMessages.DbRecordNotFound, "Tracker not found!");
                else
                    throw;
            }

            return NoContent();
        }

        // DELETE: api/TrackerMasters/5
        [HttpDelete("{id}")]
        public async Task<TrackerMaster> DeleteTrackerMaster([FromRoute] int id)
        {
            if (!ModelState.IsValid)
                throw new GenericException(ErrorMessages.BadRequest, ErrorMessages.BadRequestMsg());

            var trackerMaster = await _context.TrackerMaster.SingleOrDefaultAsync(m => m.ID == id);
            if (trackerMaster == null)
                throw new GenericException(ErrorMessages.DbRecordNotFound, "Tracker not found!");

            _context.TrackerMaster.Remove(trackerMaster);
            await _context.SaveChangesAsync();

            return trackerMaster;
        }
    }
}
