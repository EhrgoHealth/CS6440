using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using EhrgoHealth.Data;
using EhrgoHealth.Web.MVCActionResults;
using Fitbit.Api.Portable;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Owin.Security.Providers.Fitbit;

namespace EhrgoHealth.Web.Areas.Patient.Controllers
{
    public class FitbitController : PatientBaseController
    {
        private readonly IAuthenticationManager authManager;
        private readonly FitbitAuthenticationOptions fitbitAuth;
        private readonly ApplicationUserManager userManager;

        public FitbitController(ApplicationUserManager userManager, IAuthenticationManager authManager, FitbitAuthenticationOptions fitbitAuth)
        {
            this.userManager = userManager;
            this.fitbitAuth = fitbitAuth;
            this.authManager = authManager;
        }

        public async Task<ActionResult> ImportCurrentUserData()
        {
            var identity = await userManager.FindByIdAsync(this.User.Identity.GetUserId());
            if(!identity?.Claims?.Any(b => b.ClaimType.Equals(Constants.FitbitClaimsToken)) ?? true) //null coalece to true which will be false because of the ! at the start of the if
            {
                return new ChallengeResult("Fitbit", Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = Request.Url.PathAndQuery, Area = string.Empty }));
            }
            var token = identity.Claims.First(a => a.ClaimType == Constants.FitbitClaimsToken);
            var fitbitClient = new FitbitClient(new FitbitAppCredentials() { ClientId = this.fitbitAuth.ClientId, ClientSecret = this.fitbitAuth.ClientSecret }, new Fitbit.Api.Portable.OAuth2.OAuth2AccessToken() { Token = token.ClaimValue }, false);
            var totalFoodForLast30Days = Enumerable.Range(0, 5)
                .Select(a => DateTime.Now.AddDays(-a))
                .Select(a =>
                fitbitClient.GetFoodAsync(a)
                );
            var results = await Task.WhenAll(totalFoodForLast30Days);

            results
                .SelectMany(a => a.Foods)
                .Select(a =>
                new FoodLog
                {
                    NutritionalValues = AutoMapper.Mapper.Map<Fitbit.Models.NutritionalValues, Data.NutritionalValues>(a.NutritionalValues),
                    FoodName = a.LoggedFood.Name,
                    FoodBrand = a.LoggedFood.Brand,
                    LoggedDate = a.LogDate
                })
                .Where(a => !identity.FoodLogs.Any(b => b.LoggedDate == a.LoggedDate && b.FoodName == a.FoodName && b.FoodBrand == a.FoodBrand))
                .ForEach(a => identity.FoodLogs.Add(a));
            await userManager.UpdateAsync(identity);

            return Content("Imported");
        }
    }
}