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
    public class AuthenticationController : Controller
    {
       
        //
        // GET: /Authentication/
        public ActionResult Login()
        {
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
        [HttpPost]
        public ActionResult Login(Login login)
        {
            string pass = EncodePasswordMd5(login.Password);
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
                            Session["DoctorName"] = k.Name;

                        }
                        return RedirectToAction("Index", "Primary");
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
	}
}