using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.Commitments.Api.Types.Commitment.Types;
using SFA.DAS.EmployerCommitments.Web.Enums;

namespace SFA.DAS.EmployerCommitments.Web.Orchestrators
{
    public interface ICommitmentStatusCalculator
    {
        RequestStatus GetStatus(EditStatus editStatus, int apprenticeshipCount, LastAction lastAction, AgreementStatus? overallAgreementStatus);
    }
}