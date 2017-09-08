using System;

namespace SFA.DAS.EmployerCommitments.Domain.Interfaces
{
    public interface IAcademicYearValidator
    {
        bool FundingPeriodOpen(DateTime startDate);
    }
}
