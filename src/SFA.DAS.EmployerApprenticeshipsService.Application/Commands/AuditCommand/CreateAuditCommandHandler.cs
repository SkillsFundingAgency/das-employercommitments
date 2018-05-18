﻿using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerCommitments.Application.Exceptions;
using SFA.DAS.EmployerCommitments.Application.Validation;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;

namespace SFA.DAS.EmployerCommitments.Application.Commands.AuditCommand
{
    public class CreateAuditCommandHandler : AsyncRequestHandler<CreateAuditCommand>
    {
        private readonly IAuditService _auditService;
        private readonly IValidator<CreateAuditCommand> _validator;

        public CreateAuditCommandHandler(IAuditService auditService, IValidator<CreateAuditCommand> validator)
        {
            _auditService = auditService;
            _validator = validator;
        }

        protected override async Task HandleCore(CreateAuditCommand message)
        {
            var validationResult = _validator.Validate(message);

            if (!validationResult.IsValid())
            {
                throw new InvalidRequestException(validationResult.ValidationDictionary);
            }

            await _auditService.SendAuditMessage(message.EasAuditMessage);
        }
    }
}