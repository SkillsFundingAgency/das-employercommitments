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
    using SFA.DAS.EmployerCommitments.Web.ViewModels;
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/EmployerManageApprentices/ReviewChanges.cshtml")]
    public partial class ReviewChanges : System.Web.Mvc.WebViewPage<SFA.DAS.EmployerCommitments.Web.OrchestratorResponse<SFA.DAS.EmployerCommitments.Web.ViewModels.ManageApprenticeships.UpdateApprenticeshipViewModel>>
    {
        public ReviewChanges()
        {
        }
        public override void Execute()
        {
  
    ViewBag.Title = "Review changes";
    ViewBag.Section = "apprentices";
    ViewBag.PageID = "review-changes";

    Model.Data.CurrentTableHeadingText = "Previous";

    var learnerName = $"{Model.Data?.FirstName} {Model.Data?.LastName}";
    var uln = Model.Data?.OriginalApprenticeship?.ULN;
    var dateOfBirth = Model.Data?.DateOfBirth;

WriteLiteral("\r\n\r\n<div");

WriteLiteral(" class=\"grid-row\"");

WriteLiteral(">\r\n    <div");

WriteLiteral(" class=\"column-two-thirds\"");

WriteLiteral(">\r\n        <form");

WriteLiteral(" method=\"POST\"");

WriteAttribute("action", Tuple.Create(" action=\"", 660), Tuple.Create("\"", 697)
, Tuple.Create(Tuple.Create("", 669), Tuple.Create<System.Object, System.Int32>(Url.Action("ReviewChanges")
, 669), false)
);

WriteLiteral(" novalidate=\"novalidate\"");

WriteAttribute("onsubmit", Tuple.Create(" onsubmit=\"", 722), Tuple.Create("\"", 780)
, Tuple.Create(Tuple.Create("", 733), Tuple.Create("sfa.tagHelper.submitRadioForm(\'", 733), true)
                                                    , Tuple.Create(Tuple.Create("", 764), Tuple.Create<System.Object, System.Int32>(ViewBag.Title
, 764), false)
, Tuple.Create(Tuple.Create("", 778), Tuple.Create("\')", 778), true)
);

WriteLiteral(">\r\n            \r\n");

WriteLiteral("            ");

       Write(Html.AntiForgeryToken());

WriteLiteral("\r\n\r\n            <h1");

WriteLiteral(" class=\"heading-xlarge\"");

WriteLiteral(">");

                                  Write(ViewBag.Title);

WriteLiteral("</h1>\r\n            \r\n");

            
               Html.RenderPartial("_CoreDetail", new CoreDetailsViewModel
               {
                   LearnerName = learnerName,
                   ULN = uln,
                   DateOfBirth = dateOfBirth.DateTime
               }); 
WriteLiteral("\r\n            <p");

WriteLiteral(" class=\"lede\"");

WriteLiteral(">");

                       Write(Model.Data.ProviderName);

WriteLiteral(" have suggested these changes: </p>\r\n\r\n");

WriteLiteral("            ");

       Write(Html.Partial("ApprenticeshipUpdate", Model.Data));

WriteLiteral("\r\n\r\n            <h2");

WriteLiteral(" class=\"heading-medium\"");

WriteLiteral(">Are you happy to approve these changes?</h2>\r\n            <div");

WriteAttribute("class", Tuple.Create(" class=\"", 1417), Tuple.Create("\"", 1509)
, Tuple.Create(Tuple.Create("", 1425), Tuple.Create("form-group", 1425), true)
, Tuple.Create(Tuple.Create(" ", 1435), Tuple.Create<System.Object, System.Int32>(!string.IsNullOrEmpty(Model.Data.ChangesConfirmedError) ? "error" : ""
, 1436), false)
);

WriteLiteral(">\r\n                <fieldset>\r\n                    <legend");

WriteLiteral(" class=\"visually-hidden\"");

WriteLiteral(">Are you happy to approve these changes?</legend>\r\n                    \r\n        " +
"            <div");

WriteLiteral(" id=\"ChangesConfirmed\"");

WriteLiteral("></div>\r\n");

                    
                     if (!string.IsNullOrEmpty(Model.Data.ChangesConfirmedError))
                    {

WriteLiteral("                        <span");

WriteLiteral(" class=\"error-message\"");

WriteLiteral(" id=\"error-message-ChangesConfirmed\"");

WriteLiteral(">");

                                                                                   Write(Model.Data.ChangesConfirmedError);

WriteLiteral("</span>\r\n");

                    }

WriteLiteral("\r\n                    <label");

WriteLiteral(" class=\"block-label selection-button-radio\"");

WriteLiteral(" for=\"changes-approve-true\"");

WriteLiteral(">\r\n");

WriteLiteral("                        ");

                   Write(Html.RadioButton("ChangesConfirmed", true, new { id = "changes-approve-true", dataOptionName = "Yes", onclick = $"sfa.tagHelper.radioButtonClick('{ViewBag.Title}', 'Yes')" }));

WriteLiteral("\r\n                        Yes, approve these changes\r\n                    </label" +
">\r\n                    <label");

WriteLiteral(" class=\"block-label selection-button-radio\"");

WriteLiteral(" for=\"changes-approve-false\"");

WriteLiteral(">\r\n");

WriteLiteral("                        ");

                   Write(Html.RadioButton("ChangesConfirmed", false, new { id = "changes-approve-false", dataOptionName = "No", onclick = $"sfa.tagHelper.radioButtonClick('{ViewBag.Title}', 'No')" }));

WriteLiteral("\r\n                        No, reject these changes\r\n                    </label>\r" +
"\n                </fieldset>\r\n            </div>\r\n\r\n            <button");

WriteLiteral(" type=\"submit\"");

WriteLiteral(" class=\"button\"");

WriteLiteral(" aria-label=\"Continue\"");

WriteLiteral(" id=\"submit-rev-change\"");

WriteLiteral(">Continue</button>\r\n\r\n            <a");

WriteLiteral(" class=\"button-link\"");

WriteAttribute("href", Tuple.Create(" href=\"", 2944), Tuple.Create("\"", 3073)
, Tuple.Create(Tuple.Create("", 2951), Tuple.Create<System.Object, System.Int32>(Url.Action("Details", "EmployerManageApprentices", new { Model.Data.HashedAccountId, Model.Data.HashedApprenticeshipId })
, 2951), false)
);

WriteLiteral(" aria-label=\"Back to apprentice details\"");

WriteLiteral(">\r\n                Cancel and return\r\n            </a>\r\n        </form>\r\n    </di" +
"v>\r\n</div>\r\n\r\n");

DefineSection("breadcrumb", () => {

WriteLiteral("\r\n    <div");

WriteLiteral(" class=\"breadcrumbs\"");

WriteLiteral(">\r\n        <a");

WriteAttribute("href", Tuple.Create(" href=\"", 3273), Tuple.Create("\"", 3402)
, Tuple.Create(Tuple.Create("", 3280), Tuple.Create<System.Object, System.Int32>(Url.Action("Details", "EmployerManageApprentices", new { Model.Data.HashedAccountId, Model.Data.HashedApprenticeshipId} )
, 3280), false)
);

WriteLiteral(" aria-label=\"Back to apprentice details\"");

WriteLiteral(" class=\"back-link\"");

WriteLiteral(">Back to apprentice details</a>\r\n    </div>\r\n");

});

WriteLiteral("\r\n");

DefineSection("gaDataLayer", () => {

WriteLiteral("\r\n    <script>\r\n        sfa.dataLayer.vpv = \'/accounts/apprentices/manage/review-" +
"changes\';\r\n    </script>\r\n");

});

        }
    }
}
#pragma warning restore 1591
