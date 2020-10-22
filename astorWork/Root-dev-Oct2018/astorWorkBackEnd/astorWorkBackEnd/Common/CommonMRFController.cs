using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using astorWorkBackEnd.Models;
using astorWorkDAO;
using Microsoft.EntityFrameworkCore;
using System.Data;
using astorWorkShared.Services;
using astorWorkShared.MultiTenancy;
using System.Text;
using System.IO;
using DinkToPdf.Contracts;

namespace astorWorkBackEnd.Common
{
    public class CommonMRFController : CommonController
    {
        protected IAstorWorkEmail _emailService;
        protected TenantInfo _tenant;
        protected IConverter _converter;

        public CommonMRFController(astorWorkDbContext context) : base(context)
        {
        }

        public List<Location> CreateLocationsList(IOrderedQueryable<Object> materialMaster)
        {
            //List<Location> locationsList = new List<Location>();
            //Location currLevel = null;

            // foreach (var m in materialMaster)
            //{
            //    // New Level
            //    if (currLevel == null)
            //        currLevel = CreateNewLocation(currLevel, m.Level, m.Zone);
            //    else if (currLevel.Level != m.Level)
            //    {
            //        locationsList.Add(currLevel);
            //        currLevel = CreateNewLocation(currLevel, m.Level, m.Zone);
            //    }
            //    // Zone is within current level
            //    else
            //        currLevel.Zones.Add(m.Zone);
            //}

            //// Add the Level with the corresponding Zones to the Locations list
            //locationsList.Add(currLevel);

            return null;
        }

        public Location CreateNewLocation(Location currLevel, string level, string zone)
        {
            currLevel = new Location();
            currLevel.Level = level;
            currLevel.Zones = new List<string>();
            currLevel.Zones.Add(zone);

            return currLevel;
        }

        public MRF CreateNewMRF(MRFMaster m)
        {
            MRF mrf = new MRF();
            mrf.ID = m.ID;
            mrf.MrfNo = m.MRFNo;
            mrf.VendorName = m.Materials.FirstOrDefault().Vendor.Name;
            mrf.Block = m.Materials.FirstOrDefault().Block;
            mrf.Level = m.Materials.FirstOrDefault().Level;
            mrf.Zone = m.Materials.FirstOrDefault().Zone;
            mrf.PlannedCastingDate = m.PlannedCastingDate;

            mrf.MaterialTypes = GetMaterialTypes(mrf.MrfNo);
            mrf.Progress = GetProgress(mrf.ID);

            mrf.OrderDate = m.OrderDate;

            return mrf;
        }

        public List<Material> CreateMaterialList(IEnumerable<MaterialMaster> materialMaster)
        {
            var materialMasters = materialMaster
                            .Select(m => new { m.Vendor.ID, m.MaterialType })
                            .Distinct()
                            .OrderBy(m => m.ID);

            List<Material> materialsList = new List<Material>();
            Material materialWithCurrVendorID = null;

            foreach (var m in materialMasters)
            {
                if (materialWithCurrVendorID == null)
                    materialWithCurrVendorID = CreateNewMaterial(materialWithCurrVendorID, m.ID, m.MaterialType);
                else if (materialWithCurrVendorID.VendorID != m.ID)
                {
                    materialsList.Add(materialWithCurrVendorID);
                    materialWithCurrVendorID = CreateNewMaterial(materialWithCurrVendorID, m.ID, m.MaterialType);
                }
                else
                    materialWithCurrVendorID.MaterialTypes.Add(m.MaterialType);
            }

            materialsList.Add(materialWithCurrVendorID);

            return materialsList;
        }

        public Material CreateNewMaterial(Material currVendorID, int vendorID, string materialType)
        {
            currVendorID = new Material();
            currVendorID.VendorID = vendorID;
            currVendorID.MaterialTypes = new List<string>();
            currVendorID.MaterialTypes.Add(materialType);

            return currVendorID;
        }

        public List<string> GetMaterialTypes(string mRFNo)
        {
            return _context.MaterialMaster
                    .Where(mm => mm.MRF.MRFNo == mRFNo)
                    .Select(mm => mm.MaterialType)
                    .Distinct()
                    .ToList();
        }

        public async Task<MRFMaster> CreateMRFInDb(int projectID, MRF mrf)
        {
            MRFMaster mrfMaster = CreateMRFMaster(mrf);
            mrfMaster = UpdateMatchingMaterials(projectID, mrfMaster, mrf);

            if (mrfMaster.Materials != null)
            {
                _context.MRFMaster.Add(mrfMaster);
                await _context.SaveChangesAsync();
                //materialCount = mrfMaster.Materials.Count();
            }

            return mrfMaster;
        }

        public MRFMaster CreateMRFMaster(MRF mRF)
        {
            var mrf = _context.MRFMaster
                           .Where(m => m.OrderDate.Year == DateTime.Now.Year)
                           .OrderByDescending(m => m.MRFNo)
                           .Select(m => m.MRFNo);

            int MRFCount = mrf.Distinct().Count() + 1;

            MRFMaster mRFMaster = new MRFMaster();
            string MRFNo = "MRF-" + DateTime.Now.Year + "-" + (MRFCount).ToString().PadLeft(5, '0');

            // To ensure that there will not be a duplicate MRF no. created
            while (_context.MRFMaster.Where(m => m.MRFNo == MRFNo).Count() > 0)
                MRFNo = "MRF-" + DateTime.Now.Year + "-" + (MRFCount + 1).ToString().PadLeft(5, '0');

            mRFMaster.MRFNo = MRFNo;
            mRFMaster.OrderDate = mRF.OrderDate;
            mRFMaster.PlannedCastingDate = mRF.PlannedCastingDate;
            mRFMaster.ExpectedDeliveryDate = mRF.ExpectedDeliveryDate;
            UserMRFAssociation userMRFAssociation = new UserMRFAssociation();

            var contactPeople = _context.UserMaster
                                .Where(u => mRF.OfficerUserIDs.Contains(u.ID));
            if (contactPeople != null)
            {
                mRFMaster.UserMRFAssociations = new List<UserMRFAssociation>();
                userMRFAssociation.MRF = mRFMaster;

                foreach (UserMaster u in contactPeople)
                {
                    userMRFAssociation = new UserMRFAssociation();
                    userMRFAssociation.User = u;
                    mRFMaster.UserMRFAssociations.Add(userMRFAssociation);
                }
            }

            UserMaster user = _context.GetUserFromHttpContext(HttpContext);
            mRFMaster.CreatedBy = user;
            mRFMaster.CreatedDate = DateTime.Now;
            mRFMaster.UpdatedBy = user;
            mRFMaster.UpdatedDate = DateTime.Now;

            return mRFMaster;
        }

        public MRFMaster UpdateMatchingMaterials(int ProjectID, MRFMaster mRFMaster, MRF mRF)
        {
            var materials = _context.MaterialMaster
                            .Include(m => m.StageAudits)
                            .Where(m => m.Project.ID == ProjectID && m.Block == mRF.Block
                            && m.Level == mRF.Level && m.Zone == mRF.Zone
                            && mRF.MaterialTypes.Contains(m.MaterialType)
                            && m.Vendor.ID == mRF.VendorID && m.MRF == null && m.StageAudits.Count == 0);


            foreach (MaterialMaster m in materials)
            {
                m.MRF = mRFMaster;
                if (mRFMaster.Materials == null)
                    mRFMaster.Materials = new List<MaterialMaster>();
                if (m.StageAudits.Count == 0)
                    mRFMaster.Materials.Add(m);
            }

            return mRFMaster;
        }

        public bool MRFMasterExists(int ProjectID, MRF mRF)
        {
            return _context.MaterialMaster.Any(m => m.Project.ID == ProjectID && m.Block == mRF.Block
                            && m.Level == mRF.Level && m.Zone == mRF.Zone
                            && mRF.MaterialTypes.Contains(m.MaterialType)
                            && m.Vendor.ID == mRF.VendorID && m.MRF != null);
        }

        public string CreateMRFDocument(MRFMaster mrfMaster)
        {
            List<string> columnNames = new List<string>();
            columnNames.Add("Marking No.");
            columnNames.Add("Material Type");

            DataTable dataTable = _emailService.CreateDataTable("Materials", columnNames);

            // Create three new DataRow objects and add 
            // them to the DataTable
            DataRow row;
            foreach (MaterialMaster materialMaster in mrfMaster.Materials)
            {
                row = dataTable.NewRow();
                row[columnNames[0]] = materialMaster.MarkingNo;
                row[columnNames[1]] = materialMaster.MaterialType;
                dataTable.Rows.Add(row);
            }

            string filePath = string.Format("{0}MRF {1} Created.pdf", Path.GetTempPath(), mrfMaster.MRFNo);
            string header = string.Format("MRF {0} Created", mrfMaster.MRFNo);
            string subHeader = DateTime.Now.ToString("dd/MM/yyyy");

            return _emailService.CreateDocument(filePath, header, subHeader, CreateTblContent(mrfMaster), _converter);
        }

        public string CreateTblContent(MRFMaster mrfMaster) {
            List<string> columnNames = new List<string>();
            columnNames.Add("Marking No.");
            columnNames.Add("Material Type");

            DataTable dataTable = _emailService.CreateDataTable("Materials", columnNames);

            // Create three new DataRow objects and add 
            // them to the DataTable
            DataRow row;
            foreach (MaterialMaster materialMaster in mrfMaster.Materials)
            {
                row = dataTable.NewRow();
                row[columnNames[0]] = materialMaster.MarkingNo;
                row[columnNames[1]] = materialMaster.MaterialType;
                dataTable.Rows.Add(row);
            }

            StringBuilder sb = new StringBuilder();

            sb.Append(
            @"
            <table border = '1' cellspacing = '0' cellpadding = '0' style = 'border-collapse:collapse; border: none; font-family:arial'>
                <tr>");

            string[] colNames = new string[] { "Marking No.", "Material Type" };

            for (int i = 0; i < colNames.Length; i++)
                sb.AppendFormat(string.Format(@"<td width = '200' valign = 'top' style = 'width:100.1pt;border:solid windowtext 1.0pt;padding:0cm 5.4pt 0cm 5.4pt; font-size:10.0pt' >{0}</td>", colNames[i]));

            sb.AppendFormat("</tr>");

            foreach (MaterialMaster materialMaster in mrfMaster.Materials)
            {
                string[] colData = new string[] { materialMaster.MarkingNo, materialMaster.MaterialType };

                sb.AppendFormat(@"<tr>");

                for (int i = 0; i < colData.Length; i++)
                    sb.AppendFormat(string.Format(@"<td width = '200' valign = 'top' style = 'width:100.1pt;border:solid windowtext 1.0pt;padding:0cm 5.4pt 0cm 5.4pt; font-size:10.0pt' >{0}</td>", colData[i]));

                sb.AppendFormat(@"<tr>");
            }

            sb.Append(@"</table>");

            return sb.ToString();
        }
    }
}