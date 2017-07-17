using System;
using MediatR;
using SFA.DAS.EmployerCommitments.Domain.Models.Apprenticeship;

namespace SFA.DAS.EmployerCommitments.Application.Commands.UpdateApprenticeshipStatus
{
    public sealed class UpdateApprenticeshipStatusCommand : IAsyncRequest
    {
        public long ApprenticeshipId { get; set; }
        public long EmployerAccountId { get; set; }
        public string UserId { get; set; }
        public ChangeStatusType ChangeType { get; set; }
        public DateTime DateOfChange { get; set; }
        public string UserDisplayName { get; set; }
        public string UserEmailAddress { get; set; }
    }
}
