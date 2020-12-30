using System;
using System.Collections.Generic;
using SFA.DAS.Commitments.Api.Types.TrainingProgramme;

namespace SFA.DAS.EmployerCommitments.Domain.Models.ApprenticeshipCourse
{
    public class StandardsView
    {
        public DateTime CreationDate { get; set; }
        public List<TrainingProgramme> Standards { get; set; }
    }
}