using Xunit;
using Newtonsoft.Json.Linq;
using System.IO;

namespace EhrgoHealth.WebService.UnitTests
{
    public class TestPatient
    {
        JObject patientJSON;

        public TestPatient()
        {
            patientJSON = JObject.Parse(File.ReadAllText(@"..\..\Patient.json"));
        }

        [Fact]
        public void TestReadPatientGender()
        {
            Assert.Equal("male", patientJSON.GetValue("gender"));
        }

        [Fact]
        public void TestReadPatientBirthDate()
        {
            Assert.Equal("1974-12-25", patientJSON.GetValue("birthDate"));
        }
    }
}
