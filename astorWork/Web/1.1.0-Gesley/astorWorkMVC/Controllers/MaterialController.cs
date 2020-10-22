using astorWorkMVC.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace astorWorkMVC.Controllers
{
    public class MaterialController : ApiController
    {
        private astorWorkEntities db = new astorWorkEntities();

        // GET: api/Material?Status=xxx
        [Route("api/Material/Master")]
        public IHttpActionResult GetMaterialMaster([FromUri]string Status)
        {
            if (string.IsNullOrEmpty(Status))
            {
                return BadRequest("Status cannot be empty");
            }

            IQueryable<MaterialMaster> result = db.MaterialMasters.Where(mm => mm.Status == Status);
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        // POST: api/MaterialMaster
        [ResponseType(typeof(int))]
        [HttpPost]
        [Route("api/Material/Master/UpdateByVendor")]
        public async Task<IHttpActionResult> PostMaterialByVendor(IEnumerable<VendorUpdateData> vendorData)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            foreach (var item in vendorData)
            {
                var materialMaster = await db.MaterialMasters.FindAsync(item.MaterialID);

                if (materialMaster == null)
                    continue;
                else
                {
                    materialMaster.BeaconID = item.BeaconID;
                    materialMaster.CastingDate = item.CastingDate;
                    materialMaster.LotNo = item.LotNo;
                    materialMaster.Status = item.Status;
                    materialMaster.UpdatedBy = item.UserName;
                    materialMaster.UpdatedDate = DateTime.Today;

                    // Location ID 0 means it's requested only
                    if (materialMaster.LocationID == 0)
                        materialMaster.LocationID = item.UserLocationID;

                    db.Entry(materialMaster).State = EntityState.Modified;

                    string location = string.Empty;
                    string parentLocation = string.Empty;
                    string targetLocation = string.Empty;


                    var locationAssociation = db.LocationAssociations.Where(la => la.AssociationID == materialMaster.LocationID).First();
                    if(locationAssociation != null)
                    {
                        var parent = db.LocationMasters.Where(lm => lm.LocationID == locationAssociation.ParentID).First();
                        if (parent != null)
                            parentLocation = parent.Description;

                        var target = db.LocationMasters.Where(lm => lm.LocationID == locationAssociation.ChildID).First();
                        if (target != null)
                            targetLocation = target.Description;
                    }

                    
                    switch (item.Status)
                    {
                        case "Produced":
                            location = targetLocation;
                            break;
                        case "Delivered":
                            materialMaster.LocationID = locationAssociation.ChildID;
                            string ultimateLocation = string.Empty;
                            var locationAssociation2 = db.LocationAssociations.Where(la => la.AssociationID == materialMaster.LocationID).First();
                            if (locationAssociation2 != null)
                            {
                                var ultimate = db.LocationMasters.Where(lm => lm.LocationID == locationAssociation2.ChildID).First();
                                if (ultimate != null)
                                    ultimateLocation = ultimate.Description;

                                location = string.Format("{0} > {1} > {2}", parentLocation, targetLocation, ultimateLocation);
                            }
                                break;
                        case "Installed":
                            location = string.Format("{0} > {1} > {2} > {3}", targetLocation, materialMaster.Block, materialMaster.Level, materialMaster.Zone);
                            break;
                        default:
                            break;
                    }

                    var materialDetail = new MaterialDetail
                    {
                        MarkingNo = materialMaster.MarkingNo,
                        LocationID = materialMaster.LocationID,
                        Stage = item.Status,
                        CreatedBy = item.UserName,
                        CreatedDate = DateTime.Today,
                        IsQC = false,
                        MaterialNo = materialMaster.MaterialID.ToString(),
                        BeaconID = item.BeaconID,
                        Location = location
                    };

                    db.MaterialDetails.Add(materialDetail);
                }
            }

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException exc)
            {
                return Ok(vendorData.Count() - exc.Entries.Where(entity => entity.GetType().Equals(typeof(MaterialMaster))).Count());
            }

            return Ok(vendorData.Count());
        }

        private bool MaterialMasterExists(int id)
        {
            return db.MaterialMasters.Count(e => e.MaterialID == id) > 0;
        }
    }

    public class VendorUpdateData
    {
        public int MaterialID { get; set; }
        public string BeaconID { get; set; }
        public DateTime CastingDate { get; set; }
        public int LotNo { get; set; }
        public string Status { get; set; }
        public string UserName { get; set; }
        public int UserLocationID { get; set; }
    }
}
