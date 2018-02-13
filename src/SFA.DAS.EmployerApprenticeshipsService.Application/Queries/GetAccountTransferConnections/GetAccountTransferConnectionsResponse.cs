using System.Collections.Generic;
using SFA.DAS.EmployerCommitments.Domain.Models.Organisation;

namespace SFA.DAS.EmployerCommitments.Application.Queries.GetAccountTransferConnections
{
    public class GetAccountTransferConnectionsResponse
    {
        public List<TransferConnection> TransferConnections { get; set; }
    }
}