﻿using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.EmployerCommitments.Application.Domain.Commitment;

namespace SFA.DAS.EmployerCommitments.Application.Extensions
{
    /// <summary>
    /// </summary>
    /// <remarks>
    /// Ideally would belong on CommitmentListItem itself, but this is the next best thing!
    /// </remarks>
    public static class CommitmentListItemExtensions
    {
        private static readonly CommitmentStatusCalculator StatusCalculator = new CommitmentStatusCalculator();

        // pull RequestStatus down
        public static RequestStatus GetStatus(this CommitmentListItem commitment)
        {
            // could change to static, but would lose the power to inject
            // would a static contructor be injected properly? don't think so
            // what about static property injection?
            // or manual inject override method (with default?) or don't inject, why would you want to change it??
            return StatusCalculator.GetStatus(commitment.EditStatus, commitment.ApprenticeshipCount, commitment.LastAction, commitment.AgreementStatus);
        }
    }
}
