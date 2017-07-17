using System.Collections.Generic;
using SFA.DAS.EmployerCommitments.Domain.Models.ApprenticeshipCourse;

namespace SFA.DAS.EmployerCommitments.Web.ViewModels
{
    public sealed class ExtendedApprenticeshipViewModel
    {
        public ApprenticeshipViewModel Apprenticeship { get; set; }
        public List<ITrainingProgramme> ApprenticeshipProgrammes { get; set; }

        public Dictionary<string, string> ValidationErrors { get; set; }
    }

    public sealed class ApprenticeshipProgramme
    {
        public string Id { get; set; }
        public string Title { get; set; }
    }
}