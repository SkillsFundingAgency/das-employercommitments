using System;
using System.Linq;
using SFA.DAS.EmployerCommitments.Domain.Models.ApprenticeshipCourse;

namespace SFA.DAS.EmployerCommitments.Application.Extensions
{
    public static class ITrainingProgrammeExtensions
    {
        public static bool IsActiveOn(this ITrainingProgramme course, DateTime effectiveDate)
        {
            if ((!course.EffectiveFrom.HasValue || course.EffectiveFrom.Value <= effectiveDate) &&
                (!course.EffectiveTo.HasValue || course.EffectiveTo.Value >= effectiveDate)) return true;

            return false;
        }

        public static int FundingCapOn(this ITrainingProgramme course, DateTime effectiveDate)
        {
            if (!course.IsActiveOn(effectiveDate))
            {
                return 0;
            }

            var applicableFundingPeriod = course.FundingPeriods.FirstOrDefault(x =>
                (!x.EffectiveFrom.HasValue || x.EffectiveFrom.Value.Date <= effectiveDate.Date) &&
                (!x.EffectiveTo.HasValue || x.EffectiveTo.Value.Date >= effectiveDate.Date));

            return applicableFundingPeriod?.FundingCap ?? 0;
        }
    }
}
