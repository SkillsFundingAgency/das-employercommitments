using System;

namespace SFA.DAS.EmployerCommitments.Domain.Models.Organisation
{
    public class Agreement
    {
        public long Id { get; set; }
        public DateTime? SignedDate { get; set; }
        public string SignedByName { get; set; }
        public EmployerAgreementStatus Status { get; set; }
        public int TemplateVersionNumber { get; set; }
    }
}
