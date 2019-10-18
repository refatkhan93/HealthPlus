using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HealthPlus.Models
{
    public class DoctorCategory
    {
    
        public int Id { get; set; }
        public string Title { get; set; }

        public virtual List<Doctor> Doctors { get; set; }
    }
}