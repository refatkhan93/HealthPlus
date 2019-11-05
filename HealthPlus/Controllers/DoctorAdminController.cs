using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using HealthPlus.Models;
using HealthPlus.Context;
using Microsoft.Ajax.Utilities;

namespace HealthPlus.Controllers
{
    public class DoctorAdminController : Controller
    {
        public DoctorAdminController()
        {
            if (System.Web.HttpContext.Current.Session["AdminId"] == null)
            {
                RedirectToAction("AdminLogin", "Authentication");
            }
        }

        private HospitalContext db = new HospitalContext();
        BaseController baseController=new BaseController();
        // GET: /DoctorAdmin/
        public ActionResult Index()
        {
            string[] days=new string[7];
            days[0] = "Sunday";
            days[1] = "Monday";
            days[2] = "Tuesday";
            days[3] = "Wednesday";
            days[4] = "Thursday";
            days[5] = "Friday";
            days[6] = "Saturday";
            

                
            List<Doctor> doctors = db.Doctor.ToList();
            foreach (Doctor doc in doctors)
            {
                string dd = "";
                doc.Name = baseController.Decrypt(doc.Name);
                doc.Degree = baseController.Decrypt(doc.Degree);
                doc.Designation = baseController.Decrypt(doc.Designation);
                doc.Email = baseController.Decrypt(doc.Email);
                int i;
                for (i = 0; i < doc.Schedule.Length-1; i++)
                {
                    if (doc.Schedule[i] != ',')
                    {
                        int ind = Convert.ToInt32(doc.Schedule[i])-48;
                        dd = dd + days[ind] + " ,  ";
                    }
                }
                dd = dd + days[Convert.ToInt32(doc.Schedule[i])-48];
                doc.Schedule = dd;
            }
            return View(doctors);
        }


        // GET: /DoctorAdmin/Details/5
       

        // GET: /DoctorAdmin/Create
        public ActionResult Create(string message)
        {
            ViewBag.Message = message;
            ViewBag.DoctorCategory=db.DoctorCategory.ToList();
            return View();
        }

        // POST: /DoctorAdmin/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Email,Password,Designation,Degree,Fees")] Doctor doctor, HttpPostedFileBase Image, IList<int> Schedule, int CategoryId)
        {
            string s = "";
            int i;
            
            for (i = 0; i < Schedule.Count-1; i++)
            {
                s = s + Schedule[i]+",";
            }
            s = s + Schedule[i];
            doctor.Schedule = s;
            doctor.CategoryId = CategoryId;
            if (ModelState.IsValid)
            {
                if (Image != null && Image.ContentLength > 0)
                    {
                        
                        try
                        {
                            string fileName = Guid.NewGuid().ToString() + System.IO.Path.GetExtension(Image.FileName);
                            string uploadUrl = Server.MapPath("~/Photos/Doctor");
                            Image.SaveAs(Path.Combine(uploadUrl, fileName));
                            doctor.Image = "Photos/Doctor/" + fileName;
                        }
                        catch (Exception ex)
                        {
                            ViewBag.Message = "ERROR:" + ex.Message.ToString();
                        }
                    }
                doctor.Name = baseController.Encrypt(doctor.Name);
                doctor.Email = baseController.Encrypt(doctor.Email);
                doctor.Degree = baseController.Encrypt(doctor.Degree);
                doctor.Password = baseController.EncodePasswordMd5(doctor.Password);
                doctor.Designation = baseController.Encrypt(doctor.Designation);
                db.Doctor.Add(doctor);
                db.SaveChanges();
                
            }

            return RedirectToAction("Create",new{message="Doctor Successfully Appointed"});
        }

        // GET: /DoctorAdmin/Edit/5
        public ActionResult Edit(int? id)
        {
            ViewBag.DoctorCategory = db.DoctorCategory.ToList();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Doctor doctor = db.Doctor.Find(id);
            if (doctor == null)
            {
                return HttpNotFound();
            }
            doctor.Degree = baseController.Decrypt(doctor.Degree);
            doctor.Designation = baseController.Decrypt(doctor.Designation);
            doctor.Email = baseController.Decrypt(doctor.Email);
            doctor.Name = baseController.Decrypt(doctor.Name);
            string[] schedule = new string[7];
            int i;
            for (i = 0; i < doctor.Schedule.Length; i++)
            {
                if (doctor.Schedule[i] != ',')
                {
                    schedule[Convert.ToInt32(doctor.Schedule[i]) - 48] = "checked=\"checked\"";
                }
            }
            ViewBag.Schedule = schedule;
            return View(doctor);
        }

        // POST: /DoctorAdmin/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Id,Name,Email,Password,Designation,Degree,Fees")] Doctor doctor ,HttpPostedFileBase Image, IList<int> Schedule, int CategoryId,string pastImage)
        {
            if (ModelState.IsValid)
            {
                string s = "";
                int i;

                for (i = 0; i < Schedule.Count - 1; i++)
                {
                    s = s + Schedule[i] + ",";
                }
                s = s + Schedule[i];
                doctor.Schedule = s;
                doctor.CategoryId = CategoryId;

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
                        string uploadUrl = Server.MapPath("~/Photos/Doctor");
                        Image.SaveAs(Path.Combine(uploadUrl, fileName));
                        doctor.Image = "Photos/Doctor/" + fileName;
                    }
                    catch (Exception ex)
                    {
                        ViewBag.Message = "ERROR:" + ex.Message.ToString();
                    }
                }
                else
                {
                    doctor.Image = pastImage;
                }

                doctor.Name = baseController.Encrypt(doctor.Name);
                doctor.Email = baseController.Encrypt(doctor.Email);
                doctor.Degree = baseController.Encrypt(doctor.Degree);
                doctor.Password = baseController.EncodePasswordMd5(doctor.Password);
                doctor.Designation = baseController.Encrypt(doctor.Designation);
                db.Entry(doctor).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(doctor);
        }

        // GET: /DoctorAdmin/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Doctor doctor = db.Doctor.Find(id);
            if (doctor == null)
            {
                return HttpNotFound();
            }
            doctor.Degree = baseController.Decrypt(doctor.Degree);
            doctor.Designation = baseController.Decrypt(doctor.Designation);
            doctor.Email = baseController.Decrypt(doctor.Email);
            doctor.Name = baseController.Decrypt(doctor.Name);
            return View(doctor);
        }

        // POST: /DoctorAdmin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Doctor doctor = db.Doctor.Find(id);
            db.Doctor.Remove(doctor);
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
