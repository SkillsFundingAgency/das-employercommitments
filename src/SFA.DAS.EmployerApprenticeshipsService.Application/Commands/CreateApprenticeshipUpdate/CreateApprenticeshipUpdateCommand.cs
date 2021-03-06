﻿using FluentValidation.Attributes;
using MediatR;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;

namespace SFA.DAS.EmployerCommitments.Application.Commands.CreateApprenticeshipUpdate
{
    [Validator(typeof(CreateApprenticeshipUpdateCommandValidator))]
    public class CreateApprenticeshipUpdateCommand : IAsyncRequest
    {
        public string UserId { get; set; }
        public long EmployerId { get; set; }
        public ApprenticeshipUpdate ApprenticeshipUpdate { get; set; }
        public string UserDisplayName { get; set; }
        public string UserEmailAddress { get; set; }
    }
}