using SFA.DAS.EmployerCommitments.Domain.Models.ReferenceData;

namespace SFA.DAS.EmployerCommitments.Web.ViewModels.Organisation
{
    public class PublicSectorOrganisationSearchResultsViewModel: ViewModelBase
    {
        public string HashedAccountId { get; set; }

        public string SearchTerm { get; set; }
        public PagedResponse<OrganisationDetailsViewModel> Results { get; set; }
    }
}