using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using EhrgoHealth.Data;
using Fitbit.Api.Portable;
using Microsoft.AspNet.Identity;

namespace EhrgoHealth.Web.Areas.Staff.Controllers
{
    public class HomeController : StaffBaseController
    {
        private readonly ApplicationUserManager userManager;

        public HomeController(ApplicationUserManager userManager)
        {
            this.userManager = userManager;
        }

        public ActionResult Index()
        {
            var user = this.User.Identity;

            return View();
        }
    }
}