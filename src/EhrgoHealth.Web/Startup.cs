using System;
using System.Web;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
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
                    return context.GetOwinContext().Get<ApplicationSignInManager>();
                }
                return null;
            });
            builder.Register((a) =>
            {
                var context = a.Resolve<HttpContextBase>();
                if(context != null)
                {
                    return context.GetOwinContext().Get<ApplicationUserManager>();
                }
                return null;
            });
            builder.Register((a) =>
            {
                var context = a.Resolve<HttpContextBase>();
                if(context != null)
                {
                    return context.GetOwinContext().Authentication;
                }
                return null;
            });
            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));

            app.UseAutofacMiddleware(container);
            app.UseAutofacMvc();
            ConfigureAuth(app);
        }
    }
}