using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml;
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
            return View();
        }

        public ActionResult Appointment()
        {
            return View();
        }

        public ActionResult Doctor()
        {
           
            List <Patient> pList= new List<Patient>();
            pList.Add(new Patient
            {
                Id=1,
                Name="Refat Khan",
                Age=21,
                History = "Chest Pain",
                Report = "Nai"
            });
            pList.Add(new Patient
            {
                Id=2,
                Name = "Amaz",
                Age = 21,
                History = "Chest Pain",
                Report = "Nai"
            });
            pList.Add(new Patient
            {
                Id=3,
                Name = "Bithi",
                Age = 21,
                History = "Chest Pain",
                Report = "Nai"
            });
            return View(pList);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Doctor(Prescription prescription,string[] Tests )
        {
            string text = "";
            foreach (string t in Tests)
            {
                text += t +"<br>";

            }
            prescription.Tests = text;
            string name;
            name = DateTime.Now.ToString("dd/MM/yyyy") + "_" + prescription.PatientId +".pdf";
            var printpdf = new ActionAsPdf("MakePdf", prescription) { FileName = name };
            return printpdf;
           
        }
        public ActionResult MakePdf(Prescription idPrescription)
        {
           
            return View(idPrescription);
        }

        [HttpPost]
        public JsonResult GetAllDocById()
        {
           
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