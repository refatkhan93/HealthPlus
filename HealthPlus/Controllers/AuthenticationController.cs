using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using HealthPlus.Context;
using HealthPlus.Models;

namespace HealthPlus.Controllers
{
    public class AuthenticationController : Controller 
    {
        BaseController baseControl = new BaseController();
        //
        // GET: /Authentication/
        public ActionResult Login()
        {
            ViewBag.Login = "active";
            if (Session["ReceptionistId"] == null && Session["PatientId"] == null &&
                Session["DoctorId"] == null && Session["NurseId"] == null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Primary");
            }
        }
        
        [HttpPost]
        public ActionResult Login(Login login)
        {
            login.UserEmail = baseControl.Encrypt(login.UserEmail);
            string pass = baseControl.EncodePasswordMd5(login.Password);
            using (var ctx = new HospitalContext())
            {
                if (login.UserType == 1)
                {
                    var q = ctx.Patient.Where(c => c.Email == login.UserEmail && c.Password == pass).Select(c => new { c.Id, c.Name }).ToList();
                    if (q.Any())
                    {
                        foreach (var k in q)
                        {
                            Session["PatientId"] = k.Id;
                            Session["PatientName"] = k.Name;

                        }
                        return RedirectToAction("Index", "Primary");
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
                            Session["DoctorName"] = baseControl.Decrypt(k.Name);

                        }
                        return RedirectToAction("PrescribePatient", "Doctor");
                    }
                    else
                    {
                        ViewBag.Error = "Login Failed";
                    }
                }
                else if (login.UserType == 3)
                {

                }
                else if (login.UserType == 4)
                {
                    var q = ctx.Receptionist.Where(c => c.Email == login.UserEmail && c.Password == pass).Select(c => new { c.Id, c.Name }).ToList();
                    if (q.Any())
                    {
                        foreach (var k in q)
                        {
                            Session["ReceptionistId"] = k.Id;
                            Session["ReceptionistName"] = k.Name;

                        }
                        return RedirectToAction("Index", "Primary");
                    }
                    else
                    {
                        ViewBag.Error = "Login Failed";
                    }
                }
            }
            return View();
        }
        public ActionResult Register()
        {
            ViewBag.Register = "active";
            if (Session["ReceptionistId"] == null && Session["PatientId"] == null &&
                Session["DoctorId"] == null && Session["NurseId"] == null)
            {
                ViewBag.Register = "active";
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Authentication");
            }
            
        }
        [HttpPost]
        public ActionResult Register(Patient patient)
        {
            using (var ctx = new HospitalContext())
            {
                patient.Email = baseControl.Encrypt(patient.Email);
                var count = ctx.Patient.Where(c => c.Email == patient.Email).ToList().Count;
                if (count == 0)
                {
                    patient.Password = baseControl.EncodePasswordMd5(patient.Password);
                    patient.Name = baseControl.Encrypt(patient.Name);
                    patient.Address = baseControl.Encrypt(patient.Address);
                    patient.PhoneNo = baseControl.Encrypt(patient.PhoneNo);
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
        public ActionResult PatientLogout()
        {
            Session["PatientId"] = null;
            Session["PatientName"] = null;
            return RedirectToAction("Index", "Primary");
        }
        public ActionResult ReceptionistLogout()
        {
            Session["ReceptionistId"] = null;
            Session["ReceptionistName"] = null;
            return RedirectToAction("Index", "Primary");
        }

        public ActionResult DoctorLogout()
        {
            Session["DoctorId"] = null;
            Session["DoctorName"] = null;
            return RedirectToAction("Index", "Primary");
        }

        public ActionResult AdminLogin()
        {
            if (Session["AdminId"] == null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "DoctorAdmin");
            }
            
        }
        [HttpPost]
        public ActionResult AdminLogin(Login login)
        {
            string Password = baseControl.EncodePasswordMd5(login.Password);
            string Email = baseControl.Encrypt(login.UserEmail);

            using (var ctx = new HospitalContext())
            {
                var q=ctx.SystemAdmin.Where(c => c.Email == Email && c.Password == Password)
                    .Select(c => new {c.Id})
                    .FirstOrDefault();
                if (q != null)
                {
                    Session["AdminId"] = q.Id;
                }
                else
                {
                    ViewBag.Error = "Login Failed";
                    return View();
                }
            }

            return RedirectToAction("Index","DoctorAdmin");
        }
        public ActionResult AdminLogout()
        {
            Session["AdminId"] = null;
            return RedirectToAction("AdminLogin", "Authentication");
        }
	}
}