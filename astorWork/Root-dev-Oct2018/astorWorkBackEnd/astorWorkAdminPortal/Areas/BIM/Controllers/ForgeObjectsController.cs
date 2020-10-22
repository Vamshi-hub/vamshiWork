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
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace astorWorkAdminPortal.Areas.BIM.Controllers
{
    [Area("BIM")]
    public class ForgeObjectsController : Controller
    {
        private IAstorWorkForge _forge;

        public ForgeObjectsController(IAstorWorkForge forge)
        {
            _forge = forge;
        }

        // GET: BIM/ForgeBuckets
        public async Task<IActionResult> Index(string id)
        {
            ViewBag.BucketKey = id;
            ViewBag.Message = TempData["Message"] as string;

            IEnumerable<ForgeObject> objects = new List<ForgeObject>();
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return NotFound();
                }

                objects = await _forge.GetObjects(id);


            }
            catch(Exception exc)
            {
                ViewBag.Message = ExceptionUtility.GetExceptionDetails(exc);
            };
            return View(objects);
        }

        // GET: BIM/ForgeBuckets/Create
        public IActionResult Create(string bucketKey)
        {
            ViewBag.BucketKey = bucketKey;
            return View();
        }

        // POST: BIM/ForgeBuckets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BucketKey,ObjectKey")] ForgeObject forgeObject, IFormFile file)
        {
            if (ModelState.IsValid && file != null)
            {
                try
                {
                    forgeObject.ModelFile = file;
                    await _forge.AddObject(forgeObject);
                    return RedirectToAction(nameof(Index), new { id = forgeObject.BucketKey });
                }
                catch(Exception exc)
                {
                    ViewBag.Message = ExceptionUtility.GetExceptionDetails(exc);
                }
            }
            return View(forgeObject);
        }

        public async Task<IActionResult> Delete(string bucketKey, string objectKey)
        {
            if (string.IsNullOrEmpty(bucketKey) || string.IsNullOrEmpty(objectKey))
            {
                return NotFound();
            }

            try
            {
                await _forge.DeleteObject(bucketKey, objectKey);
            }
            catch (Exception exc)
            {
                TempData["Message"] = ExceptionUtility.GetExceptionDetails(exc);
            }

            return RedirectToAction(nameof(Index), new { id = bucketKey });
        }

        public async Task<IActionResult> Translate(string bucketKey, string objectId)
        {
            if (string.IsNullOrEmpty(bucketKey) || string.IsNullOrEmpty(objectId))
            {
                return NotFound();
            }

            try
            {
                var result = await _forge.TranslateObject(objectId);
                TempData["Message"] = JsonConvert.SerializeObject(result);
            }
            catch (Exception exc)
            {
                TempData["Message"] = ExceptionUtility.GetExceptionDetails(exc);
            }

            return RedirectToAction(nameof(Index), new { id = bucketKey });
        }

        public async Task<IActionResult> Details(string objectId)
        {
            if (string.IsNullOrEmpty(objectId))
            {
                return NotFound();
            }

            try
            {
                var manifest = await _forge.GetManifest(objectId);
                ViewBag.Manifest = manifest.ToJson().ToString();

                var metadata = await _forge.GetMetadata(objectId);
                ViewBag.Metadata = metadata.ToJson().ToString();

                var metadata1 = metadata.ToJson()["data"]["metadata"][0].ToObject<ForgeMetadata>();
                var viewData = await _forge.GetViewMetadata(objectId, metadata1.Guid);
                ViewBag.Viewdata = viewData.ToJson();
            }
            catch (Exception exc)
            {
                ViewBag.Message = ExceptionUtility.GetExceptionDetails(exc);
            }

            return View();
        }
    }
}
