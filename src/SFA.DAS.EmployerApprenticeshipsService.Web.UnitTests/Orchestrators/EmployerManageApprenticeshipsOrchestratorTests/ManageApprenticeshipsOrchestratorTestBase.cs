using System;

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
using SFA.DAS.EmployerCommitments.Web.Validators.Messages;
using SFA.DAS.EmployerCommitments.Web.ViewModels.ManageApprenticeships;
using SFA.DAS.EmployerUrlHelper;
using SFA.DAS.NLog.Logger;
using SFA.DAS.HashingService;
using FeatureToggle;
using SFA.DAS.EmployerCommitments.Domain.Models.FeatureToggles;

namespace SFA.DAS.EmployerCommitments.Web.UnitTests.Orchestrators.EmployerManageApprenticeshipsOrchestratorTests
{
    public abstract class ManageApprenticeshipsOrchestratorTestBase
    {
        protected Mock<IHashingService> MockHashingService;
        protected ApprenticeshipMapper ApprenticeshipMapper;
        protected Mock<IMediator> MockMediator;
        protected EmployerManageApprenticeshipsOrchestrator Orchestrator;
        protected Mock<ICurrentDateTime> MockDateTime;
        protected Mock<ILinkGenerator> MockLinkGenerator;
        protected Mock<IFeatureToggleService> MockFeatureToggleService;
        protected Mock<IFeatureToggle> MockChangeOfProviderToggle;

        public IValidateApprovedApprenticeship Validator;
        protected Mock<IAcademicYearDateProvider> AcademicYearDateProvider;
        protected Mock<IAcademicYearValidator> MockAcademicYearValidator;
        protected AcademicYearValidator AcademicYearValidator;

        protected long AccountId = 123;
        protected string HashedAccountId = "HashedAccountId";
        protected string Email = "testEmail";
        protected string Name = "testName";
        
        [SetUp]
        public void Setup()
        {
            MockAcademicYearValidator = new Mock<IAcademicYearValidator>();
            MockMediator = new Mock<IMediator>();

            MockDateTime = new Mock<ICurrentDateTime>();
            MockDateTime.Setup(x => x.Now).Returns(DateTime.UtcNow);

            MockLinkGenerator = new Mock<ILinkGenerator>();
            MockLinkGenerator
                .Setup(x => x.FinanceLink($"accounts/{HashedAccountId}/finance/{MockDateTime.Object.Now.Year}/{MockDateTime.Object.Now.Month}"))
                .Returns("testLink");

            AcademicYearDateProvider = new Mock<IAcademicYearDateProvider>();
            AcademicYearDateProvider.Setup(x => x.CurrentAcademicYearStartDate).Returns(new DateTime(2017, 8, 1));
            AcademicYearDateProvider.Setup(x => x.CurrentAcademicYearEndDate).Returns(new DateTime(2018, 7, 31));
            AcademicYearDateProvider.Setup(x => x.LastAcademicYearFundingPeriod).Returns(new DateTime(2017, 10, 19, 18, 0, 0));

            MockChangeOfProviderToggle = new Mock<IFeatureToggle>();
            MockChangeOfProviderToggle.Setup(c => c.FeatureEnabled).Returns(true);

            MockFeatureToggleService = new Mock<IFeatureToggleService>();
            MockFeatureToggleService.Setup(f => f.Get<ChangeOfProvider>()).Returns(MockChangeOfProviderToggle.Object);

            ApprenticeshipMapper = new ApprenticeshipMapper(Mock.Of<IHashingService>(), MockDateTime.Object,
                MockMediator.Object, Mock.Of<ILog>(), Mock.Of<IAcademicYearValidator>(),
                Mock.Of<IAcademicYearDateProvider>(), MockLinkGenerator.Object, MockFeatureToggleService.Object);

            MockHashingService = new Mock<IHashingService>();
            MockHashingService.Setup(x => x.DecodeValue("ABC123")).Returns(123L);
            MockHashingService.Setup(x => x.DecodeValue("ABC321")).Returns(321L);
            MockHashingService.Setup(x => x.DecodeValue("ABC456")).Returns(456L);

            MockMediator.Setup(x => x.SendAsync(It.IsAny<GetUserAccountRoleQuery>()))
                .ReturnsAsync(new GetUserAccountRoleResponse
                {
                    User = new TeamMember() { AccountId = AccountId, HashedAccountId = HashedAccountId, Email = Email, Name = Name }
                });

            var academicYearProvider = new AcademicYearDateProvider(MockDateTime.Object);

            Validator = new ApprovedApprenticeshipViewModelValidator(
                new WebApprenticeshipValidationText(academicYearProvider),
                academicYearProvider,
                new AcademicYearValidator(MockDateTime.Object, academicYearProvider),
                MockDateTime.Object,
                Mock.Of<IMediator>());

            AcademicYearValidator = new AcademicYearValidator(MockDateTime.Object, academicYearProvider);

            Orchestrator = new EmployerManageApprenticeshipsOrchestrator(
                MockMediator.Object,
                MockHashingService.Object,
                ApprenticeshipMapper,
                Validator,
                MockDateTime.Object,
                new Mock<ILog>().Object, 
                new Mock<ICookieStorageService<UpdateApprenticeshipViewModel>>().Object,
                AcademicYearDateProvider.Object,
                AcademicYearValidator,
                MockLinkGenerator.Object);
        }
    }
}