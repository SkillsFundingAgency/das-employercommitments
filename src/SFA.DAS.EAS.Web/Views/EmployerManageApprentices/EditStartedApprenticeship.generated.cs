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
    using SFA.DAS.EmployerCommitments.Web.Extensions;
    using SFA.DAS.EmployerCommitments.Web.Validators.Messages;
    using SFA.DAS.EmployerCommitments.Web.ViewModels;
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/EmployerManageApprentices/EditStartedApprenticeship.cshtml")]
    public partial class EditStartedApprenticeship : System.Web.Mvc.WebViewPage<ExtendedApprenticeshipViewModel>
    {
        public EditStartedApprenticeship()
        {
        }
        public override void Execute()
        {
WriteLiteral("\r\n");

WriteLiteral("<div");

WriteLiteral(" id=\"edit-started-apprentice\"");

WriteLiteral(">\r\n    <div");

WriteAttribute("class", Tuple.Create(" class=\"", 250), Tuple.Create("\"", 362)
, Tuple.Create(Tuple.Create("", 258), Tuple.Create("form-error-group", 258), true)
, Tuple.Create(Tuple.Create(" ", 274), Tuple.Create("form-group", 275), true)
, Tuple.Create(Tuple.Create(" ", 285), Tuple.Create<System.Object, System.Int32>(!string.IsNullOrEmpty(Model.Apprenticeship.FirstNameError) ? "error" : ""
, 286), false)
);

WriteLiteral(">\r\n");

WriteLiteral("        ");

   Write(Html.Label("FirstName", "First name", new { @class = "form-label form-label-bold" }));

WriteLiteral("\r\n");

        
         if (!string.IsNullOrEmpty(Model.Apprenticeship.FirstNameError))
        {

WriteLiteral("            <span");

WriteLiteral(" class=\"error-message\"");

WriteLiteral(" id=\"error-message-firstname\"");

WriteLiteral(">");

                                                                Write(Model.Apprenticeship.FirstNameError);

WriteLiteral("</span>\r\n");

        }

WriteLiteral("        ");

   Write(Html.TextBox("FirstName", Model.Apprenticeship.FirstName, new { @class = "form-control form-control-3-4" }));

WriteLiteral("\r\n    </div>\r\n\r\n    <div");

WriteAttribute("class", Tuple.Create(" class=\"", 811), Tuple.Create("\"", 922)
, Tuple.Create(Tuple.Create("", 819), Tuple.Create("form-error-group", 819), true)
, Tuple.Create(Tuple.Create(" ", 835), Tuple.Create("form-group", 836), true)
, Tuple.Create(Tuple.Create(" ", 846), Tuple.Create<System.Object, System.Int32>(!string.IsNullOrEmpty(Model.Apprenticeship.LastNameError) ? "error" : ""
, 847), false)
);

WriteLiteral(">\r\n");

WriteLiteral("        ");

   Write(Html.Label("LastName", "Last name", new { @class = "form-label form-label-bold" }));

WriteLiteral("\r\n");

        
         if (!string.IsNullOrEmpty(Model.Apprenticeship.LastNameError))
        {

WriteLiteral("            <span");

WriteLiteral(" class=\"error-message\"");

WriteLiteral(" id=\"error-message-lastname\"");

WriteLiteral(">");

                                                               Write(Model.Apprenticeship.LastNameError);

WriteLiteral("</span>\r\n");

        }

WriteLiteral("        ");

   Write(Html.TextBox("LastName", Model.Apprenticeship.LastName, new { @class = "form-control form-control-3-4" }));

WriteLiteral("\r\n    </div>\r\n\r\n    <div");

WriteAttribute("class", Tuple.Create(" class=\"", 1364), Tuple.Create("\"", 1478)
, Tuple.Create(Tuple.Create("", 1372), Tuple.Create("form-error-group", 1372), true)
, Tuple.Create(Tuple.Create(" ", 1388), Tuple.Create("form-group", 1389), true)
, Tuple.Create(Tuple.Create(" ", 1399), Tuple.Create<System.Object, System.Int32>(!string.IsNullOrEmpty(Model.Apprenticeship.DateOfBirthError) ? "error" : ""
, 1400), false)
);

WriteLiteral(">\r\n\t<hr />\r\n        <span");

WriteLiteral(" class=\"form-label-bold\"");

WriteLiteral(">Date of birth</span>\r\n        <span");

WriteLiteral(" class=\"form-hint\"");

WriteLiteral(">For example, 08 12 2001</span>\r\n\r\n        <div");

WriteLiteral(" class=\"form-date\"");

WriteLiteral(">\r\n\r\n");

            
             if (!string.IsNullOrEmpty(Model.Apprenticeship.DateOfBirthError))
            {

WriteLiteral("                <span");

WriteLiteral(" class=\"error-message\"");

WriteLiteral(" id=\"error-message-dateofbirth\"");

WriteLiteral(">");

                                                                      Write(Model.Apprenticeship.DateOfBirthError);

WriteLiteral("</span>\r\n");

            }

WriteLiteral("\r\n            <div");

WriteLiteral(" class=\"form-group form-group-day\"");

WriteLiteral(">\r\n                <label");

WriteLiteral(" for=\"DateOfBirth.Day\"");

WriteLiteral(">\r\n                    Day\r\n                </label>\r\n\r\n");

WriteLiteral("                ");

           Write(Html.TextBox("DateOfBirth.Day", Model.Apprenticeship.DateOfBirth.Day, new { @class = "form-control length-limit", type = "number", maxlength = "2", min = "1", max = "31", aria_labelledby = "DateOfBirth.Day" }));

WriteLiteral("\r\n\r\n            </div>\r\n            <div");

WriteLiteral(" class=\"form-group form-group-month\"");

WriteLiteral(">\r\n                <label");

WriteLiteral(" for=\"DateOfBirth.Month\"");

WriteLiteral(">\r\n                    Month\r\n                </label>\r\n\r\n");

WriteLiteral("                ");

           Write(Html.TextBox("DateOfBirth.Month", Model.Apprenticeship.DateOfBirth.Month, new { @class = "form-control length-limit", type = "number", maxlength = "2", min = "1", max = "12", aria_labelledby = "DateOfBirth.Month" }));

WriteLiteral("\r\n\r\n            </div>\r\n            <div");

WriteLiteral(" class=\"form-group form-group-year\"");

WriteLiteral(">\r\n                <label");

WriteLiteral(" for=\"DateOfBirth.Year\"");

WriteLiteral(">\r\n                    Year\r\n                </label>\r\n\r\n");

WriteLiteral("                ");

           Write(Html.TextBox("DateOfBirth.Year", Model.Apprenticeship.DateOfBirth.Year, new { @class = "form-control length-limit", type = "number", maxlength = "4", min = "1900", max = "9999", aria_labelledby = "DateOfBirth.Year" }));

WriteLiteral("\r\n            </div>\r\n        </div>\r\n    </div>\r\n\r\n    <div");

WriteLiteral(" id=\"form-error-group form-group\"");

WriteLiteral(">\r\n        <div");

WriteLiteral(" class=\"form-group\"");

WriteLiteral(">\r\n            <hr />\r\n            <label");

WriteLiteral(" class=\"form-label form-label-bold\"");

WriteLiteral(" for=\"ULN\"");

WriteLiteral(">Unique learner number</label>\r\n            <span");

WriteLiteral(" id=\"uln\"");

WriteLiteral(">");

                      Write(Model.Apprenticeship.ULN);

WriteLiteral("</span>\r\n        </div>\r\n    </div>\r\n\r\n");

    
     if (Model.Apprenticeship.IsLockedForUpdate || Model.Apprenticeship.IsUpdateLockedForStartDateAndCourse)
    {

WriteLiteral("        <div");

WriteLiteral(" class=\"form-group\"");

WriteLiteral(">\r\n            <hr />\r\n            <label");

WriteLiteral(" class=\"form-label-bold\"");

WriteLiteral(" for=\"TrainingCode\"");

WriteLiteral(">Apprenticeship training course</label>\r\n            <span>");

             Write(Model.Apprenticeship.TrainingName);

WriteLiteral("</span>\r\n        </div>\r\n");

        
   Write(Html.Hidden("TrainingCode", Model.Apprenticeship.TrainingCode));

                                                                       
        
   Write(Html.Hidden("TrainingName", Model.Apprenticeship.TrainingName));

                                                                       
    }
    else
    {

WriteLiteral("        <div");

WriteAttribute("class", Tuple.Create(" class=\"", 3962), Tuple.Create("\"", 4060)
, Tuple.Create(Tuple.Create("", 3970), Tuple.Create("form-group", 3970), true)
, Tuple.Create(Tuple.Create(" ", 3980), Tuple.Create<System.Object, System.Int32>(!string.IsNullOrEmpty(Model.Apprenticeship.TrainingCodeError) ? "error" : ""
, 3981), false)
);

WriteLiteral(">\r\n            <hr />\r\n            <label");

WriteLiteral(" class=\"form-label-bold\"");

WriteLiteral(" for=\"TrainingCode\"");

WriteLiteral(">Apprenticeship training course</label>\r\n            <span");

WriteLiteral(" class=\"form-hint\"");

WriteLiteral(">Start typing in the name of the course or choose an option from the list</span>\r" +
"\n");

            
             if (!string.IsNullOrEmpty(Model.Apprenticeship.TrainingCodeError))
            {

WriteLiteral("                <span");

WriteLiteral(" class=\"error-message\"");

WriteLiteral(" id=\"error-message-TrainingCode\"");

WriteLiteral(">Choose a training course for this apprentice</span>\r\n");

            }

WriteLiteral("            <select");

WriteLiteral(" name=\"TrainingCode\"");

WriteLiteral(" id=\"TrainingCode\"");

WriteLiteral(" class=\"form-control form-control-3-4\"");

WriteLiteral(" aria-label=\"Apprenticeship training course\"");

WriteLiteral(">\r\n                <option");

WriteLiteral(" value=\"\"");

WriteLiteral(">Please select</option>\r\n");

                
                 foreach (var apprenticeshipProduct in Model.ApprenticeshipProgrammes)
                {

WriteLiteral("                    <option");

WriteAttribute("value", Tuple.Create(" value=\"", 4876), Tuple.Create("\"", 4909)
, Tuple.Create(Tuple.Create("", 4884), Tuple.Create<System.Object, System.Int32>(apprenticeshipProduct.Id
, 4884), false)
);

WriteLiteral(" ");

                                                               if (apprenticeshipProduct.Id == Model.Apprenticeship.TrainingCode) { 
                                                                                                                               Write(Html.Raw("selected"));

                                                                                                                                                          }
WriteLiteral(">\r\n");

WriteLiteral("                        ");

                   Write(apprenticeshipProduct.Title);

WriteLiteral("\r\n                    </option>\r\n");

                }

WriteLiteral("            </select>\r\n        </div>\r\n");

    }

WriteLiteral("\r\n");

    
     if (Model.Apprenticeship.IsLockedForUpdate || Model.Apprenticeship.IsUpdateLockedForStartDateAndCourse)
    {

WriteLiteral("        <div");

WriteLiteral(" class=\"form-error-group form-group\"");

WriteLiteral(">\r\n            <hr />\r\n            <label");

WriteLiteral(" class=\"form-label-bold\"");

WriteLiteral(">Planned training start date</label>\r\n            <span>");

             Write(Model.Apprenticeship.StartDate.DateTime.Value.ToGdsFormatWithoutDay());

WriteLiteral(" </span>\r\n        </div>\r\n");


        
   Write(Html.Hidden("StartDate.Month", Model.Apprenticeship.StartDate.Month));

                                                                             
        
   Write(Html.Hidden("StartDate.Year", Model.Apprenticeship.StartDate.Year));

                                                                           
    }
    else
    {

WriteLiteral("        <div");

WriteAttribute("class", Tuple.Create(" class=\"", 5735), Tuple.Create("\"", 5847)
, Tuple.Create(Tuple.Create("", 5743), Tuple.Create("form-error-group", 5743), true)
, Tuple.Create(Tuple.Create(" ", 5759), Tuple.Create("form-group", 5760), true)
, Tuple.Create(Tuple.Create(" ", 5770), Tuple.Create<System.Object, System.Int32>(!string.IsNullOrEmpty(Model.Apprenticeship.StartDateError) ? "error" : ""
, 5771), false)
);

WriteLiteral(">\r\n            <hr />\r\n\r\n            <span");

WriteLiteral(" class=\"form-label-bold\"");

WriteLiteral(">Planned training start date</span>\r\n            <span");

WriteLiteral(" class=\"form-hint\"");

WriteLiteral(">For example, 09 2017</span>\r\n\r\n            <div");

WriteLiteral(" id=\"StartDate\"");

WriteLiteral(" class=\"form-date\"");

WriteLiteral(">\r\n\r\n");

                
                 if (!string.IsNullOrEmpty(Model.Apprenticeship.StartDateError))
                {

WriteLiteral("                    <span");

WriteLiteral(" class=\"error-message\"");

WriteLiteral(" id=\"error-message-StartDate\"");

WriteLiteral(">");

                                                                        Write(ValidationMessage.ExtractFieldMessage(Model.Apprenticeship.StartDateError));

WriteLiteral("</span>\r\n");

                }

WriteLiteral("\r\n                <div");

WriteLiteral(" class=\"form-group form-group-month\"");

WriteLiteral(">\r\n                    <label");

WriteLiteral(" for=\"StartDate.Month\"");

WriteLiteral(">Month</label>\r\n\r\n");

WriteLiteral("                    ");

               Write(Html.TextBox("StartDate.Month", Model.Apprenticeship.StartDate.Month, new { @class = "form-control length-limit", type = "number", maxlength = "2", min = "1", max = "12", aria_labelledby = "StartDate.Month" }));

WriteLiteral("\r\n\r\n                </div>\r\n                <div");

WriteLiteral(" class=\"form-group form-group-year\"");

WriteLiteral(">\r\n                    <label");

WriteLiteral(" for=\"StartDate.Year\"");

WriteLiteral(">Year</label>\r\n");

WriteLiteral("                    ");

               Write(Html.TextBox("StartDate.Year", Model.Apprenticeship.StartDate.Year, new { @class = "form-control length-limit", type = "number", maxlength = "4", min = "1900", max = "9999", aria_labelledby = "StartDate.Year" }));

WriteLiteral("\r\n                </div>\r\n            </div>\r\n\r\n        </div>\r\n");

    }

WriteLiteral("    ");

     if (Model.Apprenticeship.IsEndDateLockedForUpdate)
    {

WriteLiteral("        <div");

WriteLiteral(" class=\"form-error-group form-group\"");

WriteLiteral(">\r\n            <hr />\r\n            <span");

WriteLiteral(" class=\"form-label-bold\"");

WriteLiteral(">Planned training finish date</span>\r\n            <span");

WriteLiteral(" class=\"\"");

WriteLiteral(">");

                      Write(Model.Apprenticeship.EndDate.DateTime.Value.ToGdsFormatWithoutDay());

WriteLiteral(" </span>\r\n        </div>\r\n");


        
   Write(Html.Hidden("EndDate.Month", Model.Apprenticeship.EndDate.Month));

                                                                         
        
   Write(Html.Hidden("EndDate.Year", Model.Apprenticeship.EndDate.Year));

                                                                       
    }
    else
    {

WriteLiteral("        <div");

WriteAttribute("class", Tuple.Create(" class=\"", 7682), Tuple.Create("\"", 7792)
, Tuple.Create(Tuple.Create("", 7690), Tuple.Create("form-error-group", 7690), true)
, Tuple.Create(Tuple.Create(" ", 7706), Tuple.Create("form-group", 7707), true)
, Tuple.Create(Tuple.Create(" ", 7717), Tuple.Create<System.Object, System.Int32>(!string.IsNullOrEmpty(Model.Apprenticeship.EndDateError) ? "error" : ""
, 7718), false)
);

WriteLiteral(">\r\n\r\n            <span");

WriteLiteral(" class=\"form-label-bold\"");

WriteLiteral(">Planned training finish date</span>\r\n            <span");

WriteLiteral(" class=\"form-hint\"");

WriteLiteral(">For example, 02 2019</span>\r\n\r\n            <div");

WriteLiteral(" id=\"EndDate\"");

WriteLiteral(" class=\"form-date\"");

WriteLiteral(">\r\n");

                
                 if (!string.IsNullOrEmpty(Model.Apprenticeship.EndDateError))
                {

WriteLiteral("                    <span");

WriteLiteral(" class=\"error-message\"");

WriteLiteral(" id=\"error-message-EndDate\"");

WriteLiteral(">");

                                                                      Write(ValidationMessage.ExtractFieldMessage(Model.Apprenticeship.EndDateError));

WriteLiteral("</span>\r\n");

                }

WriteLiteral("\r\n                <div");

WriteLiteral(" class=\"form-group form-group-month\"");

WriteLiteral(">\r\n                    <label");

WriteLiteral(" for=\"EndDate.Month\"");

WriteLiteral(">\r\n                        Month\r\n                    </label>\r\n\r\n");

WriteLiteral("                    ");

               Write(Html.TextBox("EndDate.Month", Model.Apprenticeship.EndDate.Month, new { @class = "form-control length-limit", type = "number", maxlength = "2", min = "1", max = "12", aria_labelledby = "EndDate.Month" }));

WriteLiteral("\r\n\r\n                </div>\r\n                <div");

WriteLiteral(" class=\"form-group form-group-month\"");

WriteLiteral(">\r\n                    <label");

WriteLiteral(" for=\"EndDate.Year\"");

WriteLiteral(">\r\n                        Year\r\n                    </label>\r\n");

WriteLiteral("                    ");

               Write(Html.TextBox("EndDate.Year", Model.Apprenticeship.EndDate.Year, new { @class = "form-control length-limit", type = "number", maxlength = "4", min = "1900", max = "9999", aria_labelledby = "EndDate.Year" }));

WriteLiteral("\r\n                </div>\r\n            </div>\r\n        </div>\r\n");

    }

WriteLiteral("\r\n    <div");

WriteAttribute("class", Tuple.Create(" class=\"", 9166), Tuple.Create("\"", 9273)
, Tuple.Create(Tuple.Create("", 9174), Tuple.Create("form-error-group", 9174), true)
, Tuple.Create(Tuple.Create(" ", 9190), Tuple.Create("form-group", 9191), true)
, Tuple.Create(Tuple.Create(" ", 9201), Tuple.Create<System.Object, System.Int32>(!string.IsNullOrEmpty(Model.Apprenticeship.CostError) ? "error" : ""
, 9202), false)
);

WriteLiteral(">\r\n        <hr />\r\n\r\n");

        
         if (Model.Apprenticeship.IsLockedForUpdate)
        {

WriteLiteral("            <label");

WriteLiteral(" for=\"Cost\"");

WriteLiteral(">\r\n                <span");

WriteLiteral(" class=\"form-label-bold\"");

WriteLiteral(">Total agreed apprenticeship price (excluding VAT)</span>\r\n            </label>\r\n" +
"");

WriteLiteral("            <span");

WriteLiteral(" class=\"heading-small\"");

WriteLiteral(">£ </span>\r\n");

WriteLiteral("            <span>");

             Write(Model.Apprenticeship.Cost);

WriteLiteral("</span>\r\n");

            
       Write(Html.Hidden("Cost", Model.Apprenticeship.Cost));

                                                           

WriteLiteral("            <div");

WriteLiteral(" class=\"approve-alert\"");

WriteLiteral(">\r\n                <div");

WriteLiteral(" class=\"panel panel-border-wide alert-blue\"");

WriteLiteral(@">
                    If you want to change the total agreed apprenticeship price, you'll need to ask your training provider to make the changes on your behalf.
                    We'll ask you to approve any changes they make.
                </div>
            </div>
");

        }
        else
        {

WriteLiteral("            <label");

WriteLiteral(" for=\"Cost\"");

WriteLiteral(">\r\n                <span");

WriteLiteral(" class=\"form-label-bold\"");

WriteLiteral(">Total agreed apprenticeship price (excluding VAT)</span>\r\n                <span");

WriteLiteral(" class=\"form-hint\"");

WriteLiteral(">Enter the price, including any end-point assessment costs, in whole pounds.</spa" +
"n>\r\n                <span");

WriteLiteral(" class=\"form-hint\"");

WriteLiteral(">For example, for £1,500 enter 1500</span>\r\n");

                
                 if (!string.IsNullOrEmpty(Model.Apprenticeship.CostError))
                {

WriteLiteral("                    <span");

WriteLiteral(" class=\"error-message\"");

WriteLiteral(" id=\"error-message-cost\"");

WriteLiteral(">");

                                                                   Write(Model.Apprenticeship.CostError);

WriteLiteral("</span>\r\n");

                }

WriteLiteral("            </label>\r\n");

WriteLiteral("            <span");

WriteLiteral(" class=\"heading-small\"");

WriteLiteral(">£ </span>");

                                                 
                                            Write(Html.TextBox("Cost", Model.Apprenticeship.Cost, new { @class = "form-control form-control-3-4", type = "text", aria_labelledby = "Cost", maxlength = "7" }));

                                                                                                                                                                                                             
        }

WriteLiteral("\r\n    </div>\r\n\r\n    <div");

WriteAttribute("class", Tuple.Create(" class=\"", 10933), Tuple.Create("\"", 11041)
, Tuple.Create(Tuple.Create("", 10941), Tuple.Create("form-group", 10941), true)
, Tuple.Create(Tuple.Create(" ", 10951), Tuple.Create("last-child", 10952), true)
, Tuple.Create(Tuple.Create(" ", 10962), Tuple.Create<System.Object, System.Int32>(!string.IsNullOrEmpty(Model.Apprenticeship.EmployerRefError) ? "error" : ""
, 10963), false)
);

WriteLiteral(">\r\n        <hr />\r\n");

WriteLiteral("        ");

   Write(Html.Label("EmployerRef", "Reference (optional)", new { @class = "form-label-bold" }));

WriteLiteral("\r\n        <span");

WriteLiteral(" class=\"form-hint\"");

WriteLiteral(">Add a reference, such as employee number or location - this won\'t be seen by the" +
" training provider</span>\r\n");

        
         if (!string.IsNullOrEmpty(Model.Apprenticeship.EmployerRefError))
        {

WriteLiteral("            <span");

WriteLiteral(" class=\"error-message\"");

WriteLiteral(" id=\"error-message-employerref\"");

WriteLiteral(">");

                                                                  Write(Model.Apprenticeship.EmployerRefError);

WriteLiteral("</span>\r\n");

        }

WriteLiteral("        ");

   Write(Html.TextBox("EmployerRef", Model.Apprenticeship.EmployerRef, new { @class = "form-control form-control-3-4" }));

WriteLiteral("\r\n        <p");

WriteLiteral(" id=\"charCount-noJS\"");

WriteLiteral(">Enter up to a maximum of 20 characters</p>\r\n        <p");

WriteLiteral(" id=\"charCount\"");

WriteLiteral(" style=\"display:none;\"");

WriteLiteral("><span");

WriteLiteral(" name=\"countchars\"");

WriteLiteral(" id=\"countchars\"");

WriteLiteral("></span> characters remaining</p>\r\n    </div>\r\n</div>\r\n");

        }
    }
}
#pragma warning restore 1591
