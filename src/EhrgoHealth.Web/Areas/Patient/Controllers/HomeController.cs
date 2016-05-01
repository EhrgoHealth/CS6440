using EhrgoHealth.Data;
using EhrgoHealth.Web.Areas.Patient.Models;
using EhrgoHealth.Web.Models;
using Fitbit.Api.Portable;
using Hl7.Fhir;
using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;

using Microsoft.Owin.Security;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace EhrgoHealth.Web.Areas.Patient.Controllers
{
    public class HomeController : PatientBaseController
    {
        private ApplicationUserManager userManager;
        private IAuthenticationManager authManager;

        public HomeController(ApplicationUserManager userManager, IAuthenticationManager authManager)
        {
            this.userManager = userManager;
            this.authManager = authManager;
        }

        public ActionResult Index()
        {
            var user = this.User.Identity;

            return View();
        }

        public ActionResult CheckAllergy()
        {
            return View();
        }

        public async Task<ActionResult> CheckVerification(Models.Medicine med)
        {
            using(var dbcontext = new ApplicationDbContext())
            {
                var allergyIntolerance = new AllergyIntolerance(Constants.HapiFhirServerBase, userManager, authManager);

                var user = dbcontext.Users.FirstOrDefault(a => a.Email == this.User.Identity.Name);
                int patientID;
                if(user == null || !int.TryParse(user.FhirPatientId, out patientID))
                {
                    return new HttpStatusCodeResult(404, "Patient not found");
                }
                if(string.IsNullOrWhiteSpace(user.FhirPatientId))
                {
                    //todo: figure out what to show if the patient has no fhir data setup
                    return new HttpStatusCodeResult(404, "Patient does not have FHIR data");
                }
                var medications = new List<string>() { med.Name };
                med.Found = await allergyIntolerance.IsAllergicToMedications(patientID, this.User.Identity.GetUserId(), medications);
            }
            return View(med);
        }

        public async Task<ActionResult> History()
        {
            var tuple = await EhrBase.GetMedicationDetails(User.Identity.GetUserId(), userManager);
            if(tuple.Item1 == null)
            {
                return new HttpStatusCodeResult(404, "Patient not found");
            }
            else
            {
                var viewModel = new PatientData() { Medications = tuple.Item1, Patient = tuple.Item2 };
                return View(viewModel);
            }
        }

        public ActionResult AddMedicine(Models.Medicine med)
        {
            EhrBase.AddMedicationOrder(User.Identity.GetUserId(), med.Name);
            return View(med);
        }

        public ActionResult AddAllergy()
        {
            return View();
        }

        public async Task<ActionResult> AllergyHistory(Allergy all)
        {
            var userid = this.User.Identity.GetUserId();
            var ald = new AddAllergyData(userManager, null);
            var ls = await ald.GetAllergyList(userid);
            if(ls.Contains(all.MedicationName))
            {
                ls.Add(all.MedicationName);
                await ald.AddAllergyToMedication(all.MedicationName, userid);
            }

                if (all != null)
                {
                    if (ls.Contains(all.MedicationName))
                    {
                        ls.Add(all.MedicationName);
                        await ald.AddAllergyToMedication(user.FhirPatientId, all.MedicationName);
                    }
                }
                else
                    all = new Allergy();

            return View(all);
        }
    }
}