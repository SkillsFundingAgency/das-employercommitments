using System;

namespace SFA.DAS.EmployerCommitments.Domain.Interfaces
{
    public interface IAcademicYear
    {
        DateTime CurrentAcademicYearStartDate { get; }
        DateTime CurrentAcademicYearEndDate { get; }
    }
}
