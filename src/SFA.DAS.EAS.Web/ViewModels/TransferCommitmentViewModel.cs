using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SFA.DAS.Commitments.Api.Types;

namespace SFA.DAS.EmployerCommitments.Web.ViewModels
{
    public sealed class TransferCommitmentViewModel
    {
        public TransferCommitmentViewModel()
        {
            TrainingList = new List<TransferCourseSummaryViewModel>();
        }

        public string HashedTransferSenderAccountId { get; set; }
        public string HashedTransferReceiverAccountId { get; set; }
        public string LegalEntityName { get; set; }
        public string HashedCohortReference { get; set; }
        public decimal TotalCost { get; set; }
        public List<TransferCourseSummaryViewModel> TrainingList { get; set; }
        public string TransferApprovalStatusDesc { get; set; }
        public TransferApprovalStatus TransferApprovalStatus { get; set; }
        public string TransferApprovalSetBy { get; set; }
        public DateTime? TransferApprovalSetOn { get; set; }
        public bool PendingApproval => TransferApprovalStatus == TransferApprovalStatus.Pending;
        public bool EnableRejection { get; set; }

    }
}