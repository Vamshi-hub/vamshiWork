using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using astorWorkDAO;
using astorWorkJobTracking.Models;
using astorWorkShared.GlobalExceptions;
using astorWorkShared.GlobalResponse;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace astorWorkJobTracking.Controllers
{
    [Produces("application/json")]
    [Route("projects/{project_id}/trade-associations")]
    public class TradeAssociationsController : Controller
    {
        protected astorWorkDbContext _context;

        public TradeAssociationsController(astorWorkDbContext context)
        {
            _context = context;
        }

        // GET: api/<controller>    
        [HttpGet]
        public async Task<List<TradeAssociation>> ListTradeAssociations()
        {
            if (!ModelState.IsValid)
                throw new GenericException(ErrorMessages.BadRequest, ErrorMessages.BadRequestMsg());

            List<TradeAssociation> associations = new List<TradeAssociation>();
            List<TradeMaster> trades = _context.TradeMaster.ToList();
            
            foreach (TradeMaster trade in trades) {

                TradeAssociation association = new TradeAssociation()
                {
                    Name = trade.Name,
                    MaterialTypes = _context.TradeMaterialTypeAssociation
                                                  .Where(a => a.Trade == trade)
                                                  .Select(a => a.MaterialType.Name)
                                                  .ToList(),
                    JobStartedMaterialTypes = await _context.JobAudit
                                                            .Where(ja => ja.JobSchedule.TradeID == trade.ID)
                                                            .Select(ja => ' ' + ja.JobSchedule.Material.MaterialType.Name + ' ')
                                                            .Distinct()
                                                            .ToListAsync(),
                    ChecklistItems = _context.ChecklistItemAssociation
                                                   .Where(a => a.Checklist.Trade == trade &&a.IsActive==true)
                                                   .OrderBy(a => a.ChecklistItemSequence)
                                                   .Select(a => a.ChecklistItem.Name)
                                                   .ToList()
                };

                associations.Add(association);
            };
              
            return associations;
        }

        // GET api/<controller>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // PUT api/<controller>
        [HttpPut]
        public async Task CreateTradeAssociation([FromRoute] int project_id, [FromBody] List<TradeAssociation> tradeAssociations)
        {
            if (!ModelState.IsValid)
                throw new GenericException(ErrorMessages.BadRequest, ErrorMessages.BadRequestMsg());
            List<TradeMaster> lstTrade = _context.TradeMaster.ToList();
            List<MaterialTypeMaster> lstMaterialTypeMaster = _context.MaterialTypeMaster.ToList();
            List<ChecklistItemMaster> lstCheckListItemMaster = _context.ChecklistItemMaster.ToList();
            foreach (TradeAssociation tradeAssociation in tradeAssociations)
            {
                TradeMaster trade = lstTrade.Where(t => t.Name == tradeAssociation.Name).FirstOrDefault();

                List<MaterialTypeMaster> materialTypes = lstMaterialTypeMaster.Where(mt => tradeAssociation.MaterialTypes.Contains(mt.Name)).ToList();
                DeleteUnassociatedMaterialTypesFromDb(trade, materialTypes);
                if (materialTypes.Count > 0)
                    await AssociateTradeToMaterialTypes(project_id, trade, materialTypes, lstMaterialTypeMaster);

                //List<ChecklistItemMaster> checklistItems = lstCheckListItemMaster.Where(clm => tradeAssociation.ChecklistItems.Contains(clm.Name)).ToList();
                //DeleteUnassociatedChecklistItemsFromDb(trade, checklistItems);
                //if (checklistItems.Count > 0)
                //    await AssociateChecklistToChecklistItems(checklist, checklistItems, lstCheckListItemMaster);
            }

            await _context.SaveChangesAsync();
        }

        protected List<MaterialTypeMaster> GetMaterialTypes(int projectID, List<string> materialTypes)
        {
            if (materialTypes == null)
                return null;

            List<MaterialTypeMaster> materialTypeMasters = new List<MaterialTypeMaster>();

            foreach (string materialType in materialTypes)
            {
                MaterialTypeMaster materialTypeMaster = _context.MaterialTypeMaster.Where(mt => mt.Name == materialType).FirstOrDefault();
                materialTypeMasters.Add(materialTypeMaster);
            }
            return materialTypeMasters;
        }

        protected List<ChecklistItemMaster> GetChecklistItems(int projectID, List<string> checklistItems)
        {
            if (checklistItems == null)
                return null;

            List<ChecklistItemMaster> checklistItemMasters = new List<ChecklistItemMaster>();

            foreach (string checklistItem in checklistItems)
            {
                ChecklistItemMaster checklistItemMaster = _context.ChecklistItemMaster.Where(c => c.Name == checklistItem).FirstOrDefault();
                checklistItemMasters.Add(checklistItemMaster);
            }

            return checklistItemMasters;
        }

        protected List<MaterialTypeMaster> GetMaterialTypes(string materialTypes)
        {
            return _context.MaterialTypeMaster.Where(mt => materialTypes.Contains(mt.Name)).ToList();
        }

        protected void DeleteUnassociatedMaterialTypesFromDb(TradeMaster trade, List<MaterialTypeMaster> materialTypes)
        {
            List<TradeMaterialTypeAssociation> tradeMaterialTypeAssociations = _context.TradeMaterialTypeAssociation
                                                                                       .Include(a => a.MaterialType)
                                                                                       .Where(mta => mta.Trade == trade).ToList();

            foreach (TradeMaterialTypeAssociation tradeMaterialTypeAssociation in tradeMaterialTypeAssociations)
                if (IsUnassociatedMaterialTypeInDb(tradeMaterialTypeAssociation.MaterialType.Name, materialTypes) || materialTypes.Count == 0)
                    _context.TradeMaterialTypeAssociation.Remove(tradeMaterialTypeAssociation);
        }

        protected void DeleteUnassociatedChecklistItemsFromDb(TradeMaster trade, List<ChecklistItemMaster> checklistItems)
        {
            List<ChecklistItemAssociation> ChecklistItemAssociations = _context.ChecklistItemAssociation
                                                                                         .Include(a => a.ChecklistItem)
                                                                                         .Where(mta => mta.Checklist.Trade == trade).ToList();

            foreach (ChecklistItemAssociation tradeChecklistItemAssociation in ChecklistItemAssociations)
                if (IsUnassociatedChecklistItemInDb(tradeChecklistItemAssociation.ChecklistItem.Name, checklistItems) || checklistItems.Count == 0)
                    _context.ChecklistItemAssociation.Remove(tradeChecklistItemAssociation);
        }

        protected bool IsUnassociatedMaterialTypeInDb(string materialTypeName, List<MaterialTypeMaster> materialTypes)
        {
            foreach (MaterialTypeMaster materialType in materialTypes)
                if (materialType.Name == materialTypeName)
                    return false;

            return true;
        }

        protected bool IsUnassociatedChecklistItemInDb(string checklistItemName, List<ChecklistItemMaster> checklistItems)
        {
            foreach (ChecklistItemMaster checklistItem in checklistItems)
                if (checklistItem.Name == checklistItemName)
                    return false;

            return true;
        }

        protected List<ChecklistItemMaster> GetChecklistItems(string checklistItems)
        {
            return _context.ChecklistItemMaster.Where(cli => checklistItems.Contains(cli.Name)).ToList();
        }

        protected MaterialTypeMaster GetMaterialType(string materialTypeName)
        {
            MaterialTypeMaster materialType = _context.MaterialTypeMaster.Where(mt => mt.Name == materialTypeName).FirstOrDefault();

            if (materialType != null)
                return materialType;

            return null;
        }

        protected async Task AssociateTradeToMaterialTypes(int projectID, TradeMaster trade, List<MaterialTypeMaster> materialTypes, List<MaterialTypeMaster> lstMaterialType)
        {
            List<TradeMaterialTypeAssociation> tradeMaterialTypeAssociations = _context.TradeMaterialTypeAssociation.Where(a => a.Trade == trade).ToList();

            foreach (MaterialTypeMaster materialType in materialTypes)
            {
                if (!MaterialTypeAssociationExistInDb(materialType.Name, tradeMaterialTypeAssociations))
                {
                    TradeMaterialTypeAssociation tradeMaterialTypeAssociation = new TradeMaterialTypeAssociation();
                    tradeMaterialTypeAssociation.Trade = trade;
                    tradeMaterialTypeAssociation.TradeID = trade.ID;

                    MaterialTypeMaster materialTypeMaster = lstMaterialType.Where(mt => mt.Name == materialType.Name).FirstOrDefault();
                    tradeMaterialTypeAssociation.MaterialType = materialTypeMaster;
                    tradeMaterialTypeAssociation.MaterialTypeID = materialTypeMaster.ID;

                    await _context.TradeMaterialTypeAssociation.AddAsync(tradeMaterialTypeAssociation);
                }
            }
        }

        protected bool MaterialTypeAssociationExistInDb(string materialTypeName, List<TradeMaterialTypeAssociation> tradeMaterialTypeAssociations)
        {
            foreach (TradeMaterialTypeAssociation association in tradeMaterialTypeAssociations)
                if (materialTypeName == association.MaterialType.Name)
                    return true;

            return false;
        }

        protected async Task AssociateChecklistToChecklistItems(ChecklistMaster checklist, List<ChecklistItemMaster> checklistItems, List<ChecklistItemMaster> lstCheckListItemMaster)
        {
            List<ChecklistItemAssociation> checklistItemAssociations = _context.ChecklistItemAssociation.Where(a => a.Checklist == checklist).ToList();

            foreach (ChecklistItemMaster checklistItem in checklistItems)
            {
                if (!ChecklistItemAssociationExistInDb(checklistItem.Name, checklistItemAssociations))
                {
                    ChecklistItemAssociation tradeChecklistItemAssociation = new ChecklistItemAssociation();
                    tradeChecklistItemAssociation.Checklist = checklist;
                    tradeChecklistItemAssociation.ChecklistID = checklist.ID;

                    ChecklistItemMaster checklistItemMaster = lstCheckListItemMaster.Where(c => c.Name == checklistItem.Name).FirstOrDefault();
                    tradeChecklistItemAssociation.ChecklistItem = checklistItemMaster;
                    tradeChecklistItemAssociation.ChecklistItemID = checklistItemMaster.ID;

                    await _context.ChecklistItemAssociation.AddAsync(tradeChecklistItemAssociation);
                }
            }
        }

        protected bool ChecklistItemAssociationExistInDb(string checklistItemName, List<ChecklistItemAssociation> checklistItemAssociations)
        {
            foreach (ChecklistItemAssociation association in checklistItemAssociations)
                if (checklistItemName == association.ChecklistItem.Name)
                    return true;

            return false;
        }

        // PUT api/<controller>/5
        [HttpPut("{id}")]
        public async Task UpdateMaterialTypeJobAssociation(int id, [FromBody] MaterialTypeJobs materialTypeJobs)
        {
            if (!ModelState.IsValid)
                throw new GenericException(ErrorMessages.BadRequest, ErrorMessages.BadRequestMsg());

            string materialTypeName = materialTypeJobs.MaterialTypeName;
            List<TradeMaterialTypeAssociation> materialTypeJobAssociationsInDb = _context.TradeMaterialTypeAssociation.Where(mtja => mtja.MaterialType.Name == materialTypeName).ToList();
            MaterialTypeMaster materialType = materialTypeJobAssociationsInDb.FirstOrDefault().MaterialType;

            string selectedJobs = "";

            foreach (TradeMaterialTypeAssociation materialTypeJobAssociation in materialTypeJobAssociationsInDb)
                selectedJobs = selectedJobs + "," + materialTypeJobAssociation.Trade.Name;

            foreach (string jobName in materialTypeJobs.JobNames.Split(','))
            {
                if (!MaterialTypeJobAssociationExists(materialTypeName, jobName))
                {
                    TradeMaster job = _context.TradeMaster.Where(m => m.Name == jobName).FirstOrDefault();
                    TradeMaterialTypeAssociation association = new TradeMaterialTypeAssociation();
                    association.MaterialType = materialType;
                    association.Trade = job;
                }
            }

            foreach (TradeMaterialTypeAssociation association in materialTypeJobAssociationsInDb)
            {
                string jobName = association.Trade.Name;
                if (!selectedJobs.Contains(jobName))
                {
                    TradeMaterialTypeAssociation materialTypeJobAssociation = _context.TradeMaterialTypeAssociation.Where(mtja => mtja.MaterialType.Name == materialTypeName && mtja.Trade.Name == jobName).FirstOrDefault();
                    _context.TradeMaterialTypeAssociation.Remove(association);
                }
            }

            await _context.SaveChangesAsync();
        }

        // DELETE api/<controller>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        protected bool MaterialTypeJobAssociationExists(string materialType, string job)
        {
            return _context.TradeMaterialTypeAssociation.Any(mtja => mtja.MaterialType.Name == materialType && mtja.Trade.Name == job);
        }
    }
}
