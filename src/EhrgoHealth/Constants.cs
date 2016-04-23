using System.Collections.Generic;

namespace EhrgoHealth
{
    public static class Constants
    {
        //todo: probably should abstract this in a way where we can use multiple different EHR's
        public const string IndianaFhirServerBase = "http://52.72.172.54:8080/fhir/baseDstu2/";

        public const string IndianaFhirServerPatientBaseUrl = "Patient/";
        public const string FitbitClaimsToken = "accessToken:fitbit";
        

        //Medications we support
        public const string HYDROCODONE = "hydrocodone"; //Narcotic Allergy
        public const string AMOXIL = "amoxil"; //Penicillin Allergy
        public const string ALLEGRA = "allegra"; //Lactose Intolerance
        public const string TEGRETOL = "tegretol"; //Anticonvulsant (Seizure)
        public const string SULFADOXINE = "Sulfadoxine"; //Sulfonamides

        //Allergy Intolerance Codes we support
        public const string NARCOTICS = "Z88.5";
        public const string PENICILLIN = "Z88.0";
        public const string LACTOSE_INTOLERANCE = "E73.9";
        public const string ANTICONVULSANT = "E936.3";
        public const string SULFONAMIDES = "Z88.2";

        //Dictonary looks up possible allergies for a given medication. Example allergyLookup<Medication, List<Codes>>        
        //How to use: Add a new medication, along with a list of the AllergyIntolerance codes
        //Or just append to an existing list of codes for a given medication.
        //Example reference of Z88.5 ando ther codes FHIR is already using:
        //http://www.icd10data.com/ICD10CM/Codes/Z00-Z99/Z77-Z99/Z88-/Z88.5
        public static readonly IDictionary<string, List<string>> ALLERGY_LOOKUP = new Dictionary<string, List<string>>
        {
            [HYDROCODONE] = new List<string> { NARCOTICS },
            [AMOXIL] = new List<string> { PENICILLIN },
            [ALLEGRA] = new List<string> { LACTOSE_INTOLERANCE },
            [TEGRETOL] = new List<string> { ANTICONVULSANT },
            [ALLEGRA] = new List<string> { SULFONAMIDES }
        };  
    }
}