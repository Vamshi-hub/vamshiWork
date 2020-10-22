using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using astorWorkDAO;
using astorWorkShared.GlobalResponse;
using astorWorkMaterialTracking.Common;
using astorWorkMaterialTracking.Models;
using EFCore.BulkExtensions;
using astorWorkShared.Utilities;
using astorWorkShared.GlobalExceptions;

namespace astorWorkMaterialTracking.Controllers
{
    [Produces("application/json")]
    [Route("material-stages")]
    public class MaterialStageMastersController : Controller
    {
        private readonly string Module = "materialstage";
        private astorWorkDbContext _context;

        public MaterialStageMastersController(astorWorkDbContext context)
        {
            _context = context;
        }

        // GET: material-stages
        [HttpGet]
        public async Task<IEnumerable<MaterialStage>> ListMaterialStages()
        {
            List<MaterialStage> materialStages = new List<MaterialStage>();
            List<MaterialStageMaster> stages = await _context.MaterialStageMaster.ToListAsync();
            Parallel.ForEach(stages, stage =>
            {
                materialStages.Add(CreateMaterialStage(stage));
            });
            return materialStages.OrderBy(p => p.Order);
        }

        // GET: api/MaterialStageMasters/5
        [HttpGet("{id}")]
        public async Task<MaterialStage> GetMaterialStage([FromRoute] int id)
        {
            if (!ModelState.IsValid)
                throw new GenericException(ErrorMessages.BadRequest, ErrorMessages.BadRequestMsg());

            MaterialStageMaster materialStageMaster = await _context.MaterialStageMaster.FirstOrDefaultAsync(m => m.ID == id);

            if (materialStageMaster == null)
                throw new GenericException(ErrorMessages.DbRecordNotFound, ErrorMessages.DBRecordNotFoundMsg(Module, "Stage", id.ToString()));

            return CreateMaterialStage(materialStageMaster);
        }

        // POST: material-stages
        [HttpPost]
        public async Task<MaterialStageMaster> CreateMaterialStage([FromBody] MaterialStage materialStage)
        {
            if (!ModelState.IsValid)
                throw new GenericException(ErrorMessages.BadRequest, ErrorMessages.BadRequestMsg());

            try
            {
                if (MaterialStageMasterExists(materialStage.Name))
                    throw new GenericException(ErrorMessages.DbDuplicateRecord, ErrorMessages.DbDuplicateRecordMsg("Material Stage", "Name", materialStage.Name));
                else
                {
                    int order = GetNextStageOrder(materialStage.NextStageID);

                    MaterialStageMaster materialStageMaster = CreateMaterialStageMaster(materialStage, order);

                    await AddMaterialStage(materialStageMaster, order);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception exc)
            {
                Console.Write(exc.StackTrace);
                throw new GenericException(ErrorMessages.BadRequest, ErrorMessages.BadRequestMsg());
            }

            return _context.MaterialStageMaster.OrderByDescending(s => s.ID).First();
        }

        // PUT: material-stages/5
        [HttpPut("{id}")]
        public async Task<int> EditMaterialStageDetail([FromRoute] int id, [FromBody] MaterialStage materialStage)
        {
            if (!ModelState.IsValid)
                throw new GenericException(ErrorMessages.BadRequest, ErrorMessages.BadRequestMsg());

            await UpdateMaterialStage(id, materialStage, materialStage.Order);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!MaterialStageMasterExists(id))
                    throw new GenericException(ErrorMessages.DbRecordNotFound, ErrorMessages.DBRecordNotFoundMsg(Module, "Stage", id.ToString()));
                else
                    throw new GenericException(ErrorMessages.DbConcurrentUpdate, ex.Message);
            }

            return id;
        }


        // POST: material-stages/stage-list
        [HttpPost("stage-list")]
        public async Task<List<MaterialStage>> EditMaterialStageList([FromBody] List<MaterialStage> materialStages)
        {
            if (!ModelState.IsValid)
                throw new GenericException(ErrorMessages.BadRequest, ErrorMessages.BadRequestMsg());

            int order = 0;

            if (!IsCorrectOrder(materialStages))
                throw new GenericException(ErrorMessages.DbValidationError, "Stage order is not valid!");

            foreach (MaterialStage materialStage in materialStages)
            {
                order = order + 1;

                MaterialStageMaster materialStageMaster = CreateMaterialStageMaster(materialStage, order);

                if (materialStage.ID == 0)
                    await AddMaterialStage(materialStageMaster, order);
                else
                    await UpdateMaterialStage(materialStage.ID, materialStage, order);
            }

            _context.SaveChanges();

            //detete stages
            //var removelist= _context.MaterialStageMaster.Except(materialStages);
            // foreach (var rm in removelist)
            //     _context.MaterialStageMaster.Remove(rm);
            // _context.SaveChangesAsync();

            return materialStages;
        }

        // DELETE: api/MaterialStageMasters/5
        [HttpDelete("{id}")]
        public async Task<MaterialStageMaster> DeleteMaterialStageMaster([FromRoute] int id)
        {
            if (!ModelState.IsValid)
                throw new GenericException(ErrorMessages.BadRequest, ErrorMessages.BadRequestMsg());

            MaterialStageMaster materialStageMaster = await _context.MaterialStageMaster.FindAsync(id);

            if (materialStageMaster == null)
                throw new GenericException(ErrorMessages.DbRecordNotFound, ErrorMessages.DBRecordNotFoundMsg("Material Stage", "ID", id.ToString()));

            _context.MaterialStageMaster.Remove(materialStageMaster);
            await _context.SaveChangesAsync();

            ReorderMaterialStages(1, true);
            await _context.SaveChangesAsync();

            return materialStageMaster;
        }
        protected bool MaterialStageMasterExists(int id)
        {
            return _context.MaterialStageMaster.Any(e => e.ID == id);
        }

        protected bool MaterialStageMasterExists(string name)
        {
            return _context.MaterialStageMaster.Any(s => s.Name == name);
        }


        protected async Task AddMaterialStage(MaterialStageMaster materialStageMaster, int order)
        {
            ReorderMaterialStages(order);
            await _context.MaterialStageMaster.AddAsync(materialStageMaster);
        }

        protected int GetNextStageOrder(int nextStageID)
        {
            MaterialStageMaster materialStageMaster = _context.MaterialStageMaster.Where(s => s.ID == nextStageID).FirstOrDefault();
            if (materialStageMaster == null)
                return 0;
            return materialStageMaster.Order;
        }

        protected async Task UpdateMaterialStage(int id, MaterialStage materialStage, int order)
        {
            MaterialStageMaster materialStageMaster = await _context.MaterialStageMaster.Where(s => s.ID == id).FirstOrDefaultAsync();

            if (order > 0)
                materialStageMaster.Order = order;
            materialStageMaster.Name = materialStage.Name;
            materialStageMaster.Colour = materialStage.Colour;
            materialStageMaster.MaterialTypes = (materialStage.MaterialTypes == null) ? null : string.Join(',', materialStage.MaterialTypes);
            materialStageMaster.MilestoneID = materialStage.MilestoneID;
            materialStageMaster.CanIgnoreQC = materialStage.CanIgnoreQC;
            _context.Entry(materialStageMaster).State = EntityState.Modified;
        }

        protected void ReorderMaterialStages(int order, bool isDelete = false)
        {
            IEnumerable<MaterialStageMaster> materialStages = _context.MaterialStageMaster.Where(s => s.Order >= order).OrderBy(s => s.Order);

            foreach (MaterialStageMaster materialStage in materialStages)
            {
                if (isDelete)
                    materialStage.Order = materialStage.Order - 1;
                else
                    materialStage.Order = materialStage.Order + 1;
                _context.Entry(materialStage).State = EntityState.Modified;
            }
        }

        protected bool IsCorrectOrder(IEnumerable<MaterialStage> stages)
        {
            bool correct = false;
            if (stages != null && stages.Count() > 0)
            {
                IEnumerable<MaterialStage> orderedStages = stages.OrderBy(s => s.Order);
                int minMileStoneID = stages.Where(s => s.MilestoneID > 0)
                    .Min(s => s.MilestoneID);
                int maxMileStoneID = stages.Max(s => s.MilestoneID);
                if (orderedStages.FirstOrDefault()?.MilestoneID >= minMileStoneID &&
                    orderedStages.LastOrDefault()?.MilestoneID <= maxMileStoneID)
                {
                    correct = true;
                }
            }
            return correct;
        }

        protected MaterialStage CreateMaterialStage(MaterialStageMaster stage)
        {
            return new MaterialStage()
            {
                ID = stage.ID,
                Name = stage.Name,
                Colour = stage.Colour,
                MaterialTypes = stage.MaterialTypes,
                IsEditable = stage.IsEditable,
                MilestoneID = stage.MilestoneID,
                Order = stage.Order,
                CanIgnoreQC = stage.CanIgnoreQC
            };
        }

        protected MaterialStageMaster CreateMaterialStageMaster(MaterialStage materialStage, int order)
        {
            return new MaterialStageMaster()
            {
                Name = materialStage.Name,
                Colour = materialStage.Colour,
                MaterialTypes = materialStage.MaterialTypes == null ? null : string.Join(",", materialStage.MaterialTypes),
                IsEditable = true,
                MilestoneID = (materialStage.MilestoneID > 0) ? materialStage.MilestoneID : 0,
                Order = order,
                CanIgnoreQC = materialStage.CanIgnoreQC
            };
        }
    }
}