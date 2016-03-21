using Fitbit.Api.Portable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace EhrgoHealth.Web.Areas.Patient.Controllers
{
    public class HomeController : PatientBaseController
    {
        private readonly FitbitClient fitbitClient;
        private readonly ApplicationUserManager userManager;

        public HomeController(ApplicationUserManager userManager)
        {
            this.userManager = userManager;
            this.fitbitClient = fitbitClient;
        }

        public ActionResult Index()
        {
            var user = this.User.Identity;
            return View();
        }
    }
}