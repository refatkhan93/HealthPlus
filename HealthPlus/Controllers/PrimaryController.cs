using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HealthPlus.Models;

namespace HealthPlus.Controllers
{
    public class PrimaryController : Controller
    {
        //
        // GET: /Primary/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Appointment()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetAllDocById()
        {
            Doctor  doctor = new Doctor();
            List<Doctor> Dlist = new List<Doctor>();
            Dlist.Add(new Doctor
            {
                Name="Refat Khan",
                Degree="MBBS,FPS",
                Designation = "CSE",
                Photo = "Photos/Doctor/d1.jpg"
            });
            Dlist.Add(new Doctor
            {
                Name = "Refat Khan",
                Degree = "MBBS,FPS",
                Designation = "CSE",
                Photo = "Photos/Doctor/d1.jpg"
            });
            Dlist.Add(new Doctor
            {
                Name = "Refat Khan",
                Degree = "MBBS,FPS",
                Designation = "CSE",
                Photo = "Photos/Doctor/d1.jpg"
            });
            return Json(Dlist);

        }
	}
}