using System;
using System.Web.Routing;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types.Apprenticeship.Types;
using SFA.DAS.EmployerCommitments.Web.Extensions;
using SFA.DAS.EmployerCommitments.Web.ViewModels.ManageApprenticeships;
using SFA.DAS.EmployerCommitments.Web.Views.EmployerManageApprentices;

namespace SFA.DAS.EmployerCommitments.Web.UnitTests.Views.EmployerManageApprentices
{
    [TestFixture]
    public class EmployerManageApprenticesDetailsPage : OrchestratedViewModelTestingBase<ApprenticeshipDetailsViewModel, Details>
    {
        private ApprenticeshipDetailsViewModel _originalApprenticeship;
        private DateTime _stopDate;

        private const string Uln = "IAMAULN";
        private const string TrainingName = "IAMATRAININGNAME";
        private const string FirstName = "IAMAFIRSTNAME";
        private const string LastName = "IAMALASTNAME";

        [SetUp]
        public void Setup()
        {
            _originalApprenticeship = new ApprenticeshipDetailsViewModel
            {
                ULN = Uln,
                FirstName = FirstName,
                LastName = LastName,
                TrainingName = TrainingName
            };

            _stopDate = DateTime.Now;

            RouteTable.Routes.Clear();
            RouteTable.Routes.Add(
                    "ChangeStatusSelectOption",
                    new Route("commitments/accounts/{accountId}/apprentices/manage/{apprenticeId}/details/statuschange",
                    new PageRouteHandler("~/details.cshtml")));
            RouteTable.Routes.Add(
                "EditStopDateOption",
                new Route("commitments/accounts/{accountId}/apprentices/manage/{apprenticeId}/details/editstopdate",
                    new PageRouteHandler("~/editstopdate.cshtml")));
        }

        [Test]
        public void ShouldDisplayLearnerName()
        {
            var learnerName = $"{FirstName} {LastName}";
            var model = _originalApprenticeship;

            AssertParsedContent(model, "#learnerName", learnerName);
        }

        [TestCase(false, null)]
        [TestCase(true, "Edit status")]
        public void ShouldDisplayEditStatusLinkCorrectly(bool canEditStatus, string editStatsLinkText)
        {
            var model = new ApprenticeshipDetailsViewModel
            {
                PaymentStatus = PaymentStatus.Withdrawn,
                StopDate = _stopDate,
                CanEditStatus = canEditStatus
            };

            AssertParsedContent(model, "#editStatusLink", editStatsLinkText);
        }

        [TestCase(PaymentStatus.Withdrawn, "01-01-2018", "Edit stop date")]
        [TestCase(PaymentStatus.Active, null, null)]
        public void ShouldDisplayStopDateAndEditStopDateLinkCorrectly(PaymentStatus status, DateTime? stopdate, string editStopDateLinkText)
        {
            var model = new ApprenticeshipDetailsViewModel
            {
                PaymentStatus = status,
                StopDate = stopdate,
                CanEditStatus = true
            };

            AssertParsedContent(model, "#stopDate", stopdate?.ToGdsFormat());
            AssertParsedContent(model, "#editStopDateLink", editStopDateLinkText);
        }
    }
}