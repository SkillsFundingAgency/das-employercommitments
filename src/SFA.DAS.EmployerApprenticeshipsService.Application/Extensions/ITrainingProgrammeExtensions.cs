using System;
using System.Linq;
using SFA.DAS.EmployerCommitments.Domain.Models.ApprenticeshipCourse;

namespace SFA.DAS.EmployerCommitments.Application.Extensions
{
    public static class ITrainingProgrammeExtensions
    {
        public static bool IsActiveOn(this ITrainingProgramme course, DateTime effectiveDate)
        {
            return (!course.EffectiveFrom.HasValue || course.EffectiveFrom.Value.FirstOfMonth() <= effectiveDate) &&
                   (!course.EffectiveTo.HasValue || course.EffectiveTo.Value >= effectiveDate.FirstOfMonth());
        }

        public static TrainingProgrammeStatus GetStatusOn(this ITrainingProgramme course, DateTime effectiveDate)
        {
            if ((!course.EffectiveFrom.HasValue || course.EffectiveFrom.Value.FirstOfMonth() <= effectiveDate))
            {
                if(!course.EffectiveTo.HasValue || course.EffectiveTo.Value >= effectiveDate.FirstOfMonth())
                {
                    return TrainingProgrammeStatus.Active;
                }
                return TrainingProgrammeStatus.Expired;
            }

            return TrainingProgrammeStatus.Pending;
        }

        public static int FundingCapOn(this ITrainingProgramme course, DateTime effectiveDate)
        {
            if (!course.IsActiveOn(effectiveDate))
                return 0;

            // first bite at the cherry, we look for a funding band on the exact start date
            //var applicableFundingPeriod = course.FundingPeriods.FirstOrDefault(x =>
            //        (!x.EffectiveFrom.HasValue || x.EffectiveFrom.Value.Date <= effectiveDate.Date) &&
            //        (!x.EffectiveTo.HasValue || x.EffectiveTo.Value.Date >= effectiveDate.Date))
            //    // the second bite we accept the first funding band if it starts in the same month as the start date
            //    ?? course.FundingPeriods.FirstOrDefault(x =>
            //        (!x.EffectiveFrom.HasValue || x.EffectiveFrom.Value.FirstOfMonth() <= effectiveDate) &&
            //        (!x.EffectiveTo.HasValue || x.EffectiveTo.Value.Date >= effectiveDate.Date)); //x.EffectiveTo.Value >= effectiveDate.FirstOfMonth())); //todo: what to do with end date??

            var applicableFundingPeriod = // the second bite we accept the first funding band if it starts in the same month as the start date
                                          course.FundingPeriods.FirstOrDefault(x =>
                                              (!x.EffectiveFrom.HasValue || x.EffectiveFrom.Value.FirstOfMonth() <= effectiveDate) &&
                                              (!x.EffectiveTo.HasValue || x.EffectiveTo.Value.Date >= effectiveDate.Date)); //x.EffectiveTo.Value >= effectiveDate.FirstOfMonth())); //todo: what to do with end date??

            return applicableFundingPeriod?.FundingCap ?? 0;
        }
    }
}
