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


        //Create static dictionary where "key" = medication and "value" = AllergyIntolerance Code
        public static readonly Dictionary<string, List<string>> ALLERGY_LOOKUP = new Dictionary<string, List<string>>()
        {
           [HYDROCODONE] = new List<string> {NARCOTICS},
        };
        
      



    }
}