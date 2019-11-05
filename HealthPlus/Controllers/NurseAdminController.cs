using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HealthPlus.Context;
using HealthPlus.Models;

namespace HealthPlus.Controllers
{
    public class NurseAdminController : Controller
    {

        public NurseAdminController()
        {
            if (System.Web.HttpContext.Current.Session["AdminId"] == null)
            {
                RedirectToAction("AdminLogin", "Authentication");
            }
        }
        private HospitalContext db = new HospitalContext();
        BaseController baseController = new BaseController();
        //
        // GET: /NurseAdmin/
        public ActionResult Index()
        {
            List<Nurse> nurses = db.Nurse.ToList();
            foreach (Nurse nurse in nurses)
            {
                nurse.Name = baseController.Decrypt(nurse.Name);
                nurse.Designation = baseController.Decrypt(nurse.Designation);
                nurse.Email = baseController.Decrypt(nurse.Email);
            }
            return View(nurses);
        }


        // GET: /NurseAdmin/Details/5


        // GET: /NurseAdmin/Create
        public ActionResult Create(string message)
        {
            ViewBag.Message = message;
            return View();
        }

        // POST: /NurseAdmin/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Email,Password,Designation")] Nurse nurse, HttpPostedFileBase Image)
        {
            
            if (ModelState.IsValid)
            {
                if (Image != null && Image.ContentLength > 0)
                {

                    try
                    {
                        string fileName = Guid.NewGuid().ToString() + System.IO.Path.GetExtension(Image.FileName);
                        string uploadUrl = Server.MapPath("~/Photos/Nurse");
                        Image.SaveAs(Path.Combine(uploadUrl, fileName));
                        nurse.Image = "Photos/Nurse/" + fileName;
                    }
                    catch (Exception ex)
                    {
                        ViewBag.Message = "ERROR:" + ex.Message.ToString();
                    }
                }
                nurse.Name = baseController.Encrypt(nurse.Name);
                nurse.Email = baseController.Encrypt(nurse.Email);

                nurse.Password = baseController.EncodePasswordMd5(nurse.Password);
                nurse.Designation = baseController.Encrypt(nurse.Designation);
                db.Nurse.Add(nurse);
                db.SaveChanges();

            }

            return RedirectToAction("Create", new { message = "Nurse Successfully Appointed" });
        }

        // GET: /NurseAdmin/Edit/5
        public ActionResult Edit(int? id)
        {
            
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Nurse nurse = db.Nurse.Find(id);
            if (nurse == null)
            {
                return HttpNotFound();
            }
            
            nurse.Designation = baseController.Decrypt(nurse.Designation);
            nurse.Email = baseController.Decrypt(nurse.Email);
            nurse.Name = baseController.Decrypt(nurse.Name);
            return View(nurse);
        }

        // POST: /NurseAdmin/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Email,Password,Designation")] Nurse nurse, HttpPostedFileBase Image,string pastImage)
        {
            if (ModelState.IsValid)
            {
                
                if (Image != null && Image.ContentLength > 0)
                {
                    string fullPath = Request.MapPath("~/" + pastImage);
                    if (System.IO.File.Exists(fullPath))
                    {
                        System.IO.File.Delete(fullPath);
                    }
                    try
                    {
                        string fileName = Guid.NewGuid().ToString() + System.IO.Path.GetExtension(Image.FileName);
                        string uploadUrl = Server.MapPath("~/Photos/Nurse");
                        Image.SaveAs(Path.Combine(uploadUrl, fileName));
                        nurse.Image = "Photos/Nurse/" + fileName;
                    }
                    catch (Exception ex)
                    {
                        ViewBag.Message = "ERROR:" + ex.Message.ToString();
                    }
                }
                else
                {
                    nurse.Image = pastImage;
                }

                nurse.Name = baseController.Encrypt(nurse.Name);
                nurse.Email = baseController.Encrypt(nurse.Email);
              
                nurse.Password = baseController.EncodePasswordMd5(nurse.Password);
                nurse.Designation = baseController.Encrypt(nurse.Designation);
                db.Entry(nurse).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(nurse);
        }

        // GET: /NurseAdmin/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Nurse nurse = db.Nurse.Find(id);
            if (nurse == null)
            {
                return HttpNotFound();
            }
            
            nurse.Designation = baseController.Decrypt(nurse.Designation);
            nurse.Email = baseController.Decrypt(nurse.Email);
            nurse.Name = baseController.Decrypt(nurse.Name);
            return View(nurse);
        }

        // POST: /NurseAdmin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Nurse nurse = db.Nurse.Find(id);
            db.Nurse.Remove(nurse);
            db.SaveChanges();
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