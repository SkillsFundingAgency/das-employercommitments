using System.Collections.Generic;

namespace SFA.DAS.EmployerCommitments.Domain.Models.Notification
{
    public class EmailMessage
    {
        public string TemplateId { get; set; }
        public Dictionary<string, string> Tokens { get; set; }
    }
}
