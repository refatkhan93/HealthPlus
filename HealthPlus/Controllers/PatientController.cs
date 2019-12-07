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
        BaseController baseControl = new BaseController();
        public PatientController()
        {
            

            if (System.Web.HttpContext.Current.Session["PatientId"] == null)
            {
                RedirectToAction("Login", "Authentication");
            }
            else
            {
                int iid = (int)System.Web.HttpContext.Current.Session["PatientId"];

                using (var ctx = new HospitalContext())
                {
                    int not = ctx.Appointment.Where(c => c.PatientId == iid && c.IsSeen == 2).Select(c => c.Id).ToList().Count;

                    System.Web.HttpContext.Current.Session["PatientNotify"] = not;
                }
            }
        }
        //
        // GET: /Patient/
        
        public ActionResult TakeAppointment(Appointment appointment)
        {
            appointment.PatientId = Convert.ToInt32(Session["PatientId"]);
            appointment.Approval = 0;
            appointment.SerialNo = 0;
            appointment.Note = baseControl.Encrypt(appointment.Note);
            appointment.IsSeen = 0;
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

            ViewBag.PatientProfile = "active";
            ViewBag.UpdateMessage = message;
            int pid = (int) Session["PatientId"];
            
            List<DoctorAppointmentView> ProfileList = new List<DoctorAppointmentView>();
            Patient p = new Patient();
            using (var ctx = new HospitalContext())
            {
               
                 List <Appointment> lap= (from a in ctx.Appointment 
                                         where a.PatientId == pid && a.IsSeen == 2 
                                              select a).ToList();
                foreach (Appointment q in lap)
                {
                    q.IsSeen = 1;
                }
                ctx.SaveChanges();
                
                Session["PatientNotify"] = 0;

                var data = (from a in ctx.Appointment
                            join d in ctx.Doctor on a.DoctorId equals d.Id
                            where a.PatientId==pid
                            select new
                            {
                                aDate = a.Date,
                                aApproval = a.Approval,
                                aName = d.Name,
                                aPrescription = a.Prescription,
                                aDesignation = d.Designation,
                                aSerial=a.SerialNo
                            });
                foreach (var d in data)
                {
                    DoctorAppointmentView dc = new DoctorAppointmentView();
                    dc.Name = baseControl.Decrypt(d.aName);
                    dc.Designation = baseControl.Decrypt(d.aDesignation);
                    dc.Approval = d.aApproval;
                    dc.Date = d.aDate;
                    dc.Prescription = d.aPrescription;
                    dc.Serial = d.aSerial;
                    ProfileList.Add(dc);
                }
               // int id = Convert.ToInt32(Session["PatientId"]);
                var k = ctx.Patient.Where(e => e.Id == pid).Select(c => new { c.Name, c.Age, c.Address, c.PhoneNo });
                foreach (var i in k)
                {
                    p.Name = baseControl.Decrypt(i.Name);
                    p.Address = baseControl.Decrypt(i.Address);
                    p.Age = i.Age;
                    p.PhoneNo = baseControl.Decrypt(i.PhoneNo);
                }
                ViewBag.Patient = p;
            }
            return View(ProfileList);
        }

        public ActionResult UpdateUser(Patient patient)
        {
           
            int id = Convert.ToInt32(Session["PatientId"]);
            string password = baseControl.EncodePasswordMd5(patient.Password);
            using (var ctx = new HospitalContext())
            {
                Patient p = ctx.Patient.Single(c => c.Id == id);
                if (p.Password == password)
                {
                    p.Name = baseControl.Encrypt(patient.Name);
                    p.Age = patient.Age;
                    p.Address = baseControl.Encrypt(patient.Address);
                    p.PhoneNo = baseControl.Encrypt(patient.PhoneNo);
                    ctx.SaveChanges();
                    return RedirectToAction("Profile", "Patient", new { message = "Infomation Updated Successfully" });
                }

                return RedirectToAction("Profile", "Patient", new { message = "Update Failed" });

            }
        }

        public JsonResult HoverResult()
        {
            int pid = (int) Session["PatientId"];
            List<PatientAppointmentView> patient=new List<PatientAppointmentView>();
            using (var ctx = new HospitalContext())
            {
                var k = from a in ctx.Appointment
                    join d in ctx.Doctor on a.DoctorId equals d.Id
                    where a.PatientId == pid && a.IsSeen == 2
                    select new
                    {
                        DName = d.Name,
                        Appr = a.Approval,
                        
                    };
                foreach (var d in k)
                {
                    PatientAppointmentView pp=new PatientAppointmentView();
                    pp.Name = baseControl.Decrypt(d.DName);
                    pp.Approval = d.Appr;
                    patient.Add(pp);
                }
            }
            return Json(patient);

        }
       
	}
}