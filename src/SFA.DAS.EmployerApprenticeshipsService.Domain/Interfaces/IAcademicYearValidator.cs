using SFA.DAS.EmployerCommitments.Domain.Models.AcademicYear;
using System;

namespace SFA.DAS.EmployerCommitments.Domain.Interfaces
{
    public interface IAcademicYearValidator
    {
        AcademicYearValidationResult Validate(DateTime trainingStartDate);

        bool IsAfterLastAcademicYearFundingPeriod { get; }
    }
}
