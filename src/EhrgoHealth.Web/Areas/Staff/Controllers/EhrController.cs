using EhrgoHealth.Web.Areas.Staff.Models;
using EhrgoHealth.Web.Models;
using Hl7.Fhir;
using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace EhrgoHealth.Web.Areas.Staff.Controllers
{
    public class EhrController : Controller
    {
        private readonly ApplicationUserManager userManager;

        public EhrController(ApplicationUserManager userManager)
        {
            this.userManager = userManager;
        }

        // GET: Staff/Ehr
        public ActionResult Index()
        {
            //TODO: probably some kind of a page where you can search on patients
            return View();
        }

        // GET: Staff/Ehr/Details/5
        /// <summary>
        /// get a patients details
        /// </summary>
        /// <param name="id">id is a patient id</param>
        /// <returns></returns>
        public async Task<ActionResult> Details(string id)
        {
            
            var tuple = await EhrBase.GetMedicationDetails(id, userManager);
            if (tuple.Item1 == null)
            {
                return new HttpStatusCodeResult(404, "Patient not found");
            }
            else
            {
                var viewModel = new PatientData() { Medications = tuple.Item1, Patient = tuple.Item2 };
                return View(viewModel);
            }         
                //var allergies = new AllergyIntolerance(Constants.IndianaFhirServerBase).GetListOfMedicationAllergies(id, meds.Select(a=>a.))            
        }

       
        [HttpPost]
        public ActionResult Search(string patientSearch)
        {
            using(var dbcontext = new ApplicationDbContext())
            {
                var results = dbcontext
                    .Users
                    .Where(a => a.UserName.Contains(patientSearch) || a.FhirPatientId == patientSearch || a.Id == patientSearch)
                    .Select(a => new SearchResultsItem() { PatientId = a.Id, PatientName = a.UserName })
                    .ToList();
                if(results.Count < 1)
                {
                    return Content("<h4>No Results Found</h4>");
                }
                return View("SearchResultsPartialView", results);
            }
        }

        public ActionResult AddMedicationOrder(string id, string medication)
        {
            //Note: Understand that MedicationOrder and Medications are 1:1 mappings. Not 1:Many.
            //Medications contain information such the name of the manufacturer and name of the drug
            //Medication order contains information about prescriber, the dosage instruction,
            //and the date the medication was prescribed

            EhrBase.AddMedicationOrder(id, medication);

            throw new NotImplementedException();
            return View();
        }
    }
}