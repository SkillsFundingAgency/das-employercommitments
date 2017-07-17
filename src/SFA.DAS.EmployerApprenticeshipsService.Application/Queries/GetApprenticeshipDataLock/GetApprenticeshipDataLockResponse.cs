using System.Collections.Generic;

using SFA.DAS.Commitments.Api.Types.DataLock;

namespace SFA.DAS.EmployerCommitments.Application.Queries.GetApprenticeshipDataLock
{
    public class GetApprenticeshipDataLockResponse
    {
        public IEnumerable<DataLockStatus> DataLockStatus { get; set; }
    }
}