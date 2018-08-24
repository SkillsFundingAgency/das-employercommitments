using System;
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
                //                return TrainingProgramme == null ? 0 : Apprenticeships.Count(x => x.Cost > TrainingProgramme.MaxFunding);
                //return TrainingProgramme == null ? 0 : Apprenticeships.Count(x => x.Cost > TrainingProgramme.FundingCapOn(x.StartDate.Value));
                if (TrainingProgramme == null)
                    return 0;

                //todo: should we default back to MaxFunding, or startdate today, or something else?
                //return Apprenticeships.Count(x => x.Cost > (x.StartDate.HasValue ? TrainingProgramme.FundingCapOn(x.StartDate.Value) : TrainingProgramme.MaxFunding));
                //return Apprenticeships.Count(x => x.Cost > (TrainingProgramme.FundingCapOn(x.StartDate ?? DateTime.UtcNow)));
                // this is calculated at approval time, so we should always have a StartDate
                //todo: if the user hasn't entered a startdate yet, then we should not show any error relating to band cap for that apprenticeship (we should still show errors for those with a startdate)
                return Apprenticeships.Count(x => x.StartDate.HasValue && x.Cost.HasValue && x.Cost > TrainingProgramme.FundingCapOn(x.StartDate.Value));
                //OverFundingLimit extension on apprenticeship?
            }
        }

        // if all apprenticeships share the same Funding Cap, this is it.
        // if they have different funding caps, or there is no trainingprogram or apprenticeships, or there is not enough data to calculate it for each apprenticeship, this is null
        //public int? CommonFundingCap { get; set; }
        public int? CommonFundingCap
        {
            //todo: view checks if the Apprenticeship.Cost has value. do we need to do tbat too, or is the view over cautious?
            get
            {
                if (TrainingProgramme == null || Apprenticeships.Count == 0)
                    return null;

                if (Apprenticeships.Any(a => !a.Cost.HasValue || !a.StartDate.HasValue))
                    return null;

                //todo: need to find out what to do when the user hasn't provided a start date yet
                //var firstApprenticeshipsStartDate = Apprenticeships.First().StartDate;
                //if (!firstApprenticeshipsStartDate.HasValue)
                //    return null;

                var firstFundingCap = TrainingProgramme.FundingCapOn(Apprenticeships.First().StartDate.Value);

                //todo: do we check for missing cost here, or handle that seperately?
                if (Apprenticeships.Skip(1).Any(a => TrainingProgramme.FundingCapOn(a.StartDate.Value) != firstFundingCap))
                    return null;

                return firstFundingCap;
            }
        }

        //public bool AllApprenticeshipsHaveCost
        //{
        //    get { return Apprenticeships.All(a => a.Cost.HasValue); }
        //}

        //public bool ShowCommonFundingCap
        //{
        //    get { return Apprenticeships.All(a => a.Cost.HasValue && a.StartDate.HasValue); }
        //}

        //public bool ShowFundingLimitWarning
        //{
        //    get
        //    {
        //        return TrainingProgramme != null && Apprenticeships.Any(x => x.Cost > TrainingProgramme.MaxFunding);
        //    }
        //}

        public int OverlapErrorCount => Apprenticeships.Count(x=> x.OverlappingApprenticeships.Any());

        public bool ShowOverlapError => Apprenticeships.SelectMany(m => m.OverlappingApprenticeships).Any();
    }
}