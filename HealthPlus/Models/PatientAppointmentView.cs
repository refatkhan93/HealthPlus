using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HealthPlus.Models
{
    public class PatientAppointmentView
    {
        public int AppointmentId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public int Approval { get; set; }
        public string PhoneNo { get; set; }
        public string Note { get; set; }
    }
}