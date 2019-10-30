using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using HealthPlus.Context;
using HealthPlus.Models;

namespace HealthPlus.Controllers
{
    public class ReceptionistController : Controller
    {
        BaseController baseControl = new BaseController();
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

        public JsonResult ChangeApproval(int id,int value, int session,int docId,string date)
        {
            int serial = 0;
            using (var ctx = new HospitalContext())
            {
                
                    int amount = ctx.Appointment.Where(c => c.DoctorId == docId && c.Session==session
                       && c.Date==date && c.Approval==1).Select(c=>c.Id).ToList().Count();

                if (value == 1)
                {
                    serial = amount + 1;
                }
                else
                {
                    
                        serial = 0;
                    
                }
                Appointment a = ctx.Appointment.Single(c => c.Id == id);
                a.Approval = value;
                a.SerialNo = serial;
                ctx.SaveChanges();
            }
            return Json('1');
        }

        public ActionResult Report()
        {
            ViewBag.Report = "active";
            return View();
        }

        public ActionResult ReceptionistProfile(string message)
        {
            ViewBag.UpdateMessage = message;
            Receptionist r=new Receptionist();
            int id = Convert.ToInt32(Session["ReceptionistId"]);
            using (var ctx = new HospitalContext())
            {
                var k =
                    ctx.Receptionist.Where(c => c.Id == id)
                        .Select(c => new {c.Name, c.PhoneNo, c.Image})
                        .FirstOrDefault();
                
                r.PhoneNo = k.PhoneNo;
                r.Name = k.Name;
                r.Id = id;
                r.Image = k.Image;
            }
            ViewBag.Receptionist = r;
            return View();
        }

        public ActionResult UpdateReceptionist(HttpPostedFileBase file,Receptionist receptionist,string photoName)
        {
            int id = Convert.ToInt32(Session["ReceptionistId"]);
            string password = baseControl.EncodePasswordMd5(receptionist.Password);
            using (var ctx = new HospitalContext())
            {
                string image = photoName;
                Receptionist p = ctx.Receptionist.Single(c => c.Id == id);
                if (p.Password == password)
                {
                    if (file != null && file.ContentLength > 0)
                    {
                        string fullPath = Request.MapPath("~/" + photoName);
                        if (System.IO.File.Exists(fullPath))
                        {
                            System.IO.File.Delete(fullPath);
                        }
                        try
                        {
                            string fileName = Guid.NewGuid().ToString() + System.IO.Path.GetExtension(file.FileName);
                            string uploadUrl = Server.MapPath("~/Photos");
                            file.SaveAs(Path.Combine(uploadUrl, fileName));

                           /* string path = Path.Combine(Server.MapPath("~/Photos/"),

                                Path.GetFileName(file.FileName));
                            file.SaveAs(path);*/
                            image = "Photos/" + fileName;
                        }
                        catch (Exception ex)
                        {
                            ViewBag.Message = "ERROR:" + ex.Message.ToString();
                        }
                    }
                    p.Name = receptionist.Name;
                    p.Image = image;
                    p.PhoneNo = receptionist.PhoneNo;
                    ctx.SaveChanges();
                    return RedirectToAction("ReceptionistProfile", "Receptionist", new { message = "Infomation Updated Successfully" });
                }

                return RedirectToAction("ReceptionistProfile", "Receptionist", new { message = "Update Failed" });

            }

        }
        
	}
}