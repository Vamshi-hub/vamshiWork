using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using astorWork.Models;
using astorWork.Utilities;
using System.Text;

namespace astorWork.Controllers
{
    public class BIMUserInfoesController : Controller
    {
        private astorWorkEntities db = new astorWorkEntities();

        // GET: BIMUserInfoes
        public async Task<ActionResult> Index()
        {
            return View(await db.BIMUserInfoes.ToListAsync());
        }

        public async Task<ActionResult> SetPwd(int? id, string newPwd)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BIMUserInfo bIMUserInfo = await db.BIMUserInfoes.FindAsync(id);
            if (bIMUserInfo == null)
            {
                return HttpNotFound();
            }
            
            bIMUserInfo.Salt = Guid.NewGuid().ToString();
            byte[] pwd = Encoding.UTF8.GetBytes(newPwd);
            byte[] salt = Encoding.UTF8.GetBytes(bIMUserInfo.Salt);
            bIMUserInfo.Password = Convert.ToBase64String(CredentialHelper.GenerateSaltedHash(pwd, salt));
            bIMUserInfo.APIKey = Convert.ToBase64String(CredentialHelper.GenerateRandomHash());

            db.Entry(bIMUserInfo).State = EntityState.Modified;

            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // GET: BIMUserInfoes/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BIMUserInfo bIMUserInfo = await db.BIMUserInfoes.FindAsync(id);
            if (bIMUserInfo == null)
            {
                return HttpNotFound();
            }
            return View(bIMUserInfo);
        }

        // GET: BIMUserInfoes/Plugin
        public ActionResult Plugin()
        {
            return View();
        }

        // GET: BIMUserInfoes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: BIMUserInfoes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ID,UserName,Password")] BIMUserInfo bIMUserInfo)
        {
            if (ModelState.IsValid)
            {
                bIMUserInfo.Salt = Guid.NewGuid().ToString();
                byte[] pwd = Encoding.UTF8.GetBytes(bIMUserInfo.Password);
                byte[] salt = Encoding.UTF8.GetBytes(bIMUserInfo.Salt);
                bIMUserInfo.Password = Convert.ToBase64String(CredentialHelper.GenerateSaltedHash(pwd, salt));
                db.BIMUserInfoes.Add(bIMUserInfo);
                bIMUserInfo.APIKey = Convert.ToBase64String(CredentialHelper.GenerateRandomHash());
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(bIMUserInfo);
        }

        // GET: BIMUserInfoes/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BIMUserInfo bIMUserInfo = await db.BIMUserInfoes.FindAsync(id);
            if (bIMUserInfo == null)
            {
                return HttpNotFound();
            }
            return View(bIMUserInfo);
        }

        // POST: BIMUserInfoes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ID,UserName,APIKey,Password,Salt")] BIMUserInfo bIMUserInfo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bIMUserInfo).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(bIMUserInfo);
        }

        // GET: BIMUserInfoes/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BIMUserInfo bIMUserInfo = await db.BIMUserInfoes.FindAsync(id);
            if (bIMUserInfo == null)
            {
                return HttpNotFound();
            }
            return View(bIMUserInfo);
        }

        // POST: BIMUserInfoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            BIMUserInfo bIMUserInfo = await db.BIMUserInfoes.FindAsync(id);
            db.BIMUserInfoes.Remove(bIMUserInfo);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
