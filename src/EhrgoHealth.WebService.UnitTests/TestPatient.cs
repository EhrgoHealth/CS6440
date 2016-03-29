using Xunit;
using Newtonsoft.Json.Linq;
using System.IO;

/*
This Xunit tests demonstrates Patient.JSON is well formed.
I believe this allows us to be confident in the Patient.JSON when referring to
it for example data.

We may not manually parse the JSON anymore thanks to FHIR.DSTU21 package
but at least it is controlled data and may come in handy if we run into any limitations
with the FHIR.DSTU21 API.

If I extend this test class, it will be with the FHIR.DSTU21 objects
*/

namespace EhrgoHealth.WebService.UnitTests
{
    public class TestPatient
    {
        JObject patientJSON;

        //XUnit uses constructor for [SetUp]
        public TestPatient()
        {
            patientJSON = JObject.Parse(File.ReadAllText(@"..\..\Patient.json"));
        }

        //Parses well-formed JSON for the gender of the patient.
        [Fact]
        public void TestReadPatientGender()
        {
            Assert.Equal("male", patientJSON.GetValue("gender"));
        }

        //Parses well-formed JSON for the birthdate of the patient
        [Fact]
        public void TestReadPatientBirthDate()
        {
            Assert.Equal("1974-12-25", patientJSON.GetValue("birthDate"));
        }
    }
}
