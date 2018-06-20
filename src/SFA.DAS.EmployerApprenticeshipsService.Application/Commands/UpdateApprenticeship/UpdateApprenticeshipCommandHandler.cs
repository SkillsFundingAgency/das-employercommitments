using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.Commitment.Types;
using SFA.DAS.EmployerCommitments.Application.Exceptions;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;

namespace SFA.DAS.EmployerCommitments.Application.Commands.UpdateApprenticeship
{
    public class UpdateApprenticeshipCommandHandler : AsyncRequestHandler<UpdateApprenticeshipCommand>
    {
        private readonly IEmployerCommitmentApi _commitmentsApi;
        private readonly UpdateApprenticeshipCommandValidator _validator;
        private readonly IProviderEmailNotificationService _providerEmailNotificationService;

        public UpdateApprenticeshipCommandHandler(IEmployerCommitmentApi commitmentsApi,
            IProviderEmailNotificationService providerEmailNotificationService)
        {
            _commitmentsApi = commitmentsApi;
            _providerEmailNotificationService = providerEmailNotificationService;
            _validator = new UpdateApprenticeshipCommandValidator();
        }

        protected override async Task HandleCore(UpdateApprenticeshipCommand message)
        {
            var validationResult = _validator.Validate(message);

            if (!validationResult.IsValid())
                throw new InvalidRequestException(validationResult.ValidationDictionary);

            var commitment = await
                _commitmentsApi.GetEmployerCommitment(message.AccountId, message.Apprenticeship.CommitmentId);

            await _commitmentsApi.UpdateEmployerApprenticeship(message.AccountId, message.Apprenticeship.CommitmentId, message.Apprenticeship.Id, 
                new ApprenticeshipRequest
                    {
                        Apprenticeship = message.Apprenticeship,
                        UserId = message.UserId,
                        LastUpdatedByInfo = new LastUpdateInfo { EmailAddress = message.UserEmail, Name = message.UserName }
                    });

            if (commitment.TransferSender?.TransferApprovalStatus ==
                Commitments.Api.Types.TransferApprovalStatus.Rejected)
            {
                await _providerEmailNotificationService.SendProviderTransferRejectedCommitmentEditNotification(
                    commitment);
            }
        }
    }
}