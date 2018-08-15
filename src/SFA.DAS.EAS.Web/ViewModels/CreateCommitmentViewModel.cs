using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FluentValidation.Attributes;
using SFA.DAS.EmployerCommitments.Domain.Models.Organisation;
using SFA.DAS.EmployerCommitments.Web.Validators;

namespace SFA.DAS.EmployerCommitments.Web.ViewModels
{
    public sealed class SelectLegalEntityViewModel
    {
        public string TransferConnectionCode { get; set; }

        [Required(ErrorMessage = "Choose organisation")]
        public string LegalEntityCode { get; set; }

        public string CohortRef { get; set; }
        
        public IEnumerable<LegalEntity> LegalEntities { get; set; }
    }

    public sealed class SelectTransferConnectionViewModel
    {
        [Required(ErrorMessage = "Choose an option")]
        public string TransferConnectionCode { get; set; }

        public IEnumerable<TransferConnectionViewModel> TransferConnections { get; set; }
    }

    public sealed class TransferConnectionViewModel
    {
        public string TransferConnectionCode { get; set; }
        public string TransferConnectionName { get; set; }
    }


    [Validator(typeof(SelectProviderViewModelValidator))]
    public sealed class SelectProviderViewModel
    {
        public bool NotFound { get; set; } // Set when search yields no result

        public string TransferConnectionCode { get; set; }

        public string LegalEntityCode { get; set; }

        public string ProviderId { get; set; }

        public string CohortRef { get; set; }
    }

    public sealed class CreateCommitmentViewModel
    {
        public string CohortRef { get; set; }

        [Required]
        public string HashedAccountId { get; set; }

        public string TransferConnectionCode { get; set; }

        [Required]
        public string LegalEntityCode { get; set; }

        public string LegalEntityName { get; set; }

        public string LegalEntityAddress { get; set; }

        public short LegalEntitySource { get; set; }

        public string AccountLegalEntityPublicHashedId { get; set; }

        [Required]
        public long ProviderId { get; set; }

        public string ProviderName { get; set; }    

        [Required(ErrorMessage = "Choose an option")]
        public string SelectedRoute { get; set; }
    }
}