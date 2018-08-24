using System.Collections.Generic;
using System.Linq;
using SFA.DAS.EmployerCommitments.Domain.Models.ApprenticeshipCourse;
using SFA.DAS.EmployerCommitments.Application.Extensions;

namespace SFA.DAS.EmployerCommitments.Web.ViewModels
{
    public class ApprenticeshipListItemGroupViewModel
    {
        public ITrainingProgramme TrainingProgramme { get; set; }

        public IList<ApprenticeshipListItemViewModel> Apprenticeships { get; set; }

        public string GroupId => TrainingProgramme == null ? "0" : TrainingProgramme.Id;

        public string GroupName => TrainingProgramme == null ? "No training course" : TrainingProgramme.Title;

        //todo: change to property that gets set, or else cache result, or calc in ctor, or have shared cache miss method to populate next 2 props. just be careful cache doesn't get out of date.
        public int ApprenticeshipsOverFundingLimit
        {
            get
            {
                // if the user hasn't entered a startdate, cost and training programme yet, we don't show any error relating to band cap for that apprenticeship (we should still show errors for those with a startdate)
                if (TrainingProgramme == null)
                    return 0;

                return Apprenticeships.Count(x => x.StartDate.HasValue && x.Cost.HasValue && x.Cost > TrainingProgramme.FundingCapOn(x.StartDate.Value));
                //OverFundingLimit extension on apprenticeship?
            }
        }

        /// <summary>
        /// If all apprenticeships share the same Funding Cap, this is it.
        /// If they have different funding caps, or there is no trainingprogram or apprenticeships,
        /// or there is not enough data to calculate the funding cap for each apprenticeship, this is null
        /// </summary>
        public int? CommonFundingCap
        {
            get
            {
                if (TrainingProgramme == null || Apprenticeships.Count == 0)
                    return null;

                if (Apprenticeships.Any(a => !a.Cost.HasValue || !a.StartDate.HasValue))
                    return null;

                var firstFundingCap = TrainingProgramme.FundingCapOn(Apprenticeships.First().StartDate.Value);

                if (Apprenticeships.Skip(1).Any(a => TrainingProgramme.FundingCapOn(a.StartDate.Value) != firstFundingCap))
                    return null;

                return firstFundingCap;
            }
        }

        public int OverlapErrorCount => Apprenticeships.Count(x=> x.OverlappingApprenticeships.Any());

        public bool ShowOverlapError => Apprenticeships.SelectMany(m => m.OverlappingApprenticeships).Any();
    }
}