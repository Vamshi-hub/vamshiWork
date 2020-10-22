using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using astorWorkDAO;
using astorWorkShared.GlobalResponse;
using astorWorkBackEnd.Common;
using astorWorkBackEnd.Models;

namespace astorWorkBackEnd.Controllers
{
    [Produces("application/json")]
    [Route("material-stages")]
    public class MaterialStageMastersController : CommonMaterialStageMasterController
    {
        private readonly string Module = "materialstage";

        public MaterialStageMastersController(astorWorkDbContext context) : base(context)
        {
        }

        // GET: material-stages
        [HttpGet]
        public APIResponse ListMaterialStages()
        {
            List<MaterialStage> materialStages = new List<MaterialStage>();
            var lstStages = _context.MaterialStageMaster.ToList();
            Parallel.ForEach(lstStages, stage =>
            {
                materialStages.Add(new MaterialStage()
                {
                    ID =stage.ID,
                    Name = stage.Name,
                    Colour = stage.Colour,
                    IsQCStage = stage.IsQCStage,
                    MaterialTypes = stage.MaterialTypes?.Split(',').ToList(),
                    isEditable = stage.IsEditable,
                    Order = stage.Order
                });
            });
            return new APIResponse(0, materialStages.OrderBy(p=>p.Order));
        }

        // GET: api/MaterialStageMasters/5
        [HttpGet("{id}")]
        public APIResponse GetMaterialStage([FromRoute] int id)
        {
            if (!ModelState.IsValid)
                return new APIBadRequest();

            MaterialStageMaster materialStageMaster = _context.MaterialStageMaster.FirstOrDefault(m => m.ID == id);

            MaterialStage materialStage = new MaterialStage() {
                Name = materialStageMaster.Name,
                Colour = materialStageMaster.Colour,
                IsQCStage = materialStageMaster.IsQCStage,
                MaterialTypes = materialStageMaster.MaterialTypes.Split(',').ToList(),
                isEditable = materialStageMaster.IsEditable,
                Order = materialStageMaster.Order
            };

            if (materialStageMaster == null)
                return new DbRecordNotFound(Module, "Stage", id.ToString());

            return new APIResponse(0, materialStageMaster);
        }

        // POST: material-stages
        [HttpPost]
        public async Task<APIResponse> CreateMaterialStage([FromBody] MaterialStage materialStage)
        {
            if (!ModelState.IsValid)
                return new APIBadRequest();

            try
            {
                if (MaterialStageMasterExists(materialStage.Name))
                    return new DbDuplicateRecord("Material Stage", "Name", materialStage.Name);
                else
                {
                    int order = GetNextStageOrder(materialStage.NextStageID);

                    MaterialStageMaster materialStageMaster = new MaterialStageMaster()
                    {
                        Name = materialStage.Name,
                        Colour = materialStage.Colour,
                        IsQCStage = materialStage.IsQCStage,
                        MaterialTypes = string.Join(",", materialStage.MaterialTypes),
                        IsEditable = true,
                        Order = order
                    };

                    AddMaterialStage(materialStageMaster, order);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception exc)
            {
                Console.Write(exc.StackTrace);
                return new APIBadRequest();
            }

            return new APIResponse(0, _context.MaterialStageMaster.OrderByDescending(s => s.ID).First());
        }

        // PUT: material-stages/5
        [HttpPut("{id}")]
        public async Task<APIResponse> EditMaterialStageDetail([FromRoute] int id, [FromBody] MaterialStage materialStage)
        {
            if (!ModelState.IsValid)
                return new APIBadRequest();

            UpdateMaterialStage(id, materialStage);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException exc)
            {
                if (!MaterialStageMasterExists(id))
                    return new DbRecordNotFound("Material Stage does not exist!");
                else
                    return new DbConcurrentUpdate(exc.Message);
            }

            return new APIResponse(0, null);
        }


        // POST: material-stages/stage-list
        [HttpPost("stage-list")]
        public APIResponse EditMaterialStageList([FromBody] List<MaterialStage> materialStages)
        {
            if (!ModelState.IsValid)
                return new APIBadRequest();

            int order = 0;

            if (!IsCorrectOrder(materialStages.First().Name, materialStages.Last().Name))
                return new DbValidationError("Stage order is not valid!");

            foreach (MaterialStage materialStage in materialStages)
            {
                order = order + 1;
                //New Stage Entry

                MaterialStageMaster _materialStageMaster = new MaterialStageMaster();
                _materialStageMaster.ID = materialStage.ID;
                _materialStageMaster.IsEditable = materialStage.isEditable;
                _materialStageMaster.Name = materialStage.Name;
                _materialStageMaster.Colour = materialStage.Colour;
                _materialStageMaster.IsQCStage = materialStage.IsQCStage;
                _materialStageMaster.MaterialTypes = materialStage.MaterialTypes==null?null:string.Join(",", materialStage.MaterialTypes);
                _materialStageMaster.IsEditable = true;
                _materialStageMaster.Order = order;
                if (materialStage.ID == 0)
                    AddMaterialStage(_materialStageMaster, order);
                else
                {
                    materialStage.Order = order;
                    UpdateMaterialStage(_materialStageMaster.ID, materialStage);
                }
                //Update Existing Stage

            }

            _context.SaveChanges();

            //detete stages
           //var removelist= _context.MaterialStageMaster.Except(materialStages);
           // foreach (var rm in removelist)
           //     _context.MaterialStageMaster.Remove(rm);
           // _context.SaveChangesAsync();

            return new APIResponse(0, null);
        }

        // DELETE: api/MaterialStageMasters/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMaterialStageMaster([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var materialStageMaster = await _context.MaterialStageMaster.FindAsync(id);
            if (materialStageMaster == null)
            {
                return NotFound();
            }

            _context.MaterialStageMaster.Remove(materialStageMaster);
            await _context.SaveChangesAsync();
            ReorderMaterialStages(1, true);
            await _context.SaveChangesAsync();

            return Ok(materialStageMaster);
        }
    }
}