using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EhrgoHealth.Web.Areas.Patient.Controllers
{
    [Authorize]
    public class PatientBaseController : Controller
    {
        private readonly ApplicationUserManager userManager;

        public PatientBaseController()
        {
            //this.userManager = userManager;
        }
    }
}