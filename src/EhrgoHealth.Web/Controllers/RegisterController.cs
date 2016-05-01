using EhrgoHealth.Web.Models;
using Hl7.Fhir.Rest;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace EhrgoHealth.Web.Controllers
{
    public class RegisterController : AuthorizationBaseController
    {
        public ApplicationUserManager UserManager { get; set; }
        public ApplicationSignInManager SignInManager { get; set; }

        public RegisterController(ApplicationUserManager userManager, ApplicationSignInManager signinManager)
            : base(userManager)
        {
            SignInManager = signinManager;
            UserManager = userManager;
        }

        public ActionResult Index()
        {
            return RedirectToAction("Register");
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View(viewName: "Register", model: new RegisterViewModel());
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
                var client = new FhirClient(Constants.HapiFhirServerBase);
                var fhirResult = client.Create(new Hl7.Fhir.Model.Patient() { });
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email, FhirPatientId = fhirResult.Id };
                var result = await UserManager.CreateAsync(user, model.Password);
                if(result.Succeeded)
                {
                    await UserManager.AddToRolesAsync(user.Id, Enum.GetName(typeof(AccountLevel), model.Account_Level));
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
            // If we got this far, something failed, redisplay form
            return View(viewName: "Register");
        }
    }
}