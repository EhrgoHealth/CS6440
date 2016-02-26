using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EhrgoHealth.Web.Areas.Staff.Controllers
{
    [Authorize(Roles = "Staff")]
    public class StaffBaseController : Controller
    {
    }
}