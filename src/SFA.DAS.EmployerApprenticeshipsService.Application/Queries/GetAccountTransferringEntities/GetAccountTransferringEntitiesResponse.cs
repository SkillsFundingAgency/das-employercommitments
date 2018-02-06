using System.Collections.Generic;
using SFA.DAS.EmployerCommitments.Domain.Models.Organisation;

namespace SFA.DAS.EmployerCommitments.Application.Queries.GetAccountTransferringEntities
{
    public class GetAccountTransferringEntitiesResponse
    {
        public List<TransferringEntity> TransferringEntities { get; set; }
    }
}