using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HealthPlus.Context;
using HealthPlus.Models;
using Rotativa.MVC;

namespace HealthPlus.Controllers
{
    public class DoctorController : Controller
    {
        BaseController baseControl = new BaseController();
        //
        // GET: /Doctor/
        public ActionResult PrescribePatient()
        {
            ViewBag.PrescribePatient = "active";
            List<Ward> wards;
            using (var ctx =new HospitalContext())
            {
                wards = ctx.Ward.ToList();
            }
            
            return View(wards);
        }

        public JsonResult GetPatient(int id)
        {
            List<PatientAppointmentView> pt = new List<PatientAppointmentView>();
            int doc = (int)Session["DoctorId"];
            string date = DateTime.Today.ToString("MM/dd/yyyy");
            using (var ctx = new HospitalContext())
            {
                var data = from a in ctx.Appointment
                    join p in ctx.Patient
                        on a.PatientId equals p.Id
                    where a.DoctorId == doc && a.Session==id && a.Approval==1 && a.Date==date
                    orderby a.SerialNo ascending 
                    select new
                    {
                        pId=a.Id,
                        pName = p.Name,
                        pAge = p.Age,
                        pNote = a.Note
                    };
                foreach (var dc in data)
                {
                    PatientAppointmentView p = new PatientAppointmentView();
                    p.AppointmentId = dc.pId;
                    p.Name = baseControl.Decrypt(dc.pName);
                    p.Age = dc.pAge;
                    p.Note = baseControl.Decrypt(dc.pNote);
                    pt.Add(p);
                }
            }
            return Json(pt);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult PrescribePatient(Prescription prescription,int AppointmentId)
        {
            ViewBag.PrescribePatient = "active";
            string name;
            name = DateTime.Now.ToString("dd-MM-yyyy") + "_" + prescription.PatientId + "_" + Guid.NewGuid() + ".pdf";
            using (var ctx = new HospitalContext())
            {
                if (prescription.WardId > 0)
                {
                    Appointment appointment = ctx.Appointment.Find(AppointmentId);
                    appointment.WardId = prescription.WardId;
                    ctx.SaveChanges();
                }
                
                var kl = from a in ctx.Appointment
                    join p in ctx.Patient
                        on a.PatientId equals p.Id
                    join d in ctx.Doctor
                        on a.DoctorId equals d.Id
                    where a.Id == AppointmentId
                    select new
                    {
                        DoctorName= d.Name,
                        DoctorDesignation=d.Designation,
                        DoctorDegree=d.Degree,
                        PatientName=p.Name,
                        PatientPhone=p.PhoneNo,
                        PatientAge=p.Age
                    };
                foreach (var v in kl)
                {
                    prescription.DoctorDesignation = baseControl.Decrypt(v.DoctorDesignation);
                    prescription.DoctorName = baseControl.Decrypt(v.DoctorName);
                    prescription.DoctorDegree = baseControl.Decrypt(v.DoctorDegree);
                    prescription.PatientName = baseControl.Decrypt(v.PatientName);
                    prescription.PatientAge = v.PatientAge;
                    prescription.PatientPhone = baseControl.Decrypt(v.PatientPhone);
                }

                Appointment ap = ctx.Appointment.Single(c => c.Id == AppointmentId);
                ap.Prescription = "PatientPrescriptions/"+name;
                ap.Approval = 3;
                ctx.SaveChanges();
            }
            
            var printpdf = new ActionAsPdf("MakePdf", prescription) { FileName = name };
            string path = Server.MapPath("~/PatientPrescriptions");
            string pth = Path.Combine(path, name);
            var byteArray = printpdf.BuildPdf(ControllerContext);
            var fileStream = new FileStream(pth, FileMode.Create, FileAccess.Write);
            fileStream.Write(byteArray, 0, byteArray.Length);
            fileStream.Close();
            return printpdf;

        }
        public ActionResult MakePdf(Prescription idPrescription)
        {
            return View(idPrescription);
        }
	}
}