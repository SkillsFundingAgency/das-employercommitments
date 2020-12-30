using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Commitments.Api.Types.TrainingProgramme;
using SFA.DAS.EmployerCommitments.Domain.Models.ApprenticeshipCourse;
using SFA.DAS.EmployerCommitments.Application.Extensions;
using SFA.DAS.EmployerCommitments.Domain.Extensions;
using SFA.DAS.EmployerCommitments.Web.Extensions;

namespace SFA.DAS.EmployerCommitments.Web.ViewModels
{
    public class ApprenticeshipListItemGroupViewModel
    {
        public TrainingProgramme TrainingProgramme { get; }
        public IList<ApprenticeshipListItemViewModel> Apprenticeships { get; }

        public int ApprenticeshipsOverFundingLimit { get; }
        public int? CommonFundingCap { get; }

        private bool AllApprenticeshipsOverFundingLimit =>
            Apprenticeships.Any() && ApprenticeshipsOverFundingLimit == Apprenticeships.Count;

        public bool ShowCommonFundingCap => AllApprenticeshipsOverFundingLimit && CommonFundingCap != null;

        public string GroupId => TrainingProgramme?.CourseCode ?? "0";

        public string GroupName => TrainingProgramme?.Name ?? "No training course";

        public int OverlapErrorCount => Apprenticeships.Count(x => x.OverlappingApprenticeships.Any());

        public bool ShowOverlapError => Apprenticeships.SelectMany(m => m.OverlappingApprenticeships).Any();

        /// <remarks>
        /// ApprenticeshipsOverFundingLimit and CommonFundingCap are only guaraneteed to be correct if the ctor's params are not mutated after instantiation or on another thread during contruction
        /// </remarks>
        public ApprenticeshipListItemGroupViewModel(IList<ApprenticeshipListItemViewModel> apprenticeships, TrainingProgramme trainingProgramme = null)
        {
            TrainingProgramme = trainingProgramme;
            Apprenticeships = apprenticeships;

            // calculating up-front assumes apprenticeships list and training program are not mutated after being passed to ctor
            ApprenticeshipsOverFundingLimit = CalculateApprenticeshipsOverFundingLimit();
            CommonFundingCap = CalculateCommonFundingCap();
        }

        /// <remarks>
        /// if the training program is not effective on the start date, the user will get a validation message when creating the apprenticeship
        /// (e.g. This training course is only available to apprentices with a start date after 04 2018)
        /// so we shouldn't see FundingCapOn returning 0 (when the start date is outside of a funding cap)
        /// but if we see it, we treat the apprenticeship as *not* over the funding limit
        /// </remarks>
        private int CalculateApprenticeshipsOverFundingLimit()
        {
            if (TrainingProgramme == null)
                return 0;

            return Apprenticeships.Count(x => x.IsOverFundingLimit(TrainingProgramme));
        }

        /// <summary>
        /// If all apprenticeships share the same Funding Cap, this is it.
        /// If they have different funding caps, or there is no trainingprogram or apprenticeships,
        /// or there is not enough data to calculate the funding cap for each apprenticeship, this is null
        /// </summary>
        private int? CalculateCommonFundingCap()
        {
            if (TrainingProgramme == null || !Apprenticeships.Any())
                return null;

            if (Apprenticeships.Any(a => !a.StartDate.HasValue))
                return null;

            var firstFundingCap = TrainingProgramme.FundingCapOn(Apprenticeships.First().StartDate.Value);

            // check for magic 0, which means unable to calculate a funding cap (e.g. date out of bounds)
            if (firstFundingCap == 0)
                return null;

            if (Apprenticeships.Skip(1).Any(a => TrainingProgramme.FundingCapOn(a.StartDate.Value) != firstFundingCap))
                return null;

            return firstFundingCap;
        }
    }
}