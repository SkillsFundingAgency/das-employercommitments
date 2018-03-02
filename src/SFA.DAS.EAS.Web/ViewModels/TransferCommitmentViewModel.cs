using System.Collections.Generic;

namespace SFA.DAS.EmployerCommitments.Web.ViewModels
{
    public sealed class TransferCommitmentViewModel
    {
        public TransferCommitmentViewModel()
        {
            TrainingList = new List<TransferApprenticeshipSumaryViewModel>();
        }

        public string HashedTransferSenderAccountId { get; set; }
        public string HashedTransferReceiverAccountId { get; set; }
        public string LegalEntityName { get; set; }
        public string HashedCohortReference { get; set; }
        public decimal TotalCost { get; set; }
        public List<TransferApprenticeshipSumaryViewModel> TrainingList { get; set; }
    }
}