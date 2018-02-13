using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.Commitments.Api.Types.Commitment.Types;
using SFA.DAS.EmployerCommitments.Application.Queries.GetCommitment;
using SFA.DAS.EmployerCommitments.Application.Queries.GetUserAccountRole;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;
using SFA.DAS.EmployerCommitments.Domain.Models.AccountTeam;
using SFA.DAS.EmployerCommitments.Web.Orchestrators;
using SFA.DAS.EmployerCommitments.Web.Orchestrators.Mappers;
using SFA.DAS.EmployerCommitments.Web.Validators;
using SFA.DAS.HashingService;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerCommitments.Web.UnitTests.Orchestrators.EmployerCommitmentOrchestrator
{
    public abstract class OrchestratorTestBase
    {
        private Mock<ICommitmentStatusCalculator> _mockCalculator;
        private Mock<ICommitmentMapper> _mockCommitmentMapper;

        protected Mock<IApprenticeshipMapper> MockApprenticeshipMapper;
        protected Mock<ILog> MockLogger;
        protected Mock<IHashingService> MockHashingService;
        protected Mock<IAcademicYearValidator> MockAcademicYearValidator;
        protected Mock<IAcademicYearDateProvider> MockAcademicYearDateProvider;
        protected Mock<IMediator> MockMediator;
        protected EmployerCommitmentsOrchestrator EmployerCommitmentOrchestrator;
        protected Mock<IApprenticeshipViewModelValidator> MockApprenticeshipValidator;
        protected Mock<IFeatureToggleService> MockFeatureToggleService;

        [SetUp]
        public void Setup()
        {
            MockMediator = new Mock<IMediator>();
            MockLogger = new Mock<ILog>();
            _mockCalculator = new Mock<ICommitmentStatusCalculator>();
            MockApprenticeshipMapper = new Mock<IApprenticeshipMapper>();
            _mockCommitmentMapper = new Mock<ICommitmentMapper>();
            MockAcademicYearValidator = new Mock<IAcademicYearValidator>();
            MockAcademicYearDateProvider = new Mock<IAcademicYearDateProvider>();
            MockApprenticeshipValidator = new Mock<IApprenticeshipViewModelValidator>();
            MockFeatureToggleService = new Mock<IFeatureToggleService>();

            MockHashingService = new Mock<IHashingService>();
            MockHashingService.Setup(x => x.DecodeValue("ABC123")).Returns(123L);
            MockHashingService.Setup(x => x.DecodeValue("ABC321")).Returns(321L);
            MockHashingService.Setup(x => x.DecodeValue("ABC456")).Returns(456L);

            MockMediator.Setup(x => x.SendAsync(It.IsAny<GetCommitmentQueryRequest>()))
                .ReturnsAsync(new GetCommitmentQueryResponse
                {
                    Commitment = new CommitmentView
                    {
                        Id = 123,
                        LegalEntityId = "321",
                        EditStatus = EditStatus.EmployerOnly
                    }
                });

            MockMediator.Setup(x => x.SendAsync(It.IsAny<GetUserAccountRoleQuery>()))
                .ReturnsAsync(new GetUserAccountRoleResponse { User = new TeamMember() });

            EmployerCommitmentOrchestrator = new EmployerCommitmentsOrchestrator(
                MockMediator.Object,
                MockHashingService.Object,
                _mockCalculator.Object,
                MockApprenticeshipMapper.Object,
                _mockCommitmentMapper.Object,
                MockLogger.Object,
                MockApprenticeshipValidator.Object,
                MockFeatureToggleService.Object);
        }
    }
}
