﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HealthPlus.Models
{
    public class Blood
    {
        public int Id { get; set; }
        public string Name { get; set; }
       
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string BloodGroup { get; set; }
    }
}