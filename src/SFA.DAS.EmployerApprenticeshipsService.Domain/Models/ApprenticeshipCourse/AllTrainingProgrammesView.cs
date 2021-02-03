using System;
using System.Collections.Generic;
using SFA.DAS.Commitments.Api.Types.TrainingProgramme;

namespace SFA.DAS.EmployerCommitments.Domain.Models.ApprenticeshipCourse
{
    public class AllTrainingProgrammesView
    {
        public DateTime CreatedDate { get; set; }
        public List<TrainingProgramme> TrainingProgrammes { get; set; }
    }
}