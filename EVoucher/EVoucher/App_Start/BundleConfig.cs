using System.Web;
using System.Web.Optimization;

namespace EVoucher
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/plugins/jquery-ui-1.12.1.custom/jquery-ui.min.js"
                        ));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));
            bundles.Add(new ScriptBundle("~/bundles/angular").Include(
                        "~/Scripts/plugins/angular.min.js",
                        "~/Scripts/plugins/angular-route.min.js",
                        "~/Scripts/plugins/angular-animate.min.js",
                        "~/Scripts/plugins/angular-local-storage.min.js"
                        ));
            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js",
                      "~/Scripts/js/app.min.js",
                      "~/Scripts/js/common.min.js"
                      ));
            bundles.Add(new ScriptBundle("~/bundles/be").Include(
                          "~/Scripts/plugins/angular-local-storage.min.js",
                          "~/Scripts/plugins/loading-bar.min.js",
                        "~/app/app.js",
                        "~/app/services/auth-services.js",
                        "~/app/services/common-services.js",
                        "~/Scripts/plugins/paging.js"
                        ));
            

            bundles.Add(new StyleBundle("~/Content/default").Include(
                      "~/Content/bootstrap.css",
                      "~/Scripts/plugins/jquery-ui-1.12.1.custom/jquery-ui.min.css",
                      "~/Content/site.css"
                      ));
            bundles.Add(new StyleBundle("~/Content/css").Include(
                      //"~/Content/bootstrap.css",
                      //"~/Content/site.css",
                      "~/Content/css/style.css"
                      ));
        }
    }
}
