using MediatR;

namespace SFA.DAS.EmployerCommitments.Application.Commands.DeleteApprentice
{
    public class DeleteApprenticeshipCommand : IAsyncRequest
    {
        public long AccountId { get; set; }
        public long ApprenticeshipId { get; set; }
        public string UserId { get; set; }
        public string UserDisplayName { get; set; }
        public string UserEmailAddress { get; set; }
    }
}
