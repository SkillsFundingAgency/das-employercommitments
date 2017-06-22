﻿using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using MediatR;
using SFA.DAS.EAS.Application;
using SFA.DAS.EAS.Application.Queries.GetOrganisations;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Account;
using SFA.DAS.EAS.Domain.Models.ReferenceData;
using SFA.DAS.EAS.Web.ViewModels;
using SFA.DAS.EAS.Web.ViewModels.Organisation;

namespace SFA.DAS.EAS.Web.Orchestrators
{
    public class SearchOrganisationOrchestrator : UserVerificationOrchestratorBase
    {
        private readonly ICookieStorageService<EmployerAccountData> _cookieService;
        private const string CookieName = "sfa-das-employerapprenticeshipsservice-employeraccount";

        public SearchOrganisationOrchestrator(IMediator mediator, ICookieStorageService<EmployerAccountData> cookieService)
            : base(mediator)
        {
            _cookieService = cookieService;
        }

        public async Task<OrchestratorResponse<SearchOrganisationViewModel>> SearchOrganisation(string searchTerm, int pageNumber)
        {
            var response = new OrchestratorResponse<SearchOrganisationViewModel>();

            try
            {
                var result = await Mediator.SendAsync(new GetOrganisationsRequest { SearchTerm = searchTerm, PageNumber = pageNumber });
                response.Data = new SearchOrganisationViewModel
                {
                    Results = result.Organisations,
                    SearchTerm = searchTerm
                };
            }
            catch (InvalidRequestException ex)
            {
                response.Exception = ex;
                response.FlashMessage = new FlashMessageViewModel().CreateErrorFlashMessageViewModel(ex.ErrorMessages);
                response.Status = HttpStatusCode.BadRequest;
            }

            return response;
        }

        public virtual EmployerAccountData GetCookieData(HttpContextBase context)
        {
            return _cookieService.Get(CookieName);
        }

        public virtual void CreateCookieData(HttpContextBase context, EmployerAccountData data)
        {
            _cookieService.Create(data, CookieName, 365);
        }
    }
}