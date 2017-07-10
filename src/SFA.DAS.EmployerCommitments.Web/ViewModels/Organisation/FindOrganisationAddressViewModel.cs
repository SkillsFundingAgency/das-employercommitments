﻿namespace SFA.DAS.EmployerCommitments.Web.ViewModels.Organisation
{
    public class FindOrganisationAddressViewModel : OrganisationViewModelBase
    {
        public string Postcode { get; set; }
      
        public string PostcodeError => GetErrorMessage(nameof(Postcode));
    }
}