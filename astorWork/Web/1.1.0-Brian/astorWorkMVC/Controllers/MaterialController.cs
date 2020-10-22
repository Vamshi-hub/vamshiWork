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
        // GET: api/Material?Status=xxx
        [Route("api/Material/MasterWithBeacon")]
        public IHttpActionResult GetMaterialWithBeacon([FromUri]string[] beaconIds)
        {
            if (beaconIds == null)
            {
                return BadRequest("Beacon Ids cannot be empty");
            }
            
            List<MaterialBeaconData> result = new List<MaterialBeaconData>();

            foreach (var materialMaster in db.MaterialMasters.Where(mm => beaconIds.Contains(mm.BeaconID)).OrderByDescending(mm => mm.UpdatedDate))
            {
                var materialDetail = db.MaterialDetails.Where(md => md.MaterialNo == materialMaster.MaterialNo && md.Stage == materialMaster.Status).FirstOrDefault();

                if (materialDetail != null && !result.Select(r => r.BeaconID).Contains(materialMaster.BeaconID)) {
                    result.Add(new MaterialBeaconData
                    {
                        Project = materialMaster.Project,
                        MarkingNo = materialMaster.MarkingNo,
                        InInventory = false,
                        PassQC = materialDetail.QCStatus == "Pass",
                        BeaconID = materialMaster.BeaconID,
                        Status = materialMaster.Status,
                        LotNo = materialMaster.LotNo,
                        CastingDate = materialMaster.CastingDate
                    });
                }
            }

            foreach (var materialMarkingNo in db.MaterialMasterMarkingNoes.Where(mk => beaconIds.Contains(mk.BeaconID)))
            {
                if (!result.Select(r => r.BeaconID).Contains(materialMarkingNo.BeaconID))
                {
                    result.Add(new MaterialBeaconData
                    {
                        Project = materialMarkingNo.Project,
                        MarkingNo = materialMarkingNo.MarkingNo,
                        InInventory = true,
                        PassQC = true,
                        BeaconID = materialMarkingNo.BeaconID,
                        Status = string.Empty,
                        LotNo = materialMarkingNo.LotNo,
                        CastingDate = materialMarkingNo.CastingDate
                    });
                }
            }

            return Ok(result);
        }
        [Route("api/Material/MarkingNoWithBeacon")]
        public IHttpActionResult GetMarkingNoByBeaconIds([FromUri]string[] beaconIds)
        {
            if (beaconIds == null)
            {
                return BadRequest("Beacon Ids cannot be empty");
            }

            IQueryable<MaterialMasterMarkingNo> result = db.MaterialMasterMarkingNoes.Where(mmm => beaconIds.Contains(mmm.BeaconID));

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }
        // GET: api/Material/Detail?Status=xxx
        [Route("api/Material/MaterialForInstallation")]
        public IHttpActionResult GetMaterialForInstallation()
        {
            IQueryable<MaterialMaster> result = db.MaterialMasters.Where(mm =>
            mm.Status.Equals("Delivered", StringComparison.InvariantCultureIgnoreCase));

            var failedMaterials = db.MaterialDetails.Where(md => md.QCStatus == "Fail" && md.Stage == "Delivered").Select(md => md.MaterialNo).Distinct();

            result = result.Where(mm => !failedMaterials.Contains(mm.MaterialNo));

            if (result == null || result.Count() == 0)
            {
                return NotFound();
            }

            return Ok(result);
        }
        // GET: api/Material/Detail?Status=xxx
        [Route("api/Material/MaterialForEnrolment")]
        public IHttpActionResult GetMaterialForEnrolment([FromUri]string vendorName)
        {
            /*
            IQueryable<MaterialMaster> result = db.MaterialMasters.Where(mm =>
            mm.Status.Equals("Requested", StringComparison.InvariantCultureIgnoreCase) ||
            mm.Status.Equals("Pending", StringComparison.InvariantCultureIgnoreCase));
            */

            List<MaterialForEnrolmentData> listMaterial = new List<MaterialForEnrolmentData>();
            foreach(var materialMaster in db.MaterialMasters.Where(mm => mm.Status.Equals("Requested", StringComparison.InvariantCultureIgnoreCase))){
                listMaterial.Add(new MaterialForEnrolmentData
                {
                    Project = materialMaster.Project,
                    MarkingNo = materialMaster.MarkingNo,
                    MRFNo = materialMaster.MRFNo,
                    MaterialType = materialMaster.MaterialType,
                    LotNo = 0
                });
            }
            foreach(var materialMarking in db.MaterialMasterMarkingNoes.Where(mk => mk.CreatedBy == vendorName))
            {
                listMaterial.Add(new MaterialForEnrolmentData
                {
                    Project = materialMarking.Project,
                    MarkingNo = materialMarking.MarkingNo,
                    MRFNo = null,
                    MaterialType = materialMarking.MaterialType,
                    LotNo = materialMarking.LotNo.HasValue ? materialMarking.LotNo.Value : 0
                });
            }
            foreach (var materialMaster in db.MaterialMasters.Where(mm => mm.Status.Equals("Pending", StringComparison.InvariantCultureIgnoreCase)))
            {
                if (listMaterial.Where(lm => lm.Project == materialMaster.Project && lm.MarkingNo == materialMaster.MarkingNo && lm.MaterialType == materialMaster.MaterialType).Count() == 0)
                {
                    listMaterial.Add(new MaterialForEnrolmentData
                    {
                        Project = materialMaster.Project,
                        MarkingNo = materialMaster.MarkingNo,
                        MRFNo = null,
                        MaterialType = materialMaster.MaterialType,
                        LotNo = 0
                    });
                }
            }

            var maxLotNoMaterial = db.MaterialMasters.Where(mm => mm.UpdatedBy == vendorName).OrderByDescending(mm => mm.LotNo).FirstOrDefault();
            var maxLotNoMaterialMarking = db.MaterialMasterMarkingNoes.Where(mk => mk.CreatedBy == vendorName).OrderByDescending(mk => mk.LotNo).FirstOrDefault();

            int maxLotNo1 = maxLotNoMaterial == null ? 0 : maxLotNoMaterial.LotNo.Value;
            int maxLotNo2 = maxLotNoMaterialMarking == null ? 0 : maxLotNoMaterialMarking.LotNo.Value;

            var result = new
            {
                MaxLotNo = maxLotNo1 > maxLotNo2 ? maxLotNo1 : maxLotNo2,
                ListMaterial = listMaterial
            };
            
            return Ok(result);
        }

        // GET: api/Material/Detail?Status=xxx
        [Route("api/Material/MasterPendingQC")]
        public IHttpActionResult GetMaterialPendingQC()
        {
            IQueryable<MaterialMaster> result = null;

            var materialNos = db.MaterialDetails
                .Where(md => md.Stage.Equals("Produced", StringComparison.InvariantCultureIgnoreCase) &&
                string.IsNullOrEmpty(md.QCStatus))
                .Select(md => md.MaterialNo)
                .Distinct();

            if (materialNos == null)
            {
                return NotFound();
            }
            else
            {
                result = db.MaterialMasters.Where(mm => materialNos.Contains(mm.MaterialNo));
            }

            return Ok(result);
        }

        // POST: api/Material/Master/UpdateByVendor
        [ResponseType(typeof(int))]
        [HttpPost]
        [Route("api/Material/Master/UpdateByVendor")]
        public async Task<IHttpActionResult> PostMaterialByVendor(IEnumerable<VendorUpdateData> vendorData)
        {
            var currentDT = DateTime.Now;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            foreach (var item in vendorData)
            {
                if (string.IsNullOrEmpty(item.MRFNo) && string.IsNullOrEmpty(item.MarkingNo))
                    continue;
                else if (string.IsNullOrEmpty(item.MRFNo))
                {
                    var data = new MaterialMasterMarkingNo
                    {
                        Project = item.Project,
                        BeaconID = item.BeaconID,
                        CastingDate = item.CastingDate,
                        LotNo = item.LotNo,
                        MarkingNo = item.MarkingNo,
                        IsActive = true,
                        CreatedBy = item.UserName,
                        CreatedDate = currentDT
                    };
                    db.MaterialMasterMarkingNoes.Add(data);
                }
                else
                {
                    var materialMaster = await db.MaterialMasters
                        .FirstOrDefaultAsync(mm => mm.MarkingNo.Equals(item.MarkingNo) && mm.MRFNo.Equals(item.MRFNo));

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
                        if (locationAssociation != null)
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
                            IsQC = true,
                            QCStatus = item.PassQC ? "Pass" : "Fail",
                            QCBy = item.UserName,
                            QCDate = currentDT,
                            MaterialNo = materialMaster.MaterialID.ToString(),
                            BeaconID = item.BeaconID,
                            Location = location
                        };

                        db.MaterialDetails.Add(materialDetail);

                        var materialMarkingNo = db.MaterialMasterMarkingNoes.Where(mk => mk.BeaconID == materialDetail.BeaconID).FirstOrDefault();
                        if (materialMarkingNo != null)
                            db.MaterialMasterMarkingNoes.Remove(materialMarkingNo);
                    }
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
        // POST: api/Material/Master/UpdateBySite
        [ResponseType(typeof(int))]
        [HttpPost]
        [Route("api/Material/Master/UpdateBySite")]
        public async Task<IHttpActionResult> PostMaterialBySite(IEnumerable<SiteUpdateData> siteData)
        {
            var currentDT = DateTime.Now;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            foreach (var item in siteData)
            {
                if (0 <= item.Status && item.Status <= 1)
                {
                    string strStatus = string.Empty;
                    switch (item.Status)
                    {
                        case 0:
                            strStatus = "Delivered";
                            break;
                        case 1:
                            strStatus = "Installed";
                            break;
                        default:
                            continue;
                    }
                    var materialMaster = await db.MaterialMasters
                        .FirstOrDefaultAsync(mm => mm.MaterialID == item.MaterialID);

                    if (materialMaster == null)
                        continue;
                    else
                    {
                        materialMaster.Status = strStatus;
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
                        if (locationAssociation != null)
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
                            case 0:
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
                            case 1:
                                location = string.Format("{0} > {1} > {2} > {3}", targetLocation, materialMaster.Block, materialMaster.Level, materialMaster.Zone);
                                break;
                            default:
                                break;
                        }

                        var materialDetail = new MaterialDetail
                        {
                            MarkingNo = materialMaster.MarkingNo,
                            LocationID = materialMaster.LocationID,
                            Stage = strStatus,
                            CreatedBy = item.UserName,
                            CreatedDate = DateTime.Today,
                            IsQC = true,
                            QCStatus = item.PassQC ? "Pass" : "Fail",
                            QCBy = item.UserName,
                            QCDate = currentDT,
                            MaterialNo = materialMaster.MaterialID.ToString(),
                            BeaconID = materialMaster.BeaconID,
                            Location = location
                        };

                        db.MaterialDetails.Add(materialDetail);
                    }
                }
            }

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException exc)
            {
                return Ok(siteData.Count() - exc.Entries.Where(entity => entity.GetType().Equals(typeof(MaterialMaster))).Count());
            }

            return Ok(siteData.Count());
        }

        private bool MaterialMasterExists(int id)
        {
            return db.MaterialMasters.Count(e => e.MaterialID == id) > 0;
        }
    }

    public class MaterialBeaconData
    {
        public string BeaconID { get; set; }
        public bool InInventory { get; set; }
        public string Status { get; set; }
        public bool PassQC { get; set; }
        public string Project { get; set; }
        public string MarkingNo { get; set; }
        public int? LotNo { get; set; }
        public DateTime? CastingDate { get; set; }
    }

    public class MaterialForEnrolmentData
    {
        public string Project { get; set; }
        public string MRFNo { get; set; }
        public string MarkingNo { get; set; }
        public string MaterialType { get; set; }
        public int LotNo { get; set; }
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
        public string MarkingNo { get; set; }
        public string MRFNo { get; set; }
        public string Project { get; set; }
        public bool PassQC { get; set; }
    }

    public class SiteUpdateData
    {
        public int MaterialID { get; set; }
        public string UserName { get; set; }
        public int UserLocationID { get; set; }
        /*
         * Status:
         * 0 - Delivered
         * 1 - Installed
         */
        public int Status { get; set; }
        public bool PassQC { get; set; }
    }
}
