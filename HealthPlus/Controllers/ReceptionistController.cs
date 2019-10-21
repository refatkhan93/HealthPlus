using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HealthPlus.Context;
using HealthPlus.Models;

namespace HealthPlus.Controllers
{
    public class ReceptionistController : Controller
    {
        public ReceptionistController()
        {
            if (System.Web.HttpContext.Current.Session["ReceptionistId"] == null)
            {
                RedirectToAction("Login", "Authentication");
            }
        }
        //
        // GET: /Receptionist/
        public ActionResult AppointmentSchedule()
        {
            ViewBag.AppointmentSchedule = "active";
            List<DoctorCategory> doctorCategories;
            using (var ctx = new HospitalContext())
            {

                doctorCategories = ctx.DoctorCategory.ToList();
            }
            ViewBag.Category = doctorCategories;
            return View();
        }
        public JsonResult GetAllDocById(int id)
        {

            List<Doctor> Dlist = new List<Doctor>();
            using (var ctx = new HospitalContext())
            {
                var k = ctx.Doctor.Where(c => c.CategoryId.Id == id).Select(c => new { c.Id, c.Name,c.Schedule}).ToList();
                foreach (var dc in k)
                {
                    Doctor d = new Doctor();
                    d.Id = dc.Id;
                    d.Name = dc.Name;
                    d.Schedule = dc.Schedule;
                    Dlist.Add(d);
                }
            }

            return Json(Dlist);

        }

        public JsonResult GetSchedule(int id)
        {
            string Schedule=String.Empty;            
            using (var ctx = new HospitalContext())
            {
                var k = ctx.Doctor.Where(c => c.Id == id).Select(c => new {c.Schedule }).ToList();
                
                foreach (var dc in k)
                { 
                    Schedule = dc.Schedule;
                }
            }

            return Json(Schedule);
        }

        public JsonResult GetAppointments(int session,int docId,string date)
        {
            List<PatientAppointmentView> pt = new List<PatientAppointmentView>();
            using (var ctx = new HospitalContext())
            {
                var data = (from a in ctx.Appointment
                            join p in ctx.Patient on a.PatientId equals p.Id
                            where a.DoctorId == docId && a.Date == date && a.Session == session && a.Approval < 3
                            select new
                            {
                                pId=a.Id,
                                pName = p.Name,
                                pAddress = p.Address,
                                pPhone = p.PhoneNo,
                                pApprove = a.Approval,
                                pNote=a.Note
                            });
                foreach (var dc in data)
                {
                    PatientAppointmentView p=new PatientAppointmentView();
                    p.AppointmentId = dc.pId;
                    p.Name = dc.pName;
                    p.Address = dc.pAddress;
                    p.PhoneNo = dc.pPhone;
                    p.Approval = dc.pApprove;
                    p.Note = dc.pNote;
                    pt.Add(p);
                }
                
            }
            return Json(pt);
        }

        public JsonResult ChangeApproval(int id,int value)
        {
            using (var ctx = new HospitalContext())
            {
                Appointment a = ctx.Appointment.Single(c => c.Id == id);
                a.Approval = value;
                ctx.SaveChanges();
            }
            return Json('1');
        }
	}
}