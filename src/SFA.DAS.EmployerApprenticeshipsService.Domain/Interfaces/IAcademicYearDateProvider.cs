﻿using System;

namespace SFA.DAS.EmployerCommitments.Domain.Interfaces
{
    public interface IAcademicYearDateProvider
    {
        DateTime CurrentAcademicYearStartDate { get; }
        DateTime CurrentAcademicYearEndDate { get; }
        DateTime LastAcademicYearFundingPeriod { get; }
    }
}
