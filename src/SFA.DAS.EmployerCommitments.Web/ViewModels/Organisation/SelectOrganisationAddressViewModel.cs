using System.Collections.Generic;

namespace SFA.DAS.EmployerCommitments.Web.ViewModels.Organisation
{
    public class SelectOrganisationAddressViewModel : OrganisationViewModelBase
    {
        public string Postcode { get; set; }

        public string PostcodeError => GetErrorMessage(nameof(Postcode));

        public ICollection<AddressViewModel> Addresses { get; set; }
    }
}