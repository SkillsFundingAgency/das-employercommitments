using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerCommitments.Application.Exceptions;
using SFA.DAS.EmployerCommitments.Application.Validation;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerCommitments.Application.Commands.SendNotification
{
    public class SendNotificationCommandHandler : AsyncRequestHandler<SendNotificationCommand>
    {
        private readonly IValidator<SendNotificationCommand> _validator;
        private readonly ILog _logger;
        private readonly IBackgroundNotificationService _backgroundNotificationService;

        public SendNotificationCommandHandler(
            IValidator<SendNotificationCommand> validator,
            ILog logger, 
            IBackgroundNotificationService backgroundNotificationService)
        {
            _validator = validator;
            _logger = logger;
            _backgroundNotificationService = backgroundNotificationService;
        }

        protected override async Task HandleCore(SendNotificationCommand message)
        {
            var validationResult = _validator.Validate(message);

            if (!validationResult.IsValid())
            {
                _logger.Info("SendNotificationCommandHandler Invalid Request");
                throw new InvalidRequestException(validationResult.ValidationDictionary);
            }
            try
            {
                //_logger.Info($"---- === ||| -->     {message.Email.RecipientsAddress}    <-- ||| === ----");
                // todo: remove async await once happy with general approach.
                await Task.Run(() => _backgroundNotificationService.SendEmail(message.Email));
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error using background notifications service.");
            }
        }
    }
}
