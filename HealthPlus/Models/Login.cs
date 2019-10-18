using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HealthPlus.Models
{
    public class Login
    {
        public string UserEmail { get; set; }
        public string Password { get; set; }
        public int UserType { get; set; }

    }
}