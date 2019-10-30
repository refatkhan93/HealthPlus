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
        //
        // GET: /Primary/
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
            IQueryable<Doctor> doctors;
            List<Doctor> dc=new List<Doctor>();
            using (var ctx = new HospitalContext())
            {

                doctorCategories = ctx.DoctorCategory.ToList();
                doctors = ctx.Doctor.OrderBy(r => Guid.NewGuid()).Take(8);
                foreach (Doctor doctor in doctors)
                {
                    dc.Add(doctor);
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
                var k = ctx.Doctor.Where(c=>c.CategoryId.Id==id).Select(c=>new {c.Id,c.Name,c.Designation,c.Degree,c.Image,c.Fees,c.Schedule}).ToList();
                foreach (var dc in k)
                {
                    Doctor d = new Doctor();
                    d.Id = dc.Id;
                    d.Name = dc.Name;
                    d.Image = dc.Image;
                    d.Designation = dc.Designation;
                    d.Degree = dc.Degree;
                    d.Fees = dc.Fees;
                    d.Schedule = dc.Schedule;
                    Dlist.Add(d);
                }
            }
           
            return Json(Dlist);

        }

        public ActionResult Emergency()
        {
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