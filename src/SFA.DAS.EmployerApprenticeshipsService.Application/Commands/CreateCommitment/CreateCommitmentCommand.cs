using MediatR;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.Commitments.Api.Types.Commitment.Types;

namespace SFA.DAS.EmployerCommitments.Application.Commands.CreateCommitment
{
    public sealed class CreateCommitmentCommand : IAsyncRequest<CreateCommitmentCommandResponse>
    {
        public Commitment Commitment { get; set; }
        public string Message { get; set; }
        public string UserId { get; set; }
        public LastAction LastAction { get; set; }
    }
}
