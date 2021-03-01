using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Commitments.Api.Types.TrainingProgramme;
using SFA.DAS.EmployerCommitments.Application.Extensions;
using SFA.DAS.EmployerCommitments.Domain.Extensions;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;
using SFA.DAS.EmployerCommitments.Domain.Models.ApprenticeshipCourse;

namespace SFA.DAS.EmployerCommitments.Application.Queries.GetTrainingProgrammes
{
    public sealed class GetTrainingProgrammesQueryHandler : IAsyncRequestHandler<GetTrainingProgrammesQueryRequest, GetTrainingProgrammesQueryResponse>
    {
        private readonly IApprenticeshipInfoService _apprenticeshipInfoService;

        public GetTrainingProgrammesQueryHandler(IApprenticeshipInfoService apprenticeshipInfoService)
        {
            _apprenticeshipInfoService = apprenticeshipInfoService;
        }

        public async Task<GetTrainingProgrammesQueryResponse> Handle(GetTrainingProgrammesQueryRequest message)
        {
            IEnumerable<TrainingProgramme> programmes;
            
            if (!message.IncludeFrameworks)
            {
                programmes = (await _apprenticeshipInfoService.GetStandards()).Standards;
            }
            else
            {
                programmes = (await _apprenticeshipInfoService.GetAll()).TrainingProgrammes;
            }

            var result = new GetTrainingProgrammesQueryResponse();

            if (!message.EffectiveDate.HasValue)
            {
                result.TrainingProgrammes = programmes.OrderBy(m => m.Name).ToList();
            }
            else
            {
                result.TrainingProgrammes = programmes
                    .Where(x => x.IsActiveOn(message.EffectiveDate.Value))
                    .OrderBy(m => m.Name)
                    .ToList();
            }

            return result;
        }
    }
}