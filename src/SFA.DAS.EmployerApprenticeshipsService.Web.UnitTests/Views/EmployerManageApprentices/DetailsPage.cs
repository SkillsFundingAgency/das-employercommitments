using System;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types.Apprenticeship.Types;
using SFA.DAS.EmployerCommitments.Web.Extensions;
using SFA.DAS.EmployerCommitments.Web.ViewModels.ManageApprenticeships;
using SFA.DAS.EmployerCommitments.Web.Views.EmployerManageApprentices;

namespace SFA.DAS.EmployerCommitments.Web.UnitTests.Views.EmployerManageApprentices
{
    [TestFixture]
    public class DetailsPage : OrchestratedViewModelTestingBase<ApprenticeshipDetailsViewModel, Details>
    {
        private ApprenticeshipDetailsViewModel _apprenticeship;

        private DateTime _stopDate;

        [SetUp]
        public void Setup()
        {
            _apprenticeship = new ApprenticeshipDetailsViewModel();
            _stopDate = DateTime.Now;
        }

        [Test]
        public void ShouldDisplayStopDateWhenStatusIsWithdrawn()
        {
            var model = new ApprenticeshipDetailsViewModel
            {
                PaymentStatus = PaymentStatus.Withdrawn,
                StopDate = _stopDate
            };

            AssertParsedContent(model, "#stopDate", _stopDate.ToGdsFormat());
        }

        [Test]
        public void ShouldDisplayEditStopDateLinkWhenStatusIsWithdrawn()
        {
            var model = new ApprenticeshipDetailsViewModel
            {
                PaymentStatus = PaymentStatus.Withdrawn,
                StopDate = _stopDate
            };

            AssertParsedContent(model, "#editStopDate", "Edit stop date");
        }
    }
}
