using System.Web;
using System.Web.Mvc;
using EhrgoHealth.Web.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

namespace EhrgoHealth.Web.Controllers
{
    public abstract class AuthorizationBaseController : Controller
    {
        private ApplicationUserManager UserManager { get; set; }

        protected AuthorizationBaseController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
        }

        // GET: AuthorizationBase

        protected void AddErrors(IdentityResult result)
        {
            foreach(var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        protected ActionResult RedirectToLocal(string returnUrl)
        {
            if(Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        protected bool HasPassword()
        {
            var user = CurrentUserIdentity();
            if(user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        protected ApplicationUser CurrentUserIdentity()
        {
            return UserManager.FindById(User.Identity.GetUserId());
        }
    }
}