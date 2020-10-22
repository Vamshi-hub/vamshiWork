using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using astorWorkDAO;
using astorWorkMaterialTracking.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;
using astorWorkShared.Services;

namespace astorWorkMaterialTracking.Common
{
    public class CommonMaterialInfoAuditsController : CommonController
    {
        public CommonMaterialInfoAuditsController(astorWorkDbContext context) : base(context)
        {
        }

        //protected MaterialDetail CreateMaterialDetail(MaterialMaster materialMaster) {
        //    MaterialDetail materialDetail = new MaterialDetail();
        //    materialDetail.ID = materialMaster.ID;
        //    materialDetail.MarkingNo = materialMaster.MarkingNo;
        //    materialDetail.Block = materialMaster.Block;
        //    materialDetail.Level = materialMaster.Level;
        //    materialDetail.Zone = materialMaster.Zone;
        //    materialDetail.MaterialType = materialMaster.MaterialType;
        //    materialDetail.VendorName = materialMaster.Vendor?.Name;
        //    materialDetail.TrackerType = materialMaster.Tracker?.Type;
        //    materialDetail.TrackerTag = materialMaster.Tracker?.Tag;

        //    if (materialMaster.MRF != null)
        //        materialDetail.ExpectedDeliveryDate = materialMaster.MRF.ExpectedDeliveryDate;

        //    MaterialInfoAudit materialInfoAudit = _context.MaterialInfoAudit.SingleOrDefault(m => m.Material.ID == materialMaster.ID);

        //    if (materialInfoAudit != null)
        //    {
        //        materialDetail.Remarks = materialInfoAudit.Remarks;
        //        materialDetail.ExpectedDeliveryDate = materialInfoAudit.ExpectedDeliveryDate;
        //    }

        //    return materialDetail;
        //}
    }
}