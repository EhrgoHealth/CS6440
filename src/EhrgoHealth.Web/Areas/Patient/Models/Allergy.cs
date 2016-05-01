using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EhrgoHealth.Web.Areas.Patient.Models
{
    public class Allergy
    {
        public string MedicationName { get; set; }
        public IEnumerable<string> AllAllergies { get; set; }
    }
}