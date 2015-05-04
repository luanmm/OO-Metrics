using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using Hangfire;
using OOM.Model;
using Hangfire.SqlServer;

[assembly: OwinStartup(typeof(OOM.Web.Startup))]

namespace OOM.Web
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            using (var db = new OOMetricsContext())
            {
                db.Database.CreateIfNotExists();
            }

            GlobalConfiguration.Configuration.UseSqlServerStorage("OOMDB", new SqlServerStorageOptions
            {
                InvisibilityTimeout = TimeSpan.FromMinutes(20)
            });

            app.UseHangfireDashboard();
            app.UseHangfireServer();
        }
    }
}
