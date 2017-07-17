using System.Collections.Generic;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;

namespace SFA.DAS.EmployerCommitments.Application.Queries.GetPriceHistoryQueryRequest
{
    public class GetPriceHistoryQueryResponse
    {
        public List<PriceHistory> History { get; set; }
    }
}