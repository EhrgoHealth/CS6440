using System.Collections.Generic;

namespace EhrgoHealth
{
    public static class Constants
    {
        public const string FitbitClaimsToken = "accessToken:fitbit";
        

        //Medications we support
        public const string HYDROCODONE = "hydrocodone";


        //Allergy Intolerance Codes we support
        public const string NARCOTICS = "Z88.5";
        

        //Dictonary looks up possible allergies for a given medication. Example allergyLookup<Medication, List<Codes>>        
        //How to use: Add a new medication, along with a list of the AllergyIntolerance codes
        //Or just append to an existing list of codes for a given medication.
        //Example reference of Z88.5 ando ther codes FHIR is already using:
        //http://www.icd10data.com/ICD10CM/Codes/Z00-Z99/Z77-Z99/Z88-/Z88.5
        public static readonly IDictionary<string, List<string>> ALLERGY_LOOKUP = new Dictionary<string, List<string>>
        {
           [HYDROCODONE] = new List<string> {NARCOTICS},
        };
        
      



    }
}