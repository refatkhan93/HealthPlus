using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HealthPlus.Models
{
    public class Patient
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string History { get; set; }
        public string Report { get; set; }
    }
}