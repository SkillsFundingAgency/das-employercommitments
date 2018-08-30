using System.Collections.Generic;
using System.Linq;
using SFA.DAS.EmployerCommitments.Domain.Models.ApprenticeshipCourse;
using SFA.DAS.EmployerCommitments.Application.Extensions;

namespace SFA.DAS.EmployerCommitments.Web.ViewModels
{
    public class ApprenticeshipListItemGroupViewModel
    {
        public ITrainingProgramme TrainingProgramme { get; }
        public IList<ApprenticeshipListItemViewModel> Apprenticeships { get; }

        public int ApprenticeshipsOverFundingLimit { get; }
        public int? CommonFundingCap { get; }

        private bool AllApprenticeshipsOverFundingLimit =>
            Apprenticeships.Any() && ApprenticeshipsOverFundingLimit == Apprenticeships.Count;

        public bool ShowCommonFundingCap => AllApprenticeshipsOverFundingLimit && CommonFundingCap != null;

        public string GroupId => TrainingProgramme?.Id ?? "0";

        public string GroupName => TrainingProgramme?.Title ?? "No training course";

        public int OverlapErrorCount => Apprenticeships.Count(x => x.OverlappingApprenticeships.Any());

        public bool ShowOverlapError => Apprenticeships.SelectMany(m => m.OverlappingApprenticeships).Any();

        /// <remarks>
        /// ApprenticeshipsOverFundingLimit and CommonFundingCap are only guaraneteed to be correct if the ctor's params are not mutated after instantiation or on another thread during contruction
        /// </remarks>
        public ApprenticeshipListItemGroupViewModel(IList<ApprenticeshipListItemViewModel> apprenticeships, ITrainingProgramme trainingProgramme = null)
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
            // if the user hasn't entered a startdate, cost and training programme yet, we don't show any error relating to band cap for that apprenticeship (we should still show errors for those with a startdate)
            if (TrainingProgramme == null)
                return 0;

            return Apprenticeships.Count(x =>
            {
                if (!x.StartDate.HasValue)
                    return false;

                var fundingCapAtStartDate = TrainingProgramme.FundingCapOn(x.StartDate.Value);
                return x.Cost.HasValue && fundingCapAtStartDate > 0
                                       && x.Cost > fundingCapAtStartDate;
            });
            //OverFundingLimit extension on apprenticeship?
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