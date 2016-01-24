using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(EhrgoHealth.Web.Startup))]

namespace EhrgoHealth.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}