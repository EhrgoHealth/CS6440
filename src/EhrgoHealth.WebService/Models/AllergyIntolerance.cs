using System;
using System.Collections.Generic;
using System.Linq;
using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;

namespace EhrgoHealth.WebService.Models
{
    public class AllergyIntolerance
    {
        //Dictonary looks up possible allergies for a given medication. Example allergyLookup<Medication, List<Codes>>
        private Dictionary<string, List<string>> allergyLookup = new Dictionary<string, List<string>>();
        private FhirClient fhirClient = null;


        public AllergyIntolerance(string fhirServer)
        {
            if (String.IsNullOrEmpty(fhirServer))
                throw new Exception("Invalid URL passed to AllergyIntolerance Constructor");

            fhirClient = new FhirClient(fhirServer);

            //We do not use a database for the webservice. We will hardcode a handful of allergies and their intolerances
            allergyLookup.Add("hydrocodone", new List<string>() { "Z88.5" });
        }

        //The controller is expected to receive two pieces of data from an EHR.
        //The List of medications they currently know their patient is taking
        //The patient's ID from FHIR
        /// <summary>
        /// The controller is expected to receive two data structures from an EHR.
        /// </summary>       
        /// <param name="patientID"> The patient's ID from FHIR</param>
        /// <param name="medications"> The List of medications they currently know their patient is taking</param>
        public List<string> GetListOfMedicationAllergies(uint patientID, List<string> medications)
        {
            //ToDo: Discuss with team about returning a dictionary or wrapper class, so that we can inform the EHR
            //      on both the medication conflict and the allergy code.


            //First let us fetch the known allergies of the patient.
            List<string> returnedAllergies = new List<string>();
            Dictionary<string, Boolean> lookupPatientsKnownAllergies = GetPatientsKnownAllergies(patientID);
            if (lookupPatientsKnownAllergies == null)
            {
                //There are no records on the FHIR server of this patient having any allergies.
                return returnedAllergies;
            }         

            ////Now we retrieve the list of known allergies their medications can trigger
            List<string> listOfAllergicMedications = DetermineListOfAllergicMedications(medications, lookupPatientsKnownAllergies);
            return listOfAllergicMedications;  
        }//end GetListOfMedicationAllergies method


        //Populates the list of medications the patient is allergic to
        /// <summary>
        /// Private helper method.
        /// </summary>       
        /// <param name="medications"> List of patients medications</param>
        /// <param name="lookupPatientsKnownAllergies"> Dictionary of allergies the patient has, populated from FHIR</param>
        private List<string> DetermineListOfAllergicMedications(List<string> medications, Dictionary<string, bool> lookupPatientsKnownAllergies)
        {
            List<string> listOfAllergicMedications = new List<string>();
            List<string> currentAllergyCodeList = new List<string>();
            foreach (var m in medications)
            {
                allergyLookup.TryGetValue(m.ToLower(), out currentAllergyCodeList);
                //if (currentAllergyCodeList != null && currentAllergyCodeList.Count > 0)
                //{
                //    foreach (var c in currentAllergyCodeList)
                //    {                       
                //        if (lookupPatientsKnownAllergies.ContainsKey(c))
                //        {                           
                //            listOfAllergicMedications.Add(m);
                //        }
                //    }
                //}
                if (currentAllergyCodeList == null || currentAllergyCodeList.Count < 1)
                {
                    continue;
                }
                //add range is faster as it will resize the array only once to accommodate the extra items.
                listOfAllergicMedications.AddRange(
                currentAllergyCodeList
                .Where(c => lookupPatientsKnownAllergies.ContainsKey(c)));

            }
            return listOfAllergicMedications;
        }

        /// If you only care if a patient is allergic to one of their medications, call this method.
        /// <summary>
        /// This method can take use the data given to it from the EHR, as is, and determine if there is a problem.
        /// However, the downside is this method only returns a Boolean. If you want a list of the medications
        /// the patient is allergic to, then call GetListOfMedicationAllergies() instead.
        /// </summary>       
        /// <param name="patientID"> The patient's ID from FHIR</param>
        /// <param name="medications"> The List of medications they currently know their patient is taking</param>
        public Boolean IsAllergicToMedications(uint patientID, List<String> medications)
        {
            //First let us fetch the known allergies of the patient.
            List<string> returnedAllergies = new List<string>();
            Dictionary<string, Boolean> lookupPatientsKnownAllergies = GetPatientsKnownAllergies(patientID);
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
        private Boolean IsAllergic(List<string> medications, Dictionary<string, Boolean> lookupPatientsKnownAllergies)
        {
            List<string> currentAllergyCodeList = new List<string>();
            foreach (var m in medications)
            {
                allergyLookup.TryGetValue(m.ToLower(), out currentAllergyCodeList);
                if (currentAllergyCodeList != null && currentAllergyCodeList.Count > 0)
                {
                    foreach (var c in currentAllergyCodeList)
                    {                        
                        if (lookupPatientsKnownAllergies.ContainsKey(c))
                        {                           
                            return true;
                        }
                    }
                }

            }
            return false;
        }//end IsAllergic method

        /// Returns a list of patient's allergies from the FHIR server
        /// <summary>       
        /// If you want the list of allergy codes of a patient, then just pass in the patient's ID.
        /// </summary>       
        /// <param name="medications"> List of patients medications</param>
        /// <param name="lookupPatientsKnownAllergies"> Dictonary of patient's known allergies</param>
        public Dictionary<string, Boolean> GetPatientsKnownAllergies(uint patientID)
        {
            //Create a dictionary for O(1) lookup time later.
            Dictionary<string, Boolean> lookupPatientsKnownAllergies = new Dictionary<string, Boolean>();

            //Attempt to retrieve Allergy Intolerance codes of a patient from the remote FHIR server
            var allergyResource = fhirClient.Read<Hl7.Fhir.Model.AllergyIntolerance>("AllergyIntolerance/" + patientID);
            
            return allergyResource.Substance.Coding.Count == 0 ? lookupPatientsKnownAllergies :
                allergyResource.Substance.Coding.ToDictionary(a => a.Code, a => true);
        }//end GetPatientsKnownAllergies

    }
}