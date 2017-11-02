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
using SFA.DAS.NLog.Logger;
using SFA.DAS.HashingService;

namespace SFA.DAS.EmployerCommitments.Web.UnitTests.Orchestrators.EmployerManageApprenticeshipsOrchestratorTests
{
    public abstract class ManageApprenticeshipsOrchestratorTestBase
    {
        private Mock<IHashingService> _mockHashingService;
        protected ApprenticeshipMapper ApprenticeshipMapper;
        protected Mock<IApprenticeshipFiltersMapper> ApprenticeshipFiltersMapper;
        protected Mock<IMediator> MockMediator;
        protected EmployerManageApprenticeshipsOrchestrator Orchestrator;
        protected Mock<ICurrentDateTime> MockDateTime;

        public IValidateApprovedApprenticeship Validator;
        protected Mock<IAcademicYearDateProvider> AcademicYearDateProvider;
        protected Mock<IAcademicYearValidator> MockAcademicYearValidator;

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

            ApprenticeshipMapper = new ApprenticeshipMapper(Mock.Of<IHashingService>(), MockDateTime.Object, MockMediator.Object, Mock.Of<ILog>(), Mock.Of<IAcademicYearValidator>());


            _mockHashingService = new Mock<IHashingService>();
            _mockHashingService.Setup(x => x.DecodeValue("ABC123")).Returns(123L);
            _mockHashingService.Setup(x => x.DecodeValue("ABC321")).Returns(321L);
            _mockHashingService.Setup(x => x.DecodeValue("ABC456")).Returns(456L);

            MockMediator.Setup(x => x.SendAsync(It.IsAny<GetUserAccountRoleQuery>()))
                .ReturnsAsync(new GetUserAccountRoleResponse { User = new TeamMember() });

            ApprenticeshipFiltersMapper = new Mock<IApprenticeshipFiltersMapper>();
            //Mock<ICurrentDateTime> currentDateTime = new Mock<ICurrentDateTime>();
            //currentDateTime.Setup(x => x.Now).Returns(new DateTime(2018, 5, 1));
            var academicYearProvider = new AcademicYearDateProvider(MockDateTime.Object);


            Validator = new ApprovedApprenticeshipViewModelValidator(
                new WebApprenticeshipValidationText(academicYearProvider),
                MockDateTime.Object,
                academicYearProvider,
                new AcademicYearValidator(MockDateTime.Object, academicYearProvider));

            Orchestrator = new EmployerManageApprenticeshipsOrchestrator(
                MockMediator.Object,
                _mockHashingService.Object,
                ApprenticeshipMapper,
                Validator,
                MockDateTime.Object,
                new Mock<ILog>().Object, new Mock<ICookieStorageService<UpdateApprenticeshipViewModel>>().Object,
                ApprenticeshipFiltersMapper.Object,
                AcademicYearDateProvider.Object,
                new AcademicYearValidator(MockDateTime.Object, academicYearProvider)
                );
        }


    }
}