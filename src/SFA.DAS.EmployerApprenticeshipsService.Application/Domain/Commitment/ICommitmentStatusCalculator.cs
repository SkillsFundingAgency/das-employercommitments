using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.Commitments.Api.Types.Commitment.Types;

namespace SFA.DAS.EmployerCommitments.Application.Domain.Commitment
{
    public interface ICommitmentStatusCalculator
    {
        RequestStatus GetStatus(EditStatus editStatus, int apprenticeshipCount, LastAction lastAction, AgreementStatus? overallAgreementStatus);
        RequestStatus GetTransferStatus(EditStatus edit, TransferApprovalStatus transferApproval);
    }
}