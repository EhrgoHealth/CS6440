using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EhrgoHealth.Web.Controllers;

namespace EhrgoHealth.Web.Areas.Staff.Controllers
{
    public class RegisterController : RegisterBaseController
    {
        public RegisterController(ApplicationUserManager userManager, ApplicationSignInManager signinManager)
            : base(userManager, signinManager)
        {
            SignInManager = signinManager;
            UserManager = userManager;
            ViewBag.Area = "Staff";
        }

        protected override string Area
        {
            get
            {
                return "Staff";
            }
        }

        protected override IEnumerable<string> Roles { get { return new List<string> { "Staff" }; } }
    }
}