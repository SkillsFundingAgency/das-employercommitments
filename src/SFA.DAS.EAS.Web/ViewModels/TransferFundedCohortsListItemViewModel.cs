using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.EmployerCommitments.Web.Enums;

namespace SFA.DAS.EmployerCommitments.Web.ViewModels
{
    public enum ShowLink
    {
        Details,
        Edit
    }

    public sealed class TransferFundedCohortsListItemViewModel
    {
        public string HashedCommitmentId { get; set; }
        //public string Name { get; set; }
        //public string LegalEntityName { get; set; }
        public string SendingEmployer { get; set; }
        public string ProviderName { get; set; }
        //public RequestStatus Status { get; set; }

        public TransferApprovalStatus TransferApprovalStatus { get; set; }
        //public bool ShowViewLink { get; internal set; }

        public ShowLink ShowLink { get; set; }
    }
}