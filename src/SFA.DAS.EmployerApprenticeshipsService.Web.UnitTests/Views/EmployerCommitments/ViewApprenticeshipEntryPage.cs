using System.Collections.Generic;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types.Validation;
using SFA.DAS.EmployerCommitments.Web.ViewModels;
using SFA.DAS.EmployerCommitments.Web.Views.EmployerCommitments;

namespace SFA.DAS.EmployerCommitments.Web.UnitTests.Views.EmployerCommitments
{
    [TestFixture]
    public class ViewApprenticeshipEntryPage : OrchestratedViewModelTestingBase<ApprenticeshipViewModel, ViewApprenticeshipEntry>
    {
        private const string Uln = "IAMAULN";

        [Test]
        public void ShouldDisplayUln()
        {
            var model = new ApprenticeshipViewModel
            {
                ULN = Uln
            };

            AssertParsedContent(model, "#uln", Uln);
        }
    }
}