using System;

namespace SFA.DAS.EmployerCommitments.Domain.Interfaces
{
    public interface IAcademicYear
    {
        DateTime StartDate { get; }
        DateTime EndDate { get; }
        DateTime FundingPeriodCloseDate { get; }
    }
}
