using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerCommitments.Application.Queries.GetUserAccountRole;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;
using SFA.DAS.EmployerCommitments.Domain.Models.AccountTeam;
using SFA.DAS.EmployerCommitments.Infrastructure.Services;
using SFA.DAS.EmployerCommitments.Web.Orchestrators;
using SFA.DAS.EmployerCommitments.Web.Orchestrators.Mappers;
using SFA.DAS.EmployerCommitments.Web.Validators;
using SFA.DAS.EmployerCommitments.Web.ViewModels.ManageApprenticeships;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerCommitments.Web.UnitTests.Orchestrators.EmployerCommitmentOrchestrator
{
    public abstract class EmployerManageApprenticeshipsOrchestratorTestBase
    {
        private Mock<ILog> _mockLogger;

        private Mock<IHashingService> _mockHashingService;
        protected ApprenticeshipMapper ApprenticeshipMapper;
        protected Mock<IApprenticeshipFiltersMapper> ApprenticeshipFiltersMapper;

        protected Mock<IMediator> MockMediator;
        protected EmployerManageApprenticeshipsOrchestrator EmployerManageApprenticeshipsOrchestrator;
        private Mock<ICookieStorageService<UpdateApprenticeshipViewModel>> _cookieStorageService;
        protected Mock<ICurrentDateTime> MockDateTime;

        [SetUp]
        public void Setup()
        {
            MockMediator = new Mock<IMediator>();
            _mockLogger = new Mock<ILog>();
            MockDateTime = new Mock<ICurrentDateTime>();
            ApprenticeshipMapper = new ApprenticeshipMapper(Mock.Of<IHashingService>(), MockDateTime.Object, MockMediator.Object);

            _cookieStorageService = new Mock<ICookieStorageService<UpdateApprenticeshipViewModel>>();

            _mockHashingService = new Mock<IHashingService>();
            _mockHashingService.Setup(x => x.DecodeValue("ABC123")).Returns(123L);
            _mockHashingService.Setup(x => x.DecodeValue("ABC321")).Returns(321L);
            _mockHashingService.Setup(x => x.DecodeValue("ABC456")).Returns(456L);

            MockMediator.Setup(x => x.SendAsync(It.IsAny<GetUserAccountRoleQuery>()))
                .ReturnsAsync(new GetUserAccountRoleResponse { User = new TeamMember() });

            ApprenticeshipFiltersMapper = new Mock<IApprenticeshipFiltersMapper>();

            EmployerManageApprenticeshipsOrchestrator = new EmployerManageApprenticeshipsOrchestrator(
                MockMediator.Object,
                _mockHashingService.Object,
                ApprenticeshipMapper, 
                Mock.Of<ApprovedApprenticeshipViewModelValidator>(),
                new CurrentDateTime(),
                _mockLogger.Object, _cookieStorageService.Object,
                ApprenticeshipFiltersMapper.Object);
        }
    }
}