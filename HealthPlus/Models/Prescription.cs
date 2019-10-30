using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HealthPlus.Models
{
    public class Prescription
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        [AllowHtml]
        public string Prescript { get; set; }

        

    }
}