using System;

namespace SFA.DAS.EmployerCommitments.Domain.Models.EmployerAgreement
{
    public class EmployerAgreementTemplate
    {
        public int Id { get; set; }
        public string PartialViewName { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}