using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Web;
using System.Web.Mvc;

namespace HealthPlus.Models
{
    public class Prescription
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public string DoctorName { get; set; }
        public string PatientName { get; set; }
        public string PatientAddress { get; set; }
        public string DoctorDesignation { get; set; }
        public string DoctorDegree { get; set; }
        public string PatientPhone { get; set; }
        public int PatientAge { get; set; }
        public int WardId { get; set; }
        [AllowHtml]
        public string Prescript { get; set; }

        

    }
}