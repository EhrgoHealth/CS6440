using System;
using System.Linq;
using Xunit;
using Hl7.Fhir.Model;
using Newtonsoft.Json.Linq;
using System.IO;

namespace EhrgoHealth.WebService.UnitTests
{
    public class TestAllergyIntolerance
    {
        JObject allergyIntoleranceJSON;

        public TestAllergyIntolerance()
        {
          
            allergyIntoleranceJSON = JObject.Parse(File.ReadAllText(@"..\..\AllergyIntolerance.json"));
        }
        [Fact]
        public void TestManualAllergyIntoleranceCode()
        {            
            var allergyCodeIterator =
             from p in allergyIntoleranceJSON["substance"]["coding"]
             select (string)p["code"];

            Assert.Equal("Z88.5", allergyCodeIterator.First<String>());
        }

        [Fact]
        public void TestFHIRAllergyIntoleranceCode()
        {
            var fhirClient = new Hl7.Fhir.Rest.FhirClient("http://fhirtest.uhn.ca/baseDstu2/");
            var allergyResource = fhirClient.Read<AllergyIntolerance>("AllergyIntolerance/6140");
            Coding coding = allergyResource.Substance.Coding.First<Coding>();
            Assert.Equal("Z88.5", coding.Code);
        }
    }
}
