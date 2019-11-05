using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Web;
using System.Web.Mvc;
using HealthPlus.Context;
using HealthPlus.Models;

namespace HealthPlus.Controllers
{
    public class AdminController : Controller
    {
        BaseController baseController=new BaseController();
        //
        // GET: /Admin/
        public ActionResult PatientAdmitWard()
        {
            ViewBag.PatientAdmitWard = "active";
            List<Ward> wards;
            using (var ctx = new HospitalContext())
            {
                wards = ctx.Ward.ToList();
            }
             
            return View(wards);
        }

        public JsonResult GetPatientByWard(int id)
        {
            List<Prescription> prescriptions = new List<Prescription>();
            using (var ctx = new HospitalContext())
            {
                var q = from a in ctx.Appointment
                    join p in ctx.Patient
                        on a.PatientId equals p.Id
                    join d in ctx.Doctor
                        on a.DoctorId equals d.Id
                    where a.WardId == id
                    select new
                    {
                        aId=a.Id,
                        PName = p.Name,
                        PAge = p.Age,
                        pAddress = p.Address,
                        DName = d.Name,
                        Presc = a.Prescription
                    };

                foreach (var k in q)
                {
                    Prescription p = new Prescription();
                    p.Id = k.aId;
                    p.PatientName = baseController.Decrypt(k.PName);
                    p.DoctorName = baseController.Decrypt(k.DName);
                    p.PatientAge = k.PAge;
                    p.PatientAddress = baseController.Decrypt(k.pAddress);
                    p.Prescript = k.Presc;
                    prescriptions.Add(p);
                }
            }
            return Json(prescriptions);
        }

        public ActionResult ReleasePatient(int id, int wardId)
        {
            using (var ctx = new HospitalContext())
            {
                Appointment ap = ctx.Appointment.Find(id);
                ap.WardId = 0;
                ctx.SaveChanges();
            }
            return Json(1);
        }

        public ActionResult AssignNurse()
        {
            ViewBag.AssignNurse = "active";
            List<Ward> wards;
            List<Nurse> nurse=new List<Nurse>();
            using (var ctx=new HospitalContext())
            {
               var nur = ctx.Nurse.Where(c => c.WardId == 0).Select(c=>new {c.Name,c.Id}).ToList();
                foreach (var nn in nur)
                {
                    Nurse n = new Nurse();
                    n.Name = baseController.Decrypt(nn.Name);
                    n.Id = nn.Id;
                    nurse.Add(n);

                }
                ViewBag.Nurse = nurse;
                wards = ctx.Ward.ToList();
            }
            return View(wards);
        }

        public ActionResult DeAssignNurse(int id)
        {
            using (var ctx = new HospitalContext())
            {
                Nurse n=ctx.Nurse.Find(id);
                n.WardId = 0;
                ctx.SaveChanges();
            }
            return Json(1);

        }

       
        public ActionResult GetNurseByWard(int id)
        {
            List<Nurse> nurse;
            using (var ctx = new HospitalContext())
            {
                nurse = ctx.Nurse.Where(c => c.WardId == id).ToList();
            }
            foreach (Nurse n in nurse)
            {
                n.Name = baseController.Decrypt(n.Name);
                n.Designation = baseController.Decrypt(n.Designation);
                
            }
            return Json(nurse);
        }

        public ActionResult AssignNurseToWard(int nurseId,int wardId)
        {
            using (var ctx = new HospitalContext())
            {
               Nurse nurse = ctx.Nurse.Find(nurseId);
                nurse.WardId = wardId;
                ctx.SaveChanges();
            }
            return Json(1);
        }
    }
}