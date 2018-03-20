using System.Collections.Generic;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;

namespace SFA.DAS.EmployerCommitments.Application.Queries.GetApprenticeshipsByUln
{
    public class GetApprenticeshipsByUlnResponse
    {
        public List<Apprenticeship> Apprenticeships { get; set; }
    }
}