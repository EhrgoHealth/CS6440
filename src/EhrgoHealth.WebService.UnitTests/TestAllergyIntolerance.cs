using System;
using Xunit;
using Hl7.Fhir.Rest;
using System.Collections.Generic;

namespace EhrgoHealth.WebService.UnitTests
{
    public class TestAllergyIntolerance
    {

        //Pass in valid medication that a patient has an allergy to
        [Fact]
        public void TestIsAllergicToMedications01()
        {
            Models.AllergyIntolerance allergyIntolerance = new Models.AllergyIntolerance("http://fhirtest.uhn.ca/baseDstu2/");
            uint patientID = 6140;
            List<string> medications = new List<string>() { "hYdRoCoDoNe" };
            Boolean result = allergyIntolerance.IsAllergicToMedications(patientID, medications);
            Assert.True(result);
        }

        //Pass in medication that a patient does not have an allergy to.
        [Fact]
        public void TestIsAllergicToMedications02()
        {
            Models.AllergyIntolerance allergyIntolerance = new Models.AllergyIntolerance("http://fhirtest.uhn.ca/baseDstu2/");
            uint patientID = 6140;
            List<string> medications = new List<string>() { "NotARealMedicine" };
            Boolean result = allergyIntolerance.IsAllergicToMedications(patientID, medications);
            Assert.False(result);
        }

        //Pass in an invalid Patient ID that does not exist on the FHIR server.
        [Fact]
        public void TestIsAllergicToMedications03()
        {
            Models.AllergyIntolerance allergyIntolerance = new Models.AllergyIntolerance("http://fhirtest.uhn.ca/baseDstu2/");
            uint patientID = 999999;
            List<string> medications = new List<string>() { "NotARealMedicine" };
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
            Models.AllergyIntolerance allergyIntolerance = new Models.AllergyIntolerance("http://fhirtest.uhn.ca/baseDstu2/");
            uint patientID = 6140;
            List<string> medications = new List<string>() { "hydrocodone" };
            List<string> result = allergyIntolerance.GetListOfMedicationAllergies(patientID, medications);

            //We know the medication we expect to see in this list should be hydrocodone
            //because patient is alergic

            Assert.Equal("hydrocodone", result[0]);
        }

        //Pass in medication that a patient is not allergic to. We should expect an empty list.
        [Fact]
        public void TestGetListOfMedicationAllergies02()
        {
            Models.AllergyIntolerance allergyIntolerance = new Models.AllergyIntolerance("http://fhirtest.uhn.ca/baseDstu2/");
            uint patientID = 6140;
            List<string> medications = new List<string>() { "tylenol" };
            List<string> result = allergyIntolerance.GetListOfMedicationAllergies(patientID, medications);

            //The patient is not allergic to tylenol and we will be given an empty list.

            Assert.Equal(0,result.Count);
        }
    }
}
