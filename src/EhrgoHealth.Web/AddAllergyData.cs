using Microsoft.Owin.Security;
using Owin.Security.Providers.Fitbit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EhrgoHealth.Web
{
    public class AddAllergyData
    {
        private ApplicationUserManager userManager;
        private IAuthenticationManager authManager;

        public AddAllergyData()
        {
        }

        public AddAllergyData(ApplicationUserManager userManager, IAuthenticationManager authManager)
        {
            this.userManager = userManager;
            this.authManager = authManager;
        }

        public void AddAllergyToMedication(string fhirPatientId, string medicationName)
        {
        }

        public void RemoveAllergyToMedication(string fhirPatientId, string medicationName)
        {
        }

        public IEnumerable<string> GetAllergyList()
        {
            return Enumerable.Empty<string>();
        }
    }
}