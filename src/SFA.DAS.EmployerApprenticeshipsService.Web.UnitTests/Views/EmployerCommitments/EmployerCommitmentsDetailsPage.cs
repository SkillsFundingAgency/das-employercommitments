using System.Collections.Generic;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types.Validation;
using SFA.DAS.EmployerCommitments.Web.ViewModels;
using SFA.DAS.EmployerCommitments.Web.ViewModels.ManageApprenticeships;
using SFA.DAS.EmployerCommitments.Web.Views.EmployerCommitments;

namespace SFA.DAS.EmployerCommitments.Web.UnitTests.Views.EmployerCommitments
{
    [TestFixture]
    public class EmployerCommitmentsDetailsPage : OrchestratedViewModelTestingBase<CommitmentDetailsViewModel, Details>
    {
        private const string Uln = "IAMAULN";

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ShouldDisplayUln()
        {
            var item = new ApprenticeshipListItemViewModel
            {
                ApprenticeUln = Uln,
                CanBeApproved = true,
                OverlappingApprenticeships = new List<OverlappingApprenticeship>()
               };

            var model = new CommitmentDetailsViewModel
            {
                HasApprenticeships = true,
                ApprenticeshipGroups = new List<ApprenticeshipListItemGroupViewModel>
                {
                    new ApprenticeshipListItemGroupViewModel
                    {
                        Apprenticeships = new List<ApprenticeshipListItemViewModel>
                        {
                            item
                        }
                    }
                },
                Errors = new Dictionary<string, string>(),
                Warnings = new Dictionary<string, string>(),
                Apprenticeships = new List<ApprenticeshipListItemViewModel> { item }
            };

            AssertParsedContent(model, "#apprenticeUln", Uln);
        }

        [Test]
        public void ShouldNotDisplayUln()
        {
            var item = new ApprenticeshipListItemViewModel
            {
                ApprenticeUln = string.Empty,
                CanBeApproved = true,
                OverlappingApprenticeships = new List<OverlappingApprenticeship>()
            };

            var model = new CommitmentDetailsViewModel
            {
                HasApprenticeships = true,
                ApprenticeshipGroups = new List<ApprenticeshipListItemGroupViewModel>
                {
                    new ApprenticeshipListItemGroupViewModel
                    {
                        Apprenticeships = new List<ApprenticeshipListItemViewModel>
                        {
                            item
                        }
                    }
                },
                Errors = new Dictionary<string, string>(),
                Warnings = new Dictionary<string, string>(),
                Apprenticeships = new List<ApprenticeshipListItemViewModel> { item }
            };

            AssertParsedContent(model, "#apprenticeUln", "––");
        }
    }
}