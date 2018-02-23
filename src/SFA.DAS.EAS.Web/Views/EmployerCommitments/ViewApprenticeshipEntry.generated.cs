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

namespace SFA.DAS.EmployerCommitments.Web.Views.EmployerCommitments
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
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/EmployerCommitments/ViewApprenticeshipEntry.cshtml")]
    public partial class ViewApprenticeshipEntry : System.Web.Mvc.WebViewPage<OrchestratorResponse<SFA.DAS.EmployerCommitments.Web.ViewModels.ApprenticeshipViewModel>>
    {
            
    public string FormatCost(string value)
    {
        decimal cost;
        if (Decimal.TryParse(value, out cost))
        {
            return $"£{cost:n0}";
        }

        return string.Empty;
    }

        public ViewApprenticeshipEntry()
        {
        }
        public override void Execute()
        {
  
    ViewBag.Title = "View an apprentice";
    ViewBag.Section = "apprentices";
    ViewBag.PageID = "view-apprentice";

WriteLiteral("\r\n\r\n\r\n<div");

WriteLiteral(" class=\"grid-row\"");

WriteLiteral(">\r\n    <div");

WriteLiteral(" class=\"column-two-thirds\"");

WriteLiteral(">\r\n\r\n        <h1");

WriteLiteral(" class=\"heading-xlarge\"");

WriteLiteral(">View apprentice details</h1>\r\n\r\n\r\n        <table");

WriteLiteral(" id=\"apprentice-section\"");

WriteLiteral(">\r\n            <tbody>\r\n                <tr>\r\n                    <td>First name<" +
"/td>\r\n                    <td>");

                   Write(Model.Data.FirstName);

WriteLiteral("</td>\r\n                </tr>\r\n\r\n            <tr>\r\n                <td>Last name</" +
"td>\r\n                <td>");

               Write(Model.Data.LastName);

WriteLiteral("</td>\r\n            </tr>\r\n                \r\n            <tr>\r\n                <td" +
">Unique learner number</td>\r\n                <td");

WriteLiteral(" id=\"uln\"");

WriteLiteral(">");

                              if (!string.IsNullOrEmpty(Model.Data.ULN))
                             {

WriteLiteral("                                 <span>");

                                  Write(Model.Data.ULN);

WriteLiteral("</span>\r\n");

                             }
                             else
                             {

WriteLiteral("                                 <span");

WriteLiteral(" class=\"missing\"");

WriteLiteral(">&ndash;&ndash;</span>\r\n");

                             }
WriteLiteral("</td>\r\n            </tr>\r\n\r\n            <tr>\r\n                    <td>Date of bir" +
"th</td>\r\n                    <td>\r\n");

                        
                         if (Model.Data.DateOfBirth.DateTime.HasValue)
                        {

WriteLiteral("                            <span>");

                             Write(Model.Data.DateOfBirth.DateTime.Value.ToGdsFormatWithoutDay());

WriteLiteral("</span>\r\n");

                        }
                        else
                        {

WriteLiteral("                            <span");

WriteLiteral(" class=\"missing\"");

WriteLiteral(">&ndash;&ndash;</span>\r\n");

                        }

WriteLiteral("                    </td>\r\n                </tr>\r\n\r\n                <tr>\r\n       " +
"             <td>Apprenticeship training course</td>\r\n                    <td>\r\n" +
"");

                        
                         if (!string.IsNullOrWhiteSpace(Model.Data.TrainingName))
                        {

WriteLiteral("                            <span>");

                             Write(Model.Data.TrainingName);

WriteLiteral("</span>\r\n");

                        }
                        else
                        {

WriteLiteral("                            <span");

WriteLiteral(" class=\"missing\"");

WriteLiteral(">&ndash;&ndash;</span>\r\n");

                        }

WriteLiteral("\r\n                    </td>\r\n                </tr>\r\n\r\n                <tr>\r\n     " +
"               <td>Planned training start date</td>\r\n                    <td>\r\n");

                        
                         if (Model.Data.StartDate.DateTime.HasValue)
                        {

WriteLiteral("                            <span>");

                             Write(Model.Data.StartDate.DateTime.Value.ToGdsFormatLongMonthNameWithoutDay());

WriteLiteral("</span>\r\n");

                        }
                        else
                        {

WriteLiteral("                            <span");

WriteLiteral(" class=\"missing\"");

WriteLiteral(">&ndash;&ndash;</span>\r\n");

                        }

WriteLiteral("                    </td>\r\n                </tr>\r\n\r\n                <tr>\r\n       " +
"             <td>Planned training end date</td>\r\n                    <td>\r\n");

                        
                         if (Model.Data.EndDate.DateTime.HasValue)
                        {

WriteLiteral("                            <span>");

                             Write(Model.Data.EndDate.DateTime.Value.ToGdsFormatLongMonthNameWithoutDay());

WriteLiteral("</span>\r\n");

                        }
                        else
                        {

WriteLiteral("                            <span");

WriteLiteral(" class=\"missing\"");

WriteLiteral(">&ndash;&ndash;</span>\r\n");

                        }

WriteLiteral("                    </td>\r\n                </tr>\r\n\r\n                <tr>\r\n       " +
"             <td>Total agreed apprenticeship price</td>\r\n                    <td" +
">\r\n");

                        
                         if (!string.IsNullOrWhiteSpace(Model.Data.Cost))
                        {

WriteLiteral("                            <span>");

                             Write(FormatCost(Model.Data.Cost));

WriteLiteral("</span>\r\n");

                        }
                        else
                        {

WriteLiteral("                            <span");

WriteLiteral(" class=\"missing\"");

WriteLiteral(">&ndash;&ndash;</span>\r\n");

                        }

WriteLiteral("                    </td>\r\n                </tr>\r\n\r\n                <tr>\r\n       " +
"             <td>Reference</td>\r\n                    <td>\r\n");

                        
                         if (!string.IsNullOrWhiteSpace(Model.Data.EmployerRef))
                        {

WriteLiteral("                            <span>");

                             Write(Model.Data.EmployerRef);

WriteLiteral("</span>\r\n");

                        }
                        else
                        {

WriteLiteral("                            <span");

WriteLiteral(" class=\"missing\"");

WriteLiteral(">&ndash;&ndash;</span>\r\n");

                        }

WriteLiteral("                    </td>\r\n                </tr>\r\n\r\n            </tbody>\r\n       " +
" </table>\r\n\r\n    </div>\r\n</div>\r\n\r\n\r\n\r\n<a");

WriteAttribute("href", Tuple.Create(" href=\"", 4511), Tuple.Create("\"", 4579)
, Tuple.Create(Tuple.Create("", 4518), Tuple.Create<System.Object, System.Int32>(Url.Action("Details", new { Model.Data.HashedCommitmentId })
, 4518), false)
);

WriteLiteral(" aria-label=\"Back\"");

WriteLiteral(">Return to cohort view</a>\r\n\r\n\r\n");

DefineSection("breadcrumb", () => {

WriteLiteral("\r\n    <div");

WriteLiteral(" class=\"breadcrumbs\"");

WriteLiteral(">\r\n        <a");

WriteAttribute("href", Tuple.Create(" href=\"", 4694), Tuple.Create("\"", 4762)
, Tuple.Create(Tuple.Create("", 4701), Tuple.Create<System.Object, System.Int32>(Url.Action("Details", new { Model.Data.HashedCommitmentId })
, 4701), false)
);

WriteLiteral(" aria-label=\"Back\"");

WriteLiteral(" class=\"back-link\"");

WriteLiteral(">Back</a>\r\n    </div>\r\n");

});

WriteLiteral("\r\n");

WriteLiteral("\r\n");

DefineSection("gaDataLayer", () => {

WriteLiteral("\r\n    <script>\r\n        sfa.dataLayer.vpv = \'/accounts/apprentices/apprenticeship" +
"s/view/apprentice\';\r\n        sfa.dataLayer.cohortRef = \'");

                              Write(Model.Data.HashedCommitmentId);

WriteLiteral("\';\r\n    </script>\r\n");

});

        }
    }
}
#pragma warning restore 1591