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

namespace SFA.DAS.EmployerCommitments.Web.Views.Shared
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
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/Shared/ApprenticeshipFilters.cshtml")]
    public partial class ApprenticeshipFilters : System.Web.Mvc.WebViewPage<SFA.DAS.EmployerCommitments.Web.ViewModels.ManageApprenticeships.ApprenticeshipFiltersViewModel>
    {

#line default
#line hidden
public System.Web.WebPages.HelperResult RenderFilterOptions(string groupName, List<KeyValuePair<string, string>> options, List<string> selected)
{
#line default
#line hidden
return new System.Web.WebPages.HelperResult(__razor_helper_writer => {
 
    foreach (var option in options)
    {
        var checkedAtt = selected != null && selected.Contains(option.Key) ? "checked=\"checked\"" : "";

WriteLiteralTo(__razor_helper_writer, "        <div");

WriteLiteralTo(__razor_helper_writer, " class=\"item\"");

WriteLiteralTo(__razor_helper_writer, ">\r\n            <label");

WriteAttributeTo(__razor_helper_writer, "for", Tuple.Create(" for=\"", 2506), Tuple.Create("\"", 2536)
, Tuple.Create(Tuple.Create("", 2512), Tuple.Create<System.Object, System.Int32>(groupName
, 2512), false)
, Tuple.Create(Tuple.Create("", 2524), Tuple.Create("-", 2524), true)
, Tuple.Create(Tuple.Create("", 2525), Tuple.Create<System.Object, System.Int32>(option.Key
, 2525), false)
);

WriteLiteralTo(__razor_helper_writer, ">\r\n            <input");

WriteAttributeTo(__razor_helper_writer, "name", Tuple.Create(" name=\"", 2558), Tuple.Create("\"", 2575)
, Tuple.Create(Tuple.Create("", 2565), Tuple.Create<System.Object, System.Int32>(groupName
, 2565), false)
);

WriteAttributeTo(__razor_helper_writer, "value", Tuple.Create(" value=\"", 2576), Tuple.Create("\"", 2595)
, Tuple.Create(Tuple.Create("", 2584), Tuple.Create<System.Object, System.Int32>(option.Key
, 2584), false)
);

WriteLiteralTo(__razor_helper_writer, " class=\"js-option-select\"");

WriteAttributeTo(__razor_helper_writer, "id", Tuple.Create(" id=\"", 2621), Tuple.Create("\"", 2650)
       , Tuple.Create(Tuple.Create("", 2626), Tuple.Create<System.Object, System.Int32>(groupName
, 2626), false)
, Tuple.Create(Tuple.Create("", 2638), Tuple.Create("-", 2638), true)
                   , Tuple.Create(Tuple.Create("", 2639), Tuple.Create<System.Object, System.Int32>(option.Key
, 2639), false)
);

WriteLiteralTo(__razor_helper_writer, " type=\"checkbox\"");

WriteLiteralTo(__razor_helper_writer, " ");

                                                                                                  WriteTo(__razor_helper_writer, checkedAtt);

WriteLiteralTo(__razor_helper_writer, " aria-controls=\"js-search-results-info\">\r\n            <span>");

WriteTo(__razor_helper_writer, option.Value);

WriteLiteralTo(__razor_helper_writer, "</span>\r\n            </label>\r\n        </div>\r\n");

    }

});

#line default
#line hidden
}
#line default
#line hidden

        public ApprenticeshipFilters()
        {
        }
        public override void Execute()
        {
WriteLiteral("<div");

WriteLiteral(" class=\"filter-option-select\"");

WriteLiteral(">\r\n    <div");

WriteLiteral(" class=\"container-head js-container-head\"");

WriteLiteral(">\r\n        <div");

WriteLiteral(" class=\"option-select-label\"");

WriteLiteral(">Status</div>\r\n    </div>\r\n    <div");

WriteLiteral(" class=\"options-container\"");

WriteLiteral(">\r\n");

WriteLiteral("        ");

   Write(RenderFilterOptions("Status", Model.ApprenticeshipStatusOptions, Model.Status));

WriteLiteral("\r\n    </div>\r\n</div>\r\n\r\n<div");

WriteLiteral(" class=\"filter-option-select\"");

WriteLiteral(">\r\n    <div");

WriteLiteral(" class=\"container-head js-container-head\"");

WriteLiteral(">\r\n        <div");

WriteLiteral(" class=\"option-select-label\"");

WriteLiteral(">Alerts</div>\r\n    </div>\r\n    <div");

WriteLiteral(" class=\"options-container\"");

WriteLiteral(">\r\n");

WriteLiteral("        ");

   Write(RenderFilterOptions("RecordStatus", Model.RecordStatusOptions, Model.RecordStatus));

WriteLiteral("\r\n    </div>\r\n</div>\r\n\r\n\r\n<div");

WriteLiteral(" class=\"filter-option-select\"");

WriteLiteral(">\r\n    <div");

WriteLiteral(" class=\"container-head js-container-head\"");

WriteLiteral(">\r\n        <div");

WriteLiteral(" class=\"option-select-label\"");

WriteLiteral(">Training Courses</div>\r\n    </div>\r\n    <div");

WriteLiteral(" class=\"options-container\"");

WriteLiteral(">\r\n");

WriteLiteral("        ");

   Write(RenderFilterOptions("Course", Model.TrainingCourseOptions, Model.Course));

WriteLiteral("\r\n    </div>\r\n</div>\r\n\r\n<div");

WriteLiteral(" class=\"filter-option-select\"");

WriteLiteral(">\r\n    <div");

WriteLiteral(" class=\"container-head js-container-head\"");

WriteLiteral(">\r\n        <div");

WriteLiteral(" class=\"option-select-label\"");

WriteLiteral(">Provider</div>\r\n    </div>\r\n    <div");

WriteLiteral(" class=\"options-container\"");

WriteLiteral(">\r\n");

WriteLiteral("        ");

   Write(RenderFilterOptions("Provider", Model.ProviderOrganisationOptions, Model.Provider));

WriteLiteral("\r\n    </div>\r\n</div>\r\n\r\n<div");

WriteLiteral(" class=\"filter-option-select\"");

WriteLiteral(">\r\n    <div");

WriteLiteral(" class=\"container-head js-container-head\"");

WriteLiteral(">\r\n        ");

WriteLiteral("\r\n        <div");

WriteLiteral(" class=\"option-select-label\"");

WriteLiteral(">Funding Status</div>\r\n    </div>\r\n    <div");

WriteLiteral(" class=\"options-container\"");

WriteLiteral(">\r\n");

WriteLiteral("        ");

   Write(RenderFilterOptions("FundingStatus", Model.FundingStatusOptions, Model.FundingStatus));

WriteLiteral("\r\n    </div>\r\n</div>\r\n\r\n");

WriteLiteral("\r\n\r\n");

        }
    }
}
#pragma warning restore 1591
