using EhrgoHealth.Data;
using EhrgoHealth.Web.Areas.Staff.Models;
using EhrgoHealth.Web.Models;
using Fitbit.Api.Portable;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace EhrgoHealth.Web.Areas.Staff.Controllers
{
    public class HomeController : StaffBaseController
    {
        private readonly IAuthenticationManager authManager;
        private readonly ApplicationUserManager userManager;

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

                var user = dbcontext.Users.FirstOrDefault(a => a.FhirPatientId == med.UserFhirID);
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

        public async Task<ActionResult> Details(Models.Medicine med)
        {
            EhrBase.AddMedicationOrder(User.Identity.GetUserId(), med.Name);

            using(var dbcontext = new ApplicationDbContext())
            {
                var user = dbcontext.Users.FirstOrDefault(a => a.FhirPatientId == med.UserFhirID);

                var tuple = await EhrBase.GetMedicationDetails(user.Id, userManager);
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
        }
    }
}