using System;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using MediatR;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.Commitment.Types;
using SFA.DAS.EmployerCommitments.Application.Exceptions;
using SFA.DAS.EmployerCommitments.Application.Validation;

namespace SFA.DAS.EmployerCommitments.Application.Commands.UpdateApprenticeshipRedundancy
{
    public sealed class UpdateApprenticeshipRedundancyCommandHandler : AsyncRequestHandler<UpdateApprenticeshipRedundancyCommand>
    {
        private readonly IEmployerCommitmentApi _commitmentsApi;
        private readonly IValidator<UpdateApprenticeshipRedundancyCommand> _validator;

        public UpdateApprenticeshipRedundancyCommandHandler(IEmployerCommitmentApi commitmentsApi,
            IValidator<UpdateApprenticeshipRedundancyCommand> validator)
        {
            _commitmentsApi = commitmentsApi;
            _validator = validator;
        }

        protected override async Task HandleCore(UpdateApprenticeshipRedundancyCommand command)
        {
            var validationResult = _validator.Validate(command);

            if (!validationResult.IsValid())
                throw new InvalidRequestException(validationResult.ValidationDictionary);

            var apprenticeshipSubmission = new ApprenticeshipSubmission
            {
                MadeRedundant = command.MadeRedundant,
                UserId = command.UserId,
                LastUpdatedByInfo = new LastUpdateInfo { EmailAddress = command.UserEmailAddress, Name = command.UserDisplayName }
            };

            try
            {
                var json = new JavaScriptSerializer().Serialize(apprenticeshipSubmission);
                await _commitmentsApi.PatchEmployerApprenticeship(command.EmployerAccountId, command.ApprenticeshipId, apprenticeshipSubmission);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
           
        }
    }
}
