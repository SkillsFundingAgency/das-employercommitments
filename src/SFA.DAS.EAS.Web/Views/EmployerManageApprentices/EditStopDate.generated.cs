﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SFA.DAS.EmployerCommitments.Web.Views.EmployerManageApprentices
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Web;
    using System.Web.Helpers;
    using System.Web.Mvc;
    using System.Web.Mvc.Ajax;
    using System.Web.Mvc.Html;
    using System.Web.Optimization;
    using System.Web.Routing;
    using System.Web.Security;
    using System.Web.UI;
    using System.Web.WebPages;
    using SFA.DAS.EmployerCommitments;
    using SFA.DAS.EmployerCommitments.Web;
    using SFA.DAS.EmployerCommitments.Web.Extensions;
    using SFA.DAS.EmployerCommitments.Web.ViewModels.ManageApprenticeships;
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/EmployerManageApprentices/EditStopDate.cshtml")]
    public partial class EditStopDate : System.Web.Mvc.WebViewPage<OrchestratorResponse<EditApprenticeshipStopDateViewModel>>
    {
        public EditStopDate()
        {
        }
        public override void Execute()
        {
WriteLiteral("\r\n");

  
    ViewBag.Title = "Edit stop date";
    ViewBag.PageId = "edit-stopdate";
    ViewBag.Section = "apprentices";

WriteLiteral("\r\n\r\n");

DefineSection("pageSpecificJS", () => {

WriteLiteral("\r\n");

WriteLiteral("    ");

Write(Scripts.Render("~/comt-assets/bundles/lengthLimitation"));

WriteLiteral("\r\n");

});

WriteLiteral("<div");

WriteLiteral(" class=\"grid-row\"");

WriteLiteral(">\r\n    <div");

WriteLiteral(" class=\"column-two-thirds\"");

WriteLiteral(">\r\n        <form");

WriteLiteral(" method=\"post\"");

WriteAttribute("action", Tuple.Create(" action=\"", 538), Tuple.Create("\"", 580)
, Tuple.Create(Tuple.Create("", 547), Tuple.Create<System.Object, System.Int32>(Url.RouteUrl("PostEditStopDate")
, 547), false)
);

WriteLiteral(" novalidate");

WriteAttribute("onsubmit", Tuple.Create(" onsubmit=\"", 592), Tuple.Create("\"", 650)
, Tuple.Create(Tuple.Create("", 603), Tuple.Create("sfa.tagHelper.submitRadioForm(\'", 603), true)
                                            , Tuple.Create(Tuple.Create("", 634), Tuple.Create<System.Object, System.Int32>(ViewBag.Title
, 634), false)
, Tuple.Create(Tuple.Create("", 648), Tuple.Create("\')", 648), true)
);

WriteLiteral(">\r\n");

WriteLiteral("            ");

       Write(Html.AntiForgeryToken());

WriteLiteral("\r\n");

WriteLiteral("            ");

       Write(Html.Partial("ValidationSummary", Html.ViewData.ModelState));

WriteLiteral("\r\n            <h1");

WriteLiteral(" class=\"heading-xlarge\"");

WriteLiteral(">");

                                  Write(ViewBag.Title);

WriteLiteral("</h1>\r\n            <table");

WriteLiteral(" class=\"apprentice-to-stop\"");

WriteLiteral(">\r\n                <thead>\r\n                    <tr>\r\n                        <th" +
"");

WriteLiteral(" colspan=\"2\"");

WriteLiteral(" class=\"visually-hidden\"");

WriteLiteral(">Stop apprentice</th>\r\n                    </tr>\r\n                </thead>\r\n     " +
"           <tbody>\r\n                    <tr>\r\n                        <td>Name</" +
"td>\r\n                        <td");

WriteLiteral(" class=\"bold\"");

WriteLiteral(">");

                                    Write(Model.Data.ApprenticeshipName);

WriteLiteral("</td>\r\n                    </tr>\r\n                    <tr>\r\n                     " +
"   <td>Unique learner number</td>\r\n                        <td");

WriteLiteral(" class=\"bold\"");

WriteLiteral(">");

                                    Write(Model.Data.ApprenticeshipULN);

WriteLiteral("</td>\r\n                    </tr>\r\n                    <tr>\r\n                     " +
"   <td>Current stop date</td>\r\n                        <td");

WriteLiteral(" class=\"bold\"");

WriteLiteral(">\r\n");

WriteLiteral("                            ");

                       Write(Model.Data.CurrentStopDate.ToGdsFormat());

WriteLiteral("\r\n                        </td>\r\n                    </tr>\r\n                </tbo" +
"dy>\r\n            </table>\r\n\r\n            <fieldset>\r\n                <div");

WriteAttribute("class", Tuple.Create(" class=\"", 1789), Tuple.Create("\"", 1880)
, Tuple.Create(Tuple.Create("", 1797), Tuple.Create("form-error-group", 1797), true)
, Tuple.Create(Tuple.Create(" ", 1813), Tuple.Create("form-group", 1814), true)
, Tuple.Create(Tuple.Create(" ", 1824), Tuple.Create<System.Object, System.Int32>(Html.AddClassIfPropertyInError("NewStopDate", "error")
, 1825), false)
);

WriteLiteral(" id=\"edit-stopdate-effective\"");

WriteLiteral(">\r\n                    <span");

WriteLiteral(" class=\"form-label-bold\"");

WriteLiteral(">Enter new stop date</span>\r\n                    <span");

WriteLiteral(" class=\"form-hint\"");

WriteLiteral(">The earliest date you can enter is ");

                                                                          Write(Model.Data.EarliestDate.ToFullDateEntryFormat());

WriteLiteral("</span>\r\n                    <div");

WriteLiteral(" class=\"form-date\"");

WriteLiteral(">\r\n");

WriteLiteral("                        ");

                   Write(Html.ValidationMessage("NewStopDate", new {id = "error-message-NewStopDate", @class = "error-message"}));

WriteLiteral("\r\n                        <div");

WriteLiteral(" class=\"form-group form-group-day\"");

WriteLiteral(">\r\n                            <label");

WriteLiteral(" for=\"NewStopDate.Day\"");

WriteLiteral(">\r\n                                Day\r\n                            </label>\r\n");

WriteLiteral("                            ");

                       Write(Html.TextBox("NewStopDate.Day", Model.Data.NewStopDate.Day, new {@class = "form-control length-limit", type = "number", maxlength = "2", min = "1", max = "31", aria_labelledby = "NewStopDate.Day" }));

WriteLiteral("\r\n                        </div>\r\n                        <div");

WriteLiteral(" class=\"form-group form-group-month\"");

WriteLiteral(">\r\n                            <label");

WriteLiteral(" for=\"NewStopDate.Month\"");

WriteLiteral(">\r\n                                Month\r\n                            </label>\r\n");

WriteLiteral("                            ");

                       Write(Html.TextBox("NewStopDate.Month", Model.Data.NewStopDate.Month, new {@class = "form-control length-limit", type = "number", maxlength = "2", min = "1", max = "12", aria_labelledby = "NewStopDate.Month" }));

WriteLiteral("\r\n                        </div>\r\n                        <div");

WriteLiteral(" class=\"form-group form-group-year\"");

WriteLiteral(">\r\n                            <label");

WriteLiteral(" for=\"NewStopDate.Year\"");

WriteLiteral(">\r\n                                Year\r\n                            </label>\r\n");

WriteLiteral("                            ");

                       Write(Html.TextBox("NewStopDate.Year", Model.Data.NewStopDate.Year, new {@class = "form-control length-limit", type = "number", maxlength = "4", min = "1900", max = "9999", aria_labelledby = "NewStopDate.Year" }));

WriteLiteral("\r\n                        </div>\r\n                    </div>\r\n                </d" +
"iv>\r\n                <div");

WriteLiteral(" class=\"form-group\"");

WriteLiteral(">\r\n                    <button");

WriteLiteral(" class=\"button\"");

WriteLiteral(" type=\"submit\"");

WriteLiteral(" id=\"submit-apply-change\"");

WriteLiteral(">Confirm new stop date</button> &nbsp;&nbsp;&nbsp;\r\n                    <a");

WriteLiteral(" class=\"text-link cancel-link\"");

WriteAttribute("href", Tuple.Create(" href=", 4023), Tuple.Create("", 4078)
, Tuple.Create(Tuple.Create("", 4029), Tuple.Create<System.Object, System.Int32>(Url.RouteUrl("OnProgrammeApprenticeshipDetails")
, 4029), false)
);

WriteLiteral(" aria-label=\"Cancel\"");

WriteLiteral(">Cancel</a>\r\n                </div>\r\n            </fieldset>\r\n        </form>\r\n  " +
"  </div>\r\n</div>\r\n\r\n");

DefineSection("breadcrumb", () => {

WriteLiteral("\r\n    <div");

WriteLiteral(" class=\"breadcrumbs\"");

WriteLiteral(">\r\n        <a");

WriteAttribute("href", Tuple.Create(" href=\"", 4263), Tuple.Create("\"", 4319)
, Tuple.Create(Tuple.Create("", 4270), Tuple.Create<System.Object, System.Int32>(Url.RouteUrl("OnProgrammeApprenticeshipDetails")
, 4270), false)
);

WriteLiteral(" aria-label=\"Back to apprentices details\"");

WriteLiteral(" class=\"back-link\"");

WriteLiteral(">Back</a>\r\n    </div>\r\n");

});

DefineSection("gaDataLayer", () => {

WriteLiteral("\r\n    <script>\r\n        sfa.dataLayer.vpv = \'/accounts/apprentices/manage/when-to" +
"-apply-stop\'\r\n    </script>\r\n");

});

        }
    }
}
#pragma warning restore 1591
