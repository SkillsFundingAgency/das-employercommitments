using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;
using SFA.DAS.EmployerCommitments.Domain.Models.ApprenticeshipCourse;

namespace SFA.DAS.EmployerCommitments.Application.Queries.GetTrainingProgrammes
{
    public sealed class GetTrainingProgrammesQueryHandler : IAsyncRequestHandler<GetTrainingProgrammesQueryRequest, GetTrainingProgrammesQueryResponse>
    {
        private readonly IApprenticeshipInfoServiceWrapper _apprenticeshipInfoServiceWrapper;

        public GetTrainingProgrammesQueryHandler(IApprenticeshipInfoServiceWrapper apprenticeshipInfoServiceWrapper)
        {
            _apprenticeshipInfoServiceWrapper = apprenticeshipInfoServiceWrapper;
        }

        public async Task<GetTrainingProgrammesQueryResponse> Handle(GetTrainingProgrammesQueryRequest message)
        {
            IEnumerable<ITrainingProgramme> programmes;
            var standardsTask = _apprenticeshipInfoServiceWrapper.GetStandardsAsync();
            if (!message.IncludeFrameworks)
            {
                programmes = (await standardsTask).Standards;
            }
            else
            {
                var getFrameworksTask = _apprenticeshipInfoServiceWrapper.GetFrameworksAsync();
                programmes = (await standardsTask).Standards.Union((await getFrameworksTask).Frameworks.Cast<ITrainingProgramme>());
            }

            return new GetTrainingProgrammesQueryResponse
            {
                TrainingProgrammes = programmes.OrderBy(m => m.Title).ToList()
            };
        }
    }
}