﻿using System.Collections.Generic;

namespace SFA.DAS.EmployerCommitments.Web.ViewModels
{
    public sealed class TransferCommitmentViewModel
    {
        public TransferCommitmentViewModel()
        {
            TrainingList = new List<TransferApprenticeshipSumaryViewModel>();
        }

        public string HashedAccountId { get; set; }
        public string LegalEntityName { get; set; }
        public string HashedCohortReference { get; set; }
        public decimal TotalCost { get; set; }
        public List<TransferApprenticeshipSumaryViewModel> TrainingList { get; set; }
    }
}