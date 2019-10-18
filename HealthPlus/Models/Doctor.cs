using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HealthPlus.Models
{
    public class Doctor
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Image { get; set; }
        public string Designation { get; set; }
        public string Degree { get; set; }
        public int Fees { get; set; }
        public string Schedule { get; set; }

        public virtual  DoctorCategory CategoryId  { get; set; }

    }
}