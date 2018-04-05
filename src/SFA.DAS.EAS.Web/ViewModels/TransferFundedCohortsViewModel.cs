using System.Collections.Generic;

namespace SFA.DAS.EmployerCommitments.Web.ViewModels
{
    public sealed class TransferFundedCohortsViewModel
    {
        public IEnumerable<TransferFundedCohortsListItemViewModel> Commitments { get; set; }
    }
}