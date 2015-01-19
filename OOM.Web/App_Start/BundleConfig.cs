using System.Diagnostics;
using System.Web;
using System.Web.Optimization;

namespace OOM.Web
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/angular").Include(
                      "~/Scripts/angular.js",
                      "~/Scripts/angular-mocks.js",
                      "~/Scripts/angular-route.js"));

            bundles.Add(new ScriptBundle("~/bundles/angular-ui").Include(
                      "~/Scripts/angular-ui/ui-bootstrap.js"));

            bundles.Add(new ScriptBundle("~/bundles/app")
                .IncludeDirectory("~/Scripts/app/controllers", "*.js")
                .IncludeDirectory("~/Scripts/app/factories", "*.js")
                .Include("~/Scripts/app/app.js"));

            bundles.Add(new ScriptBundle("~/bundles/ie-fix").Include(
                      "~/Scripts/html5shiv.js",
                      "~/Scripts/html5shiv-printshiv.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/site.css"));

            if (!Debugger.IsAttached)
                BundleTable.EnableOptimizations = true;
        }
    }
}
