using System.Collections.Generic;

namespace SFA.DAS.EmployerCommitments.Application.Queries.GetProviders
{
    public class GetProvidersQueryResponse
    {
        public List<Domain.Models.ApprenticeshipProvider.Provider> Providers { get; set; }
    }
}