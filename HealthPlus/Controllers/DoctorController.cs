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
            return View();
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
                    p.PatientId = dc.pId;
                    p.Name = baseControl.Decrypt(dc.pName);
                    p.Age = dc.pAge;
                    p.Note = dc.pNote;
                    pt.Add(p);
                }
            }
            return Json(pt);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult PrescribePatient(Prescription prescription,int PatientId)
        {
            ViewBag.Doctor = "active";
            string name;
            name = DateTime.Now.ToString("dd-MM-yyyy") + "_" + prescription.PatientId+"_"+Guid.NewGuid()+ ".pdf";
            var printpdf = new ActionAsPdf("MakePdf", prescription) { FileName = name };
            string path = Server.MapPath("~/PatientPrescriptions");
            string pth=Path.Combine(path, name);
            var byteArray = printpdf.BuildPdf(ControllerContext);
            var fileStream = new FileStream(pth, FileMode.Create, FileAccess.Write);
            fileStream.Write(byteArray, 0, byteArray.Length);
            fileStream.Close();
            using (var ctx = new HospitalContext())
            {
                Appointment ap = ctx.Appointment.Single(c => c.Id == PatientId);
                ap.Prescription = "PatientPrescriptions/"+name;
                ap.Approval = 3;
                ctx.SaveChanges();
            }

            return printpdf;

        }
        public ActionResult MakePdf(Prescription idPrescription)
        {
            return View(idPrescription);
        }
	}
}