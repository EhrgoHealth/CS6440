using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EhrgoHealth.Web.Areas.Staff.Models
{
    public class SearchResultsItem
    {
        public string PatientId { get; set; }

        [Display(Name = "Patient Email")]
        public string PatientName { get; set; }

        public string FhirId { get; set; }
    }
}