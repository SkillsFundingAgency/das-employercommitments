using System;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;

namespace SFA.DAS.EmployerCommitments.Application.Extensions
{
    public static class ApprenticeshipExtensions
    {
        public static bool IsWaitingToStart(this Apprenticeship apprenticeship, ICurrentDateTime currentDateTime)
        {
            return apprenticeship.StartDate.Value > new DateTime(currentDateTime.Now.Year, currentDateTime.Now.Month, 1);
        }
    }
}