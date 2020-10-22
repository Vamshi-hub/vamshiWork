using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using astorWorkBackEnd.Models;
using astorWorkDAO;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace astorWorkBackEnd.Common
{
    public class CommonMaterialStageMasterController : CommonController
    {
        public CommonMaterialStageMasterController(astorWorkDbContext context) : base(context)
        {
        }

        protected bool MaterialStageMasterExists(int id)
        {
            return _context.MaterialStageMaster.Any(e => e.ID == id);
        }

        protected bool MaterialStageMasterExists(string name)
        {
            return _context.MaterialStageMaster.Any(s => s.Name == name);
        }


        protected void AddMaterialStage(MaterialStageMaster materialStage , int order)
        {
            ReorderMaterialStages(order);
            _context.MaterialStageMaster.Add(materialStage);
        }

        protected int GetNextStageOrder(int nextStageID) {
            MaterialStageMaster materialStageMaster = _context.MaterialStageMaster.Where(s => s.ID == nextStageID).FirstOrDefault();
            if (materialStageMaster == null)
                return 0;
            return materialStageMaster.Order;
        }

        protected void UpdateMaterialStage(int id, MaterialStage materialStage)
        {
            MaterialStageMaster materialStageMaster = _context.MaterialStageMaster.Where(s => s.ID == id).FirstOrDefault();

            if (materialStage.Order > 0)
                materialStageMaster.Order = materialStage.Order;
            materialStageMaster.Name = materialStage.Name;
            materialStageMaster.Colour = materialStage.Colour;
            materialStageMaster.MaterialTypes = (materialStage.MaterialTypes==null)?null:string.Join(',', materialStage.MaterialTypes);
            materialStageMaster.IsQCStage = materialStage.IsQCStage;
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

        protected bool IsCorrectOrder(string firstStageName, string lastStageName)
        {
            IEnumerable<MaterialStageMaster> materialStages = _context.MaterialStageMaster.OrderBy(s => s.Order);

            //string firstStage = materialStages.First().Name;
            string lastStage = materialStages.Last().Name;

            return (lastStageName == lastStage);
        }
    }
}