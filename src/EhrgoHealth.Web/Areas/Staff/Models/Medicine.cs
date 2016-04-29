using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EhrgoHealth.Web.Areas.Staff.Models
{
    public class Medicine
    {     
        public string Name { get; set; }
        public bool Found { get; set; }
        public string UserFhirID { get; set; }
    }
}