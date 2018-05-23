using System.Collections.Generic;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.EmployerCommitments.Domain.Models.Organisation;
using SFA.DAS.EmployerCommitments.Web.ViewModels;

namespace SFA.DAS.EmployerCommitments.Web.Orchestrators.Mappers
{
    public interface ICommitmentMapper
    {
        CommitmentViewModel MapToCommitmentViewModel(CommitmentView commitment);
        TransferRequestViewModel MapToTransferRequestViewModel(TransferRequest transferRequest);
        IEnumerable<TransferConnectionViewModel> MapToTransferConnectionsViewModel(List<TransferConnection> transferConnections);
    }
}