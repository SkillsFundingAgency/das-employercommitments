using System.Collections.Generic;
using SFA.DAS.EmployerCommitments.Web.ViewModels.Interfaces;

namespace SFA.DAS.EmployerCommitments.Web.ViewModels.ManageApprenticeships
{
    public class ManageApprenticeshipsViewModel : IPaginationViewModel
    {
        public IEnumerable<ApprenticeshipDetailsViewModel> Apprenticeships { get; set; }

        public ApprenticeshipFiltersViewModel Filters { get; set; }

        public string HashedAccountId { get; set; }
        public int TotalResults { get; set; }
        public int PageNumber { get; internal set; }
        public int TotalPages { get; internal set; }
        public int PageSize { get; internal set; }

        public int TotalApprenticeshipsBeforeFilter { get; set; }
    }
}