using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFA.DAS.EmployerCommitments.Web.ViewModels
{
    public sealed class TransferFundedCohortsViewModel
    {
        public IEnumerable<TransferFundedCohortsListItemViewModel> Commitments { get; set; }
    }
}