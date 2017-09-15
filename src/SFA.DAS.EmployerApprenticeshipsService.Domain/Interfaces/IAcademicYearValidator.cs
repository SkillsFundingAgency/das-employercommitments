﻿using SFA.DAS.EmployerCommitments.Domain.Models.AcademicYear;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerCommitments.Domain.Interfaces
{
    public interface IAcademicYearValidator
    {
        AcademicYearValidationResult Validate(DateTime trainingStartDate);
    }
}