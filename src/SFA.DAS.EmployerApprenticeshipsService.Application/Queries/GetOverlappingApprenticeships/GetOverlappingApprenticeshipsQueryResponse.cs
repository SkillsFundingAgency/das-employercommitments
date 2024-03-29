using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Commitments.Api.Types.Validation;

namespace SFA.DAS.EmployerCommitments.Application.Queries.GetOverlappingApprenticeships
{
    public class GetOverlappingApprenticeshipsQueryResponse
    {
        public IEnumerable<ApprenticeshipOverlapValidationResult> Overlaps { get; set; }

        public IEnumerable<OverlappingApprenticeship> GetOverlappingApprenticeships(long apprenticeshipId)
        {
            return Overlaps.FirstOrDefault(m => m.Self.ApprenticeshipId == apprenticeshipId)
                       ?.OverlappingApprenticeships
                   ?? Enumerable.Empty<OverlappingApprenticeship>();
        }

        public IEnumerable<OverlappingApprenticeship> GetFirstOverlappingApprenticeships()
        {
            return Overlaps.FirstOrDefault()?.OverlappingApprenticeships
                   ?? Enumerable.Empty<OverlappingApprenticeship>();
        }
    }
}