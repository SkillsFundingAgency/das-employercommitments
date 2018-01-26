using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.EmployerCommitments.Application.Validation;

namespace SFA.DAS.EmployerCommitments.Application.Commands.UpdateApprenticeshipStopDate
{
    public sealed class UpdateApprenticeshipStopDateCommandHandler : AsyncRequestHandler<UpdateApprenticeshipStopDateCommand>
    {
        private readonly IEmployerCommitmentApi _commitmentsApi;
        private readonly IValidator<UpdateApprenticeshipStopDateCommand> _validator;

        public UpdateApprenticeshipStopDateCommandHandler(IEmployerCommitmentApi commitmentsApi, IValidator<UpdateApprenticeshipStopDateCommand> validator, IMediator mediator)
        {
            _commitmentsApi = commitmentsApi;
            _validator = validator;
        }

        protected override async Task HandleCore(UpdateApprenticeshipStopDateCommand command)
        {
            var validationResult = _validator.Validate(command);

            if (!validationResult.IsValid())
                throw new InvalidRequestException(validationResult.ValidationDictionary);

            var apprenticeshipSubmission = new ApprenticeshipSubmission
            {
                UserId = command.UserId,
            };

            await _commitmentsApi.PatchEmployerApprenticeship(command.EmployerAccountId, command.ApprenticeshipId, apprenticeshipSubmission);
        }
    }
}
