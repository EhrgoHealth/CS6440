using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using EhrgoHealth.Web.Models;
using Microsoft.AspNet.Identity;

namespace EhrgoHealth.Web.Controllers
{
    public abstract class RegisterBaseController : AuthorizationBaseController
    {
        public ApplicationUserManager UserManager { get; set; }
        public ApplicationSignInManager SignInManager { get; set; }

        //overridden by inheritors
        protected abstract IEnumerable<string> Roles { get; }

        protected abstract string Area { get; }

        protected RegisterBaseController(ApplicationUserManager userManager, ApplicationSignInManager signinManager)
            : base(userManager)
        {
            SignInManager = signinManager;
            UserManager = userManager;
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View(viewName: "Register", model: new RegisterViewModel() { Area = Area });
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if(ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email }; //, UserLevel = model.Account_Level };
                var result = await UserManager.CreateAsync(user, model.Password);
                if(result.Succeeded)
                {
                    await UserManager.AddToRolesAsync(user.Id, this.Roles.ToArray());
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                    return RedirectToAction("Index", "Home");
                }
                AddErrors(result);
            }
            model.Area = Area;
            // If we got this far, something failed, redisplay form
            return View(viewName: "Register", model: model);
        }
    }
}