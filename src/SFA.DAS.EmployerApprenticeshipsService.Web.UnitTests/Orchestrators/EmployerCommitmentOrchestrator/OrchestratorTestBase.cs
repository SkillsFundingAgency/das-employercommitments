using System;
using System.Collections.Generic;
using System.Linq;
using FeatureToggle;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.Commitments.Api.Types.Commitment.Types;
using SFA.DAS.EmployerCommitments.Application.Domain.Commitment;
using SFA.DAS.EmployerCommitments.Application.Queries.GetApprenticeship;
using SFA.DAS.EmployerCommitments.Application.Queries.GetCommitment;
using SFA.DAS.EmployerCommitments.Application.Queries.GetTrainingProgrammes;
using SFA.DAS.EmployerCommitments.Application.Queries.GetUserAccountRole;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;
using SFA.DAS.EmployerCommitments.Domain.Models.AccountTeam;
using SFA.DAS.EmployerCommitments.Domain.Models.ApprenticeshipCourse;
using SFA.DAS.EmployerCommitments.Domain.Models.FeatureToggles;
using SFA.DAS.EmployerCommitments.Web.Orchestrators;
using SFA.DAS.EmployerCommitments.Web.Orchestrators.Mappers;
using SFA.DAS.EmployerCommitments.Web.PublicHashingService;
using SFA.DAS.EmployerCommitments.Web.Validators;
using SFA.DAS.EmployerCommitments.Web.ViewModels;
using SFA.DAS.HashingService;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerCommitments.Web.UnitTests.Orchestrators.EmployerCommitmentOrchestrator
{
    public abstract class OrchestratorTestBase
    {
        protected Mock<ICommitmentMapper> MockCommitmentMapper;
        protected Mock<IApprenticeshipCoreValidator> MockApprenticeshipCoreValidator;
        protected Mock<IApprenticeshipMapper> MockApprenticeshipMapper;
        protected Mock<ILog> MockLogger;
        protected Mock<IHashingService> MockHashingService;
        protected Mock<IPublicHashingService> MockPublicHashingService;
        protected Mock<IMediator> MockMediator;
        protected EmployerCommitmentsOrchestrator EmployerCommitmentOrchestrator;
        protected Mock<IFeatureToggleService> MockFeatureToggleService;
        protected Mock<IFeatureToggle> MockEmployerCommitmentsV2FeatureToggleOn;
        protected Mock<IFeatureToggle> MockTransfersFeatureToggleOn;
        protected CommitmentView CommitmentView;

        [SetUp]
        public void Setup()
        {
            MockMediator = new Mock<IMediator>();
            MockLogger = new Mock<ILog>();
            MockApprenticeshipMapper = new Mock<IApprenticeshipMapper>();
            MockApprenticeshipCoreValidator = new Mock<IApprenticeshipCoreValidator>();
            MockCommitmentMapper = new Mock<ICommitmentMapper>();

            MockEmployerCommitmentsV2FeatureToggleOn = new Mock<IFeatureToggle>();
            MockEmployerCommitmentsV2FeatureToggleOn.Setup(x => x.FeatureEnabled).Returns(true);
            MockTransfersFeatureToggleOn = new Mock<IFeatureToggle>();
            MockTransfersFeatureToggleOn.Setup(x => x.FeatureEnabled).Returns(true);
            MockFeatureToggleService = new Mock<IFeatureToggleService>();
            MockFeatureToggleService.Setup(x => x.Get<Transfers>()).Returns(MockTransfersFeatureToggleOn.Object);

            MockHashingService = new Mock<IHashingService>();
            MockHashingService.Setup(x => x.DecodeValue("ABC123")).Returns(123L);
            MockHashingService.Setup(x => x.DecodeValue("ABC321")).Returns(321L);
            MockHashingService.Setup(x => x.DecodeValue("ABC456")).Returns(456L);

            MockPublicHashingService = new Mock<IPublicHashingService>();

            CommitmentView = new CommitmentView
            {
                Id = 123,
                LegalEntityId = "321",
                EditStatus = EditStatus.EmployerOnly,
                Messages = new List<MessageView>(),
                AccountLegalEntityPublicHashedId = "ALE123"
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
                MockPublicHashingService.Object,
                MockApprenticeshipCoreValidator.Object,
                MockApprenticeshipMapper.Object,
                MockCommitmentMapper.Object,
                MockLogger.Object,
                MockFeatureToggleService.Object);
        }

        protected CommitmentListItem GetTestCommitmentOfStatus(long id, RequestStatus requestStatus)
        {
            switch (requestStatus)
            {
                case RequestStatus.WithProviderForApproval:
                    return new CommitmentListItem
                    {
                        Id = id,
                        Reference = id.ToString(),
                        EditStatus = EditStatus.ProviderOnly,
                        LastAction = LastAction.Approve,
                        ApprenticeshipCount = 1,
                        AgreementStatus = AgreementStatus.EmployerAgreed
                    };
                case RequestStatus.SentForReview:
                    return new CommitmentListItem
                    {
                        Id = id,
                        Reference = id.ToString(),
                        EditStatus = EditStatus.ProviderOnly,
                        LastAction = LastAction.Amend,
                        ApprenticeshipCount = 1,
                        AgreementStatus = AgreementStatus.NotAgreed
                    };
                case RequestStatus.SentToProvider:
                    return new CommitmentListItem
                    {
                        Id = id,
                        Reference = id.ToString(),
                        EditStatus = EditStatus.ProviderOnly,
                        LastAction = LastAction.Amend,
                        ApprenticeshipCount = 1,
                        AgreementStatus = AgreementStatus.NotAgreed
                    };
                case RequestStatus.ReadyForReview:
                    return new CommitmentListItem
                    {
                        Id = id,
                        Reference = id.ToString(),
                        EditStatus = EditStatus.EmployerOnly,
                        LastAction = LastAction.Amend,
                        ApprenticeshipCount = 1,
                        AgreementStatus = AgreementStatus.NotAgreed
                    };
                default:
                    Assert.Fail("Add the RequestStatus you require above, or else fix your test!");
                    throw new NotImplementedException();
            }
        }

        protected IEnumerable<CommitmentListItem> GetTestCommitmentsOfStatus(long startId, params RequestStatus[] requestStatuses)
        {
            return requestStatuses.Select(s => GetTestCommitmentOfStatus(startId++, s));
        }
    }
}
