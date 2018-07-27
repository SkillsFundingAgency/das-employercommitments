﻿using System.Collections.Generic;
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
            var standardsTask = _apprenticeshipInfoServiceWrapper.GetStandardsAsync();
            var frameworksTask = message.IncludeFrameworks
                ? _apprenticeshipInfoServiceWrapper.GetFrameworksAsync()
                : Task.FromResult(new FrameworksView{ Frameworks = new List<EmployerCommitments.Domain.Models.ApprenticeshipCourse.Framework>() });

            await Task.WhenAll(standardsTask, frameworksTask);

            var programmes = standardsTask.Result.Standards
                .Union(frameworksTask.Result.Frameworks.Cast<ITrainingProgramme>())
                .OrderBy(m => m.Title)
                .ToList();

            return new GetTrainingProgrammesQueryResponse
            {
                TrainingProgrammes = programmes
            };
        }
    }
}