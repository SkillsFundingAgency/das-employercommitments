using Microsoft.Azure;
using System;
using System.Web.Optimization;

namespace SFA.DAS.EmployerCommitments.Web
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            var commitmentsAssetsFolder = CloudConfigurationManager.GetSetting("CommitmentsAssetsFolder");

            bundles.Add(new ScriptBundle("~/comt-dist/bundles/sfajs").Include(
                      "~/comt-dist/javascripts/jquery-1.11.0.min.js",
                      "~/comt-dist/javascripts/govuk-template.js",
                      "~/comt-dist/javascripts/selection-buttons.js",
                      "~/comt-dist/javascripts/showhide-content.js",
                      "~/comt-dist/javascripts/stacker.js",
                      "~/comt-dist/javascripts/app.js"));

            bundles.Add(new ScriptBundle("~/comt-dist/bundles/apprentice").Include(
                      "~/comt-dist/javascripts/apprentice/select2.min.js",
                      "~/comt-dist/javascripts/apprentice/dropdown.js"

                      ));

            bundles.Add(new ScriptBundle("~/comt-dist/bundles/characterLimitation").Include(
                    "~/comt-dist/javascripts/character-limit.js"
                    ));

            bundles.Add(new ScriptBundle("~/comt-dist/bundles/lengthLimitation").Include(
                    "~/comt-dist/javascripts/length-limit.js"
                    ));

            bundles.Add(new ScriptBundle("~/comt-dist/bundles/paymentOrder").Include(
                  "~/comt-dist/javascripts/payment-order.js"
                 ));

            bundles.Add(new ScriptBundle("~/comt-dist/bundles/jqueryvalcustom").Include(
                      "~/Scripts/jquery.validate.js", "~/Scripts/jquery.validate.unobtrusive.custom.js"));

            bundles.Add(new ScriptBundle("~/comt-dist/bundles/lodash").Include(
                        "~/comt-dist/javascripts/lodash.js"));

            bundles.Add(new StyleBundle("~/comt-dist/bundles/screenie6").Include("~/comt-dist/css/screen-ie6.css"));
            bundles.Add(new StyleBundle("~/comt-dist/bundles/screenie7").Include("~/comt-dist/css/screen-ie7.css"));
            bundles.Add(new StyleBundle("~/comt-dist/bundles/screenie8").Include("~/comt-dist/css/screen-ie8.css"));
            bundles.Add(new StyleBundle("~/comt-dist/bundles/screen").Include("~/comt-dist/css/screen.css"));
        }
    }
}
