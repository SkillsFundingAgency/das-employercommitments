using System;
using MediatR;

namespace SFA.DAS.EmployerCommitments.Application.Commands.UpdateApprenticeshipStopDate
{
    public sealed class UpdateApprenticeshipStopDateCommand : IAsyncRequest
    {
        public long ApprenticeshipId { get; set; }
        public long EmployerAccountId { get; set; }
        public string UserId { get; set; }
        public DateTime NewStopDate { get; set; }
        public string UserDisplayName { get; set; }
        public string UserEmailAddress { get; set; }
        public long CommitmentId { get; set; }
    }
}
