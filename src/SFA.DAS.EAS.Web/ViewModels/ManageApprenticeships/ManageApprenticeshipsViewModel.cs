﻿using System.Collections.Generic;
using SFA.DAS.EmployerCommitments.Web.ViewModels.Interfaces;

namespace SFA.DAS.EmployerCommitments.Web.ViewModels.ManageApprenticeships
{
    public class ManageApprenticeshipsViewModel : IPaginationViewModel
    {
        public IEnumerable<ApprenticeshipDetailsViewModel> Apprenticeships { get; set; }

        public ApprenticeshipFiltersViewModel Filters { get; set; }

        public string HashedAccountId { get; set; }
        public int TotalResults { get; set; }
        public int PageNumber { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }

        public int TotalApprenticeshipsBeforeFilter { get; set; }

        public string SearchInputPlaceholder { get; set; }
    }
}