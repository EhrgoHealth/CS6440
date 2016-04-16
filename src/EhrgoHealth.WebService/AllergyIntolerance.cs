using System;
using System.Collections.Generic;
using System.Linq;
using Hl7.Fhir.Rest;
using Hl7.Fhir.Model;
using System.Net;

namespace EhrgoHealth.WebService
{
    public class AllergyIntolerance
    {       
        private FhirClient fhirClient = null;


        public AllergyIntolerance(string fhirServer)
        {
            if (String.IsNullOrEmpty(fhirServer))
                throw new Exception("Invalid URL passed to AllergyIntolerance Constructor");

            fhirClient = new FhirClient(fhirServer);
           
        }

        //The controller is expected to receive two pieces of data from an EHR.
        //The List of medications they currently know their patient is taking
        //The patient's ID from FHIR
        /// <summary>
        /// The controller is expected to receive two data structures from an EHR.
        /// </summary>       
        /// <param name="patientID"> The patient's ID from FHIR</param>
        /// <param name="medications"> The List of medications they currently know their patient is taking</param>
        public IEnumerable<string> GetListOfMedicationAllergies(int patientID, IList<string> medications)
        {
            //ToDo: Discuss with team about returning a dictionary or wrapper class, so that we can inform the EHR
            //      on both the medication conflict and the allergy code.


            //First let us fetch the known allergies of the patient.
            var returnedAllergies = new List<string>();
            var lookupPatientsKnownAllergies = GetPatientsKnownAllergies(patientID);
            if (lookupPatientsKnownAllergies == null || lookupPatientsKnownAllergies.Count == 0)
            {
                //There are no records on the FHIR server of this patient having any allergies.
                return returnedAllergies;
            }         

            ////Now we retrieve the list of known allergies their medications can trigger
            var listOfAllergicMedications = DetermineListOfAllergicMedications(medications, lookupPatientsKnownAllergies);
            return listOfAllergicMedications;  
        }//end GetListOfMedicationAllergies method


        //Returns an IEnumerable of medications the patient is allergic to
        /// <summary>
        /// Private helper method.
        /// </summary>       
        /// <param name="medications"> List of patients medications</param>
        /// <param name="lookupPatientsKnownAllergies"> Dictionary of allergies the patient has, populated from FHIR</param>
        private IEnumerable<string> DetermineListOfAllergicMedications(IList<string> medications, IDictionary<string, bool> lookupPatientsKnownAllergies)
        {
            return
                medications
                .Where(a => Constants.ALLERGY_LOOKUP.ContainsKey(a))
                .Select(a => new Tuple<string, List<string>>(a, Constants.ALLERGY_LOOKUP[a]))
                .Where(a => a.Item2.Any(c => lookupPatientsKnownAllergies.ContainsKey(c)))
                .Select(a => a.Item1);
        }

        /// If you only care if a patient is allergic to one of their medications, call this method.
        /// <summary>
        /// This method can take use the data given to it from the EHR, as is, and determine if there is a problem.
        /// However, the downside is this method only returns a Boolean. If you want a list of the medications
        /// the patient is allergic to, then call GetListOfMedicationAllergies() instead.
        /// </summary>       
        /// <param name="patientID"> The patient's ID from FHIR</param>
        /// <param name="medications"> The List of medications they currently know their patient is taking</param>
        public Boolean IsAllergicToMedications(int patientID, IList<String> medications)
        {
            //First let us fetch the known allergies of the patient.
            var returnedAllergies = new List<string>();
            var lookupPatientsKnownAllergies = GetPatientsKnownAllergies(patientID);
            if (lookupPatientsKnownAllergies == null)
            {
                //There are no records on the FHIR server of this patient having any allergies.
                return false;
            }
            return IsAllergic(medications, lookupPatientsKnownAllergies);

        }//end IsAllergicToMedications method



        /// Private helper method for comparing the patient's medications against their known allergies
        /// <summary>       
        /// I'm the least happy with this method, because I know I can refactor this to do more to also give me a list or dictionary of my results.
        /// </summary>       
        /// <param name="medications"> List of patients medications</param>
        /// <param name="lookupPatientsKnownAllergies"> Dictonary of patient's known allergies</param>
        private Boolean IsAllergic(IList<string> medications, IDictionary<string, Boolean> lookupPatientsKnownAllergies)
        {
            var currentAllergyCodeList = new List<string>();
            
            return medications
                 .Where(a=>!string.IsNullOrEmpty(a))
                 .Select(a=>a.ToLower())
                 .Where(a => Constants.ALLERGY_LOOKUP.ContainsKey(a))
                 .Select(a => new Tuple<string, List<string>>(a, Constants.ALLERGY_LOOKUP[a]))
                 .Any(a => a.Item2.Any(c => lookupPatientsKnownAllergies.ContainsKey(c)));
        }//end IsAllergic method

         
        /// <summary>       
        /// Returns a dictionary of patient's allergies (ICD-10-CM Diagnosis Codes) from the FHIR server
        /// </summary>       
        /// <param name="patientID"> Unique ID of patient for FHIR server</param>
        public IDictionary<string, Boolean> GetPatientsKnownAllergies(int patientID)
        {
            var lookupPatientsKnownAllergies = new Dictionary<string, Boolean>();

            var listOfAllergyIntoleranceIDs = GetAllergyIntoleranceIDs(patientID.ToString());
           
            if(listOfAllergyIntoleranceIDs.Count == 0)
            {
                //return empty dictionary
                return lookupPatientsKnownAllergies; 
            }

            //Now we go through each AllergyIntoleranceID and add their contents to the dictionary

            foreach(var id in listOfAllergyIntoleranceIDs)
            {
                var allergyResource = fhirClient.Read<Hl7.Fhir.Model.AllergyIntolerance>("AllergyIntolerance/" + id);
                lookupPatientsKnownAllergies = allergyResource.Substance.Coding.Count == 0 ? lookupPatientsKnownAllergies :
                allergyResource.Substance.Coding.ToDictionary(a => a.Code, a => true);
            }

            return lookupPatientsKnownAllergies;
            ////Create a dictionary for O(1) lookup time later.
            //var lookupPatientsKnownAllergies = new Dictionary<string, Boolean>();
            //// var something = fhirClient.WholeSystemSearch(new string[] { "id=6116" });
            ////Attempt to retrieve Allergy Intolerance codes of a patient from the remote FHIR server
            //// var incl = new string[] { "id" };
            ////Bundle results = fhirClient.SearchById<Hl7.Fhir.Model.AllergyIntolerance>("6116", incl);

            //WebRequest webRequest = WebRequest.Create("http://fhirtest.uhn.ca/baseDstu2/AllergyIntolerance?patient=6116");
            //webRequest.Method = "GET";
            //WebResponse webResp = webRequest.GetResponse();
            //var somethingstream = webResp.GetResponseStream();

            //var allergyResource = fhirClient.Read<Hl7.Fhir.Model.AllergyIntolerance>("AllergyIntolerance/" + patientID);

            //return allergyResource.Substance.Coding.Count == 0 ? lookupPatientsKnownAllergies :
            //    allergyResource.Substance.Coding.ToDictionary(a => a.Code, a => true);
        }//end GetPatientsKnownAllergies

        /// <summary>       
        /// Returns a dictionary of patient's allergies (ICD-10-CM Diagnosis Codes) from the FHIR server
        /// </summary>       
        /// <param name="patientID"> Unique ID of patient for FHIR server</param>
        private IList<string> GetAllergyIntoleranceIDs(string patientID)
        {
            IList<string> listOfAllergyIntoleranceIDs = new List<string>();

            //First we need to set up the Search Param Object
            SearchParams mySearch = new SearchParams();

            //Create a tuple containing search parameters for SearchParam object
            // equivalent of "AllergyIntolerance?patient=6116";
            Tuple<string, string> mySearchTuple = new Tuple<string, string>("patient", patientID.ToString());
            mySearch.Parameters.Add(mySearchTuple);

            //Query the fhir server with search parameters, we will retrieve a bundle
            var searchResultResponse = fhirClient.Search<Hl7.Fhir.Model.AllergyIntolerance>(mySearch);

            //There is an array of "entries" that can return. Get a list of all the entries.
            var listOfentries = searchResultResponse.Entry;

            if (listOfentries.Count == 0)
                return listOfAllergyIntoleranceIDs;

           
            //Let us pull out only the Allery Intolerance IDs from the bundle objects
            foreach (var entry in listOfentries)
            {
                listOfAllergyIntoleranceIDs.Add(entry.Resource.Id);
            }

            return listOfAllergyIntoleranceIDs;
          
        }
    }
}