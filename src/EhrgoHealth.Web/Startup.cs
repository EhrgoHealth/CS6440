using System;
using System.Web;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using EhrgoHealth.Web.App_Start;
using EhrgoHealth.Web.Models;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Owin;

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
                if(context != null)
                {
                    return context.GetOwinContext();
                }
                return null;
            });
            builder.Register((a) =>
            {
                var context = a.Resolve<IOwinContext>();
                if(context != null)
                {
                    return context.Get<ApplicationSignInManager>();
                }
                return null;
            });
            builder.Register((a) =>
            {
                var context = a.Resolve<IOwinContext>();
                if(context != null)
                {
                    return context.Get<ApplicationUserManager>();
                }
                return null;
            });
            builder.Register((a) =>
            {
                var context = a.Resolve<IOwinContext>();
                if(context != null)
                {
                    return context.Get<ApplicationRoleManager>();
                }
                return null;
            });

            builder.Register((a) =>
            {
                var context = a.Resolve<IOwinContext>();
                if(context != null)
                {
                    return context.Authentication;
                }
                return null;
            });
            builder.RegisterType<ApplicationDbContext>();
            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
            var db = container.Resolve<ApplicationDbContext>();
            app.UseAutofacMiddleware(container);
            app.UseAutofacMvc();
            ConfigureAuth(app);
            DatabaseBootstrap.Bootstrap(db);
        }
    }
}