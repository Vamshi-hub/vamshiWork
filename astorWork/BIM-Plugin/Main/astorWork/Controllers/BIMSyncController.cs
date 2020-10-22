using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using astorWork.Models;
using astorWork.Utilities;

namespace astorWork.Controllers
{
    public class BIMSyncController : ApiController
    {
        private astorWorkEntities db = new astorWorkEntities();

        public object CrendentialHelper { get; private set; }

        [Route("api/BIMSync/BIMMaterials")]
        [ResponseType(typeof(IQueryable<MaterialMaster>))]
        public async Task<IHttpActionResult> GetBIMMaterials(string api_key, long last_sync_file_time)
        {
            var bIMUser = await db.BIMUserInfoes.Where(u => u.APIKey == api_key).FirstOrDefaultAsync();

            if (bIMUser != null)
            {
                var last_sync_dt = DateTime.FromFileTimeUtc(last_sync_file_time);
                string[] validStatuses = new string[] { "Requested", "Produced", "Delivered", "Installed" };
                var result = db.MaterialMasters.Where(mm => mm.UpdatedDate > last_sync_dt && validStatuses.Contains(mm.Status));
                var bimSync = bIMUser.BIMSyncCurrs.FirstOrDefault();

                if (bimSync != null && !string.IsNullOrEmpty(bimSync.MissingMaterialIds))
                {
                    var ids = bimSync.MissingMaterialIds.Split(',').Select(i => long.Parse(i));
                    result = result.Union(db.MaterialMasters.Where(mm => ids.Contains(mm.MaterialID)));
                }
                result = result.Distinct();

                return Ok(result);
            }
            else
                return Unauthorized();
        }

        [Route("api/BIMSync/Materials")]
        [ResponseType(typeof(IQueryable<BIMMaterial>))]
        public async Task<IHttpActionResult> GetMaterialsForSync(string api_key, string project_id, DateTime last_sync_utc_time)
        {
            var bIMUser = await db.BIMUserInfoes.Where(u => u.APIKey == api_key).FirstOrDefaultAsync();

            if (bIMUser != null)
            {
                string[] validStatuses = new string[] { "Produced", "Delivered", "Installed" };
                var materials = db.MaterialDetails.Where(md => md.CreatedDate.HasValue && md.CreatedDate.Value >= last_sync_utc_time && validStatuses.Contains(md.Stage));
                var bimSync = bIMUser.BIMSyncCurrs.Where(bs => bs.BIMProjectID == project_id).FirstOrDefault();

                if (bimSync != null && !string.IsNullOrEmpty(bimSync.MissingMaterialIds))
                {
                    var ids = bimSync.MissingMaterialIds.Split(',').Select(i => i);
                    materials = materials.Union(db.MaterialDetails.Where(md => ids.Contains(md.MaterialNo)));
                }
                materials = materials.Distinct();

                List<BIMMaterial> result = new List<BIMMaterial>();
                foreach (var md in materials)
                {
                    var mm = await db.MaterialMasters.FindAsync(long.Parse(md.MaterialNo));
                    var item = new BIMMaterial
                    {
                        MaterialNo = mm.MaterialID,
                        MarkingNo = mm.MarkingNo,
                        Block = mm.Block,
                        Level = mm.Level,
                        Zone = mm.Zone,
                        CurrentStatus = mm.Status,
                        Status = md.Stage,
                        UpdateDT = md.CreatedDate.HasValue ? md.CreatedDate.Value : DateTime.MinValue
                    };
                    result.Add(item);
                }
                return Ok(result);
            }
            else
                return Unauthorized();
        }

        [Route("api/BIMSync/BIMUser")]
        [ResponseType(typeof(BIMUserInfo))]
        public async Task<IHttpActionResult> GetBIMUser(string user_name, string password)
        {
            var bIMUser = await db.BIMUserInfoes.Where(u => u.UserName == user_name).FirstOrDefaultAsync();

            if (bIMUser != null)
            {
                byte[] pwd = Encoding.UTF8.GetBytes(password);
                byte[] salt = Encoding.UTF8.GetBytes(bIMUser.Salt);
                string pwd_encrtpted = Convert.ToBase64String(CredentialHelper.GenerateSaltedHash(pwd, salt));
                if (pwd_encrtpted.Equals(bIMUser.Password))
                    return Ok(bIMUser);
                else
                    return Unauthorized();
            }
            else
                return NotFound();
        }

        [ResponseType(typeof(BIMSyncCurr))]
        public async Task<IHttpActionResult> GetBIMSyncCurr(string api_key)
        {
            var bIMUser = await db.BIMUserInfoes.Where(u => u.APIKey == api_key).FirstOrDefaultAsync();

            if (bIMUser != null)
            {
                var bIMSyncCurr = await db.BIMSyncCurrs.Where(s => s.UserID == bIMUser.ID).FirstOrDefaultAsync();
                if (bIMSyncCurr == null)
                {
                    return NotFound();
                }
                else
                    return Ok(bIMSyncCurr);
            }
            else
                return Unauthorized();
        }

        // POST: api/BIMSync
        [ResponseType(typeof(BIMSyncCurr))]
        public async Task<IHttpActionResult> PostBIMSyncCurr(string api_key, [FromBody]BIMSyncCurr bIMSyncCurr)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var bIMUser = await db.BIMUserInfoes.Where(u => u.APIKey == api_key).FirstOrDefaultAsync();

            if (bIMUser != null)
            {
                var result = bIMUser.BIMSyncCurrs.Where(b => b.BIMProjectID == bIMSyncCurr.BIMProjectID).FirstOrDefault();
                if (result != null)
                {
                    result.SyncedMaterialIds = bIMSyncCurr.SyncedMaterialIds;
                    result.MissingMaterialIds = bIMSyncCurr.MissingMaterialIds;
                    result.Status = bIMSyncCurr.Status;
                    result.LastSyncDate = bIMSyncCurr.LastSyncDate;
                    result.VisualizeURL = bIMSyncCurr.VisualizeURL;
                    result.Remarks = bIMSyncCurr.Remarks;
                    db.Entry(result).State = EntityState.Modified;
                }
                else
                {
                    bIMSyncCurr.BIMUserInfo = bIMUser;
                    db.BIMSyncCurrs.Add(bIMSyncCurr);
                }

                await db.SaveChangesAsync();

                return CreatedAtRoute("DefaultApi", new { id = bIMSyncCurr.ID }, bIMSyncCurr);
            }
            else
                return Unauthorized();
        }

        [HttpPost]
        [Route("api/BIMSync/VideoUpload")]
        public async Task<HttpResponseMessage> PostVideoFile(string api_key)
        {
            HttpResponseMessage response = null;
            var bIMUser = await db.BIMUserInfoes.Where(u => u.APIKey == api_key).FirstOrDefaultAsync();

            if (bIMUser != null)
            {
                try
                {
                    // Read the form data and return an async task.
                    var content = await Request.Content.ReadAsByteArrayAsync();

                    var fileName = await AzureHelper.UploadBIMVisualization(bIMUser.ID, content);
                    if (string.IsNullOrEmpty(fileName))
                        response = Request.CreateResponse(HttpStatusCode.NotFound);
                    else
                        response = Request.CreateResponse(HttpStatusCode.OK, fileName);
                }
                catch (Exception exc)
                {
                    response = Request.CreateResponse(HttpStatusCode.InternalServerError, exc.Message);
                }
                

                /*
                var root = HttpContext.Current.Server.MapPath("~/Upload/BIM");
                var folderPath = string.Format("{0}\\{1}", root, bIMUser.ID);
                var fileName = Guid.NewGuid();
                Directory.CreateDirectory(folderPath);
                var path = string.Format("{0}\\{1}.mp4", folderPath, fileName);
                var writer = new FileStream(path, FileMode.Create);
                try
                {
                    await writer.WriteAsync(content, 0, content.Length);
                    var webPath = string.Format("../Upload/BIM/{0}/{1}.mp4", bIMUser.ID, fileName);
                    response = Request.CreateResponse(HttpStatusCode.OK, webPath);
                }
                catch(Exception exc)
                {
                    response = Request.CreateResponse(HttpStatusCode.InternalServerError, exc.Message);
                }
                */

            }
            else
            {
                response = Request.CreateResponse(HttpStatusCode.Unauthorized);
            }

            return response;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool BIMSyncCurrExists(int id)
        {
            return db.BIMSyncCurrs.Count(e => e.ID == id) > 0;
        }
    }
}