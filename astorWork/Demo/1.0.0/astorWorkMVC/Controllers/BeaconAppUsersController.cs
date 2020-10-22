using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using astorWorkMVC.Models;
using astorWorkMVC.Utilities;
using System.Text;

namespace astorWorkMVC.Controllers
{
    public class BeaconAppUsersController : Controller
    {
        private astorWorkEntities db = new astorWorkEntities();

        // GET: BIMUserInfoes
        public async Task<ActionResult> Index()
        {
            return View(await db.BeaconAppUsers.ToListAsync());
        }

        public async Task<ActionResult> SetPwd(int? id, string newPwd)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BeaconAppUser beaconAppUser = await db.BeaconAppUsers.FindAsync(id);
            if (beaconAppUser == null)
            {
                return HttpNotFound();
            }
            
            beaconAppUser.Salt = Guid.NewGuid().ToString();
            byte[] pwd = Encoding.UTF8.GetBytes(newPwd);
            byte[] salt = Encoding.UTF8.GetBytes(beaconAppUser.Salt);
            beaconAppUser.Password = Convert.ToBase64String(CredentialHelper.GenerateSaltedHash(pwd, salt));
            beaconAppUser.APIKey = Convert.ToBase64String(CredentialHelper.GenerateRandomHash());

            db.Entry(beaconAppUser).State = EntityState.Modified;

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
            BeaconAppUser beaconAppUser = await db.BeaconAppUsers.FindAsync(id);
            if (beaconAppUser == null)
            {
                return HttpNotFound();
            }
            return View(beaconAppUser);
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
        public async Task<ActionResult> Create([Bind(Include = "ID,UserName,Password")] BeaconAppUser beaconAppUser)
        {
            if (ModelState.IsValid)
            {
                beaconAppUser.Salt = Guid.NewGuid().ToString();
                byte[] pwd = Encoding.UTF8.GetBytes(beaconAppUser.Password);
                byte[] salt = Encoding.UTF8.GetBytes(beaconAppUser.Salt);
                beaconAppUser.Password = Convert.ToBase64String(CredentialHelper.GenerateSaltedHash(pwd, salt));
                db.BeaconAppUsers.Add(beaconAppUser);
                beaconAppUser.APIKey = Convert.ToBase64String(CredentialHelper.GenerateRandomHash());
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(beaconAppUser);
        }

        // GET: BIMUserInfoes/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BeaconAppUser beaconAppUser = await db.BeaconAppUsers.FindAsync(id);
            if (beaconAppUser == null)
            {
                return HttpNotFound();
            }
            return View(beaconAppUser);
        }

        // POST: BIMUserInfoes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ID,UserName,APIKey,Password,Salt")] BeaconAppUser beaconAppUser)
        {
            if (ModelState.IsValid)
            {
                db.Entry(beaconAppUser).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(beaconAppUser);
        }

        // GET: BIMUserInfoes/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BeaconAppUser beaconAppUser = await db.BeaconAppUsers.FindAsync(id);
            if (beaconAppUser == null)
            {
                return HttpNotFound();
            }
            return View(beaconAppUser);
        }

        // POST: BIMUserInfoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            BeaconAppUser beaconAppUser = await db.BeaconAppUsers.FindAsync(id);
            db.BeaconAppUsers.Remove(beaconAppUser);
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
