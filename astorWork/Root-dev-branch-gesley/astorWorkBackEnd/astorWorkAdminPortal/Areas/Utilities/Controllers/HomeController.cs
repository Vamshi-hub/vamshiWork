using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using astorWorkShared.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace astorWorkAdminPortal.Areas.Utilities.Controllers
{
    [Area("Utilities")]
    public class HomeController : Controller
    {
        IDistributedCache _cache;

        public HomeController(IDistributedCache cache)
        {
            _cache = cache;
        }

        public IActionResult Index(string Op)
        {
            var message = string.Empty;
            try
            {
                switch (Op)
                {
                    default:
                        message = "Invalid operation";
                        break;
                }
            }
            catch (Exception exc)
            {
                message = ExceptionUtility.GetExceptionDetails(exc);
            }

            ViewBag.Message = message;
            return View();
        }
    }
}