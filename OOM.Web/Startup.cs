using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(OOM.Web.Startup))]
namespace OOM.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
