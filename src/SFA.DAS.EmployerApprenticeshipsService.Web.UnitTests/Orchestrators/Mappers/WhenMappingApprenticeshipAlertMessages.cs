using System.Linq;

using FluentAssertions;
using NUnit.Framework;

using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.Apprenticeship.Types;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;
using SFA.DAS.EmployerCommitments.Web.Orchestrators;
using SFA.DAS.EmployerCommitments.Web.Orchestrators.Mappers;
using SFA.DAS.NLog.Logger;
using SFA.DAS.HashingService;

namespace SFA.DAS.EmployerCommitments.Web.UnitTests.Orchestrators.Mappers
{
    [TestFixture]
    public class WhenMappingApprenticeshipAlertMessages : ApprenticeshipMapperBase
    {
        [Test]
        public void ShouldNotCreateAlertsForEmptyApprenticeship()
        {
            var apprenticeship = new Apprenticeship();
            var viewModel = Sut.MapToApprenticeshipDetailsViewModel(apprenticeship);
            viewModel.Alerts.Count().Should().Be(0);
        }

        [Test]
        public void EmployerCreatesChangeOfCircs()
        {
            var apprenticeship = new Apprenticeship { PendingUpdateOriginator = Originator.Employer };
            var viewModel = Sut.MapToApprenticeshipDetailsViewModel(apprenticeship);
            viewModel.Alerts.Count().Should().Be(1);
            viewModel.Alerts.FirstOrDefault().Should().Be("Changes pending");
        }

        [Test]
        public void ProviderCreatesChangeOfCircs()
        {
            var apprenticeship = new Apprenticeship { PendingUpdateOriginator = Originator.Provider };
            var viewModel = Sut.MapToApprenticeshipDetailsViewModel(apprenticeship);
            viewModel.Alerts.Count().Should().Be(1);
            viewModel.Alerts.FirstOrDefault().Should().Be("Changes for review");
        }

        [Test]
        public void UnTriagedDataLock07()
        {
            var apprenticeship = new Apprenticeship { DataLockPrice = true};
            var viewModel = Sut.MapToApprenticeshipDetailsViewModel(apprenticeship);
            viewModel.Alerts.Count().Should().Be(0);
        }

        [Test]
        public void TriagedDataLock07()
        {
            var apprenticeship = new Apprenticeship { DataLockPriceTriaged = true };
            var viewModel = Sut.MapToApprenticeshipDetailsViewModel(apprenticeship);
            viewModel.Alerts.Count().Should().Be(1);
            viewModel.Alerts.FirstOrDefault().Should().Be("Changes for review");
        }

        [Test]
        public void UnTriagedDataLockCourse()
        {
            var apprenticeship = new Apprenticeship { DataLockCourse = true };
            var viewModel = Sut.MapToApprenticeshipDetailsViewModel(apprenticeship);
            viewModel.Alerts.Count().Should().Be(0);
        }

        [Test]
        public void TriagedDataLockCourse()
        {
            var apprenticeship = new Apprenticeship { DataLockCourseTriaged = true };
            var viewModel = Sut.MapToApprenticeshipDetailsViewModel(apprenticeship);
            viewModel.Alerts.Count().Should().Be(1);
            viewModel.Alerts.FirstOrDefault().Should().Be("Changes requested");
        }

        [Test]
        public void CoCAndUntriagedPriceDataLock()
        {
            var apprenticeship = new Apprenticeship { PendingUpdateOriginator = Originator.Provider ,  DataLockPrice = true };
            var viewModel = Sut.MapToApprenticeshipDetailsViewModel(apprenticeship);
            viewModel.Alerts.Count().Should().Be(1);
            viewModel.Alerts.FirstOrDefault().Should().Be("Changes for review");
        }

        [Test]
        public void CoCAndUntriagedCourseDataLock()
        {
            var apprenticeship = new Apprenticeship { PendingUpdateOriginator = Originator.Provider, DataLockCourse = true };
            var viewModel = Sut.MapToApprenticeshipDetailsViewModel(apprenticeship);
            viewModel.Alerts.Count().Should().Be(1);
            viewModel.Alerts.FirstOrDefault().Should().Be("Changes for review");
        }

        [Test]
        public void DataLockPriceOneTriagedAndOneNotTriaged()
        {
            var apprenticeship = new Apprenticeship { DataLockPrice = true, DataLockPriceTriaged = true };
            var viewModel = Sut.MapToApprenticeshipDetailsViewModel(apprenticeship);
            viewModel.Alerts.Count().Should().Be(1);
            viewModel.Alerts.FirstOrDefault().Should().Be("Changes for review");
        }

        [Test]
        public void DataLockCourseOneTriagedAndOneNotTriaged()
        {
            var apprenticeship = new Apprenticeship { DataLockPrice = true, DataLockCourseTriaged = true };
            var viewModel = Sut.MapToApprenticeshipDetailsViewModel(apprenticeship);
            viewModel.Alerts.Count().Should().Be(1);
            viewModel.Alerts.FirstOrDefault().Should().Be("Changes requested");
        }

    }
}
