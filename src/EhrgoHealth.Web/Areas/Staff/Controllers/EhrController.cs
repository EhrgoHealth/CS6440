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
            using(var dbcontext = new ApplicationDbContext())
            {
                // Should be FhirID
                var user = await userManager.FindByIdAsync(id);
                if(user == null)
                {
                    return new HttpStatusCodeResult(404, "Patient not found");
                }

                var client = new FhirClient(Constants.HapiFhirServerBase);
                if(string.IsNullOrWhiteSpace(user.FhirPatientId))
                {
                    var result = client.Create(new Hl7.Fhir.Model.Patient() { });
                    user.FhirPatientId = result.Id;
                    await userManager.UpdateAsync(user);
                }
                var patientData = client.Read<Hl7.Fhir.Model.Patient>(Constants.PatientBaseUrl + user.FhirPatientId);

                var meds = await GetMedicationDataForPatientAsync(user.FhirPatientId, client);
                var viewModel = new PatientData() { Medications = meds, Patient = patientData };
                return View(viewModel);
                //var allergies = new AllergyIntolerance(Constants.IndianaFhirServerBase).GetListOfMedicationAllergies(id, meds.Select(a=>a.))
            }
        }

        private static async Task<List<Medication>> GetMedicationDataForPatientAsync(string patientId, FhirClient client)
        {
            var mySearch = new SearchParams();
            mySearch.Parameters.Add(new Tuple<string, string>("patient", patientId));

            try
            {
                //Query the fhir server with search parameters, we will retrieve a bundle
                var searchResultResponse = await Task.Run(() => client.Search<Hl7.Fhir.Model.MedicationOrder>(mySearch));
                //There is an array of "entries" that can return. Get a list of all the entries.
                return
                    searchResultResponse
                        .Entry
                            .AsParallel() //as parallel since we are making network requests
                            .Select(entry =>
                            {
                                var medOrders = client.Read<MedicationOrder>("MedicationOrder/" + entry.Resource.Id);
                                var safeCast = (medOrders?.Medication as ResourceReference)?.Reference;
                                if(string.IsNullOrWhiteSpace(safeCast)) return null;
                                return client.Read<Medication>(safeCast);
                            })
                            .Where(a => a != null)
                            .ToList(); //tolist to force the queries to occur now
            }
            catch(AggregateException e)
            {
                throw e.Flatten();
            }
            catch(FhirOperationException)
            {
                // if we have issues we likely got a 404 and thus have no medication orders...
                return new List<Medication>();
            }
        }

        [HttpPost]
        public ActionResult Search(string patientSearch)
        {
            using(var dbcontext = new ApplicationDbContext())
            {
                var results = dbcontext
                    .Users
                    .Where(a => a.UserName.Contains(patientSearch) || a.Id == patientSearch)
                    .Select(a => new SearchResultsItem() { PatientId = a.Id, PatientName = a.UserName })
                    .ToList();
                if(results.Count < 1)
                {
                    return Content("<h4>No Results Found</h4>");
                }
                return View("SearchResultsPartialView", results);
            }
        }

        public ActionResult AddMedicationOrder(string id)
        {
            //Note: Understand that MedicationOrder and Medications are 1:1 mappings. Not 1:Many.
            //Medications contain information such the name of the manufacturer and name of the drug
            //Medication order contains information about prescriber, the dosage instruction,
            //and the date the medication was prescribed

            using(var dbcontext = new ApplicationDbContext())
            {
                var user = dbcontext.Users.FirstOrDefault(a => a.Id == id);
                //todo: I'm not sure about the web portion with regards with what the view should return, I leave
                //      leave that to you, but I left logic to return the medication order ID if you desire. To
                //      show whether the post was successful

                //todo: I do not know where the medicationName will be pulled from, so change harcoded "medicationName"
                //      to the parameter name you expect to use.

                //Full list of Parameters you may also decide to pass in:
                //patientID (done), medicationName, system, and display

                //First let us create the FHIR client
                var fhirClient = new FhirClient(Constants.HapiFhirServerBase);

                //First we need to create our medication
                var medication = new Medication();
                medication.Code = new CodeableConcept("ICD-10", "medicationName");

                //Now we need to push this to the server and grab the ID
                var medicationResource = fhirClient.Create<Hl7.Fhir.Model.Medication>(medication);
                var medicationResourceID = medicationResource.Id;

                //Create an empty medication order resource and then assign attributes
                var fhirMedicationOrder = new Hl7.Fhir.Model.MedicationOrder();

                //There is no API for "Reference" in MedicationOrder model, unlike Patient model.
                //You must initialize ResourceReference inline.
                fhirMedicationOrder.Medication = new ResourceReference()
                {
                    Reference = fhirClient.Endpoint.OriginalString + "Medication/" + medicationResourceID,
                    Display = "EhrgoHealth"
                };

                //Now associate Medication Order to a Patient
                fhirMedicationOrder.Patient = new ResourceReference();
                fhirMedicationOrder.Patient.Reference = "Patient/" + id;

                //Push the local patient resource to the FHIR Server and expect a newly assigned ID
                var medicationOrderResource = fhirClient.Create<Hl7.Fhir.Model.MedicationOrder>(fhirMedicationOrder);

                /* Uncoment or use the below logic if you want to put a break point or store the
                 * the medicationOrderID for testing.

                 String returnID = "The newly created Medication ID is: ";
                 returnID += medicationOrderResource.Id;
               */
            }

            throw new NotImplementedException();
            return View();
        }
    }
}