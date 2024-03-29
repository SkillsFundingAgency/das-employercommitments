﻿using System.ComponentModel.DataAnnotations;
using SFA.DAS.EmployerCommitments.Domain.Models.ApprenticeshipProvider;

namespace SFA.DAS.EmployerCommitments.Web.ViewModels
{
    public class ConfirmProviderViewModel
    {
        public string CohortRef { get; set; }
        public string HashedAccountId { get; set; }
        public string TransferConnectionCode { get; set; }
        public string LegalEntityCode { get; set; }
        public string LegalEntityName { get; set; }
        public int ProviderId { get; set; }
        public string ProviderName { get; set; }
        public Provider Provider { get; set; }

        [Required(ErrorMessage = "Select a training provider")]
        public bool? Confirmation { get; set; }
    }
}