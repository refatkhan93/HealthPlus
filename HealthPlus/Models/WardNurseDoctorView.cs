using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HealthPlus.Models
{
    public class WardNurseDoctorView
    {
        public int Id { get; set; }
        public int WardId { get; set; }
        public string WardName { get; set; }
        public int NurseId { get; set; }
        public string NurseName { get; set; }
        public string  Designation { get; set; }


    }
}