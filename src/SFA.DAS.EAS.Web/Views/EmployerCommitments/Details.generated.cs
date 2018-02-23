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
    using SFA.DAS.EmployerCommitments.Web.ViewModels;
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/EmployerCommitments/Details.cshtml")]
    public partial class Details : System.Web.Mvc.WebViewPage<OrchestratorResponse<CommitmentDetailsViewModel>>
    {
 

    public string GetApprenticesCountText(IList<ApprenticeshipListItemViewModel> apprenticeships)
    {
        return apprenticeships.Count == 1 ? "1 Apprentice" : $"{apprenticeships.Count} Apprentices";
    }

    readonly Func<string, int, string> _addS = (word, count) => count == 1 ? word : $"{word}s";

    public string PluraliseApprentice(int count)
    {
        return count == 1 ? "Apprentice" : "Apprentices";
    }

    public string PluraliseIncompleteRecords(int count)
    {
        return count == 1 ? "Incomplete record" : "Incomplete records";
    }

    private string GetClickableClass(int requestsCount)
    {
        return requestsCount > 0 ? "clickable" : "";
    }

    public string FormatCost(decimal? cost)
    {
        return !cost.HasValue ? string.Empty : $"£{cost.Value:n0}";
    }

#line default
#line hidden
public System.Web.WebPages.HelperResult GetValueOrDefault(string property)
{
#line default
#line hidden
return new System.Web.WebPages.HelperResult(__razor_helper_writer => {
 
if (!string.IsNullOrEmpty(property))
{

WriteLiteralTo(__razor_helper_writer, "        <span>");

WriteTo(__razor_helper_writer, property);

WriteLiteralTo(__razor_helper_writer, " &nbsp;</span>\r\n");

}
else
{

WriteLiteralTo(__razor_helper_writer, "        <span");

WriteLiteralTo(__razor_helper_writer, " class=\"missing\"");

WriteLiteralTo(__razor_helper_writer, ">");

WriteTo(__razor_helper_writer, Html.Raw("&ndash;"));

WriteLiteralTo(__razor_helper_writer, "</span>\r\n");

}

});

#line default
#line hidden
}
#line default
#line hidden

        public Details()
        {
        }
        public override void Execute()
        {
WriteLiteral("\r\n");

  
    ViewBag.Title = "Review your cohort";
    ViewBag.Section = "apprentices";
    ViewBag.PageID = "apprentice-details";

WriteLiteral("\r\n\r\n");

WriteLiteral("<div");

WriteLiteral(" id=\"cohort-details\"");

WriteLiteral(">\r\n    <div");

WriteLiteral(" class=\"grid-row\"");

WriteLiteral(">\r\n        <div");

WriteLiteral(" class=\"column-full\"");

WriteLiteral(">\r\n\r\n");

            
             if (Model.Data.Errors.Any())
            {

WriteLiteral("                <div");

WriteLiteral(" class=\"validation-summary-errors error-summary\"");

WriteLiteral(">\r\n                    <h1");

WriteLiteral(" id=\"error-summary\"");

WriteLiteral(" class=\"heading-medium error-summary-heading\"");

WriteLiteral(">\r\n                        There are errors on this page that need your attention" +
"\r\n                    </h1>\r\n                    <ul");

WriteLiteral(" class=\"error-summary-list\"");

WriteLiteral(">\r\n");

                        
                         foreach (var errorMsg in Model.Data.Errors)
                        {

WriteLiteral("                            <li>\r\n                                <a");

WriteAttribute("href", Tuple.Create(" href=\"", 973), Tuple.Create("\"", 1008)
, Tuple.Create(Tuple.Create("", 980), Tuple.Create("#error-message-", 980), true)
, Tuple.Create(Tuple.Create("", 995), Tuple.Create<System.Object, System.Int32>(errorMsg.Key
, 995), false)
);

WriteLiteral(" data-focuses=\"error-message-");

                                                                                              Write(errorMsg.Key);

WriteLiteral("\"");

WriteLiteral(">\r\n");

WriteLiteral("                                    ");

                               Write(errorMsg.Value);

WriteLiteral("\r\n                                </a>\r\n                            </li>\r\n");

                        }

WriteLiteral("                    </ul>\r\n                </div>\r\n");

            }
            else if (Model.Data.Warnings.Any())
            {

WriteLiteral("                <div");

WriteLiteral(" class=\"validation-summary-max-funding error-summary\"");

WriteLiteral(">\r\n                    <h1");

WriteLiteral(" id=\"warning-summary\"");

WriteLiteral(" class=\"heading-medium warning-summary-heading\"");

WriteLiteral(">\r\n                        Warnings for your attention\r\n                    </h1>" +
"\r\n                    <ul");

WriteLiteral(" class=\"max-funding-summary-list\"");

WriteLiteral(">\r\n");

                        
                         foreach (var warning in Model.Data.Warnings)
                        {

WriteLiteral("                            <li>\r\n                                <a");

WriteAttribute("href", Tuple.Create(" href=\"", 1813), Tuple.Create("\"", 1851)
, Tuple.Create(Tuple.Create("", 1820), Tuple.Create("#max-funding-group-", 1820), true)
, Tuple.Create(Tuple.Create("", 1839), Tuple.Create<System.Object, System.Int32>(warning.Key
, 1839), false)
);

WriteLiteral(" data-focuses=\"max-funding-group-");

                                                                                                     Write(warning.Key);

WriteLiteral("\"");

WriteLiteral(">\r\n");

WriteLiteral("                                    ");

                               Write(warning.Value);

WriteLiteral("\r\n                                </a>\r\n                            </li>\r\n");

                        }

WriteLiteral("                    </ul>\r\n                </div>\r\n");

            }

WriteLiteral("\r\n\r\n");

            
             if (Model.Data.IsReadOnly)
            {

WriteLiteral("                <h1");

WriteLiteral(" class=\"heading-xlarge\"");

WriteLiteral(">View your cohort</h1>\r\n");

            }
            else
            {

WriteLiteral("                <h1");

WriteLiteral(" class=\"heading-xlarge\"");

WriteLiteral(">Review your cohort</h1>\r\n");

            }

WriteLiteral("\r\n\r\n        </div>\r\n    </div>\r\n\r\n    <div");

WriteLiteral(" class=\"grid-row\"");

WriteLiteral(" id=\"review-cohorts\"");

WriteLiteral(">\r\n        <div");

WriteLiteral(" class=\"column-one-third all-apps\"");

WriteLiteral(">\r\n            <div>\r\n                <h2");

WriteLiteral(" class=\"bold-xlarge\"");

WriteLiteral(">");

                                   Write(Model.Data.Apprenticeships.Count);

WriteLiteral("</h2>\r\n                <p");

WriteLiteral(" class=\"heading-small\"");

WriteLiteral(">");

                                    Write(PluraliseApprentice(Model.Data.Apprenticeships.Count));

WriteLiteral("</p>\r\n            </div>\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"column-one-third incomplete-apps\"");

WriteLiteral(">\r\n            <div>\r\n                <h2");

WriteLiteral(" class=\"bold-xlarge\"");

WriteLiteral(">");

                                   Write(Model.Data.Apprenticeships.Count(x => !x.CanBeApproved));

WriteLiteral("</h2>\r\n                <p");

WriteLiteral(" class=\"heading-small\"");

WriteLiteral(">");

                                    Write(PluraliseIncompleteRecords(Model.Data.Apprenticeships.Count(x => !x.CanBeApproved)));

WriteLiteral("</p>\r\n            </div>\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"column-one-third total-cost\"");

WriteLiteral(">\r\n            <div");

WriteLiteral(" class=\"dynamic-cost-display\"");

WriteLiteral(">\r\n");

                
                  
                    var totalCost = Model.Data.Apprenticeships.Sum(x => x.Cost ?? 0).ToString("N0");
                    var totalClass = string.Empty;


                    if (totalCost.Length > 3)
                    {
                        totalClass = "short";
                    }
                    if (totalCost.Length > 6)
                    {
                        totalClass = "long";
                    }
                    if (totalCost.Length > 9)
                    {
                        totalClass = "longer";
                    }

                
WriteLiteral("\r\n                <h2");

WriteAttribute("class", Tuple.Create(" class=\"", 3825), Tuple.Create("\"", 3856)
, Tuple.Create(Tuple.Create("", 3833), Tuple.Create<System.Object, System.Int32>(totalClass
, 3833), false)
, Tuple.Create(Tuple.Create(" ", 3844), Tuple.Create("bold-xlarge", 3845), true)
);

WriteLiteral(">&pound;");

                                                      Write(totalCost);

WriteLiteral("</h2>\r\n\r\n                <p");

WriteLiteral(" class=\"heading-small\"");

WriteLiteral(">Total cost</p>\r\n            </div>\r\n        </div>\r\n    </div>\r\n\r\n    <div");

WriteLiteral(" class=\"grid-row\"");

WriteLiteral(">\r\n        <div");

WriteLiteral(" class=\"column-one-third employer-details\"");

WriteLiteral(">\r\n            <p><span");

WriteLiteral(" class=\"strong\"");

WriteLiteral(">Training provider:</span> ");

                                                         Write(Model.Data.ProviderName);

WriteLiteral("</p>\r\n            <p><span");

WriteLiteral(" class=\"strong\"");

WriteLiteral(">Status:</span> ");

                                              Write(Model.Data.Status.GetDescription());

WriteLiteral("</p>\r\n        </div>\r\n\r\n\r\n        <div");

WriteLiteral(" class=\"column-two-thirds employer-details\"");

WriteLiteral(">\r\n            <p");

WriteLiteral(" class=\"strong\"");

WriteLiteral(">Message:</p>\r\n            <p>");

           Write(string.IsNullOrWhiteSpace(Model.Data.LatestMessage) ? "No message added" : Model.Data.LatestMessage);

WriteLiteral("</p>\r\n        </div>\r\n    </div>\r\n\r\n    <div");

WriteLiteral(" class=\"grid-row\"");

WriteLiteral(">\r\n        <div");

WriteLiteral(" class=\"column-full\"");

WriteLiteral(">\r\n");

            
              
                var finishEditingText = Model.Data.ShowApproveOnlyOption ? "Continue to approval" : "Save and continue";
            
WriteLiteral("\r\n\r\n");

            
             if (!Model.Data.IsReadOnly)
            {


WriteLiteral("                <div");

WriteLiteral(" class=\"grid-row\"");

WriteLiteral(">\r\n                    <div");

WriteLiteral(" class=\"column-full\"");

WriteLiteral(">\r\n                        <div");

WriteLiteral(" class=\"emptyStateButton\"");

WriteLiteral(">\r\n                            <hr");

WriteLiteral(" class=\"hr-top\"");

WriteLiteral(">\r\n                            <a");

WriteLiteral(" class=\"button finishEditingBtn\"");

WriteAttribute("href", Tuple.Create(" href=\"", 5066), Tuple.Create("\"", 5103)
, Tuple.Create(Tuple.Create("", 5073), Tuple.Create<System.Object, System.Int32>(Url.Action("FinishedEditing")
, 5073), false)
);

WriteAttribute("aria-label", Tuple.Create(" aria-label=\"", 5104), Tuple.Create("\"", 5135)
                                 , Tuple.Create(Tuple.Create("", 5117), Tuple.Create<System.Object, System.Int32>(finishEditingText
, 5117), false)
);

WriteLiteral(">");

                                                                                                                                Write(finishEditingText);

WriteLiteral("</a>\r\n                            <a");

WriteAttribute("href", Tuple.Create(" href=\"", 5191), Tuple.Create("\"", 5238)
, Tuple.Create(Tuple.Create("", 5198), Tuple.Create<System.Object, System.Int32>(Url.Action("CreateApprenticeshipEntry")
, 5198), false)
);

WriteLiteral(" class=\"button button-secondary\"");

WriteLiteral(" aria-label=\"Add an apprentice\"");

WriteLiteral(">Add an apprentice</a>\r\n                        </div>\r\n                    </div" +
">\r\n                </div>\r\n");


            }

WriteLiteral("\r\n            <hr");

WriteLiteral(" class=\"hr-bottom\"");

WriteLiteral(">\r\n\r\n");

            
             if (!Model.Data.HasApprenticeships)
            {


WriteLiteral("                <div");

WriteLiteral(" class=\"grid-row\"");

WriteLiteral(" id=\"empty-alert-top\"");

WriteLiteral(">\r\n                    <div");

WriteLiteral(" class=\"column-full\"");

WriteLiteral(">\r\n                        <div");

WriteLiteral(" class=\"panel panel-border-wide alert-default\"");

WriteLiteral(">\r\n\r\n");

                            
                             if (Model.Data.IsReadOnly)
                            {

WriteLiteral("                                <p>Apprentices will appear here when the training" +
" provider adds them to your cohort.</p>\r\n");

                            }
                            else
                            {

WriteLiteral("                                <p>\r\n                                    You have" +
"n’t added any apprentices yet - <a");

WriteAttribute("href", Tuple.Create(" href=\"", 6142), Tuple.Create("\"", 6189)
     , Tuple.Create(Tuple.Create("", 6149), Tuple.Create<System.Object, System.Int32>(Url.Action("CreateApprenticeshipEntry")
, 6149), false)
);

WriteLiteral("> add an apprentice </a>\r\n                                </p>\r\n");

                            }

WriteLiteral("\r\n                        </div>\r\n                    </div>\r\n                </d" +
"iv>\r\n");

            }
            else
            {


WriteLiteral("                <div");

WriteLiteral(" class=\"grid-row\"");

WriteLiteral(">\r\n                    <div");

WriteLiteral(" class=\"column-full\"");

WriteLiteral(">\r\n\r\n");

                        
                         foreach (var group in Model.Data.ApprenticeshipGroups)
                        {
                            var groupTitle = string.Format($"{@group.Apprenticeships.Count} x {group.GroupName}");


WriteLiteral("                            <div");

WriteLiteral(" class=\"group-header\"");

WriteLiteral(">\r\n\r\n                                <p");

WriteLiteral(" class=\"heading-medium\"");

WriteLiteral(">");

                                                     Write(groupTitle);

WriteLiteral("</p>\r\n");

                                
                                 if (group.TrainingProgramme != null)
                                {

WriteLiteral("                                    <p>Training code: ");

                                                 Write(group.TrainingProgramme.Id);

WriteLiteral("</p>\r\n");

                                }

WriteLiteral("\r\n                            </div>\r\n");


                            if(group.OverlapErrorCount > 0)
                            {

WriteLiteral("                                <div");

WriteLiteral(" class=\"overlap-notification\"");

WriteAttribute("id", Tuple.Create(" id=\"", 7294), Tuple.Create("\"", 7327)
, Tuple.Create(Tuple.Create("", 7299), Tuple.Create("error-message-", 7299), true)
    , Tuple.Create(Tuple.Create("", 7313), Tuple.Create<System.Object, System.Int32>(group.GroupId
, 7313), false)
);

WriteLiteral(">\r\n                                    <p");

WriteLiteral(" class=\"heading-small\"");

WriteLiteral(">\r\n");

WriteLiteral("                                        ");

                                   Write(_addS("Apprenticeship", group.OverlapErrorCount));

WriteLiteral(@" can't have overlapping training dates
                                    </p>
                                    <p>
                                        Please update training dates to ensure they do not overlap.
                                    </p>
                                </div>
");

                               
                            }
                            else if (group.ApprenticeshipsOverFundingLimit > 0 && !Model.Data.Errors.Any())
                            {

WriteLiteral("                                <div");

WriteLiteral(" class=\"funding-cap-alert\"");

WriteAttribute("id", Tuple.Create(" id=\"", 8055), Tuple.Create("\"", 8092)
, Tuple.Create(Tuple.Create("", 8060), Tuple.Create("max-funding-group-", 8060), true)
     , Tuple.Create(Tuple.Create("", 8078), Tuple.Create<System.Object, System.Int32>(group.GroupId
, 8078), false)
);

WriteLiteral(">\r\n                                    <p");

WriteLiteral(" class=\"heading-small\"");

WriteLiteral(">\r\n");

WriteLiteral("                                        ");

                                    Write($"{group.ApprenticeshipsOverFundingLimit}  {_addS("apprenticeship", group.ApprenticeshipsOverFundingLimit).ToLower()} above funding band maximum");

WriteLiteral("\r\n                                    </p>\r\n                                    <" +
"p>\r\n                                        The costs are above the &pound;");

                                                                  Write(group.TrainingProgramme.MaxFunding.ToString("N0"));

WriteLiteral(" <a");

WriteLiteral(" target=\"_blank\"");

WriteLiteral(" href=\"https://www.gov.uk/government/publications/apprenticeship-funding-and-perf" +
"ormance-management-rules-2017-to-2018\"");

WriteLiteral(@">maximum value of the funding band</a> for this apprenticeship. You'll need to pay the difference directly to the training provider - this can't be funded from your account.
                                    </p>

                                </div>
");


                            }


WriteLiteral("                            <table");

WriteLiteral(" class=\"tableResponsive viewCommitment\"");

WriteLiteral(">\r\n                                <thead>\r\n                                    <" +
"tr>\r\n                                        <th");

WriteLiteral(" scope=\"col\"");

WriteLiteral(">Name</th>\r\n                                        <th");

WriteLiteral(" scope=\"col\"");

WriteLiteral(">Unique learner number</th>\r\n                                        <th");

WriteLiteral(" scope=\"col\"");

WriteLiteral(">Date of birth</th>\r\n                                        <th");

WriteLiteral(" scope=\"col\"");

WriteLiteral(">Training dates</th>\r\n                                        <th");

WriteLiteral(" scope=\"col\"");

WriteLiteral(" colspan=\"2\"");

WriteLiteral(">Cost</th>\r\n                                    </tr>\r\n                          " +
"      </thead>\r\n                                <tbody>\r\n");

                                    
                                     foreach (var apprenticeship in group.Apprenticeships.OrderBy(a => a.CanBeApproved))
                                    {

WriteLiteral("                                        <tr>\r\n                                   " +
"         <td>\r\n");

WriteLiteral("                                                ");

                                           Write(GetValueOrDefault(apprenticeship.ApprenticeName));

WriteLiteral("\r\n                                            </td>\r\n                            " +
"                <td");

WriteLiteral(" id=\"apprenticeUln\"");

WriteLiteral(">\r\n");

                                                
                                                 if (!string.IsNullOrEmpty(apprenticeship.ApprenticeUln))
                                                {

WriteLiteral("                                                    <span>");

                                                     Write(apprenticeship.ApprenticeUln);

WriteLiteral("</span>\r\n");

                                                }
                                                else
                                                {

WriteLiteral("                                                    <span");

WriteLiteral(" class=\"missing\"");

WriteLiteral(">&ndash;</span>\r\n");

                                                }

WriteLiteral("                                                \r\n                               " +
"             </td>\r\n                                            <td>\r\n");

                                                
                                                 if (apprenticeship.ApprenticeDateOfBirth.HasValue)
                                                {

WriteLiteral("                                                    <span>\r\n");

WriteLiteral("                                                        ");

                                                   Write(apprenticeship.ApprenticeDateOfBirth.Value.ToGdsFormat());

WriteLiteral("\r\n                                                    </span>\r\n");

                                                }
                                                else
                                                {

WriteLiteral("                                                    <span");

WriteLiteral(" class=\"missing\"");

WriteLiteral(">&ndash;</span>\r\n");

                                                }

WriteLiteral("                                            </td>\r\n");

                                            
                                             if (apprenticeship.StartDate != null && apprenticeship.EndDate != null)
                                            {
                                                if (apprenticeship.OverlappingApprenticeships.Any())
                                                {

WriteLiteral("                                                    <td");

WriteLiteral(" class=\"overlap-alert\"");

WriteLiteral(">\r\n                                                        <a");

WriteAttribute("href", Tuple.Create(" href=\"", 12023), Tuple.Create("\"", 12059)
, Tuple.Create(Tuple.Create("", 12030), Tuple.Create("#error-message-", 12030), true)
, Tuple.Create(Tuple.Create("", 12045), Tuple.Create<System.Object, System.Int32>(group.GroupId
, 12045), false)
);

WriteLiteral("\r\n                                                           aria-label=\"The uniq" +
"ue learner number already exists for these training dates\"");

WriteAttribute("aria-describedby", Tuple.Create("\r\n                                                           aria-describedby=\"", 12199), Tuple.Create("\"", 12310)
, Tuple.Create(Tuple.Create("", 12278), Tuple.Create("max-funding-group-", 12278), true)
              , Tuple.Create(Tuple.Create("", 12296), Tuple.Create<System.Object, System.Int32>(group.GroupId
, 12296), false)
);

WriteLiteral("\r\n                                                           title=\"The unique le" +
"arner number already exists for these training dates\"");

WriteLiteral(">\r\n");

WriteLiteral("                                                            ");

                                                       Write(apprenticeship.StartDate.Value.ToGdsFormatWithoutDay());

WriteLiteral(" to ");

                                                                                                                  Write(apprenticeship.EndDate.Value.ToGdsFormatWithoutDay());

WriteLiteral(" &nbsp;\r\n                                                        </a>\r\n          " +
"                                          </td>\r\n");

                                                }
                                                else
                                                {

WriteLiteral("                                                    <td>\r\n");

WriteLiteral("                                                        ");

                                                   Write(apprenticeship.StartDate.Value.ToGdsFormatWithoutDay());

WriteLiteral(" to ");

                                                                                                              Write(apprenticeship.EndDate.Value.ToGdsFormatWithoutDay());

WriteLiteral("\r\n                                                    </td>\r\n");

                                                }
                                            }
                                            else
                                            {

WriteLiteral("                                                <td>\r\n                           " +
"                         <span");

WriteLiteral(" class=\"missing\"");

WriteLiteral(">&ndash;</span>\r\n                                                </td>\r\n");

                                            }

WriteLiteral("\r\n");

                                          
                                           if (group.TrainingProgramme != null && apprenticeship.Cost.HasValue && apprenticeship.Cost > group.TrainingProgramme.MaxFunding && !Model.Data.Errors.Any())
                                          {

WriteLiteral("                                                <td");

WriteLiteral(" class=\"funding-cap-alert-td\"");

WriteLiteral(">\r\n                                                    <a");

WriteAttribute("href", Tuple.Create(" href=\"", 14019), Tuple.Create("\"", 14059)
, Tuple.Create(Tuple.Create("", 14026), Tuple.Create("#max-funding-group-", 14026), true)
, Tuple.Create(Tuple.Create("", 14045), Tuple.Create<System.Object, System.Int32>(group.GroupId
, 14045), false)
);

WriteLiteral(" aria-label=\"Cost is above the maximum funding band\"");

WriteAttribute("aria-describedby", Tuple.Create(" aria-describedby=\"", 14112), Tuple.Create("\"", 14163)
, Tuple.Create(Tuple.Create("", 14131), Tuple.Create("max-funding-group-", 14131), true)
                                                                                                       , Tuple.Create(Tuple.Create("", 14149), Tuple.Create<System.Object, System.Int32>(group.GroupId
, 14149), false)
);

WriteLiteral(" title=\"Cost is above the maximum funding band\"");

WriteLiteral("> ");

                                                                                                                                                                                                                                                   Write(GetValueOrDefault(FormatCost(apprenticeship.Cost)));

WriteLiteral("</a>\r\n                                                </td>\r\n");

                                            }
                                            else
                                            {

WriteLiteral("                                                <td>\r\n");

WriteLiteral("                                                    ");

                                               Write(GetValueOrDefault(FormatCost(apprenticeship.Cost)));

WriteLiteral("\r\n                                                </td>\r\n");

                                            }

WriteLiteral("\r\n                                            <td>\r\n");

                                                
                                                 if (!Model.Data.IsReadOnly)
                                                {

WriteLiteral("                                                    <a");

WriteAttribute("href", Tuple.Create(" href=\"", 14965), Tuple.Create("\"", 15075)
, Tuple.Create(Tuple.Create("", 14972), Tuple.Create<System.Object, System.Int32>(Url.Action("EditApprenticeship", new {hashedApprenticeshipId = apprenticeship.HashedApprenticeshipId})
, 14972), false)
);

WriteAttribute("aria-label", Tuple.Create(" aria-label=\"", 15076), Tuple.Create("\"", 15124)
, Tuple.Create(Tuple.Create("", 15089), Tuple.Create("Edit", 15089), true)
                                                                                                     , Tuple.Create(Tuple.Create(" ", 15093), Tuple.Create<System.Object, System.Int32>(apprenticeship.ApprenticeName
, 15094), false)
);

WriteLiteral(">Edit</a>\r\n");

                                                }
                                                else
                                                {

WriteLiteral("                                                    <a");

WriteAttribute("href", Tuple.Create(" href=\"", 15346), Tuple.Create("\"", 15456)
, Tuple.Create(Tuple.Create("", 15353), Tuple.Create<System.Object, System.Int32>(Url.Action("ViewApprenticeship", new {hashedApprenticeshipId = apprenticeship.HashedApprenticeshipId})
, 15353), false)
);

WriteAttribute("aria-label", Tuple.Create(" aria-label=\"", 15457), Tuple.Create("\"", 15505)
, Tuple.Create(Tuple.Create("", 15470), Tuple.Create("Edit", 15470), true)
                                                                                                     , Tuple.Create(Tuple.Create(" ", 15474), Tuple.Create<System.Object, System.Int32>(apprenticeship.ApprenticeName
, 15475), false)
);

WriteLiteral(">View</a>\r\n");

                                                }

WriteLiteral("                                            </td>\r\n                              " +
"          </tr>\r\n");

                                    }

WriteLiteral("                                </tbody>\r\n                            </table>\r\n");


                        }

WriteLiteral("\r\n                    </div>\r\n                </div>\r\n");


            }

WriteLiteral("\r\n        </div>\r\n    </div>\r\n</div>\r\n\r\n");

 if (!Model.Data.IsReadOnly)
{

WriteLiteral("    <a");

WriteLiteral(" class=\"button delete-button\"");

WriteAttribute("href", Tuple.Create(" href=\"", 15993), Tuple.Create("\"", 16027)
, Tuple.Create(Tuple.Create("", 16000), Tuple.Create<System.Object, System.Int32>(Url.Action("DeleteCohort")
, 16000), false)
);

WriteLiteral(" aria-label=\"Delete cohort\"");

WriteLiteral(">Delete cohort</a>\r\n");

}

WriteLiteral("\r\n");

WriteLiteral("\r\n");

WriteLiteral("\r\n\r\n");

DefineSection("breadcrumb", () => {

WriteLiteral("\r\n    <div");

WriteLiteral(" class=\"breadcrumbs\"");

WriteLiteral(">\r\n        <a");

WriteAttribute("href", Tuple.Create(" href=\"", 17216), Tuple.Create("\"", 17246)
, Tuple.Create(Tuple.Create("", 17223), Tuple.Create<System.Object, System.Int32>(Model.Data.BackLinkUrl
, 17223), false)
);

WriteLiteral(" aria-label=\"Back\"");

WriteLiteral(" class=\"back-link\"");

WriteLiteral(">Back</a>\r\n    </div>\r\n");

});

WriteLiteral("\r\n\r\n");

DefineSection("gaDataLayer", () => {

WriteLiteral("\r\n    <script>\r\n        sfa.dataLayer.vpv = \'/accounts/apprentices/details/appren" +
"tice-detail\';\r\n        sfa.dataLayer.cohortRef = \'");

                              Write(Model.Data.HashedId);

WriteLiteral("\';\r\n    </script>\r\n");

});

        }
    }
}
#pragma warning restore 1591