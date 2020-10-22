using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using astorWorkDAO;

namespace astorWorkAdminPortal.Areas.TenantManagement.Controllers
{
    [Area("TenantManagement")]
    public class SystemHealthMastersController : Controller
    {
        private readonly astorWorkDbContext _context;

        public SystemHealthMastersController(astorWorkDbContext context)
        {
            _context = context;
        }

        // GET: TenantManagement/SystemHealthMasters
        public async Task<IActionResult> Index()
        {
            return View(await _context.SystemHealthMaster.ToListAsync());
        }

        // GET: TenantManagement/SystemHealthMasters/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var systemHealthMaster = await _context.SystemHealthMaster
                .FirstOrDefaultAsync(m => m.ID == id);
            if (systemHealthMaster == null)
            {
                return NotFound();
            }

            return View(systemHealthMaster);
        }

        // GET: TenantManagement/SystemHealthMasters/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TenantManagement/SystemHealthMasters/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Type,Status,Message,Reference,LastUpdated")] SystemHealthMaster systemHealthMaster)
        {
            if (ModelState.IsValid)
            {
                _context.Add(systemHealthMaster);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(systemHealthMaster);
        }

        // GET: TenantManagement/SystemHealthMasters/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var systemHealthMaster = await _context.SystemHealthMaster.FindAsync(id);
            if (systemHealthMaster == null)
            {
                return NotFound();
            }
            return View(systemHealthMaster);
        }

        // POST: TenantManagement/SystemHealthMasters/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Type,Status,Message,Reference,LastUpdated")] SystemHealthMaster systemHealthMaster)
        {
            if (id != systemHealthMaster.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(systemHealthMaster);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SystemHealthMasterExists(systemHealthMaster.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(systemHealthMaster);
        }

        // GET: TenantManagement/SystemHealthMasters/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var systemHealthMaster = await _context.SystemHealthMaster
                .FirstOrDefaultAsync(m => m.ID == id);
            if (systemHealthMaster == null)
            {
                return NotFound();
            }

            return View(systemHealthMaster);
        }

        // POST: TenantManagement/SystemHealthMasters/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var systemHealthMaster = await _context.SystemHealthMaster.FindAsync(id);
            _context.SystemHealthMaster.Remove(systemHealthMaster);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SystemHealthMasterExists(int id)
        {
            return _context.SystemHealthMaster.Any(e => e.ID == id);
        }
    }
}
