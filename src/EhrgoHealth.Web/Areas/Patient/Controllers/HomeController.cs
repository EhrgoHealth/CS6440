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
using Hl7.Fhir;
using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;

namespace EhrgoHealth.Web.Areas.Patient.Controllers
{
    public class HomeController : PatientBaseController
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

                var allergyIntolerance = new AllergyIntolerance("http://fhirtest.uhn.ca/baseDstu2/");

                var user = dbcontext.Users.FirstOrDefault(a => a.Email == this.User.Identity.Name);
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

        public ActionResult History(Models.Medicine med)
        {
            return View(med);
        }
        public ActionResult HistoryBak()
        {
            /* 
            using (var dbcontext = new ApplicationDbContext())
            {
                var user = dbcontext.Users.FirstOrDefault(a => a.Email == this.User.Identity.Name);

                if (user == null)
                {
                    return new HttpStatusCodeResult(404, "Patient not found");
                }
                if (string.IsNullOrWhiteSpace(user.FhirPatientId))
                {
                    //todo: figure out what to show if the patient has no fhir data setup
                }
                var fhirClient = new FhirClient("http://fhirtest.uhn.ca/baseDstu2/");
                //var patientData = client.Read<Hl7.Fhir.Model.Patient>(Constants.IndianaFhirServerPatientBaseUrl + user.FhirPatientId);

                //Returns a list of medications
                //Probably should be its own method

                IList<Resource> listOfAllergyIntoleranceIDs = new List<Resource>();

                //First we need to set up the Search Param Object
                SearchParams mySearch = new SearchParams();

                //Create a tuple containing search parameters for SearchParam object
                // equivalent of "AllergyIntolerance?patient=6116";
                Tuple<string, string> mySearchTuple = new Tuple<string, string>("patient", user.FhirPatientId.ToString());
                mySearch.Parameters.Add(mySearchTuple);

                //Query the fhir server with search parameters, we will retrieve a bundle
                var searchResultResponse = fhirClient.Search<Hl7.Fhir.Model.AllergyIntolerance>(mySearch);

                //There is an array of "entries" that can return. Get a list of all the entries.
                var listOfentries = searchResultResponse.Entry;

                if (listOfentries.Count == 0)
                    return null;


                //Let us pull out only the Allery Intolerance IDs from the bundle objects
                foreach (var entry in listOfentries)
                {
                    listOfAllergyIntoleranceIDs.Add(entry.Resource);
                }

                IList<string> listOfMedications = new List<string>();




                //Query the fhir server with search parameters, we will retrieve a bundle
                var medSearchResultResponse = fhirClient.Search<Hl7.Fhir.Model.MedicationOrder>(mySearch);

                //There is an array of "entries" that can return. Get a list of all the entries.
                var listOfMedentries = medSearchResultResponse.Entry;

                //If no MedicationOrders associated with the patient
                if (listOfMedentries.Count == 0)
                    return null; //Not sure what we want to return



                //Initializing in for loop is not the greatest.
                foreach (var entry in listOfMedentries)
                {

                    //The entries we have, do not contain the medication reference.

                    var medicationOrderResource = fhirClient.Read<Hl7.Fhir.Model.MedicationOrder>("MedicationOrder/" + entry.Resource.Id);

                    //Casted this because ((ResourceReference)medicationOrderResource.Medication).Reference
                    //is not pretty as a parameter
                    ResourceReference castedResourceReference = (ResourceReference)medicationOrderResource.Medication;

                    var medicationResource = fhirClient.Read<Hl7.Fhir.Model.Medication>(castedResourceReference.Reference);

                    CodeableConcept castedCodeableConcept = medicationResource.Code;
                    List<Coding> listOfCodes = castedCodeableConcept.Coding;


                    //Now let us add the medication itself to our list
                    foreach (var c in listOfCodes)
                    {
                        listOfMedications.Add(c.Code);
                    }



                }
            }*/
            return View();
        }

    }
}