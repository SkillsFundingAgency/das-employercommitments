using System;
using MediatR;
using SFA.DAS.EmployerCommitments.Domain.Models.Apprenticeship;

namespace SFA.DAS.EmployerCommitments.Application.Queries.ValidateStatusChangeDate
{
    public sealed class ValidateStatusChangeDateQuery : IAsyncRequest<ValidateStatusChangeDateQueryResponse>
    {
        public long AccountId { get; set; }

        public long ApprenticeshipId { get; set; }

        public DateTime? DateOfChange { get; set; }

        public ChangeOption ChangeOption { get; set; }
    }
}
