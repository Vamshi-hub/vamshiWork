using astorWorkShared.Services;
using astorWorkShared.Utilities;
using Autodesk.Forge.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace astorWorkAdminPortal.Areas.BIM.Controllers
{
    [Area("BIM")]
    public class HomeController : Controller
    {
        private IMemoryCache _cache;
        private IAstorWorkForge _forge;

        public static readonly string KEY_FORGE_TOKEN = "FORGE_TOKEN";

        public HomeController(IMemoryCache cache, IAstorWorkForge forge)
        {
            _cache = cache;
            _forge = forge;
        }

        public IActionResult Index(string Op)
        {
            var message = string.Empty;
            JToken token = null;
            var listBuckets = new List<BucketsItems>();
            try
            {
                switch (Op)
                {
                    case "Authenticate":
                        if (token != null)
                        {
                            _cache.Set(KEY_FORGE_TOKEN, token);
                            message = "Authenticate with Forge successfully";
                        }
                        else
                            message = "Fail to authenticate with Forge";
                        break;
                    case "Get Buckets":
                        token = _cache.Get<JToken>(KEY_FORGE_TOKEN);
                        listBuckets = null;
                        if (listBuckets != null)
                        {
                            message = "Get buckets successfully";
                        }
                        else
                            message = "Fail to get buckets";
                        break;
                    default:
                        break;
                }
            }
            catch (Exception exc)
            {
                message = $"Exception happended: {exc.Message}";
                if (exc.InnerException != null)
                {
                    message += Environment.NewLine;
                    message += exc.InnerException.Message;
                }
                else
                {
                    message += Environment.NewLine;
                    message += exc.StackTrace;
                }
            }

            ViewBag.Message = message;
            return View(listBuckets);
        }
    }
}