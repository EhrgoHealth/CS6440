using Autofac;
using Autofac.Integration.Mvc;
using EhrgoHealth.Web.App_Start;
using EhrgoHealth.Web.Models;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Owin;
using Owin.Security.Providers.Fitbit;
using Owin.Security.Providers.Fitbit.Provider;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

[assembly: OwinStartupAttribute(typeof(EhrgoHealth.Web.Startup))]

namespace EhrgoHealth.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var builder = new ContainerBuilder();

            builder.RegisterControllers(typeof(MvcApplication).Assembly);
            builder.RegisterModule(new AutofacWebTypesModule());
            builder.Register((a) =>
            {
                var context = a.Resolve<HttpContextBase>();
                if (context != null)
                {
                    return context.GetOwinContext();
                }
                return null;
            }).InstancePerRequest();
            builder.Register((a) =>
            {
                var context = a.Resolve<IOwinContext>();
                if (context != null)
                {
                    return context.Get<ApplicationSignInManager>();
                }
                return null;
            }).InstancePerRequest();
            builder.Register((a) =>
            {
                var context = a.Resolve<IOwinContext>();
                if (context != null)
                {
                    return context.Get<ApplicationUserManager>();
                }
                return null;
            }).InstancePerRequest();
            builder.Register((a) =>
            {
                var context = a.Resolve<IOwinContext>();
                if (context != null)
                {
                    return context.Get<ApplicationRoleManager>();
                }
                return null;
            }).InstancePerRequest();

            builder.Register((a) =>
            {
                var context = a.Resolve<IOwinContext>();
                if (context != null)
                {
                    return context.Authentication;
                }
                return null;
            }).InstancePerRequest();
            builder.Register(a =>
            {
                var options = new FitbitAuthenticationOptions()
                {
                    //probably shouldnt bake api keys here, but for now it works
                    ClientId = "227PBF",
                    ClientSecret = "dab2395907e7bec0317723bd2f13f4d1"
                };
                options.Scope.Add("nutrition"); //get nutrition data
                options.Provider = new FitbitAuthenticationProvider()
                {
                    OnAuthenticated = b =>
                    {
                        b.Identity.AddClaim(new System.Security.Claims.Claim(Constants.FitbitClaimsToken, b.AccessToken));
                        return Task.CompletedTask; // we don't need to do anything that is async
                    }
                };
                return options;
            });
            builder.Register(a =>
            {
                var context = a.Resolve<HttpContextBase>();
                var fitbitOptions = a.Resolve<FitbitAuthenticationOptions>();
                if (context.User.Identity.IsAuthenticated && (((ClaimsIdentity)context.User?.Identity)?.HasClaim(b => b.Type.Equals(Constants.FitbitClaimsToken)) ?? false))
                {
                    return new Fitbit.Api.Portable.FitbitClient(new Fitbit.Api.Portable.FitbitAppCredentials() { ClientId = fitbitOptions.ClientId, ClientSecret = fitbitOptions.ClientSecret }, new Fitbit.Api.Portable.OAuth2.OAuth2AccessToken() { Token = ((ClaimsIdentity)context.User.Identity).FindFirst(b => b.Type.Equals(Constants.FitbitClaimsToken)).Value });
                }
                return null;
            }).InstancePerRequest();
            builder.RegisterType<ApplicationDbContext>();
            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
            var db = container.Resolve<ApplicationDbContext>();
            app.UseAutofacMiddleware(container);
            app.UseAutofacMvc();
            ConfigureAuth(app, container.Resolve<FitbitAuthenticationOptions>());
            DatabaseBootstrap.Bootstrap(db);
        }
    }
}