using System.Collections.Generic;

namespace SFA.DAS.EmployerCommitments.Domain.Models.WhileList
{
    public class UserWhiteListLookUp
    {
        public IEnumerable<string> EmailPatterns { get; set; }
    }
}
