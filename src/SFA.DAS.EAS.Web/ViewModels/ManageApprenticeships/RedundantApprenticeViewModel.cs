using System;
using SFA.DAS.EmployerCommitments.Web.Authentication;

namespace SFA.DAS.EmployerCommitments.Web.ViewModels.ManageApprenticeships
{
    public class RedundantApprenticeViewModel
    {
        public bool? RedundancyConfirm { get; set; }
        public string ApprenticeshipName { get; set; }
        public ChangeStatusType? ChangeType { get; set; }
        public DateTime? DateOfChange { get; set; }
        public string HashedAccountId { get; set; }
        public string HashedApprenticeshipId { get; set; }
    }
}