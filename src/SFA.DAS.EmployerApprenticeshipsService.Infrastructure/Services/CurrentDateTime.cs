using System;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;

namespace SFA.DAS.EmployerCommitments.Infrastructure.Services
{
    public sealed class CurrentDateTime : ICurrentDateTime
    {
        public DateTime Now { get; }

        public CurrentDateTime()
        {
            Now = DateTime.UtcNow;
        }

        public CurrentDateTime(DateTime time)
        {
            Now = time;
        }
    }
}
