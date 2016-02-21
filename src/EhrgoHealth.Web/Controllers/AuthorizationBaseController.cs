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
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        // GET: AuthorizationBase

        protected override void Dispose(bool disposing)
        {
            if(disposing)
            {
                if(_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if(_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        protected IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

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

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
    }
}