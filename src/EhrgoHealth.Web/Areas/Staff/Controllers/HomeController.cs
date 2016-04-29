using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using EhrgoHealth.Data;
using Fitbit.Api.Portable;
using Microsoft.AspNet.Identity;
using EhrgoHealth.Web.Models;

namespace EhrgoHealth.Web.Areas.Staff.Controllers
{
    public class HomeController : StaffBaseController
    {
        private readonly ApplicationUserManager userManager;

        public HomeController(ApplicationUserManager userManager)
        {
            this.userManager = userManager;
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

        public ActionResult CheckVerification(Models.Medicine med)
        {
            using (var dbcontext = new ApplicationDbContext())
            {
              
                var allergyIntolerance = new AllergyIntolerance(Constants.HapiFhirServerBase);

                var user = dbcontext.Users.FirstOrDefault(a => a.FhirPatientId == med.UserFhirID);
                int patientID;
                if (user == null || !int.TryParse(user.FhirPatientId, out patientID))
                {
                    return new HttpStatusCodeResult(404, "Patient not found");
                }
                if (string.IsNullOrWhiteSpace(user.FhirPatientId))
                {
                    //todo: figure out what to show if the patient has no fhir data setup
                    return new HttpStatusCodeResult(404, "Patient does not have FHIR data");
                }
                var medications = new List<string>() { med.Name };
                med.Found = allergyIntolerance.IsAllergicToMedications(patientID, medications);
            }
            return View(med);
        }
   
    }
}