using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerCommitments.Application.Extensions;
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
            IEnumerable<ITrainingProgramme> programmes;
            var standardsTask = _apprenticeshipInfoService.GetStandardsAsync();
            if (!message.IncludeFrameworks)
            {
                programmes = (await standardsTask).Standards;
            }
            else
            {
                var getFrameworksTask = _apprenticeshipInfoService.GetFrameworksAsync();
                programmes = (await standardsTask).Standards.Union((await getFrameworksTask).Frameworks.Cast<ITrainingProgramme>());
            }

            var result = new GetTrainingProgrammesQueryResponse();

            if (!message.EffectiveDate.HasValue)
            {
                result.TrainingProgrammes = programmes.OrderBy(m => m.Title).ToList();
            }
            else
            {
                result.TrainingProgrammes = programmes.Where(x => x.IsActiveOn(message.EffectiveDate.Value))
                    .OrderBy(m => m.Title)
                    .ToList();
            }

            return result;
        }
    }
}