using System.Collections.Generic;
using SFA.DAS.EmployerCommitments.Domain.Models.EmployerAgreement;

namespace SFA.DAS.EmployerCommitments.Web.ViewModels
{
    public class EmployerAgreementListViewModel
    {
        public long AccountId { get; set; }
        public List<EmployerAgreementView> EmployerAgreements { get; set; }
        public string HashedAccountId { get; set; }
    }
}