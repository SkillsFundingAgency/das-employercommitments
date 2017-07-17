using System.Collections.Generic;
using SFA.DAS.EmployerCommitments.Domain.Models.ApprenticeshipCourse;

namespace SFA.DAS.EmployerCommitments.Application.Queries.GetTrainingProgrammes
{
    public sealed class GetTrainingProgrammesQueryResponse
    {
        public List<ITrainingProgramme> TrainingProgrammes { get; set; }
    }
}