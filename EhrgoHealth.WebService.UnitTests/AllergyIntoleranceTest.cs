using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace EhrgoHealth.WebService.UnitTests
{

    [TestClass]
    public class AllergyIntoleranceTest
    {
        static JObject allergyIntoleranceJSON;

        [ClassInitialize()]
        public static void ClassInit(TestContext context)
        {
            allergyIntoleranceJSON = JObject.Parse(File.ReadAllText(@"..\..\AllergyIntolerance.json"));
        }


        [TestMethod]
        public void TestAllergyIntoleranceCode()
        {
            var allergyCodeIterator =
            from p in allergyIntoleranceJSON["substance"]["coding"]
            select (string)p["code"];

            Assert.AreEqual("Z88.5", allergyCodeIterator.First<String>());
        }
    }
}
