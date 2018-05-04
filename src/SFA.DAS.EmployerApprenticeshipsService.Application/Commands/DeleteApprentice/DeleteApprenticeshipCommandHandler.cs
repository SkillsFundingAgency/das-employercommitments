using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.Commitments.Api.Types.Commitment.Types;
using SFA.DAS.EmployerCommitments.Application.Exceptions;
using SFA.DAS.EmployerCommitments.Application.Validation;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;

namespace SFA.DAS.EmployerCommitments.Application.Commands.DeleteApprentice
{
    public sealed class DeleteApprenticeshipCommandHandler : AsyncRequestHandler<DeleteApprenticeshipCommand>
    {
        private readonly IEmployerCommitmentApi _commitmentsApi;
        private readonly IValidator<DeleteApprenticeshipCommand> _validator;
        private readonly IProviderEmailNotificationService _providerEmailNotificationService;

        public DeleteApprenticeshipCommandHandler(
            IEmployerCommitmentApi commitmentsApi, 
            IValidator<DeleteApprenticeshipCommand> validator, IProviderEmailNotificationService providerEmailNotificationService)
        {
            _validator = validator;
            _providerEmailNotificationService = providerEmailNotificationService;
            _commitmentsApi = commitmentsApi;
        }

        protected override async Task HandleCore(DeleteApprenticeshipCommand message)
        {
            var validationResult = _validator.Validate(message);

            if (!validationResult.IsValid())
            {
                throw new InvalidRequestException(validationResult.ValidationDictionary);
            }

            var commitment = await
                _commitmentsApi.GetEmployerCommitment(message.AccountId, message.ApprenticeshipId);

            await _commitmentsApi.DeleteEmployerApprenticeship(message.AccountId, message.ApprenticeshipId,
                new DeleteRequest { UserId = message.UserId, LastUpdatedByInfo = new LastUpdateInfo { EmailAddress = message.UserEmailAddress, Name = message.UserDisplayName } });

            if (commitment.TransferSender?.TransferApprovalStatus ==
                Commitments.Api.Types.TransferApprovalStatus.Rejected)
            {
                await _providerEmailNotificationService.SendProviderTransferRejectedCommitmentEditNotification(
                    commitment);
            }
        }
    }
}
