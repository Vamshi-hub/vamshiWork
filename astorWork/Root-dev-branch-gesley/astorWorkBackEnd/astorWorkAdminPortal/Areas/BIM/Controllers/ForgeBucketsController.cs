using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using astorWorkAdminPortal.Models;
using astorWorkShared.Models;
using Microsoft.Extensions.Caching.Memory;
using astorWorkShared.Services;
using Newtonsoft.Json.Linq;
using astorWorkShared.Utilities;

namespace astorWorkAdminPortal.Areas.BIM.Controllers
{
    [Area("BIM")]
    public class ForgeBucketsController : Controller
    {
        private IMemoryCache _cache;
        private IAstorWorkForge _forge;

        public ForgeBucketsController(IMemoryCache cache, IAstorWorkForge forge)
        {
            _cache = cache;
            _forge = forge;
        }

        // GET: BIM/ForgeBuckets
        public async Task<IActionResult> Index()
        {
            ViewBag.Message = TempData["Message"] as string;
            try
            {
                var buckets = await _forge.GetBuckets();
                return View(buckets);
            }
            catch(Exception exc)
            {
                ViewBag.Message = ExceptionUtility.GetExceptionDetails(exc);
                return View();
            };
        }

        // GET: BIM/ForgeBuckets/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: BIM/ForgeBuckets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BucketKey,PolicyKey")] ForgeBucket forgeBucket)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _forge.AddBucket(forgeBucket);
                    return RedirectToAction(nameof(Index));
                }
                catch(Exception exc)
                {
                    ViewBag.Message = ExceptionUtility.GetExceptionDetails(exc);
                }
            }
            return View(forgeBucket);
        }

        // GET: BIM/ForgeBuckets/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            try
            {
                await _forge.DeleteBucket(id);
            }
            catch (Exception exc)
            {
                TempData["Message"] = ExceptionUtility.GetExceptionDetails(exc);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
