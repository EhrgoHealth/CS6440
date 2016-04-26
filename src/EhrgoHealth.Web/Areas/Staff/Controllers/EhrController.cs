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
            using (var dbcontext = new ApplicationDbContext())
            {
                // Should be FhirID
                var user = dbcontext.Users.FirstOrDefault(a => a.FhirPatientId == id);
                if (user == null)
                {
                    return new HttpStatusCodeResult(404, "Patient not found");
                }
                if (string.IsNullOrWhiteSpace(user.FhirPatientId))
                {
                    //todo: figure out what to show if the patient has no fhir data setup
                }
                var client = new FhirClient(Constants.IndianaFhirServerBase);
                var patientData = client.Read<Hl7.Fhir.Model.Patient>(Constants.IndianaFhirServerPatientBaseUrl + user.FhirPatientId);
                //Returns a list of medications
                //Probably should be its own method
                IList<string> listOfMedications = new List<string>();

                //First we need to set up the Search Param Object
                SearchParams mySearch = new SearchParams();

                //Create a tuple containing search parameters for SearchParam object
                // equivalent of "MedicationOrder?patient=6116";
                Tuple<string, string> mySearchTuple = new Tuple<string, string>("patient", id);
                mySearch.Parameters.Add(mySearchTuple);

                //Query the fhir server with search parameters, we will retrieve a bundle
                var searchResultResponse = client.Search<Hl7.Fhir.Model.MedicationOrder>(mySearch);

                //There is an array of "entries" that can return. Get a list of all the entries.
                var listOfentries = searchResultResponse.Entry;

                //If no MedicationOrders associated with the patient
                if (listOfentries.Count == 0)
                    return null; //Not sure what we want to return



                //Initializing in for loop is not the greatest.
                foreach (var entry in listOfentries)
                {

                    //The entries we have, do not contain the medication reference.

                    var medicationOrderResource = client.Read<Hl7.Fhir.Model.MedicationOrder>("MedicationOrder/" + entry.Resource.Id);

                    //Casted this because ((ResourceReference)medicationOrderResource.Medication).Reference
                    //is not pretty as a parameter
                    ResourceReference castedResourceReference = (ResourceReference)medicationOrderResource.Medication;

                    var medicationResource = client.Read<Hl7.Fhir.Model.Medication>(castedResourceReference.Reference);

                    CodeableConcept castedCodeableConcept = medicationResource.Code;
                    List<Coding> listOfCodes = castedCodeableConcept.Coding;


                    //Now let us add the medication itself to our list
                    foreach (var c in listOfCodes)
                    {
                        listOfMedications.Add(c.Code);
                    }


                }

                /**Use this for debugging if you want*/
                //string returnResult = String.Empty;
                //foreach (var m in listOfMedications)
                //{
                //    returnResult += m + "\n"; //Stringbuilder class would be better
                //}

            /**At this point, you have a list of all the medications a patient is taking
             * pulled from the FHIR server.  Access it from listOfMedications.
             */ 

            }//end using statement

            //todo: create view probably some kind of basic ehr
            throw new NotImplementedException();
            return View();
        }


        public ActionResult AddMedicationOrder(string id)
        {
            //Note: Understand that MedicationOrder and Medications are 1:1 mappings. Not 1:Many.
            //Medications contain information such the name of the manufacturer and name of the drug
            //Medication order contains information about prescriber, the dosage instruction,
            //and the date the medication was prescribed

            using (var dbcontext = new ApplicationDbContext())
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
                var fhirClient = new FhirClient(Constants.IndianaFhirServerBase);

                //First we need to create our medication
                Medication medication = new Medication();
                medication.Code = new CodeableConcept("ICD-10", "medicationName");

                //Now we need to push this to the server and grab the ID
                var medicationResource = fhirClient.Create<Hl7.Fhir.Model.Medication>(medication);
                string medicationResourceID = medicationResource.Id;

                //Create an empty medication order resource and then assign attributes
                Hl7.Fhir.Model.MedicationOrder fhirMedicationOrder = new Hl7.Fhir.Model.MedicationOrder();

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
