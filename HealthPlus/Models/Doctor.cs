using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

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

        [DisplayName("Category")]
        public int CategoryId { get; set; }

    }
}