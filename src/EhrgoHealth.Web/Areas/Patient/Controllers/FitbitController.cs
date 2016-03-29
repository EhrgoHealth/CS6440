using EhrgoHealth.Data;
using EhrgoHealth.Web.MVCActionResults;
using Fitbit.Api.Portable;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Owin.Security.Providers.Fitbit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

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

        /// <summary>
        /// Imports data from fitbit
        /// </summary>
        /// <param name="daysSince">get data since x days ago</param>
        /// <returns></returns>
        public async Task<ActionResult> Import(int daysSince = 90)
        {
            var identity = await userManager.FindByIdAsync(this.User.Identity.GetUserId());
            if(!identity?.Claims?.Any(b => b.ClaimType.Equals(Constants.FitbitClaimsToken)) ?? true) //null coalece to true which will be false because of the ! at the start of the if
            {
                return new ChallengeResult("Fitbit", Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = Request.Url.PathAndQuery, Area = string.Empty }));
            }
            var token = identity.Claims.First(a => a.ClaimType == Constants.FitbitClaimsToken);
            var fitbitClient = new FitbitClient(new FitbitAppCredentials() { ClientId = this.fitbitAuth.ClientId, ClientSecret = this.fitbitAuth.ClientSecret }, new Fitbit.Api.Portable.OAuth2.OAuth2AccessToken() { Token = token.ClaimValue }, false);
            var totalFoodForRange = Enumerable.Range(0, daysSince)
                .Select(a => DateTime.Now.AddDays(-a))
                .Select(a =>
                fitbitClient.GetFoodAsync(a)
                );
            var results = await Task.WhenAll(totalFoodForRange);

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