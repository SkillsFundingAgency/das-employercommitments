using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.EmployerCommitments.Application.Domain.Commitment;

namespace SFA.DAS.EmployerCommitments.Application.Extensions
{
    /// <remarks>
    /// Intuitively it feels this shouldn't be duplicating CommitmentListItemExtensions.
    /// Perhaps CommitmentView should derive from CommitmentListItem?
    /// </remarks>
    public static class CommitmentViewExtensions
    {
        private static readonly CommitmentStatusCalculator StatusCalculator = new CommitmentStatusCalculator();

        public static RequestStatus GetStatus(this CommitmentView commitment)
        {
            return StatusCalculator.GetStatus(commitment.EditStatus, commitment.Apprenticeships.Count, commitment.LastAction, commitment.AgreementStatus);
        }
    }
}
