using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using Newtonsoft.Json.Linq;

namespace EhrgoHealth.WebService.UnitTests
{
    [TestClass]
    public class JSONParserTest
    {
        static JObject patientJSON;

        [ClassInitialize()]
        public static void ClassInit(TestContext context)
        {
            patientJSON = JObject.Parse(File.ReadAllText(@"..\..\Patient.json"));
        }

        [TestMethod]
        public void TestReadPatientGender()
        {         
            Assert.AreEqual("male", patientJSON.GetValue("gender"));
        }

        [TestMethod]
        public void TestReadPatientBirthDate()
        {
            Assert.AreEqual("1974-12-25", patientJSON.GetValue("birthDate"));
        }
    }
}
