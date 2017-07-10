using System.Collections.Generic;

namespace SFA.DAS.EmployerCommitments.Web.ViewModels.ManageApprenticeships
{
    public class ManageApprenticeshipsViewModel
    {
        public IEnumerable<ApprenticeshipDetailsViewModel> Apprenticeships { get; set; }

        public ApprenticeshipFiltersViewModel Filters { get; set; }

        public string HashedAccountId { get; set; }
        public int TotalApprenticeships { get; set; }
    }
}