using System.Web.Mvc;

namespace EhrgoHealth.Web.Controllers
{
    public class HomeController : Controller
    {
        [Route("~/")]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Ehrgo Health provides treatment plan verification for patients and their providers";

            return View();
        }

        public ActionResult Patient()
        {
            ViewBag.Message = "EHR Patient Treatment Tracking and Data";

            return View();
        }

        public ActionResult Provider()
        {
            ViewBag.Message = "EHR Provider Treatment Plan Verifier";

            return View();
        }


        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}