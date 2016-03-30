using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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
            if (fhirServer == null || fhirServer == String.Empty)
                throw new Exception("Invalid URL passed to AllergyIntolerance Constructor");

            fhirClient = new FhirClient(fhirServer);

            //We do not use a database for the webservice. We will hardcode a handful of allergies and their intolerances
            allergyLookup.Add("hydrocodone", new List<string>() { "Z88.5" });
        }

        //The controller is expected to receive two items from an EHR.
        //The List of medications they currently know their patient is taking
        //The patient's ID from FHIR
        /// <summary>
        /// The controller is expected to receive two items from an EHR.
        /// </summary>       
        /// <param name="patientID"> The patient's ID from FHIR</param>
        /// <param name="medications"> The List of medications they currently know their patient is taking</param>
        public List<string> GetListOfMedicationAllergies(int patientID, List<String> medications)
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
            //coding.get

            ////Now we retrieve the list of known allergies their medications can trigger

            List<string> listOfAllergicMedications = DetermineListOfAllergicMedications(medications, lookupPatientsKnownAllergies);


            return listOfAllergicMedications;

            //Now that we have a list of all possible allergies for all the medications the patient is taking
            //We will see if any allergy intolerance currently exists.



        }//end GetListOfMedicationAllergies method

        private List<string> DetermineListOfAllergicMedications(List<string> medications, Dictionary<string, bool> lookupPatientsKnownAllergies)
        {
            List<string> listOfAllergicMedications = new List<string>();
            List<string> currentAllergyCodeList = new List<string>();
            foreach (var m in medications)
            {
                allergyLookup.TryGetValue(m.ToLower(), out currentAllergyCodeList);
                if (currentAllergyCodeList != null && currentAllergyCodeList.Count > 0)
                {
                    foreach (var c in currentAllergyCodeList)
                    {
                        //listOfPossibleAllergies = listOfPossibleAllergies.Concat(currentAllergyList).ToList();
                        if (lookupPatientsKnownAllergies.ContainsKey(c))
                        {
                            //Instead of returning, consider adding to a list so that we can return the list of allergies
                            listOfAllergicMedications.Add(m);
                        }
                    }
                }

            }
            return listOfAllergicMedications;
        }

        //The controller is expected to receive two items from an EHR.
        //The List of medications they currently know their patient is taking
        //The patient's ID from FHIR
        /// <summary>
        /// The controller is expected to receive two items from an EHR.
        /// </summary>       
        /// <param name="patientID"> The patient's ID from FHIR</param>
        /// <param name="medications"> The List of medications they currently know their patient is taking</param>
        public Boolean IsAllergicToMedications(int patientID, List<String> medications)
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

        public Boolean IsAllergic(List<string> medications, Dictionary<string, Boolean> lookupPatientsKnownAllergies)
        {
            List<string> currentAllergyCodeList = new List<string>();
            foreach (var m in medications)
            {
                allergyLookup.TryGetValue(m.ToLower(), out currentAllergyCodeList);
                if (currentAllergyCodeList != null && currentAllergyCodeList.Count > 0)
                {
                    foreach (var c in currentAllergyCodeList)
                    {
                        //listOfPossibleAllergies = listOfPossibleAllergies.Concat(currentAllergyList).ToList();
                        if (lookupPatientsKnownAllergies.ContainsKey(c))
                        {
                            //Instead of returning, consider adding to a list so that we can return the list of allergies
                            return true;
                        }
                    }
                }

            }
            return false;
        }//end IsAllergic method

        public Dictionary<string, Boolean> GetPatientsKnownAllergies(int patientID)
        {
            var allergyResource = fhirClient.Read<Hl7.Fhir.Model.AllergyIntolerance>("AllergyIntolerance/" + patientID);
            //Coding coding = allergyResource.Substance.Coding.First<Coding>();
            if (allergyResource.Substance.Coding.Count == 0)
            {
                //Patient has no known allergies, exit early to avoid unnecessary logic execution
                return null;
            }
            List<Coding> patientsKnownAllergies = allergyResource.Substance.Coding.ToList();

            //Create a dictionary for O(1) lookup time later.
            Dictionary<string, Boolean> lookupPatientsKnownAllergies = new Dictionary<string, Boolean>();

            foreach (var a in patientsKnownAllergies)
            {
                lookupPatientsKnownAllergies.Add(a.Code, true);
            }

            return lookupPatientsKnownAllergies;
        }//end GetPatientsKnownAllergies



    }
}