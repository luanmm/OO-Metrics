using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using Hangfire;

[assembly: OwinStartup(typeof(OOM.Web.Startup))]

namespace OOM.Web
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            GlobalConfiguration.Configuration.UseSqlServerStorage("OOMDB");

            app.UseHangfireDashboard();
            app.UseHangfireServer();
        }
    }
}
