using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using EhrgoHealth.Web.Models;
using System.Web.Mvc;

namespace EhrgoHealth.Web
{
    public static class EhrBase
    {

        public static async Task<List<Medication>> GetMedicationDataForPatientAsync(string patientId, FhirClient client)
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
                                if (string.IsNullOrWhiteSpace(safeCast)) return null;
                                return client.Read<Medication>(safeCast);
                            })
                            .Where(a => a != null)
                            .ToList(); //tolist to force the queries to occur now
            }
            catch (AggregateException e)
            {
                throw e.Flatten();
            }
            catch (FhirOperationException)
            {
                // if we have issues we likely got a 404 and thus have no medication orders...
                return new List<Medication>();
            }
        }

        public async static Task<Tuple<List<Medication>, Hl7.Fhir.Model.Patient>> GetMedicationDetails(string id, ApplicationUserManager userManager)
        {
            Tuple<List<Medication>, Hl7.Fhir.Model.Patient> tup;
            using (var dbcontext = new ApplicationDbContext())
            {
                // Should be FhirID
                var user = await userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return null;
                }

                var client = new FhirClient(Constants.HapiFhirServerBase);
                if (string.IsNullOrWhiteSpace(user.FhirPatientId))
                {
                    var result = client.Create(new Hl7.Fhir.Model.Patient() { });
                    user.FhirPatientId = result.Id;
                    await userManager.UpdateAsync(user);
                }
                var patient = client.Read<Hl7.Fhir.Model.Patient>(Constants.PatientBaseUrl + user.FhirPatientId);
                tup= new Tuple<List<Medication>, Patient>(await EhrBase.GetMedicationDataForPatientAsync(user.FhirPatientId, client), patient);
                return tup;
            }

        }

        public static void AddMedicationOrder(string id, string medicationName)
        {
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
                var fhirClient = new FhirClient(Constants.HapiFhirServerBase);

                //First we need to create our medication
                var medication = new Medication();
                medication.Code = new CodeableConcept("ICD-10", medicationName);

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
                fhirMedicationOrder.Patient.Reference = "Patient/" + user.FhirPatientId;

                //Push the local patient resource to the FHIR Server and expect a newly assigned ID
                var medicationOrderResource = fhirClient.Create<Hl7.Fhir.Model.MedicationOrder>(fhirMedicationOrder);


            }
        }
    }
}