using EhrgoHealth.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EhrgoHealth.Web.Areas.Patient.Models
{
    public class FitbitViewModel
    {
        public string ToastText { get; set; }
        public string ToastClass { get; set; }
        public ICollection<FoodLog> FoodLog { get; set; }
    }
}