using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.Apprenticeship.Types;
using SFA.DAS.EmployerCommitments.Application.Queries.GetApprenticeship;
using SFA.DAS.EmployerCommitments.Application.Queries.GetApprenticeshipsByUln;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;
using SFA.DAS.EmployerCommitments.Web.Orchestrators;
using SFA.DAS.EmployerCommitments.Web.Orchestrators.Mappers;
using SFA.DAS.EmployerCommitments.Web.Validators;
using SFA.DAS.EmployerCommitments.Web.ViewModels.ManageApprenticeships;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerCommitments.Web.UnitTests.Orchestrators.EmployerManageApprenticeshipsOrchestratorTests
{
    [TestFixture]
    public class WhenValidateApprenticeshipStopDate : ManageApprenticeshipsOrchestratorTestBase
    {
        private Mock<IApprenticeshipMapper> _apprenticeshipMapper;

        private const long ApprenticeshipId = 123456789L;
        private const string HashedApprenticeshipId = "hashedApprenticeshipId";

        private const string Uln = "Uln";

        private Apprenticeship _mockApprenticeship;
        private EditStopDateViewModel _editStopDateViewModel;

        private Mock<IValidateApprovedApprenticeship> _approvedApprenticeshipValidator;

        [SetUp]
        public void Setup()
        {
            _mockApprenticeship = new Apprenticeship() { Id = ApprenticeshipId, StartDate = DateTime.Now, ULN = Uln, PaymentStatus = PaymentStatus.Active };

            MockMediator.Setup(x => x.SendAsync(It.IsAny<GetApprenticeshipQueryRequest>()))
                .ReturnsAsync(new GetApprenticeshipQueryResponse
                {
                    Apprenticeship = _mockApprenticeship
                });

            MockMediator.Setup(x => x.SendAsync(It.IsAny<GetApprenticeshipsByUlnRequest>()))
                .ReturnsAsync(new GetApprenticeshipsByUlnResponse
                {
                    Apprenticeships = new List<Apprenticeship>()
                        {
                            _mockApprenticeship
                        }
                });

            _apprenticeshipMapper = new Mock<IApprenticeshipMapper>();

            _apprenticeshipMapper
                .Setup(x => x.MapToEditApprenticeshipStopDateViewModel(It.IsAny<Apprenticeship>()))
                .Returns(new EditApprenticeshipStopDateViewModel());

            _approvedApprenticeshipValidator = new Mock<IValidateApprovedApprenticeship>();
            _approvedApprenticeshipValidator.Setup(x =>
                x.ValidateNewStopDate(It.IsAny<EditStopDateViewModel>(), It.IsAny<DateTime>())).Returns(new Dictionary<string, string>());

            Orchestrator = new EmployerManageApprenticeshipsOrchestrator(
                MockMediator.Object,
                _mockHashingService.Object,
                _apprenticeshipMapper.Object,
                _approvedApprenticeshipValidator.Object,
                MockDateTime.Object,
                new Mock<ILog>().Object, new Mock<ICookieStorageService<UpdateApprenticeshipViewModel>>().Object,
                ApprenticeshipFiltersMapper.Object,
                AcademicYearDateProvider.Object,
                AcademicYearValidator);

            _mockHashingService.Setup(x => x.DecodeValue(HashedApprenticeshipId)).Returns(ApprenticeshipId);
            _mockHashingService.Setup(x => x.DecodeValue(HashedAccountId)).Returns(AccountId);

            _editStopDateViewModel = new EditStopDateViewModel();
        }

        [Test]
        public async Task ThenShouldFlagErrorIfAnyOtherApprenticeshipsFound()
        {
            var otherApprenticeshipId = ApprenticeshipId + 1;
            var otherActiveApprenticeship = new Apprenticeship() { Id = otherApprenticeshipId, StartDate = DateTime.Now, ULN = Uln, PaymentStatus = PaymentStatus.Active };

            MockMediator.Setup(x => x.SendAsync(It.IsAny<GetApprenticeshipsByUlnRequest>()))
                .ReturnsAsync(new GetApprenticeshipsByUlnResponse
                {
                    Apprenticeships = new List<Apprenticeship>()
                    {
                        _mockApprenticeship,
                        otherActiveApprenticeship
                    }
                });

            var result = await Orchestrator.ValidateApprenticeshipStopDate(HashedAccountId, HashedApprenticeshipId, _editStopDateViewModel);

            result.Should().NotBeEmpty();
        }

        [Test]
        public async Task ThenShouldReturnEmptyResultWhenNoErrorsFound()
        {
            var result = await Orchestrator.ValidateApprenticeshipStopDate(HashedAccountId, HashedApprenticeshipId, _editStopDateViewModel);

            result.Should().BeEmpty();
        }

        [Test]
        public async Task ThenShouldValidateTheNewStopDate()
        {
            await Orchestrator.ValidateApprenticeshipStopDate(HashedAccountId, HashedApprenticeshipId, _editStopDateViewModel);

            _approvedApprenticeshipValidator
                .Verify(x => x.ValidateNewStopDate(
                    It.Is<EditStopDateViewModel>(c => c.Equals(_editStopDateViewModel)),
                    It.IsAny<DateTime>()),
                    Times.Once);
        }

        [Test]
        public async Task ThenShouldCallMediatorToGetApprenticeshipDetails()
        {
            await Orchestrator.ValidateApprenticeshipStopDate(HashedAccountId, HashedApprenticeshipId, _editStopDateViewModel);

            MockMediator.Verify(x => x.SendAsync(It.Is<GetApprenticeshipQueryRequest>(c =>
                c.AccountId.Equals(AccountId) &&
                c.ApprenticeshipId == ApprenticeshipId
            )), Times.Once);
        }

        [Test]
        public async Task ThenShouldCallMediatorToGetApprenticeshipsForUln()
        {
            await Orchestrator.ValidateApprenticeshipStopDate(HashedAccountId, HashedApprenticeshipId, _editStopDateViewModel);

            MockMediator.Verify(x => x.SendAsync(It.Is<GetApprenticeshipsByUlnRequest>(c =>
                c.AccountId.Equals(AccountId) &&
                c.Uln == Uln
            )), Times.Once);
        }
    }
}
