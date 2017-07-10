﻿using MediatR;
using SFA.DAS.Commitments.Api.Types.Commitment;

namespace SFA.DAS.EmployerCommitments.Application.Commands.CreateCommitment
{
    public sealed class CreateCommitmentCommand : IAsyncRequest<CreateCommitmentCommandResponse>
    {
        public Commitment Commitment { get; set; }
        public string Message { get; set; }
        public string UserId { get; set; }
    }
}