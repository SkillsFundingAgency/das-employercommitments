using System;

namespace SFA.DAS.EmployerCommitments.Domain.Models.ApprenticeshipCourse
{
    public class FundingPeriod
    {
        public DateTime? EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public int FundingCap { get; set; }
    }
}
