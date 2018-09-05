using System;
using System.Linq;
using SFA.DAS.EmployerCommitments.Domain.Models.ApprenticeshipCourse;

namespace SFA.DAS.EmployerCommitments.Application.Extensions
{
    public static class ITrainingProgrammeExtensions
    {
        public static bool IsActiveOn(this ITrainingProgramme course, DateTime effectiveDate)
        {
            return DateWithinRange(course.EffectiveFrom, course.EffectiveTo, effectiveDate);
        }

        public static TrainingProgrammeStatus GetStatusOn(this ITrainingProgramme course, DateTime effectiveDate)
        {
            var dateOnly = effectiveDate.Date;

            if (course.EffectiveFrom.HasValue && course.EffectiveFrom.Value.FirstOfMonth() > dateOnly)
                return TrainingProgrammeStatus.Pending;

            if(!course.EffectiveTo.HasValue || course.EffectiveTo.Value >= dateOnly)
                return TrainingProgrammeStatus.Active;

            return TrainingProgrammeStatus.Expired;

        }

        public static int FundingCapOn(this ITrainingProgramme course, DateTime effectiveDate)
        {
            if (!course.IsActiveOn(effectiveDate))
                return 0;

            var applicableFundingPeriod = course.FundingPeriods.FirstOrDefault(x => DateWithinRange(x.EffectiveFrom, x.EffectiveTo, effectiveDate));

            return applicableFundingPeriod?.FundingCap ?? 0;
        }

        /// <remarks>
        /// we make use of the same logic to determinge ActiveOn and FundingBandOn so that if the programme is active, it should fall within a funding band
        /// </remarks>
        private static bool DateWithinRange(DateTime? effectiveFrom, DateTime? effectiveTo, DateTime date)
        {
            //todo: still have 2 sources of truth (this and GetStatusOn) need only 1, e.g. return GetStatusOn == Active here in effect
            var dateOnly = date.Date;
            return (!effectiveFrom.HasValue || effectiveFrom.Value.FirstOfMonth() <= dateOnly) &&
                   (!effectiveTo.HasValue || effectiveTo.Value >= dateOnly);
        }
    }
}
