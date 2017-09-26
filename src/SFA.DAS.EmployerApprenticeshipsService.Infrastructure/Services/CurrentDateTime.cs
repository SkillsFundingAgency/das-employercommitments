using System;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;

namespace SFA.DAS.EmployerCommitments.Infrastructure.Services
{
    public sealed class CurrentDateTime : ICurrentDateTime
    {
        private readonly DateTime? _time;

        public DateTime Now => _time ?? DateTime.UtcNow;

        public CurrentDateTime()
        {
        }

        public CurrentDateTime(DateTime? time)
        {
            _time = time;
        }
    }
}
