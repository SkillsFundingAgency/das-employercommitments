using System;

using SFA.DAS.EmployerCommitments.Domain.Interfaces;
using SFA.DAS.EmployerCommitments.Domain.Models.Apprenticeship;

namespace SFA.DAS.EmployerCommitments.Infrastructure.Services
{
    public class AcademicYearValidator : IAcademicYearValidator
    {
        public readonly ICurrentDateTime _currentDateTime;
        public readonly IAcademicYear _academicYear;

        public AcademicYearValidator(ICurrentDateTime currentDateTime, IAcademicYear academicYear)
        {
            _currentDateTime = currentDateTime;
            _academicYear = academicYear;
        }

        public AcademicYearValidationResult Validate(DateTime startDate)
        {
            if (startDate < _academicYear.CurrentAcademicYearStartDate
                && _currentDateTime.Now > _academicYear.CurrentAcademicYearFundingPeriod)
            {
                return AcademicYearValidationResult.NotWithinFundingPeriod;
            }

            return AcademicYearValidationResult.Success;
        }
    }
}
