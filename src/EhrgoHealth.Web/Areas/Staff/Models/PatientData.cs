using Hl7.Fhir.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EhrgoHealth.Web.Areas.Staff.Models
{
    public class PatientData
    {
        public Hl7.Fhir.Model.Patient Patient { get; set; }

        public virtual ICollection<Medication> Medications { get; set; }
    }
}