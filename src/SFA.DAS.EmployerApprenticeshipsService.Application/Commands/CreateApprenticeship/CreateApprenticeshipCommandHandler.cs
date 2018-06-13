using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.Commitment.Types;
using SFA.DAS.EmployerCommitments.Application.Exceptions;
using SFA.DAS.EmployerCommitments.Application.Services;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;

namespace SFA.DAS.EmployerCommitments.Application.Commands.CreateApprenticeship
{
    public class CreateApprenticeshipCommandHandler : AsyncRequestHandler<CreateApprenticeshipCommand>
    {
        private readonly IEmployerCommitmentApi _commitmentsApi;
        private readonly CreateApprenticeshipCommandValidator _validator;
        private readonly IProviderEmailNotificationService _providerEmailNotificationService;

        public CreateApprenticeshipCommandHandler(IEmployerCommitmentApi commitmentsApi,
            IProviderEmailNotificationService providerEmailNotificationService)
        {
            _commitmentsApi = commitmentsApi;
            _providerEmailNotificationService = providerEmailNotificationService;
            _validator = new CreateApprenticeshipCommandValidator();
        }

        protected override async Task HandleCore(CreateApprenticeshipCommand message)
        {
            var validationResult = _validator.Validate(message);

            if (!validationResult.IsValid())
                throw new InvalidRequestException(validationResult.ValidationDictionary);

            var commitment = await
                _commitmentsApi.GetEmployerCommitment(message.AccountId, message.Apprenticeship.CommitmentId);

            var apprenticeshipRequest = new ApprenticeshipRequest
            {
                Apprenticeship = message.Apprenticeship,
                UserId = message.UserId,
                LastUpdatedByInfo = new LastUpdateInfo { EmailAddress = message.UserEmailAddress, Name = message.UserDisplayName }
            };
            await _commitmentsApi.CreateEmployerApprenticeship(message.AccountId, message.Apprenticeship.CommitmentId, apprenticeshipRequest);

            if (commitment.TransferSender?.TransferApprovalStatus ==
                Commitments.Api.Types.TransferApprovalStatus.Rejected)
            {
                await _providerEmailNotificationService.SendProviderTransferRejectedCommitmentEditNotification(
                    commitment);
            }
        }
    }
}