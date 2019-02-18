using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.Commitments.Api.Types.Commitment.Types;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;

namespace SFA.DAS.EmployerCommitments.Application.Commands.CreateCommitment
{
    public sealed class CreateCommitmentCommandHandler :
        IAsyncRequestHandler<CreateCommitmentCommand, CreateCommitmentCommandResponse>
    {
        private readonly IEmployerCommitmentApi _commitmentApi;
        private readonly IProviderEmailNotificationService _providerEmailNotificationService;

        public CreateCommitmentCommandHandler(
            IEmployerCommitmentApi commitmentApi, 
            IProviderEmailNotificationService providerEmailNotificationService)
        {
            _commitmentApi = commitmentApi;
            _providerEmailNotificationService = providerEmailNotificationService;
        }

        public async Task<CreateCommitmentCommandResponse> Handle(CreateCommitmentCommand request)
        {
            var commitmentRequest = new CommitmentRequest
            {
                Commitment = request.Commitment,
                UserId = request.UserId,
                Message = request.Message,
                LastAction = request.LastAction
            };

            var commitmentId = (await _commitmentApi.CreateEmployerCommitment(request.Commitment.EmployerAccountId, commitmentRequest)).Id;

            //todo: do we really need to fetch this?
            var commitment = await _commitmentApi.GetEmployerCommitment(request.Commitment.EmployerAccountId, commitmentId);

#if DEBUG
            await _providerEmailNotificationService.SendCreateCommitmentNotification(commitment); // TODO REMOVE THIS ONLY FOR LOCAL TESTING
#endif

            if (request.Commitment.CommitmentStatus == CommitmentStatus.Active)
            {
                await _providerEmailNotificationService.SendCreateCommitmentNotification(commitment);
            }

            return new CreateCommitmentCommandResponse { CommitmentId = commitment.Id };
        }      
    }
}
