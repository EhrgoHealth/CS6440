using System;
using System.Linq;
using Xunit;
using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Collections.Generic;

namespace EhrgoHealth.WebService.UnitTests
{
    public class TestAllergyIntolerance
    {
        JObject allergyIntoleranceJSON;
        FhirClient fhirClient;

        //XUnit uses constructor for [SetUp]
        public TestAllergyIntolerance()
        {          
            allergyIntoleranceJSON = JObject.Parse(File.ReadAllText(@"..\..\AllergyIntolerance.json"));

            //Test server with public data. I plan to MOQ this with controlled data later
            //As this test will cause tests to fail if the Admin purges the data on their FHIR server
            fhirClient = new FhirClient("http://fhirtest.uhn.ca/baseDstu2/");
        }

        //Parses our own wellformed AllergyIntolerance.json
        //I believe this is useful for having a known-good controlled JSON file of AllergyIntolerance
        //to easily see "real" example data compared to the abstract in the documentation.
        [Fact]
        public void TestManualAllergyIntoleranceCode()
        {            
            var allergyCodeIterator =
             from p in allergyIntoleranceJSON["substance"]["coding"]
             select (string)p["code"];

            Assert.Equal("Z88.5", allergyCodeIterator.First<String>());
        }

        //Grabs deserializes the AllergyIntolerance resource into an object
        //Test to make sure we can grab the allergy intolerance code.
        //This is real data from a real FHIR server at the moment, I do plan on using MOQ
        [Fact]
        public void TestFHIRAllergyIntoleranceCode()
        {         
            var allergyResource = fhirClient.Read<AllergyIntolerance>("AllergyIntolerance/6140");
            Coding coding = allergyResource.Substance.Coding.First<Coding>();
            Assert.Equal("Z88.5", coding.Code);
        }

        [Fact]
        public void TestGetListOfMedicationAllergies01()
        {
            //var allergyResource = fhirClient.Read<AllergyIntolerance>("AllergyIntolerance/6140");
            //Coding coding = allergyResource.Substance.Coding.First<Coding>();
            //Assert.Equal("Z88.5", coding.Code);
            Models.AllergyIntolerance allergyIntolerance = new Models.AllergyIntolerance("http://fhirtest.uhn.ca/baseDstu2/");
            int patientID = 6140;
            List<string> medications = new List<string>() {"Hydrocodone"};
            Boolean result = allergyIntolerance.IsAllergicToMedications(patientID, medications);
            Assert.Equal(true, result);
        }

        [Fact]
        public void TestGetListOfMedicationAllergies02()
        {
            //var allergyResource = fhirClient.Read<AllergyIntolerance>("AllergyIntolerance/6140");
            //Coding coding = allergyResource.Substance.Coding.First<Coding>();
            //Assert.Equal("Z88.5", coding.Code);
            Models.AllergyIntolerance allergyIntolerance = new Models.AllergyIntolerance("http://fhirtest.uhn.ca/baseDstu2/");
            int patientID = 6140;
            List<string> medications = new List<string>() { "zydrocodone" };
            Boolean result = allergyIntolerance.IsAllergicToMedications(patientID, medications);
            Assert.Equal(false, result);
        }
    }
}
