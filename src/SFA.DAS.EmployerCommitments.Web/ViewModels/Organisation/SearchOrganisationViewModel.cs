using SFA.DAS.EmployerCommitments.Domain.Models.Organisation;
using SFA.DAS.EmployerCommitments.Domain.Models.ReferenceData;

namespace SFA.DAS.EmployerCommitments.Web.ViewModels.Organisation
{
    public class SearchOrganisationViewModel
    {
        public string SearchTerm { get; set; }
        public OrganisationType? OrganisationType { get; set; }
        public PagedResponse<OrganisationDetailsViewModel> Results { get; set; }
    }
}