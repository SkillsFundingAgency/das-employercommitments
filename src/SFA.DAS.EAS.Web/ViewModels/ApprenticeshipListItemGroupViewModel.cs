﻿using System;
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

        //todo: change to property that gets set, or else cache result, or calc in ctor, or have shared cache miss method to populate next 2 props
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
                //todo: need to find out what to do when the user hasn't provided a start date yet
                return Apprenticeships.Count(x => x.Cost > TrainingProgramme.FundingCapOn(x.StartDate ?? DateTime.UtcNow));
            }
        }

        // if all apprenticeships share the same Funding Cap, this is it. If they have different funding caps, or there is no trainingprogram or apprenticeships, this is null
        //public int? CommonFundingCap { get; set; }
        public int? CommonFundingCap
        {
            //todo: view checks if the Apprenticeship.Cost has value. do we need to do tbat too, or is the view over cautious?
            get
            {
                if (TrainingProgramme == null || Apprenticeships.Count == 0)
                    return null;

                //todo: need to find out what to do when the user hasn't provided a start date yet
                var firstFundingCap = TrainingProgramme.FundingCapOn(Apprenticeships.First().StartDate.Value);

                if (Apprenticeships.Skip(1).Any(a => TrainingProgramme.FundingCapOn(a.StartDate.Value) != firstFundingCap))
                    return null;

                return firstFundingCap;
            }
        }

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