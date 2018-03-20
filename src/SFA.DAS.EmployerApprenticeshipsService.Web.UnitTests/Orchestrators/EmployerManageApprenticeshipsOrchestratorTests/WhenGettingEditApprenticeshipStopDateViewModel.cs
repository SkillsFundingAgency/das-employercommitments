using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.EmployerCommitments.Application.Queries.GetApprenticeship;
using SFA.DAS.EmployerCommitments.Application.Queries.GetUserAccountRole;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;
using SFA.DAS.EmployerCommitments.Web.Orchestrators;
using SFA.DAS.EmployerCommitments.Web.Orchestrators.Mappers;
using SFA.DAS.EmployerCommitments.Web.ViewModels.ManageApprenticeships;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerCommitments.Web.UnitTests.Orchestrators.EmployerManageApprenticeshipsOrchestratorTests
{
    [TestFixture]
    public class WhenGettingEditApprenticeshipStopDateViewModel : ManageApprenticeshipsOrchestratorTestBase
    {
        private Mock<IApprenticeshipMapper> _apprenticeshipMapper;

        private const long ApprenticeshipId = 123456789L;
        private const string HashedApprenticeshipId = "hashedApprenticeshipId";

        private const string ExternalUserId = "UserId";

        [SetUp]
        public void Setup()
        {
            MockMediator.Setup(x => x.SendAsync(It.IsAny<GetApprenticeshipQueryRequest>()))
                .ReturnsAsync(new GetApprenticeshipQueryResponse
                {
                    Apprenticeship = new Apprenticeship() { StartDate = DateTime.Now }
                });

            _apprenticeshipMapper = new Mock<IApprenticeshipMapper>();

            _apprenticeshipMapper
                .Setup(x => x.MapToEditApprenticeshipStopDateViewModel(It.IsAny<Apprenticeship>()))
                .Returns(new EditApprenticeshipStopDateViewModel());

            Orchestrator = new EmployerManageApprenticeshipsOrchestrator(
                MockMediator.Object,
                _mockHashingService.Object,
                _apprenticeshipMapper.Object,
                Validator,
                MockDateTime.Object,
                new Mock<ILog>().Object, new Mock<ICookieStorageService<UpdateApprenticeshipViewModel>>().Object,
                ApprenticeshipFiltersMapper.Object,
                AcademicYearDateProvider.Object,
                AcademicYearValidator);

            _mockHashingService.Setup(x => x.DecodeValue(HashedApprenticeshipId)).Returns(ApprenticeshipId);
            _mockHashingService.Setup(x => x.DecodeValue(HashedAccountId)).Returns(AccountId);
        }

        [Test]
        public async Task ThenShouldMapResultsToViewModel()
        {
            await Orchestrator.GetEditApprenticeshipStopDateViewModel(HashedAccountId, HashedApprenticeshipId, ExternalUserId);

            _apprenticeshipMapper.Verify(
                x => x.MapToEditApprenticeshipStopDateViewModel(It.IsAny<Apprenticeship>())
                , Times.Once());
        }

        [Test]
        public async Task ThenShouldCallMediatorToCheckAuthorisation()
        {
            await Orchestrator.GetEditApprenticeshipStopDateViewModel(HashedAccountId, HashedApprenticeshipId, ExternalUserId);

            MockMediator.Verify(x => x.SendAsync(It.Is<GetUserAccountRoleQuery>(c =>
                            c.HashedAccountId.Equals(HashedAccountId) &&
                            c.UserId == ExternalUserId
                    )), Times.Once);
        }

        [Test]
        public async Task ThenShouldCallMediatorToGetApprenticeshipDetails()
        {
            await Orchestrator.GetEditApprenticeshipStopDateViewModel(HashedAccountId, HashedApprenticeshipId, ExternalUserId);

            MockMediator.Verify(x => x.SendAsync(It.Is<GetApprenticeshipQueryRequest>(c =>
                c.AccountId.Equals(AccountId) &&
                c.ApprenticeshipId == ApprenticeshipId
            )), Times.Once);
        }
    }
}
