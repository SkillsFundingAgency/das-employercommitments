﻿using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.EAS.Web.Models;
using SFA.DAS.EAS.Web.Orchestrators;

namespace SFA.DAS.EAS.Web.Controllers
{
    [RoutePrefix("accounts")]
    public class EmployerAccountOrganisationController : BaseController
    {
        private readonly OrganisationOrchestrator _orchestrator;
        private readonly IMapper _mapper;

        public EmployerAccountOrganisationController(
            IOwinWrapper owinWrapper,
            OrganisationOrchestrator orchestrator,
            IFeatureToggle featureToggle,
            IUserWhiteList userWhiteList,
            IMapper mapper) 
            :base(owinWrapper, featureToggle, userWhiteList)
        {
            _orchestrator = orchestrator;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("Organisation/Add")]
        public ActionResult AddOrganisation()
        {
            return View("AddOrganisation",
                new OrchestratorResponse<AddLegalEntityViewModel> {Data = new AddLegalEntityViewModel()});
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Organisation/Add")]
        public async Task<ActionResult> AddOrganisation(OrganisationType orgType, string companiesHouseNumber, string publicBodyName, string charityRegNo)
        {
            OrchestratorResponse<OrganisationDetailsViewModel> response;

            string hashedAccountId = string.Empty;

            switch (orgType)
            {
                case OrganisationType.Charities:
                    response = await FindCharity(charityRegNo, hashedAccountId, OwinWrapper.GetClaimValue(@"sub"));
                    break;
                case OrganisationType.CompaniesHouse:
                    response = await FindCompany(companiesHouseNumber, hashedAccountId, OwinWrapper.GetClaimValue(@"sub"));

                    break;
                case OrganisationType.PublicBodies:
                    var searchResponse = await FindPublicSectorOrganisation(publicBodyName, hashedAccountId, OwinWrapper.GetClaimValue(@"sub"));

                    if (searchResponse.Data.Results.Data.Count != 1 || searchResponse.Data.Results.Data.All(x => x.AddedToAccount))
                    {
                        return View("ViewPublicSectorOrganisationSearchResults", searchResponse);
                    }

                    response = new OrchestratorResponse<OrganisationDetailsViewModel>
                    {
                        Data = searchResponse.Data.Results.Data.FirstOrDefault(),
                        Status = searchResponse.Status
                    };
                    break;

                case OrganisationType.Other:
                    return RedirectToAction("AddCustomOrganisationDetails");

                default:
                    throw new NotImplementedException("Org Type Not Implemented");
            }

            if (response.Status == HttpStatusCode.OK)
            {
                //Removes empty address entries (i.e. ' , , ')
                var address = (response.Data?.Address ?? string.Empty).Replace(",", string.Empty);

                if (string.IsNullOrWhiteSpace(address))
                {
                    var addressViewModel = _mapper.Map<AddOrganisationAddressModel>(response.Data);

                    var addressResponse = new OrchestratorResponse<AddOrganisationAddressModel>
                    {
                        Data = addressViewModel
                    };

                    return View("AddOrganisationAddress", addressResponse);
                }

                return RedirectToAction("GatewayInform", "EmployerAccount", response.Data);
            }

            var errorResponse = new OrchestratorResponse<AddLegalEntityViewModel>
            {
                Data = new AddLegalEntityViewModel { HashedAccountId = hashedAccountId },
                Status = HttpStatusCode.OK,
            };

            return View("AddOrganisation", errorResponse);
        }

        [HttpGet]
        [Route("Organisation/UpdateAddress")]
        public ActionResult AddOrganisationAddress(AddOrganisationAddressModel request)
        {
            var response = new OrchestratorResponse<AddOrganisationAddressModel>
            {
                Data = request,
                Status = HttpStatusCode.OK
            };

            return View("AddOrganisationAddress", response);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Organisation/UpdateAddress")]
        public ActionResult UpdateOrganisationAddress(AddOrganisationAddressModel request)
        {
            var response = _orchestrator.AddOrganisationAddress(request);

            if (response.Status == HttpStatusCode.BadRequest)
            {
                request.ErrorDictionary = response.Data.ErrorDictionary;

                var errorResponse = new OrchestratorResponse<AddOrganisationAddressModel>
                {
                    Data = request,
                    Status = HttpStatusCode.BadRequest,
                    Exception = response.Exception,
                    FlashMessage = response.FlashMessage
                };

                return View("AddOrganisationAddress", errorResponse);
            }

            return RedirectToAction("GatewayInform", "EmployerAccount", response.Data);
        }
        
        [HttpGet]
        [Route("Organisation/Add/Other")]
        public ActionResult AddCustomOrganisationDetails()
        {
            var response = new OrchestratorResponse<OrganisationDetailsViewModel>
            {
                Data = new OrganisationDetailsViewModel()
            };

            return View("AddCustomOrganisationDetails", response);
        }

        [HttpPost]
        [Route("Organisation/Add/Other")]
        public async Task<ActionResult> AddCustomOrganisationDetails(OrganisationDetailsViewModel model)
        {
            var response = await _orchestrator.ValidateLegalEntityName(model);
            
            if (response.Status == HttpStatusCode.BadRequest)
            {
                return View("AddCustomOrganisationDetails", response);
            }

            model.Type = OrganisationType.Other;
            model.Status = "active";

            var addressModel = _mapper.Map<AddOrganisationAddressModel>(response.Data);


            return RedirectToAction("AddOrganisationAddress", addressModel);
        }

        private async Task<OrchestratorResponse<PublicSectorOrganisationSearchResultsViewModel>> FindPublicSectorOrganisation(string publicSectorOrganisationName, string hashedAccountId, string userIdClaim)
        {
            var response = await _orchestrator.FindPublicSectorOrganisation(publicSectorOrganisationName, hashedAccountId, userIdClaim);

            switch (response.Status)
            {
                case HttpStatusCode.NotFound:
                    TempData["publicBodyError"] = "No public organsiations were not found using your search term";
                    break;
            }

            return response;
        }

        private async Task<OrchestratorResponse<OrganisationDetailsViewModel>> FindCompany(string companiesHouseNumber, string hashedAccountId, string userIdClaim)
        {
            var response = await _orchestrator.GetLimitedCompanyByRegistrationNumber(companiesHouseNumber, hashedAccountId, userIdClaim);

            switch (response.Status)
            {
                case HttpStatusCode.NotFound:
                    TempData["companyError"] = "Company not found";
                    break;
                case HttpStatusCode.Conflict:
                    TempData["companyError"] = "Enter a company that isn't already registered";
                    break;
            }

            return response;
        }

        private async Task<OrchestratorResponse<OrganisationDetailsViewModel>> FindCharity(string charityRegNo, string hashedAccountId, string userIdClaim)
        {
            var response = await _orchestrator.GetCharityByRegistrationNumber(charityRegNo, hashedAccountId, userIdClaim);

            switch (response.Status)
            {
                case HttpStatusCode.NotFound:
                    TempData["charityError"] = "Charity not found";
                    break;
                case HttpStatusCode.BadRequest:
                    TempData["charityError"] = "Charity is removed";
                    break;
                case HttpStatusCode.Conflict:
                    TempData["charityError"] = "Charity is already added";
                    break;
            }

            return response;
        }
    }
}