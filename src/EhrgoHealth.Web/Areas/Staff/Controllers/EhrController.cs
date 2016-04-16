using EhrgoHealth.Web.Models;
using Hl7.Fhir;
using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EhrgoHealth.Web.Areas.Staff.Controllers
{
    public class EhrController : Controller
    {
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
        public ActionResult Details(string id)
        {
            using(var dbcontext = new ApplicationDbContext())
            {
                var user = dbcontext.Users.FirstOrDefault(a => a.Id == id);
                if(user == null)
                {
                    return new HttpStatusCodeResult(404, "Patient not found");
                }
                if(string.IsNullOrWhiteSpace(user.FhirPatientId))
                {
                    //todo: figure out what to show if the patient has no fhir data setup
                }
                var client = new FhirClient(Constants.IndianaFhirServerBase);
                var patientData = client.Read<Hl7.Fhir.Model.Patient>(Constants.IndianaFhirServerPatientBaseUrl + user.FhirPatientId);
                //todo: figure out how to search on medication orders
                //var medications = client.Search<Hl7.Fhir.Model.MedicationOrder>(new SearchParams() {  })

                //todo: create view probably some kind of basic ehr
                throw new NotImplementedException();
                return View();
            }
        }

        public ActionResult AddMedicationOrder(string id)
        {
            using(var dbcontext = new ApplicationDbContext())
            {
                var user = dbcontext.Users.FirstOrDefault(a => a.Id == id);
                //todo: figure out how to add a medication order into fhir, probably also use james' service to determin if someone will dieeeee
            }

            throw new NotImplementedException();
            return View();
        }
    }
}