using System;
using SFA.DAS.EmployerCommitments.Application.Validation;

namespace SFA.DAS.EmployerCommitments.Application.Queries.ValidateStatusChangeDate
{
    public sealed class ValidateStatusChangeDateQueryResponse
    {
        public ValidationResult ValidationResult { get; set; }
        public DateTime ValidatedChangeOfDate { get; internal set; }
    }
}
