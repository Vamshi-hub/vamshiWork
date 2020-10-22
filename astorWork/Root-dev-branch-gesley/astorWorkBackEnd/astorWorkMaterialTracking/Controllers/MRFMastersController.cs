using astorWorkDAO;
using astorWorkMaterialTracking.Common;
using astorWorkMaterialTracking.Models;
using astorWorkShared.GlobalExceptions;
using astorWorkShared.GlobalModels;
using astorWorkShared.GlobalResponse;
using astorWorkShared.Services;
using astorWorkShared.Utilities;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace astorWorkMaterialTracking.Controllers
{
    [Produces("application/json")]
    [Route("projects/{project_id}/mrfs")]
    public class MRFMastersController : CommonTrackerMastersController
    {
        private readonly string Module = "MRF";
        private IConverter _converter;
        private TenantInfo _tenant;
        private IAstorWorkEmail _emailService;

        public MRFMastersController(astorWorkDbContext context, IAstorWorkEmail emailService, TenantInfo tenantInfo, IConverter converter) : base(context)
        {
            _emailService = emailService;
            _tenant = tenantInfo;
            _converter = converter;
        }

        // GET: /projects/{project_id}/mrfs?block={block}&marking_no={marking_no}&vendor_id={vendor_id}
        [HttpGet]
        public async Task<List<MRF>> ListMRFs([FromRoute] int project_id, [FromQuery] string marking_no = "", [FromQuery] string block = "")
        {
            if (!ModelState.IsValid)
                throw new GenericException(ErrorMessages.BadRequest, ErrorMessages.BadRequestMsg());

            UserMaster user = await _context.GetUserFromHttpContext(HttpContext);

            List<MaterialMaster> materials = await GetMaterials(project_id, marking_no, block, user);

            if (materials == null || materials.Count() == 0)
                throw new GenericException(ErrorMessages.DbRecordNotFound, $"No MRF found!");

            List<MRF> mrfs = GetMRFs(materials);

            return SetMRFMaterialTypes(mrfs, materials);
        }

        // POST: projects/{project_id}/mrfs
        [HttpPost]
        public async Task<MRF> CreateMRF([FromRoute] int project_id, [FromBody] MRF mrf)
        {
            if (!ModelState.IsValid)
                throw new GenericException(ErrorMessages.BadRequest, ErrorMessages.BadRequestMsg());

            MRF mrfExist = await MRFExists(project_id, mrf);

            if (mrfExist != null && mrfExist.Block != null && mrfExist.Level != null && mrf.Zone != null)
            {
                string blocks = String.Join(", ", mrfExist.Block.ToArray());
                string levels = String.Join(", ", mrfExist.Level.ToArray());
                string zones = String.Join(", ", mrfExist.Zone.ToArray());
                throw new GenericException(ErrorMessages.DbDuplicateRecord, ErrorMessages.DbDuplicateRecordMsg(Module, "Block, Level, Zone", "Blocks " + blocks + "; Levels " + levels + "; Zones " + zones + " "));
            }

            MRFMaster mrfMaster = await CreateMRFInDb(project_id, mrf);
            List<UserMaster> receipients = await GetEmailReceipients(mrf);
            receipients.Add(await _context.GetUserFromHttpContext(HttpContext));

            await UpdateNotificationAudit(receipients, 0, 0, mrfMaster.ID.ToString());

            return new MRF { ID = mrfMaster.ID, MrfNo = mrfMaster.MRFNo, MaterialCount = mrfMaster.Materials.Count() };
        }

        // GET: projects/{projectID}/mrfs/location?block={block}
        [HttpGet("location")]
        public async Task<List<Location>> GetLocations([FromRoute] int project_id, [FromQuery] string block = "")
        {
            if (!ModelState.IsValid)
                throw new GenericException(ErrorMessages.BadRequest, ErrorMessages.BadRequestMsg());

            var materialMaster = await _context.MaterialMaster
                                               .Where(m => m.Project.ID == project_id && block.Contains(m.Block) && m.MRF == null) 
                                               .Select(m => new { m.Level, m.Zone })
                                               .Distinct()
                                               .OrderBy(m => m.Level.PadLeft(3)).ToListAsync();

            if (materialMaster == null)
                throw new GenericException(ErrorMessages.DbRecordNotFound, ErrorMessages.DBRecordNotFoundMsg("Location", "Block", block));
            List<Location> locations = new List<Location>();
            if (materialMaster.Count() == 0)
                return locations;

            Location currLevel = null;

            foreach (var m in materialMaster)
            {
                // New Level
                if (currLevel == null)
                    currLevel = CreateNewLocation(currLevel, m.Level, m.Zone);
                else if (currLevel.Level != m.Level)
                {
                    locations.Add(currLevel);
                    currLevel = CreateNewLocation(currLevel, m.Level, m.Zone);
                }
                // Zone is within current level
                else
                    currLevel.Zones.Add(m.Zone);
            }

            // Add the Level with the corresponding Zones to the Locations list
            locations.Add(currLevel);

            return locations;
        }

        // GET: projects/{projectID}/mrfs/material?block={block}&level={level}&zone={zone}
        [HttpGet("material")]
        public async Task<List<VendorMaterialType>> GetVendorsAndMaterialTypes([FromRoute] int project_id, [FromQuery] string block, [FromQuery] string level, [FromQuery] string zone)
        {
            if (!ModelState.IsValid)
                throw new GenericException(ErrorMessages.BadRequest, ErrorMessages.BadRequestMsg());

            string[] blocks = block.Split(',');
            string[] levels = level.Split(',');
            string[] zones = zone.Split(',');

            List<MaterialMaster> materials = await _context.MaterialMaster
                                                           .Include(m => m.Organisation)
                                                           .Include(m => m.MaterialType)
                                                           .Where(m => m.Project.ID == project_id
                                                                       && blocks.Contains(m.Block)
                                                                       && levels.Contains(m.Level)
                                                                       && zones.Contains(m.Zone)
                                                                       && m.MRF == null)
                                                           .ToListAsync();

            if (materials == null || materials.Count == 0)
                throw new GenericException(ErrorMessages.DbRecordNotFound, ErrorMessages.DBRecordNotFoundMsg("Material", "Block, Level, Zone", block + ", " + level + ", " + zone));

            List<VendorMaterialType> result = new List<VendorMaterialType>();
            foreach (int organisationID in materials.Select(m => m.OrganisationID).Distinct())
            {
                result.Add(new VendorMaterialType
                {
                    OrganisationId = organisationID,
                    MaterialTypes = materials.Where(m => m.OrganisationID == organisationID)
                                             .Select(m => m.MaterialType.Name)
                                             .Distinct()
                                             .ToList()
                });
            }

            return result;
        }

        // DELETE /projects/{project_id}/mrfs/{material_id}
        [HttpDelete("{id}")]
        public async Task<MRFMaster> DeleteMRFMaster([FromRoute] int id)
        {
            if (!ModelState.IsValid)
                throw new GenericException(ErrorMessages.BadRequest, ErrorMessages.BadRequestMsg());

            MRFMaster mrf = await _context.MRFMaster.Include(m => m.Materials).SingleOrDefaultAsync(m => m.ID == id);
            if (mrf == null)
                throw new GenericException(ErrorMessages.DbRecordNotFound, ErrorMessages.DBRecordNotFoundMsg(Module, "ID", id.ToString()));

            _context.MRFMaster.Remove(mrf);
            await _context.SaveChangesAsync();

            return mrf;
        }

        // GET: projects/{projectID}/mrfs/{id}/list-qr-code
        [HttpGet("{mrf_id}/list-qr-code")]
        public async Task<List<PrintQRItem>> GetListQRCode([FromRoute] int project_id, [FromRoute] int mrf_id)
        {
            List<PrintQRItem> result = new List<PrintQRItem>();
            if (!ModelState.IsValid)
                throw new GenericException(ErrorMessages.BadRequest, ErrorMessages.BadRequestMsg());

            UserMaster user = await _context.GetUserFromHttpContext(HttpContext);
            
            result = await _context.TrackerMaster.Include(m => m.Material)
                                                 .ThenInclude(mrf => mrf.MRF)
                                                 .Include(vm => vm.Material.Organisation)
                                                 .Include(mt => mt.Material.MaterialType)
                                                 .Where(t => t.Type == Enums.TagType.QR_Code.ToString().Replace('_', ' ')
                                                          && t.Material.MRF.ID == mrf_id
                                                          && t.Material.ProjectID == project_id)
                                                 .Select(t => new PrintQRItem
                                                 {
                                                     Tag = t.Tag,
                                                     VendorName = t.Material.Organisation.Name,
                                                     Block = t.Material.Block,
                                                     Level = t.Material.Level,
                                                     MarkingNo = t.Material.MarkingNo,
                                                     MaterialType = t.Material.MaterialType.Name,
                                                     Zone = t.Material.Zone,
                                                     ProjectManagerName = user.PersonName,
                                                     AssemblyLocation = t.Material.AssemblyLocation
                                                 })
                                                 .ToListAsync();

            if (result == null)
                throw new GenericException(ErrorMessages.DbRecordNotFound, ErrorMessages.DBRecordNotFoundMsg(Module, "ID", mrf_id.ToString()));

            return result;

        }

        protected Location CreateNewLocation(Location currLevel, string level, string zone)
        {
            currLevel = new Location();
            currLevel.Level = level;
            currLevel.Zones = new List<string>();
            currLevel.Zones.Add(zone);

            return currLevel;
        }

        protected async Task<MRF> CreateNewMRF(MRFMaster mrf)
        {
            return new MRF()
            {
                ID = mrf.ID,
                MrfNo = mrf.MRFNo,
                OrganisationName = mrf.Materials.FirstOrDefault().Organisation.Name,
                Block = mrf.Materials.FirstOrDefault().Block,
                Level = mrf.Materials.Select(mm => mm.Level).Distinct().ToList(),
                Zone = mrf.Materials.Select(mm => mm.Zone).Distinct().ToList(),
                PlannedCastingDate = mrf.PlannedCastingDate,
                MaterialTypes = await GetMaterialTypes(mrf.MRFNo),
                Progress = await GetProgress(mrf.ID),
                OrderDate = mrf.OrderDate
            };
        }

        protected List<VendorMaterialType> GetMaterialsVendorAndType(List<MaterialMaster> materialMaster)
        {
            var materialMasters = materialMaster
                            .Select(m => new { m.Organisation.ID, m.MaterialType.Name })
                            .Distinct()
                            .OrderBy(m => m.ID);

            List<VendorMaterialType> materials = new List<VendorMaterialType>();
            VendorMaterialType materialWithCurrOrganisationID = null;

            foreach (var m in materialMasters)
            {
                if (materialWithCurrOrganisationID == null)
                    materialWithCurrOrganisationID = CreateNewMaterial(materialWithCurrOrganisationID, m.ID, m.Name);
                else if (materialWithCurrOrganisationID.OrganisationId != m.ID)
                {
                    materials.Add(materialWithCurrOrganisationID);
                    materialWithCurrOrganisationID = CreateNewMaterial(materialWithCurrOrganisationID, m.ID, m.Name);
                }
                else
                    materialWithCurrOrganisationID.MaterialTypes.Add(m.Name);
            }

            materials.Add(materialWithCurrOrganisationID);

            return materials;
        }

        protected VendorMaterialType CreateNewMaterial(VendorMaterialType currOrganisationID, int organisationID, string materialType)
        {
            currOrganisationID = new VendorMaterialType();
            currOrganisationID.OrganisationId = organisationID;
            currOrganisationID.MaterialTypes = new List<string>();
            currOrganisationID.MaterialTypes.Add(materialType);

            return currOrganisationID;
        }

        protected async Task<List<string>> GetMaterialTypes(string mRFNo)
        {
            return await _context.MaterialMaster
                                 .Where(mm => mm.MRF.MRFNo == mRFNo)
                                 .Select(mm => mm.MaterialType.Name)
                                 .Distinct()
                                 .ToListAsync();
        }

        protected async Task<MRFMaster> CreateMRFInDb(int projectID, MRF mrf)
        {
            MRFMaster mrfMaster = await CreateMRFMaster(mrf);
            mrfMaster = await UpdateMatchingMaterials(projectID, mrfMaster, mrf);

            if (mrfMaster.Materials != null)
            {
                var config = _context.ConfigurationMaster.Where(C => C.Cofiguration.ToLower() == "QRFormat".ToLower()).FirstOrDefault();
                // CommonTrackerMastersController commonTrackerMastersController = new CommonTrackerMastersController(_context);
                foreach (MaterialMaster material in mrfMaster.Materials)
                    await GenerateAndAssociateQRCodes(material,config);

                _context.MRFMaster.Add(mrfMaster);
                await _context.SaveChangesAsync();
            }

            return mrfMaster;
        }

        protected async Task<MRFMaster> CreateMRFMaster(MRF mRF)
        {
            List<string> mrf = await _context.MRFMaster.Include(m => m.Materials).ThenInclude(d =>d.DrawingAssociations).ThenInclude(dw =>dw.Drawing)
                                             .Where(m => m.OrderDate.Year == DateTime.Now.Year)
                                             .OrderByDescending(m => m.MRFNo)
                                             .Select(m => m.MRFNo)
                                             .ToListAsync();

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

            List<UserMaster> contactPeople = await _context.UserMaster
                                                           .Where(u => mRF.OfficerUserIDs
                                                           .Contains(u.ID))
                                                           .ToListAsync();

            if (contactPeople != null)
            {
                mRFMaster.UserMRFAssociations = new List<UserMRFAssociation>();
                userMRFAssociation.MRF = mRFMaster;

                foreach (UserMaster contactPerson in contactPeople)
                {
                    userMRFAssociation = new UserMRFAssociation();
                    userMRFAssociation.User = contactPerson;
                    mRFMaster.UserMRFAssociations.Add(userMRFAssociation);
                }
            }

            UserMaster user = await _context.GetUserFromHttpContext(HttpContext);
            mRFMaster.CreatedBy = user;
            mRFMaster.CreatedDate = DateTime.Now;
            mRFMaster.UpdatedBy = user;
            mRFMaster.UpdatedDate = DateTime.Now;

            return mRFMaster;
        }

        protected async Task<MRFMaster> UpdateMatchingMaterials(int ProjectID, MRFMaster mRFMaster, MRF mRF)
        {
            List<MaterialMaster> materials = await _context.MaterialMaster.Include(m => m.StageAudits)
                                                                          .Include(m => m.MaterialType)
                                                                          .Include(m => m.Organisation)
                                                                          .Where(m => m.Project.ID == ProjectID && mRF.Block.Contains(m.Block)
                                                                                   && mRF.Level.Contains(m.Level) && mRF.Zone.Contains(m.Zone)
                                                                                   && mRF.MaterialTypes.Contains(m.MaterialType.Name)
                                                                                   && m.Organisation.ID == mRF.OrganisationId && m.MRF == null
                                                                                   && m.StageAudits.Count == 0)
                                                                          .ToListAsync();

            mRFMaster.Materials = materials;

            return mRFMaster;
        }

        protected async Task<List<MaterialMaster>> GetMaterials()
        {
            return await _context.MaterialMaster.Include(mm => mm.MRF)
                                          .Include(mm => mm.Organisation)
                                          .Include(mm => mm.StageAudits)
                                          .ThenInclude(sa => sa.Stage)
                                          .Include(mm => mm.MaterialType)
                                          .ToListAsync();
        }

        protected async Task<List<MaterialMaster>> GetMaterials(int projectID, string markingNo, string block, UserMaster user)
        {
            List<MaterialMaster> materials = await GetMaterials();

            // When marking no. is given, return MRFs for vendor
            if (!string.IsNullOrEmpty(markingNo))
                materials = materials.Where(mm => mm.MRF != null && mm.ProjectID == projectID && mm.MarkingNo == markingNo && mm.StageAudits.Count == 0).ToList();
            else if (!string.IsNullOrEmpty(block))
                materials = materials.Where(mm => mm.MRF != null && mm.ProjectID == projectID && mm.Block == block).ToList();
            else
                materials = materials.Where(mm => mm.MRF != null && mm.ProjectID == projectID).ToList();

            return GetVendorMaterials(materials, user);
        }

        protected List<MaterialMaster> GetVendorMaterials(List<MaterialMaster> materials, UserMaster user)
        {
            if ((user.RoleID == 7 || user.RoleID == 8) && user.Organisation != null)
                return materials.Where(mm => mm.OrganisationID == user.Organisation.ID).ToList();

            return materials;
        }

        protected List<string> GetMRFMaterialTypes(IEnumerable<MaterialMaster> materials, int mrfID)
        {
            return materials.Where(mm => mm.MRF.ID == mrfID).Select(mm => mm.MaterialType.Name).Distinct().ToList();
        }

        protected List<MRF> SetMRFMaterialTypes(List<MRF> mrfs, List<MaterialMaster> materials)
        {
            foreach (MRF mrf in mrfs)
            {
                mrf.MaterialTypes = GetMRFMaterialTypes(materials, mrf.ID);
            }

            return mrfs;
        }

        protected List<MRF> GetMRFs(List<MaterialMaster> materials)
        {
            return materials.GroupBy(mm => mm.MRF).Select(mrf => new MRF
            {
                ID = mrf.Key.ID,
                MrfNo = mrf.Key.MRFNo,
                OrganisationName = mrf.First().Organisation.Name,
                Block = mrf.FirstOrDefault().Block,
                Level = mrf.Select(m => m.Level).Distinct().ToList(),
                Zone = mrf.Select(m => m.Zone).Distinct().ToList(),
                Progress = mrf.Key.MRFCompletion,
                PlannedCastingDate = mrf.Key.PlannedCastingDate,
                OrderDate = mrf.Key.OrderDate,
                ExpectedDeliveryDate = mrf.Key.ExpectedDeliveryDate
            }).Distinct().OrderByDescending(mrf => mrf.PlannedCastingDate).ToList();
        }

        protected async Task<List<MaterialMaster>> GetMaterials(int projectID, string block, string level, string zone)
        {
            return await _context.MaterialMaster
                                 .Include(m => m.Organisation)
                                 .Where(m => m.Project.ID == projectID
                                          && m.Block == block && m.Level == level
                                          && m.Zone == zone)
                                 .ToListAsync();
        }

        protected async Task<List<UserMaster>> GetEmailReceipients(MRF mrf)
        {
            return await _context.UserMaster.Where(u => mrf.OfficerUserIDs.Contains(u.ID) || u.ID == mrf.CreatedByUserID).ToListAsync();
        }

        protected async Task<MRF> MRFExists(int ProjectID, MRF mRF)
        {
            List<MaterialMaster> materialMasters = await _context.MaterialMaster.Where(m => m.Project.ID == ProjectID
                                                                                   && mRF.Block.Contains(m.Block)
                                                                                   && mRF.Level.Contains(m.Level) && mRF.Zone.Contains(m.Zone)
                                                                                   && mRF.MaterialTypes.Contains(m.MaterialType.Name)
                                                                                   && m.Organisation.ID == mRF.OrganisationId && m.MRF != null)
                                                                                   .ToListAsync();

            var materials = materialMasters.Select(m => new { m.Block, m.Level, m.Zone }).Distinct().ToList();

            MRF mrf = new MRF();

            mrf.Level = new List<string>();
            mrf.Zone = new List<string>();

            foreach (var material in materials)
            {
                mrf.Block = material.Block;
                mrf.Level.Add(material.Level);
                mrf.Zone.Add(material.Zone);
            }

            return mrf;
        }
    }
}