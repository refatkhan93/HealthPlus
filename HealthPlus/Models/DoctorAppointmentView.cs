using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HealthPlus.Models
{
    public class DoctorAppointmentView
    {
        public string Date { get; set; }
        public string Prescription { get; set; }
        public string Name { get; set; }
        public int Approval { get; set; }
        public string Designation { get; set; }

    }
}