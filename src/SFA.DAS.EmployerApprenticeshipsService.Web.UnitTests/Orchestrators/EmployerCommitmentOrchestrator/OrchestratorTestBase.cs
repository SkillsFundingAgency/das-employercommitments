﻿using System.Collections.Generic;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.Commitments.Api.Types.Commitment.Types;
using SFA.DAS.EmployerCommitments.Application.Queries.GetApprenticeship;
using SFA.DAS.EmployerCommitments.Application.Queries.GetCommitment;
using SFA.DAS.EmployerCommitments.Application.Queries.GetFrameworks;
using SFA.DAS.EmployerCommitments.Application.Queries.GetStandards;
using SFA.DAS.EmployerCommitments.Application.Queries.GetTrainingProgrammes;
using SFA.DAS.EmployerCommitments.Application.Queries.GetUserAccountRole;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;
using SFA.DAS.EmployerCommitments.Domain.Models.AccountTeam;
using SFA.DAS.EmployerCommitments.Domain.Models.ApprenticeshipCourse;
using SFA.DAS.EmployerCommitments.Web.Orchestrators;
using SFA.DAS.EmployerCommitments.Web.Orchestrators.Mappers;
using SFA.DAS.EmployerCommitments.Web.Validators;
using SFA.DAS.EmployerCommitments.Web.ViewModels;
using SFA.DAS.HashingService;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerCommitments.Web.UnitTests.Orchestrators.EmployerCommitmentOrchestrator
{
    public abstract class OrchestratorTestBase
    {
        private Mock<ICommitmentStatusCalculator> _mockCalculator;

        protected Mock<ICommitmentMapper> MockCommitmentMapper;
        protected Mock<IApprenticeshipMapper> MockApprenticeshipMapper;
        protected Mock<ILog> MockLogger;
        protected Mock<IHashingService> MockHashingService;
        protected Mock<IAcademicYearValidator> MockAcademicYearValidator;
        protected Mock<IAcademicYearDateProvider> MockAcademicYearDateProvider;
        protected Mock<IMediator> MockMediator;
        protected EmployerCommitmentsOrchestrator EmployerCommitmentOrchestrator;
        protected Mock<IApprenticeshipViewModelValidator> MockApprenticeshipValidator;
        protected Mock<IFeatureToggleService> MockFeatureToggleService;
        protected CommitmentView CommitmentView;

        [SetUp]
        public void Setup()
        {
            MockMediator = new Mock<IMediator>();
            MockLogger = new Mock<ILog>();
            _mockCalculator = new Mock<ICommitmentStatusCalculator>();
            MockApprenticeshipMapper = new Mock<IApprenticeshipMapper>();
            MockCommitmentMapper = new Mock<ICommitmentMapper>();
            MockAcademicYearValidator = new Mock<IAcademicYearValidator>();
            MockAcademicYearDateProvider = new Mock<IAcademicYearDateProvider>();
            MockApprenticeshipValidator = new Mock<IApprenticeshipViewModelValidator>();
            MockFeatureToggleService = new Mock<IFeatureToggleService>();

            MockHashingService = new Mock<IHashingService>();
            MockHashingService.Setup(x => x.DecodeValue("ABC123")).Returns(123L);
            MockHashingService.Setup(x => x.DecodeValue("ABC321")).Returns(321L);
            MockHashingService.Setup(x => x.DecodeValue("ABC456")).Returns(456L);

            CommitmentView = new CommitmentView
            {
                Id = 123,
                LegalEntityId = "321",
                EditStatus = EditStatus.EmployerOnly,
                Messages = new List<MessageView>()
            };

            MockMediator.Setup(x => x.SendAsync(It.IsAny<GetCommitmentQueryRequest>()))
                .ReturnsAsync(new GetCommitmentQueryResponse
                {
                    Commitment = CommitmentView
                });

            MockMediator.Setup(x => x.SendAsync(It.IsAny<GetUserAccountRoleQuery>()))
                .ReturnsAsync(new GetUserAccountRoleResponse { User = new TeamMember() });

            MockMediator.Setup(m => m.SendAsync(It.IsAny<GetTrainingProgrammesQueryRequest>()))
                .ReturnsAsync(new GetTrainingProgrammesQueryResponse
                {
                    TrainingProgrammes = new List<ITrainingProgramme>()
                });

            MockMediator.Setup(m => m.SendAsync(It.IsAny<GetStandardsQueryRequest>()))
                .ReturnsAsync(new GetStandardsQueryResponse
                {
                    Standards = new List<Standard>()
                });

            MockMediator.Setup(m => m.SendAsync(It.IsAny<GetFrameworksQueryRequest>()))
                .ReturnsAsync(new GetFrameworksQueryResponse
                {
                    Frameworks = new List<Framework>()
                });

            MockMediator.Setup(m => m.SendAsync(It.IsAny<GetApprenticeshipQueryRequest>()))
                .ReturnsAsync(new GetApprenticeshipQueryResponse
                {
                    Apprenticeship = new Apprenticeship()
                });

            MockApprenticeshipMapper.Setup(x =>
                    x.MapToApprenticeshipViewModel(It.IsAny<Apprenticeship>(), It.IsAny<CommitmentView>()))
                .Returns(new ApprenticeshipViewModel());

            EmployerCommitmentOrchestrator = new EmployerCommitmentsOrchestrator(
                MockMediator.Object,
                MockHashingService.Object,
                _mockCalculator.Object,
                MockApprenticeshipMapper.Object,
                MockCommitmentMapper.Object,
                MockLogger.Object,
                MockApprenticeshipValidator.Object,
                MockFeatureToggleService.Object);
        }
    }
}
