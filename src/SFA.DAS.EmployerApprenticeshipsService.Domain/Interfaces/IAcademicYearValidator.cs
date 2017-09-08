using System;

using SFA.DAS.EmployerCommitments.Domain.Models.Apprenticeship;

namespace SFA.DAS.EmployerCommitments.Domain.Interfaces
{
    public interface IAcademicYearValidator
    {
        AcademicYearValidationResult Validate(DateTime startDate);
    }
}
