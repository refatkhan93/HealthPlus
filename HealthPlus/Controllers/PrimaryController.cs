using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using HealthPlus.Context;
using HealthPlus.Models;
using Rotativa;
using Rotativa.MVC;

namespace HealthPlus.Controllers
{
    public class PrimaryController : Controller
    {

        public PrimaryController()
        {
            

            if (System.Web.HttpContext.Current.Session["PatientId"] != null)
            {
                int iid = (int)System.Web.HttpContext.Current.Session["PatientId"];

                using (var ctx = new HospitalContext())
                {
                    int not = ctx.Appointment.Where(c => c.PatientId == iid && c.IsSeen == 2).Select(c => c.Id).ToList().Count;

                    System.Web.HttpContext.Current.Session["PatientNotify"] = not;
                }
            }
        }

        BaseController baseController=new BaseController();
        public ActionResult Index()
        {
            ViewBag.Index = "active";
            List<Service> services;
            using (var ctx = new HospitalContext())
            {

                services = ctx.Service.ToList();
            }
            return View(services);
        }

        public ActionResult Appointment(int? val)
        {
            ViewBag.Appointment = "active";
            List<DoctorCategory> doctorCategories;
            
            List<Doctor> dc=new List<Doctor>();
            using (var ctx = new HospitalContext())
            {

                doctorCategories = ctx.DoctorCategory.ToList();
                var doctors = ctx.Doctor.OrderBy(r => Guid.NewGuid()).Select(c => new { c.Id, c.Name, c.Degree, c.Designation, c.Fees, c.Image, c.Schedule }).Take(8);
                foreach (var doc in doctors)
                {
                    Doctor doct =new Doctor();
                    doct.Id = doc.Id;
                    doct.Name = baseController.Decrypt(doc.Name);
                    doct.Degree = baseController.Decrypt(doc.Degree);
                    doct.Designation = baseController.Decrypt(doc.Designation);
                    doct.Fees = doc.Fees;
                    doct.Image = doc.Image;
                    doct.Schedule = doc.Schedule;
                    dc.Add(doct);
                }
            }
            
            ViewBag.Category = doctorCategories;
            ViewBag.Doctor = dc;
            ViewBag.Message = val;
            return View();
        }


        [HttpPost]
        public JsonResult GetAllDocById(int id)
        {
           
            List<Doctor> Dlist = new List<Doctor>();
            using (var ctx = new HospitalContext())
            {
                var k = ctx.Doctor.Where(c=>c.CategoryId==id).Select(c=>new {c.Id,c.Name,c.Designation,c.Degree,c.Image,c.Fees,c.Schedule}).ToList();
                foreach (var dc in k)
                {
                    Doctor d = new Doctor();
                    d.Id = dc.Id;
                    d.Name = baseController.Decrypt(dc.Name);
                    d.Image = dc.Image;
                    d.Designation = baseController.Decrypt(dc.Designation);
                    d.Degree = baseController.Decrypt(dc.Degree);
                    d.Fees = dc.Fees;
                    d.Schedule = dc.Schedule;
                    Dlist.Add(d);
                }
            }
           
            return Json(Dlist);

        }

        public ActionResult Emergency()
        {
            ViewBag.Emergency = "active";
            return View();
        }

        public ActionResult BloodBank()
        {
            
            return View();
        }

        public ActionResult GetBlood(string id)
        {
            List<Blood> Dlist = new List<Blood>();
            using (var ctx = new HospitalContext())
            {
                var k = ctx.Blood.Where(c => c.BloodGroup == id).Select(c => new {  c.Name,c.Address,c.Phone }).ToList();
                foreach (var dc in k)
                {
                    Blood d = new Blood();
                    d.Name = dc.Name;
                    d.Address = dc.Address;
                    d.Phone = dc.Phone;
                    Dlist.Add(d);
                }
            }

            return Json(Dlist);
        }
      
	}
}