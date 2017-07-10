using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerCommitments.Domain.Data.Entities.Account;
using SFA.DAS.EmployerCommitments.Domain.Models.Organisation;

namespace SFA.DAS.EmployerCommitments.Web.Helpers
{
    internal class AccountLegalEntitiesHelper
    {
        private readonly IMediator _mediator;

        internal AccountLegalEntitiesHelper(IMediator mediator)
        {
            _mediator = mediator;
        }

        internal async Task<List<LegalEntity>> GetAccountLegalEntities(string hashedLegalEntityId, string userIdClaim)
        {
            //TODO API CALL
            //var accountEntities = await _mediator.SendAsync(new GetAccountLegalEntitiesRequest
            //{
            //    HashedLegalEntityId = hashedLegalEntityId,
            //    UserId = userIdClaim
            //});
            return new List<LegalEntity>();//TODO API CALL accountEntities.Entites.LegalEntityList;
        }

        internal bool IsLegalEntityAlreadyAddedToAccount(List<LegalEntity> accountLegalEntities, string organisationName, string organisationCode, OrganisationType organisationType)
        {
            if (organisationType == OrganisationType.Charities || organisationType == OrganisationType.CompaniesHouse)
            {
                return accountLegalEntities.Any(x => x.Code.Equals(organisationCode.Trim(), StringComparison.CurrentCultureIgnoreCase) && x.Source == (short)organisationType);
            }
            else
            {
                return accountLegalEntities.Any(x => x.Name.Equals(organisationName.Trim(), StringComparison.CurrentCultureIgnoreCase) && x.Source == (short)organisationType);
            }
        }
    }
}