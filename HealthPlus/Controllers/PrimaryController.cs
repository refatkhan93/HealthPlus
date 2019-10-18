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

        public ActionResult TakeAppointment(Appointment appointment)
        {
            appointment.PatientId = Convert.ToInt32(Session["PatientId"]);
            appointment.Approval = 0;
            appointment.SerialNo = 0;
           // appointment.Date= appointment.Date.ToString("dd/MM/yyyy");
            using (var ctx = new HospitalContext())
            {
                    ctx.Appointment.Add(appointment);
                    ctx.SaveChanges();
            }
           
            return RedirectToAction("Appointment", "Primary", new { val = 1 });
        }
        public ActionResult Doctor()
        {
            ViewBag.Doctor= "active";
            List <Patient> pList= new List<Patient>();
           
            return View(pList);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Doctor(Prescription prescription,string[] Tests )
        {
            ViewBag.Doctor = "active";
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
        public JsonResult GetAllDocById(int id)
        {
           
            List<Doctor> Dlist = new List<Doctor>();
            using (var ctx = new HospitalContext())
            {
                //Dlist = ctx.Doctor.SqlQuery("SELECT * FROM Doctor WHERE CategoryId_Id="+id).ToList();
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

        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(Login login)
        {
            string pass = EncodePasswordMd5(login.Password);
            using (var ctx = new HospitalContext())
            {
                if (login.UserType == 1)
                { 
                    var q = ctx.Patient.Where(c => c.Email == login.UserEmail && c.Password==pass).Select(c=>new {c.Id,c.Name}).ToList();
                    if (q.Any())
                    {
                        foreach (var k in q)
                        {
                           Session["PatientId"] = k.Id;
                           Session["PatientName"] = k.Name;
                           
                        }
                       return RedirectToAction("Index","Primary");
                    }
                    else
                    {
                        ViewBag.Error = "Login Failed";
                    }
                }
                else if (login.UserType == 2)
                {
                    var q = ctx.Doctor.Where(c => c.Email == login.UserEmail && c.Password == pass).Select(c => new { c.Id, c.Name }).ToList();
                    if (q.Any())
                    {
                        foreach (var k in q)
                        {
                            Session["DoctorId"] = k.Id;
                            Session["DoctorName"] = k.Name;

                        }
                        return RedirectToAction("Index", "Primary");
                    }
                    else
                    {
                        ViewBag.Error = "Login Failed";
                    }
                }
                else
                {

                }
            }
            return View();
        }

        public ActionResult Register()
        {
            ViewBag.Register = "active";
            return View();
        }
        [HttpPost]
        public ActionResult Register(Patient patient)
        {
            using (var ctx = new HospitalContext())
            {
                var count = ctx.Patient.Where(c => c.Email == patient.Email).ToList().Count;
                if (count == 0)
                {
                    patient.Password = EncodePasswordMd5(patient.Password);
                    ctx.Patient.Add(patient);
                    ctx.SaveChanges();
                }
                else
                {
                    ViewBag.Error = "Already Registered With This Email";
                }
            }
            ViewBag.Success = '1';
            return View();
        }
        public static string EncodePasswordMd5(string pass) //Encrypt using MD5    
        {
            Byte[] originalBytes;
            Byte[] encodedBytes;
            MD5 md5;
            //Instantiate MD5CryptoServiceProvider, get bytes for original password and compute hash (encoded password)    
            md5 = new MD5CryptoServiceProvider();
            originalBytes = ASCIIEncoding.Default.GetBytes(pass);
            encodedBytes = md5.ComputeHash(originalBytes);
            //Convert encoded bytes back to a 'readable' string    
            return BitConverter.ToString(encodedBytes);
        }

        public ActionResult PatientLogout()
        {
            Session["PatientId"]=null;
            Session["PatientName"]=null;
            return RedirectToAction("Index", "Primary");
        }
        public ActionResult DoctorLogout()
        {
            Session["DoctorId"] = null;
            Session["DoctorName"] = null;
            return RedirectToAction("Index", "Primary");
        }
	}
}