using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HealthPlus.Models
{
    public class Appointment
    {
        public int Id { get; set; }
        public int DoctorId { get; set; }
        public int PatientId { get; set; }
        public string Note { get; set; }
        public int Approval { get; set; }
        public int SerialNo { get; set; }
        public string Date { get; set; }
        public int Session { get; set; }
        public string Prescription { get; set; }
        public int WardId { get; set; }

    }
}