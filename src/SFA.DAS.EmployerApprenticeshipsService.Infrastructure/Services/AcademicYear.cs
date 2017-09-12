using System;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;

namespace SFA.DAS.EmployerCommitments.Infrastructure.Services
{
    public class AcademicYear : IAcademicYear
    {
        private readonly ICurrentDateTime _currentDateTime;

        public AcademicYear(ICurrentDateTime currentDateTime)
        {
            _currentDateTime = currentDateTime;
        }

        public AcademicYear(DateTime start)
        {
            _currentDateTime = new CurrentDateTime(start);
        }

        public DateTime StartDate
        {
            get
            {
                var now = _currentDateTime.Now;

                var cutoff = new DateTime(now.Year, 8, 1);

                return now >= cutoff ? cutoff : new DateTime(now.Year - 1, 8, 1);
            }
        }

        public DateTime EndDate => StartDate.AddYears(1).AddDays(-1);

        public DateTime FundingPeriodCloseDate
        {
            get
            {
                // TODO GET DATE FROM SOURCE
                return EndDate.AddDays(80);
            }
        }
    }
}
