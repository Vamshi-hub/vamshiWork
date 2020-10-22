using astorWorkMVC.Models;
using astorWorkMVC.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

namespace astorWorkMVC.Controllers
{
    public class BeaconAppController : ApiController
    {
        private astorWorkEntities db = new astorWorkEntities();
        
        [Route("api/BeaconApp/Echo")]
        [HttpPost]
        public async Task<string> EchoPostAsync()
        {
            string result = await Request.Content.ReadAsStringAsync();
            if (!string.IsNullOrEmpty(result))
            {                
                var t = JsonConvert.DeserializeObject<object>(result);
                db.BeaconGateWayDatas.Add(new BeaconGateWayData
                {
                    Source = HttpContext.Current == null ? string.Empty : HttpContext.Current.Request.UserHostAddress,
                    Method = Request.Method.Method,
                    ReceiveTime = DateTime.Now,
                    Message = JsonConvert.SerializeObject(t, Formatting.Indented)
                });

                await db.SaveChangesAsync();
            }
            return result;
        }

        [Route("api/BeaconApp/Echo")]
        [HttpGet]
        public async Task<string> EchoGetAsync()
        {
            string result = string.Empty;
            var query = Request.GetQueryNameValuePairs();
            if (query != null && query.Count() > 0)
            {
                foreach(var pair in query)
                {
                    result += string.Format("{0}: {1}\r\n", pair.Key, pair.Value);
                }
                db.BeaconGateWayDatas.Add(new BeaconGateWayData
                {
                    Source = HttpContext.Current == null ? string.Empty : HttpContext.Current.Request.UserHostAddress,
                    Method = Request.Method.Method,
                    ReceiveTime = DateTime.Now,
                    Message = result
                });

                await db.SaveChangesAsync();
            }
            return result;
        }

        [Route("api/BeaconApp/Login")]
        [ResponseType(typeof(BeaconAppUser))]
        public async Task<IHttpActionResult> LoginUser([FromBody] BeaconAppUser userIn)
        {
            var user = await db.BeaconAppUsers.Where(u => u.UserName == userIn.UserName).FirstOrDefaultAsync();

            if (user != null)
            {
                byte[] pwd = Encoding.UTF8.GetBytes(userIn.Password);
                byte[] salt = Encoding.UTF8.GetBytes(user.Salt);
                string pwd_encrtpted = Convert.ToBase64String(CredentialHelper.GenerateSaltedHash(pwd, salt));
                if (pwd_encrtpted.Equals(user.Password))
                    return Ok(user);
                else
                    return Unauthorized();
            }
            else
                return NotFound();
        }
    }
}
