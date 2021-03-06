﻿using System;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using SFA.DAS.EmployerCommitments.Application.Exceptions;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;
using SFA.DAS.EmployerCommitments.Web.Authentication;
using SFA.DAS.EmployerCommitments.Web.ViewModels;

namespace SFA.DAS.EmployerCommitments.Web.Controllers
{
    public class BaseController : Controller
    {
        private const string FlashMessageCookieName = "sfa-das-employerapprenticeshipsservice-flashmessage";

        private readonly IMultiVariantTestingService _multiVariantTestingService;
        private readonly ICookieStorageService<FlashMessageViewModel> _flashMessage;

        public readonly IOwinWrapper OwinWrapper;

        public BaseController(
            IOwinWrapper owinWrapper, 
            IMultiVariantTestingService multiVariantTestingService,
            ICookieStorageService<FlashMessageViewModel> flashMessage )
        {
            OwinWrapper = owinWrapper;
            _multiVariantTestingService = multiVariantTestingService;
            _flashMessage = flashMessage;
        }

        protected override ViewResult View(string viewName, string masterName, object model)
        {
            var orchestratorResponse = model as OrchestratorResponse;

            if (orchestratorResponse == null)
            {
                return base.View(viewName, masterName, model);
            }
            
            var invalidRequestException = orchestratorResponse.Exception as InvalidRequestException;

            if (invalidRequestException != null)
            {
                foreach (var errorMessageItem in invalidRequestException.ErrorMessages)
                {
                    ModelState.AddModelError(errorMessageItem.Key, errorMessageItem.Value);
                }
            }

            if (orchestratorResponse.Status == HttpStatusCode.OK || orchestratorResponse.Status == HttpStatusCode.BadRequest)
                return ReturnViewResult(viewName, masterName, orchestratorResponse);

            if (orchestratorResponse.Status == HttpStatusCode.Unauthorized)
            {
                //Get the account id
                var accountId = Request.Params["HashedAccountId"];
                if (accountId != null)
                {
                    ViewBag.AccountId = accountId;
                }

                return base.View(@"AccessDenied", masterName, orchestratorResponse);
            }

            if (orchestratorResponse.Status == HttpStatusCode.NotFound)
            {
                return base.View("NotFound");
            }

            return base.View(@"GenericError", masterName, orchestratorResponse);
        }

        private ViewResult ReturnViewResult(string viewName, string masterName, OrchestratorResponse orchestratorResponse)
        {

            var userViews = _multiVariantTestingService.GetMultiVariantViews();

            if (userViews == null)
            {
                return base.View(viewName, masterName, orchestratorResponse);
            }

            var controllerName = ControllerContext.RouteData.Values["Controller"].ToString();
            var actionName = ControllerContext.RouteData.Values["Action"].ToString();
            var userView = userViews.Data.SingleOrDefault(c => c.Controller.Equals(controllerName, StringComparison.CurrentCultureIgnoreCase)
                            && c.Action.Equals(actionName, StringComparison.CurrentCultureIgnoreCase));

            if (userView != null)
            {
                if (!userView.SplitAccessAcrossUsers)
                {
                    var userEmail = OwinWrapper.GetClaimValue("email");

                    foreach (var view in userView.Views)
                    {
                        if (view.EmailAddresses.Any(pattern => Regex.IsMatch(userEmail, pattern, RegexOptions.IgnoreCase)))
                        {
                            return base.View(view.ViewName, masterName, orchestratorResponse);
                        }
                    }
                }
                else
                {
                    var randomViewName = _multiVariantTestingService.GetRandomViewNameToShow(userView.Views);
                    
                    if (string.IsNullOrEmpty(randomViewName))
                    {
                        return base.View(viewName, masterName, orchestratorResponse);
                    }

                    return base.View(randomViewName, masterName, orchestratorResponse);
                }
            }

            return base.View(viewName, masterName, orchestratorResponse);
        }
       
        public void AddFlashMessageToCookie(FlashMessageViewModel model)
        {
            _flashMessage.Delete(FlashMessageCookieName);

            _flashMessage.Create(model, FlashMessageCookieName);
        }

        public FlashMessageViewModel GetFlashMessageViewModelFromCookie()
        {
            var flashMessageViewModelFromCookie = _flashMessage.Get(FlashMessageCookieName);
            _flashMessage.Delete(FlashMessageCookieName);
            return flashMessageViewModelFromCookie;
        }

        public void RemoveFlashMessageFromCookie()
        {
            _flashMessage.Delete(FlashMessageCookieName);
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            if (filterContext.Exception is InvalidStateException)
            {
                var hashedAccountId = filterContext.RouteData.Values["hashedAccountId"];

                filterContext.ExceptionHandled = true;
                filterContext.Result = RedirectToAction("InvalidState", "Error", new { hashedAccountId });
            }
        }
    }
}