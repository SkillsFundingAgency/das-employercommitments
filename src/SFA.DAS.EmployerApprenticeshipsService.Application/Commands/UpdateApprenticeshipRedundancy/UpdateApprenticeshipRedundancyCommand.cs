using MediatR;

namespace SFA.DAS.EmployerCommitments.Application.Commands.UpdateApprenticeshipRedundancy
{
    public sealed class UpdateApprenticeshipRedundancyCommand : IAsyncRequest
    {
        public long ApprenticeshipId { get; set; }
        public long EmployerAccountId { get; set; }
        public string UserId { get; set; }
        public bool? MadeRedundant { get; set; }
        public string UserEmailAddress { get; set; }
        public string UserDisplayName { get; set; }
    }
}
