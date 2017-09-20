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
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerCommitments.Web.UnitTests.Orchestrators.EmployerCommitmentOrchestrator
{
    public abstract class OrchestratorTestBase
    {
        private Mock<ILog> _mockLogger;
        private Mock<IHashingService> _mockHashingService;
        private Mock<ICommitmentStatusCalculator> _mockCalculator;
        private Mock<IApprenticeshipMapper> _mockApprenticeshipMapper;
        private Mock<ICommitmentMapper> _mockCommitmentMapper;
        private Mock<IAcademicYearValidator> _mockAcademicYearValidator;

        protected Mock<IMediator> MockMediator;
        protected EmployerCommitmentsOrchestrator EmployerCommitmentOrchestrator;

        [SetUp]
        public void Setup()
        {
            MockMediator = new Mock<IMediator>();
            _mockLogger = new Mock<ILog>();
            _mockCalculator = new Mock<ICommitmentStatusCalculator>();
            _mockApprenticeshipMapper = new Mock<IApprenticeshipMapper>();
            _mockCommitmentMapper = new Mock<ICommitmentMapper>();
            _mockAcademicYearValidator = new Mock<IAcademicYearValidator>();

            _mockHashingService = new Mock<IHashingService>();
            _mockHashingService.Setup(x => x.DecodeValue("ABC123")).Returns(123L);
            _mockHashingService.Setup(x => x.DecodeValue("ABC321")).Returns(321L);
            _mockHashingService.Setup(x => x.DecodeValue("ABC456")).Returns(456L);

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
                _mockHashingService.Object,
                _mockCalculator.Object,
                _mockApprenticeshipMapper.Object,
                _mockCommitmentMapper.Object,
                _mockLogger.Object,
                _mockAcademicYearValidator.Object);
        }
    }
}
