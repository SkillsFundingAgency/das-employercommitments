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
            bundles.Add(new ScriptBundle("~/comt-assets/bundles/sfajs").Include(
                      "~/comt-assets/javascripts/jquery-1.11.0.min.js",
                      "~/comt-assets/javascripts/govuk-template.js",
                      "~/comt-assets/javascripts/selection-buttons.js",
                      "~/comt-assets/javascripts/showhide-content.js",
                      "~/comt-assets/javascripts/stacker.js",
                      "~/comt-assets/javascripts/app.js"));

            bundles.Add(new ScriptBundle("~/comt-assets/bundles/apprentice").Include(
                      "~/comt-assets/javascripts/apprentice/select2.min.js",
                      "~/comt-assets/javascripts/apprentice/dropdown.js"

                      ));

            bundles.Add(new ScriptBundle("~/comt-assets/bundles/characterLimitation").Include(
                    "~/comt-assets/javascripts/character-limit.js"
                    ));

            bundles.Add(new ScriptBundle("~/comt-assets/bundles/lengthLimitation").Include(
                    "~/comt-assets/javascripts/length-limit.js"
                    ));

            bundles.Add(new ScriptBundle("~/comt-assets/bundles/paymentOrder").Include(
                  "~/comt-assets/javascripts/payment-order.js"
                 ));

            bundles.Add(new ScriptBundle("~/comt-assets/bundles/jqueryvalcustom").Include(
                      "~/Scripts/jquery.validate.js", "~/Scripts/jquery.validate.unobtrusive.custom.js"));

            bundles.Add(new ScriptBundle("~/comt-assets/bundles/lodash").Include(
                        "~/comt-assets/javascripts/lodash.js"));

            bundles.Add(new StyleBundle("~/comt-assets/bundles/screenie6").Include("~/comt-assets/css/screen-ie6.css"));
            bundles.Add(new StyleBundle("~/comt-assets/bundles/screenie7").Include("~/comt-assets/css/screen-ie7.css"));
            bundles.Add(new StyleBundle("~/comt-assets/bundles/screenie8").Include("~/comt-assets/css/screen-ie8.css"));
            bundles.Add(new StyleBundle("~/comt-assets/bundles/screen").Include("~/comt-assets/css/screen.css"));
        }
    }
}
