using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Collections.Generic;

namespace EhrgoHealth.WebService.UnitTests
{
    [TestClass]
    public class AllergyIntoleranceTest
    {
        [TestMethod]
        public void TestReadPatientName()
        {
            using (StreamReader r = new StreamReader(@"..\..\Patient.json"))
            {
                string json = r.ReadToEnd();
                Console.WriteLine(json);
               // List<Item> items = JsonConvert.DeserializeObject<List<Item>>(json);
            }
        }
    }
}
