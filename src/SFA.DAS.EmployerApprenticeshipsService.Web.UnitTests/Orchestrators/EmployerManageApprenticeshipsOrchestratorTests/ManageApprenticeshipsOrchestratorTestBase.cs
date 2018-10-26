using System;
using System.Collections.Generic;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types.DataLock;
using SFA.DAS.EmployerCommitments.Application.Queries.GetApprenticeshipDataLockSummary;
using SFA.DAS.EmployerCommitments.Application.Queries.GetUserAccountRole;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;
using SFA.DAS.EmployerCommitments.Domain.Models.AccountTeam;
using SFA.DAS.EmployerCommitments.Infrastructure.Services;
using SFA.DAS.EmployerCommitments.Web.Orchestrators;
using SFA.DAS.EmployerCommitments.Web.Orchestrators.Mappers;
using SFA.DAS.EmployerCommitments.Web.Validators;
using SFA.DAS.EmployerCommitments.Web.Validators.Messages;
using SFA.DAS.EmployerCommitments.Web.ViewModels.ManageApprenticeships;
using SFA.DAS.NLog.Logger;
using SFA.DAS.HashingService;

namespace SFA.DAS.EmployerCommitments.Web.UnitTests.Orchestrators.EmployerManageApprenticeshipsOrchestratorTests
{
    public abstract class ManageApprenticeshipsOrchestratorTestBase
    {
        protected Mock<IHashingService> MockHashingService;
        protected ApprenticeshipMapper ApprenticeshipMapper;
        protected Mock<IApprenticeshipFiltersMapper> ApprenticeshipFiltersMapper;
        protected Mock<IMediator> MockMediator;
        protected EmployerManageApprenticeshipsOrchestrator Orchestrator;
        protected Mock<ICurrentDateTime> MockDateTime;

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

            AcademicYearDateProvider = new Mock<IAcademicYearDateProvider>();
            AcademicYearDateProvider.Setup(x => x.CurrentAcademicYearStartDate).Returns(new DateTime(2017, 8, 1));
            AcademicYearDateProvider.Setup(x => x.CurrentAcademicYearEndDate).Returns(new DateTime(2018, 7, 31));
            AcademicYearDateProvider.Setup(x => x.LastAcademicYearFundingPeriod).Returns(new DateTime(2017, 10, 19, 18, 0, 0));

            ApprenticeshipMapper = new ApprenticeshipMapper(Mock.Of<IHashingService>(), MockDateTime.Object,
                MockMediator.Object, Mock.Of<ILog>(), Mock.Of<IAcademicYearValidator>(),
                Mock.Of<IAcademicYearDateProvider>());

            MockHashingService = new Mock<IHashingService>();
            MockHashingService.Setup(x => x.DecodeValue("ABC123")).Returns(123L);
            MockHashingService.Setup(x => x.DecodeValue("ABC321")).Returns(321L);
            MockHashingService.Setup(x => x.DecodeValue("ABC456")).Returns(456L);

            MockMediator.Setup(x => x.SendAsync(It.IsAny<GetUserAccountRoleQuery>()))
                .ReturnsAsync(new GetUserAccountRoleResponse
                {
                    User = new TeamMember() { AccountId = AccountId, HashedAccountId = HashedAccountId, Email = Email, Name = Name }
                });

            MockMediator.Setup(x => x.SendAsync(It.IsAny<GetDataLockSummaryQueryRequest>()))
                .ReturnsAsync(new GetDataLockSummaryQueryResponse
                {
                    DataLockSummary = new DataLockSummary
                    {
                        DataLockWithOnlyPriceMismatch = new List<DataLockStatus>(),
                        DataLockWithCourseMismatch = new List<DataLockStatus>()
                    }
                });

            ApprenticeshipFiltersMapper = new Mock<IApprenticeshipFiltersMapper>();

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
                Mock.Of<IFiltersCookieManager>(),
                ApprenticeshipFiltersMapper.Object,
                AcademicYearDateProvider.Object,
                AcademicYearValidator,
                Mock.Of<IApprovedApprenticeshipMapper>());
        }
    }
}