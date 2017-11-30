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
    using SFA.DAS.EmployerCommitments.Web.ViewModels;
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/Shared/EditApprenticeship.cshtml")]
    public partial class EditApprenticeship : System.Web.Mvc.WebViewPage<ExtendedApprenticeshipViewModel>
    {
        public EditApprenticeship()
        {
        }
        public override void Execute()
        {
WriteLiteral("\r\n<div");

WriteAttribute("class", Tuple.Create(" class=\"", 99), Tuple.Create("\"", 194)
, Tuple.Create(Tuple.Create("", 107), Tuple.Create("form-group", 107), true)
, Tuple.Create(Tuple.Create(" ", 117), Tuple.Create<System.Object, System.Int32>(!string.IsNullOrEmpty(Model.Apprenticeship.FirstNameError) ? "error" : ""
, 118), false)
);

WriteLiteral(">\r\n");

WriteLiteral("    ");

Write(Html.Label("FirstName", "First name", new { @class = "form-label form-label-bold" }));

WriteLiteral("\r\n");

    
     if (!string.IsNullOrEmpty(Model.Apprenticeship.FirstNameError))
    {

WriteLiteral("        <span");

WriteLiteral(" class=\"error-message\"");

WriteLiteral(" id=\"error-message-FirstName\"");

WriteLiteral(">");

                                                            Write(Model.Apprenticeship.FirstNameError);

WriteLiteral("</span>\r\n");

    }

WriteLiteral("    ");

Write(Html.TextBox("FirstName", Model.Apprenticeship.FirstName, new { @class = "form-control form-control-3-4" }));

WriteLiteral("\r\n</div>\r\n\r\n<div");

WriteAttribute("class", Tuple.Create(" class=\"", 611), Tuple.Create("\"", 722)
, Tuple.Create(Tuple.Create("", 619), Tuple.Create("form-error-group", 619), true)
, Tuple.Create(Tuple.Create(" ", 635), Tuple.Create("form-group", 636), true)
, Tuple.Create(Tuple.Create(" ", 646), Tuple.Create<System.Object, System.Int32>(!string.IsNullOrEmpty(Model.Apprenticeship.LastNameError) ? "error" : ""
, 647), false)
);

WriteLiteral(">\r\n");

WriteLiteral("    ");

Write(Html.Label("LastName", "Last name", new { @class = "form-label form-label-bold" }));

WriteLiteral("\r\n");

    
     if (!string.IsNullOrEmpty(Model.Apprenticeship.LastNameError))
    {

WriteLiteral("        <span");

WriteLiteral(" class=\"error-message\"");

WriteLiteral(" id=\"error-message-LastName\"");

WriteLiteral(">");

                                                           Write(Model.Apprenticeship.LastNameError);

WriteLiteral("</span>\r\n");

    }

WriteLiteral("    ");

Write(Html.TextBox("LastName", Model.Apprenticeship.LastName, new { @class = "form-control form-control-3-4" }));

WriteLiteral("\r\n\r\n</div>\r\n\r\n<div");

WriteAttribute("class", Tuple.Create(" class=\"", 1134), Tuple.Create("\"", 1248)
, Tuple.Create(Tuple.Create("", 1142), Tuple.Create("form-error-group", 1142), true)
, Tuple.Create(Tuple.Create(" ", 1158), Tuple.Create("form-group", 1159), true)
, Tuple.Create(Tuple.Create(" ", 1169), Tuple.Create<System.Object, System.Int32>(!string.IsNullOrEmpty(Model.Apprenticeship.DateOfBirthError) ? "error" : ""
, 1170), false)
);

WriteLiteral(">\r\n    <hr />\r\n    <span");

WriteLiteral(" class=\"form-label-bold\"");

WriteLiteral(">Date of birth</span>\r\n    <span");

WriteLiteral(" class=\"form-hint\"");

WriteLiteral(">For example, 08 12 2001</span>\r\n\r\n    <div");

WriteLiteral(" id=\"DateOfBirth\"");

WriteLiteral(" class=\"form-date\"");

WriteLiteral(">\r\n\r\n");

        
         if (!string.IsNullOrEmpty(Model.Apprenticeship.DateOfBirthError))
        {

WriteLiteral("            <span");

WriteLiteral(" class=\"error-message\"");

WriteLiteral(" id=\"error-message-DateOfBirth\"");

WriteLiteral(">");

                                                                  Write(Model.Apprenticeship.DateOfBirthError);

WriteLiteral("</span>\r\n");

        }

WriteLiteral("\r\n        <div");

WriteLiteral(" class=\"form-group form-group-day\"");

WriteLiteral(">\r\n            <label");

WriteLiteral(" for=\"DateOfBirth.Day\"");

WriteLiteral(">\r\n                Day\r\n            </label>\r\n\r\n");

WriteLiteral("            ");

       Write(Html.TextBox("DateOfBirth.Day", Model.Apprenticeship.DateOfBirth.Day, new { @class = "form-control length-limit", type = "number", maxlength = "2", min = "1", max = "31", aria_labelledby = "DateOfBirth.Day" }));

WriteLiteral("\r\n\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"form-group form-group-month\"");

WriteLiteral(">\r\n            <label");

WriteLiteral(" for=\"DateOfBirth.Month\"");

WriteLiteral(">\r\n                Month\r\n            </label>\r\n\r\n");

WriteLiteral("            ");

       Write(Html.TextBox("DateOfBirth.Month", Model.Apprenticeship.DateOfBirth.Month, new { @class = "form-control length-limit", type = "number", maxlength = "2", min = "1", max = "12", aria_labelledby = "DateOfBirth.Month" }));

WriteLiteral("\r\n\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"form-group form-group-year\"");

WriteLiteral(">\r\n            <label");

WriteLiteral(" for=\"DateOfBirth.Year\"");

WriteLiteral(">\r\n                Year\r\n            </label>\r\n\r\n");

WriteLiteral("            ");

       Write(Html.TextBox("DateOfBirth.Year", Model.Apprenticeship.DateOfBirth.Year, new { @class = "form-control length-limit", type = "number", maxlength = "4", min = "1900", max = "9999", aria_labelledby = "DateOfBirth.Year" }));

WriteLiteral("\r\n        </div>\r\n    </div>\r\n</div>\r\n\r\n\r\n");

 if (!string.IsNullOrEmpty(Model.Apprenticeship?.ULN))
{

WriteLiteral("    <div");

WriteLiteral(" class=\"form-error-group form-group\"");

WriteLiteral(">\r\n        <hr/>\r\n        <p");

WriteLiteral(" class=\"form-label form-label-bold\"");

WriteLiteral(">Unique learner number</p>\r\n        <p");

WriteLiteral(" id=\"uln\"");

WriteLiteral(">");

               Write(Model.Apprenticeship.ULN);

WriteLiteral("</p>\r\n        ");

WriteLiteral("\r\n    </div>\r\n");

}
else
{

WriteLiteral("    <div");

WriteLiteral(" class=\"form-error-group form-group normal-form-label\"");

WriteLiteral(">\r\n        <hr />\r\n        <p");

WriteLiteral(" class=\"form-label form-label-bold\"");

WriteLiteral(">Unique learner number</p>\r\n        <p");

WriteLiteral(" class=\"grey-text\"");

WriteLiteral(">This will be added by your training provider.</p>\r\n    </div>\r\n");

}

WriteLiteral("\r\n<div");

WriteAttribute("class", Tuple.Create(" class=\"", 3586), Tuple.Create("\"", 3684)
, Tuple.Create(Tuple.Create("", 3594), Tuple.Create("form-group", 3594), true)
, Tuple.Create(Tuple.Create(" ", 3604), Tuple.Create<System.Object, System.Int32>(!string.IsNullOrEmpty(Model.Apprenticeship.TrainingCodeError) ? "error" : ""
, 3605), false)
);

WriteLiteral(">\r\n    <hr />\r\n    <label");

WriteLiteral(" class=\"form-label-bold\"");

WriteLiteral(" for=\"TrainingCode\"");

WriteLiteral(">Apprenticeship training course</label>\r\n    <span");

WriteLiteral(" class=\"form-hint\"");

WriteLiteral(">Start typing in the name of the course or choose an option from the list</span>\r" +
"\n");

    
     if (!string.IsNullOrEmpty(Model.Apprenticeship.TrainingCodeError))
    {

WriteLiteral("        <span");

WriteLiteral(" class=\"error-message\"");

WriteLiteral(" id=\"error-message-TrainingCode\"");

WriteLiteral(">Choose a training course for this apprentice</span>\r\n");

    }

WriteLiteral("    <select");

WriteLiteral(" name=\"TrainingCode\"");

WriteLiteral(" id=\"TrainingCode\"");

WriteLiteral(" class=\"form-control form-control-3-4\"");

WriteLiteral(" aria-label=\"Apprenticeship training course\"");

WriteLiteral(">\r\n        <option");

WriteLiteral(" value=\"\"");

WriteLiteral(">Please select</option>\r\n");

        
         foreach (var apprenticeshipProduct in Model.ApprenticeshipProgrammes)
        {

WriteLiteral("            <option");

WriteAttribute("value", Tuple.Create(" value=\"", 4404), Tuple.Create("\"", 4437)
, Tuple.Create(Tuple.Create("", 4412), Tuple.Create<System.Object, System.Int32>(apprenticeshipProduct.Id
, 4412), false)
);

WriteLiteral(" ");

                                                       if (apprenticeshipProduct.Id == Model.Apprenticeship.TrainingCode) { 
                                                                                                                       Write(Html.Raw("selected"));

                                                                                                                                                     }
WriteLiteral(">\r\n");

WriteLiteral("                ");

           Write(apprenticeshipProduct.Title);

WriteLiteral("\r\n            </option>\r\n");

        }

WriteLiteral("    </select>\r\n\r\n</div>\r\n\r\n<div");

WriteAttribute("class", Tuple.Create(" class=\"", 4649), Tuple.Create("\"", 4830)
, Tuple.Create(Tuple.Create("", 4657), Tuple.Create("form-error-group", 4657), true)
, Tuple.Create(Tuple.Create(" ", 4673), Tuple.Create("form-group", 4674), true)
, Tuple.Create(Tuple.Create(" ", 4684), Tuple.Create<System.Object, System.Int32>(!string.IsNullOrEmpty(Model.Apprenticeship.StartDateError) || !string.IsNullOrEmpty(Model.Apprenticeship.StartDateOverlapError) ? "error" : ""
, 4685), false)
);

WriteLiteral(">\r\n    <hr />\r\n\r\n    <span");

WriteLiteral(" class=\"form-label-bold\"");

WriteLiteral(">Planned training start date</span>\r\n    <span");

WriteLiteral(" class=\"form-hint\"");

WriteLiteral(">For example, 09 2017</span>\r\n\r\n    <div");

WriteLiteral(" id=\"StartDate\"");

WriteLiteral(" class=\"form-date\"");

WriteLiteral(">\r\n\r\n");

        
         if (!string.IsNullOrEmpty(Model.Apprenticeship.StartDateError))
        {

WriteLiteral("            <span");

WriteLiteral(" class=\"error-message\"");

WriteLiteral(" id=\"error-message-StartDate\"");

WriteLiteral(">");

                                                                Write(Model.Apprenticeship.StartDateError);

WriteLiteral("</span>\r\n");

        }

WriteLiteral("        ");

         if (!string.IsNullOrEmpty(Model.Apprenticeship.StartDateOverlapError))
        {

WriteLiteral("            <span");

WriteLiteral(" class=\"error-message\"");

WriteLiteral(" id=\"error-message-StartDate\"");

WriteLiteral(">The date overlaps with existing training dates for the same apprentice. Please c" +
"heck the date - contact your training provider for help.</span>\r\n");

        }

WriteLiteral("\r\n        <div");

WriteLiteral(" class=\"form-group form-group-month\"");

WriteLiteral(">\r\n            <label");

WriteLiteral(" for=\"StartDate.Month\"");

WriteLiteral(">\r\n                Month\r\n            </label>\r\n\r\n");

WriteLiteral("            ");

       Write(Html.TextBox("StartDate.Month", Model.Apprenticeship.StartDate.Month, new { @class = "form-control length-limit", type = "number", maxlength = "2", min = "1", max = "12", aria_labelledby = "StartDate.Month" }));

WriteLiteral("\r\n\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"form-group form-group-month\"");

WriteLiteral(">\r\n            <label");

WriteLiteral(" for=\"StartDate.Year\"");

WriteLiteral(">\r\n                Year\r\n            </label>\r\n");

WriteLiteral("            ");

       Write(Html.TextBox("StartDate.Year", Model.Apprenticeship.StartDate.Year, new { @class = "form-control length-limit", type = "number", maxlength = "4", min = "1900", max = "9999", aria_labelledby = "StartDate.Year" }));

WriteLiteral("\r\n        </div>\r\n    </div>\r\n\r\n</div>\r\n\r\n<div");

WriteAttribute("class", Tuple.Create(" class=\"", 6342), Tuple.Create("\"", 6519)
, Tuple.Create(Tuple.Create("", 6350), Tuple.Create("form-error-group", 6350), true)
, Tuple.Create(Tuple.Create(" ", 6366), Tuple.Create("form-group", 6367), true)
, Tuple.Create(Tuple.Create(" ", 6377), Tuple.Create<System.Object, System.Int32>(!string.IsNullOrEmpty(Model.Apprenticeship.EndDateError) || !string.IsNullOrEmpty(Model.Apprenticeship.EndDateOverlapError) ? "error" : ""
, 6378), false)
);

WriteLiteral(">\r\n\r\n    <span");

WriteLiteral(" class=\"form-label-bold\"");

WriteLiteral(">Planned training finish date</span>\r\n    <span");

WriteLiteral(" class=\"form-hint\"");

WriteLiteral(">For example, 02 2019</span>\r\n\r\n    <div");

WriteLiteral(" id=\"EndDate\"");

WriteLiteral(" class=\"form-date\"");

WriteLiteral(">\r\n");

        
         if (!string.IsNullOrEmpty(Model.Apprenticeship.EndDateError))
        {

WriteLiteral("            <span");

WriteLiteral(" class=\"error-message\"");

WriteLiteral(" id=\"error-message-EndDate\"");

WriteLiteral(">");

                                                              Write(Model.Apprenticeship.EndDateError);

WriteLiteral("</span>\r\n");

        }

WriteLiteral("        ");

         if (!string.IsNullOrEmpty(Model.Apprenticeship.EndDateOverlapError))
        {

WriteLiteral("            <span");

WriteLiteral(" class=\"error-message\"");

WriteLiteral(" id=\"error-message-EndDate\"");

WriteLiteral(">The date overlaps with existing training dates for the same apprentice. Please c" +
"heck the date - contact your training provider for help.</span>\r\n");

        }

WriteLiteral("\r\n        <div");

WriteLiteral(" class=\"form-group form-group-month\"");

WriteLiteral(">\r\n            <label");

WriteLiteral(" for=\"EndDate.Month\"");

WriteLiteral(">\r\n                Month\r\n            </label>\r\n\r\n");

WriteLiteral("            ");

       Write(Html.TextBox("EndDate.Month", Model.Apprenticeship.EndDate.Month, new { @class = "form-control length-limit", type = "number", maxlength = "2", min = "1", max = "12", aria_labelledby = "EndDate.Month" }));

WriteLiteral("\r\n\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"form-group form-group-month\"");

WriteLiteral(">\r\n            <label");

WriteLiteral(" for=\"EndDate.Year\"");

WriteLiteral(">\r\n                Year\r\n            </label>\r\n");

WriteLiteral("            ");

       Write(Html.TextBox("EndDate.Year", Model.Apprenticeship.EndDate.Year, new { @class = "form-control length-limit", type = "number", maxlength = "4", min = "1900", max = "9999", aria_labelledby = "EndDate.Year" }));

WriteLiteral("\r\n        </div>\r\n    </div>\r\n\r\n\r\n</div>\r\n\r\n<div");

WriteAttribute("class", Tuple.Create(" class=\"", 7992), Tuple.Create("\"", 8099)
, Tuple.Create(Tuple.Create("", 8000), Tuple.Create("form-error-group", 8000), true)
, Tuple.Create(Tuple.Create(" ", 8016), Tuple.Create("form-group", 8017), true)
, Tuple.Create(Tuple.Create(" ", 8027), Tuple.Create<System.Object, System.Int32>(!string.IsNullOrEmpty(Model.Apprenticeship.CostError) ? "error" : ""
, 8028), false)
);

WriteLiteral(">\r\n\r\n    <hr />\r\n    <label");

WriteLiteral(" for=\"Cost\"");

WriteLiteral(">\r\n        <span");

WriteLiteral(" class=\"form-label-bold\"");

WriteLiteral(">Total agreed apprenticeship price (excluding VAT)</span>\r\n        <span");

WriteLiteral(" class=\"form-hint\"");

WriteLiteral(">Enter the price, including any end-point assessment costs, in whole pounds.</spa" +
"n>\r\n        <span");

WriteLiteral(" class=\"form-hint\"");

WriteLiteral(">For example, for £1,500 enter 1500</span>\r\n");

        
         if (!string.IsNullOrEmpty(Model.Apprenticeship.CostError))
        {

WriteLiteral("            <span");

WriteLiteral(" class=\"error-message\"");

WriteLiteral(" id=\"error-message-Cost\"");

WriteLiteral(">");

                                                           Write(Model.Apprenticeship.CostError);

WriteLiteral("</span>\r\n");

        }

WriteLiteral("\r\n    </label>\r\n\r\n    <span");

WriteLiteral(" class=\"heading-small\"");

WriteLiteral(">£ </span>");

                                    Write(Html.TextBox("Cost", Model.Apprenticeship.Cost, new { @class = "form-control form-control-3-4", type = "text", aria_labelledby = "Cost", maxlength = "7" }));

WriteLiteral("\r\n\r\n</div>\r\n\r\n<div");

WriteAttribute("class", Tuple.Create(" class=\"", 8856), Tuple.Create("\"", 8966)
, Tuple.Create(Tuple.Create("", 8864), Tuple.Create("form-group", 8864), true)
, Tuple.Create(Tuple.Create(" ", 8874), Tuple.Create("optional-ref", 8875), true)
, Tuple.Create(Tuple.Create(" ", 8887), Tuple.Create<System.Object, System.Int32>(!string.IsNullOrEmpty(Model.Apprenticeship.EmployerRefError) ? "error" : ""
, 8888), false)
);

WriteLiteral(">\r\n    <hr />\r\n");

WriteLiteral("    ");

Write(Html.Label("EmployerRef", "Reference (optional)", new { @class = "form-label-bold" }));

WriteLiteral("\r\n    <span");

WriteLiteral(" class=\"form-hint\"");

WriteLiteral(">Add a reference, such as employee number or location - this won\'t be seen by the" +
" training provider</span>\r\n");

    
     if (!string.IsNullOrEmpty(Model.Apprenticeship.EmployerRefError))
    {

WriteLiteral("        <span");

WriteLiteral(" class=\"error-message\"");

WriteLiteral(" id=\"error-message-EmployerRef\"");

WriteLiteral(">");

                                                              Write(Model.Apprenticeship.EmployerRefError);

WriteLiteral("</span>\r\n");

    }

WriteLiteral("    ");

Write(Html.TextBox("EmployerRef", Model.Apprenticeship.EmployerRef, new { @class = "form-control form-control-3-4" }));

WriteLiteral("\r\n    <p");

WriteLiteral(" id=\"charCount-noJS\"");

WriteLiteral(">Enter up to a maximum of 20 characters</p>\r\n    <p");

WriteLiteral(" id=\"charCount\"");

WriteLiteral(" style=\"display:none;\"");

WriteLiteral("><span");

WriteLiteral(" name=\"countchars\"");

WriteLiteral(" id=\"countchars\"");

WriteLiteral("></span> characters remaining</p>\r\n</div>\r\n\r\n");

        }
    }
}
#pragma warning restore 1591
