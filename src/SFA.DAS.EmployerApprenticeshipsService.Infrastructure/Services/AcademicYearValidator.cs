using System;

using SFA.DAS.EmployerCommitments.Domain.Interfaces;

namespace SFA.DAS.EmployerCommitments.Infrastructure.Services
{
    public class AcademicYearValidator : IAcademicYearValidator
    {
        public readonly ICurrentDateTime _currentDateTime;

        public AcademicYearValidator(ICurrentDateTime currentDateTime)
        {
            _currentDateTime = currentDateTime;
        }

        public bool FundingPeriodOpen(DateTime startDate)
        {
            IAcademicYear academicYear = new AcademicYear(startDate);
            var submissionClosed = _currentDateTime.Now > academicYear.FundingPeriodCloseDate.AddDays(-1);

            return !submissionClosed;
        }
    }
}
