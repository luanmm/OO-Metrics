using System.Diagnostics;
using System.Web;
using System.Web.Optimization;

namespace OOM.Web
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                "~/Scripts/bootstrap.js",
                "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/ie-fix").Include(
                "~/Scripts/html5shiv.js",
                "~/Scripts/html5shiv-printshiv.js",
                "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/d3").Include(
                "~/Scripts/d3.js",
                "~/Scripts/eventDrops.js",
                "~/Scripts/d3-tip.js"));

            bundles.Add(new ScriptBundle("~/bundles/mathjax").Include(
                "~/Scripts/MathJax/config/oom.js",
                "~/Scripts/MathJax/MathJax.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                "~/Content/site.css"));

            if (!Debugger.IsAttached)
                BundleTable.EnableOptimizations = true;
        }
    }
}
