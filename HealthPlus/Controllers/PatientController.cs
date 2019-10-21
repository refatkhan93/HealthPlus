using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using HealthPlus.Context;
using HealthPlus.Models;

namespace HealthPlus.Controllers
{
    public class PatientController : Controller
    {
        public PatientController()
        {
            if (System.Web.HttpContext.Current.Session["PatientId"] == null)
            {
                RedirectToAction("Login", "Authentication");
            }
        }
        //
        // GET: /Patient/
        
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
        public ActionResult Profile(string message)
        {
            
            ViewBag.Profile = "active";
            ViewBag.UpdateMessage = message;
            List<DoctorAppointmentView> ProfileList = new List<DoctorAppointmentView>();
            Patient p = new Patient();
            using (var ctx = new HospitalContext())
            {
                var data = (from a in ctx.Appointment
                            join d in ctx.Doctor on a.DoctorId equals d.Id
                            where a.DoctorId == d.Id
                            select new
                            {
                                aDate = a.Date,
                                aApproval = a.Approval,
                                aName = d.Name,
                                aPrescription = a.Prescription,
                                aDesignation = d.Designation
                            });
                foreach (var d in data)
                {
                    DoctorAppointmentView dc = new DoctorAppointmentView();
                    dc.Name = d.aName;
                    dc.Designation = d.aDesignation;
                    dc.Approval = d.aApproval;
                    dc.Date = d.aDate;
                    dc.Prescription = d.aPrescription;
                    ProfileList.Add(dc);
                }
                int id = Convert.ToInt32(Session["PatientId"]);
                var k = ctx.Patient.Where(e => e.Id == id).Select(c => new { c.Name, c.Age, c.Address, c.PhoneNo });
                foreach (var i in k)
                {
                    p.Name = i.Name;
                    p.Address = i.Address;
                    p.Age = i.Age;
                    p.PhoneNo = i.PhoneNo;
                }
                ViewBag.Patient = p;
            }

            return View(ProfileList);
        }

        public ActionResult UpdateUser(Patient patient)
        {
           
            int id = Convert.ToInt32(Session["PatientId"]);
            string password = EncodePasswordMd5(patient.Password);
            using (var ctx = new HospitalContext())
            {
                Patient p = ctx.Patient.Single(c => c.Id == id);
                if (p.Password == password)
                {
                    p.Name = patient.Name;
                    p.Age = patient.Age;
                    p.Address = patient.Address;
                    p.PhoneNo = patient.PhoneNo;
                    ctx.SaveChanges();
                    return RedirectToAction("Profile", "Patient", new { message = "Infomation Updated Successfully" });
                }

                return RedirectToAction("Profile", "Patient", new { message = "Update Failed" });

            }
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
	}
}