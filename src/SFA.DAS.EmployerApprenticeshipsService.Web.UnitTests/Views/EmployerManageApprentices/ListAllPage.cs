using System.Collections.Generic;
using NUnit.Framework;
using SFA.DAS.EmployerCommitments.Web.ViewModels.ManageApprenticeships;
using SFA.DAS.EmployerCommitments.Web.Views.EmployerManageApprentices;

namespace SFA.DAS.EmployerCommitments.Web.UnitTests.Views.EmployerManageApprentices
{
    [TestFixture]
    public class ListAllPage : OrchestratedViewModelTestingBase<ManageApprenticeshipsViewModel, ListAll>
    {
        private const string Uln = "IAMAULN";

        [Test]
        public void ShouldDisplayUln()
        {
            var model = new ManageApprenticeshipsViewModel
            {
                PageNumber = 1,
                PageSize = 1,
                TotalPages = 1,
                TotalResults = 1,
                TotalApprenticeshipsBeforeFilter = 1,
                Filters = new ApprenticeshipFiltersViewModel
                {
                    ApprenticeshipStatusOptions = new List<KeyValuePair<string, string>>(),
                    Course = new List<string>(),
                    Provider = new List<string>(),
                    ProviderOrganisationOptions = new List<KeyValuePair<string, string>>(),
                    RecordStatus = new List<string>(),
                    RecordStatusOptions = new List<KeyValuePair<string, string>>(),
                    Status = new List<string>(),
                    TrainingCourseOptions = new List<KeyValuePair<string, string>>(),
                    PageNumber = 1
                },
                Apprenticeships = new List<ApprenticeshipDetailsViewModel>
                {
                    new ApprenticeshipDetailsViewModel
                    {
                        Alerts = new List<string>(),
                        ULN = Uln
                    }
                }
            };
            
            AssertParsedContent(model, "#apprenticeUln", Uln);
        }
    }
}