using System.Collections.Generic;
using SFA.DAS.EmployerCommitments.Domain.Models.EmployerAgreement;

namespace SFA.DAS.EmployerCommitments.Web.ViewModels.Organisation
{
    public class LegalAgreementsToRemoveViewModel : ViewModelBase
    {
        public List<RemoveEmployerAgreementView> Agreements { get; set; }
    }
}