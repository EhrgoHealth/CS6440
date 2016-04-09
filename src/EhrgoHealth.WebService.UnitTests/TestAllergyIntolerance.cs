using Xunit;
using Hl7.Fhir.Rest;
using System.Collections.Generic;
using System.Linq;

namespace EhrgoHealth.WebService.UnitTests
{
    public class TestAllergyIntolerance
    {

        //Pass in valid medication that a patient has an allergy to
        [Fact]
        public void TestIsAllergicToMedications01()
        {
            var allergyIntolerance = new AllergyIntolerance("http://fhirtest.uhn.ca/baseDstu2/");
            var patientID = 6140;
            var medications = new List<string>() { "hYdRoCoDoNe" };
            var result = allergyIntolerance.IsAllergicToMedications(patientID, medications);
            Assert.True(result);
        }

        //Pass in medication that a patient does not have an allergy to.
        [Fact]
        public void TestIsAllergicToMedications02()
        {
            var allergyIntolerance = new AllergyIntolerance("http://fhirtest.uhn.ca/baseDstu2/");
            var patientID = 6140;
            var medications = new List<string>() { "NotARealMedicine" };
            var result = allergyIntolerance.IsAllergicToMedications(patientID, medications);
            Assert.False(result);
        }

        //Pass in an invalid Patient ID that does not exist on the FHIR server.
        [Fact]
        public void TestIsAllergicToMedications03()
        {
            var allergyIntolerance = new AllergyIntolerance("http://fhirtest.uhn.ca/baseDstu2/");
            var patientID = 999999;
            var medications = new List<string>() { "NotARealMedicine" };
            FhirOperationException ex = Assert.Throws<FhirOperationException>(() => allergyIntolerance.IsAllergicToMedications(patientID, medications));
            string expected =
                @"Operation was unsuccessful, and returned status 404.";
            string actual = ex.Message.Substring(0, 52);            
            Assert.Equal(expected, actual);
        }

        //Pass in valid medication that a patient has an allergy to and expect a returned list of medication the patient
        //is allergic to.
        [Fact]
        public void TestGetListOfMedicationAllergies01()
        {
            var allergyIntolerance = new AllergyIntolerance("http://fhirtest.uhn.ca/baseDstu2/");
            var patientID = 6140; //Patient ID for FHIR Server
            var medications = new List<string>() { "hydrocodone", "aspirin" }; //medications the patient is taking
            var result = allergyIntolerance.GetListOfMedicationAllergies(patientID, medications).ToList();
            
            //We know the medication we expect to see in this list should be hydrocodone
            //because patient is alergic

            Assert.Equal("hydrocodone", result[0]);
        }

        //Pass in medication that a patient is not allergic to. We should expect an empty list.
        [Fact]
        public void TestGetListOfMedicationAllergies02()
        {
            var allergyIntolerance = new AllergyIntolerance("http://fhirtest.uhn.ca/baseDstu2/");
            var patientID = 6140;
            var medications = new List<string>() { "tylenol" };
            var result = allergyIntolerance.GetListOfMedicationAllergies(patientID, medications).ToList();

            //The patient is not allergic to tylenol and we will be given an empty list.

            Assert.Equal(0,result.Count);
        }
    }
}
