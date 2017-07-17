using System.Collections.Generic;
using SFA.DAS.EmployerCommitments.Domain.Models.Organisation;

namespace SFA.DAS.EmployerCommitments.Application.Queries.GetAccountLegalEntities
{
    public class GetAccountLegalEntitiesResponse
    {
        public List<LegalEntity> LegalEntities { get; set; }
    }
}