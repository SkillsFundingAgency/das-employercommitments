using System.Collections.Generic;
using System.Threading.Tasks;

using Moq;
using NUnit.Framework;

using SFA.DAS.Commitments.Api.Types.Validation;
using SFA.DAS.EmployerCommitments.Application.Queries.GetOverlappingApprenticeships;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;
using SFA.DAS.EmployerCommitments.Infrastructure.Services;
using SFA.DAS.EmployerCommitments.Web.Orchestrators;
using SFA.DAS.EmployerCommitments.Web.Orchestrators.Mappers;
using SFA.DAS.EmployerCommitments.Web.Validators;
using SFA.DAS.EmployerCommitments.Web.ViewModels;
using SFA.DAS.EmployerCommitments.Web.ViewModels.ManageApprenticeships;
using SFA.DAS.NLog.Logger;
using SFA.DAS.HashingService;

namespace SFA.DAS.EmployerCommitments.Web.UnitTests.Orchestrators.EmployerManageApprenticeshipsOrchestratorTests
{
    [TestFixture]
    public class WhenValidateingApprenticeshipUpdate  : ManageApprenticeshipsOrchestratorTestBase
    {
        public Mock<IValidateApprovedApprenticeship> mockValidator;
        private Mock<IApprenticeshipMapper> mockMapper;
        protected readonly Mock<ICurrentDateTime> CurrentDateTime = new Mock<ICurrentDateTime>();

        [SetUp]
        public void SetUp()
        {
            mockValidator = new Mock<IValidateApprovedApprenticeship>();
            mockMapper = new Mock<IApprenticeshipMapper>();
            mockMapper.Setup(m => m.MapOverlappingErrors(It.IsAny<GetOverlappingApprenticeshipsQueryResponse>()))
                .Returns(new Dictionary<string, string>());

            mockValidator.Setup(m => m.ValidateToDictionary(It.IsAny<ApprenticeshipViewModel>()))
                .Returns(new Dictionary<string, string>());

            mockValidator.Setup(m => m.ValidateAcademicYear(It.IsAny<UpdateApprenticeshipViewModel>()))
                .Returns(new Dictionary<string, string>());

            var academicYearDateProvider = Mock.Of<IAcademicYearDateProvider>();
            
            Orchestrator = new EmployerManageApprenticeshipsOrchestrator(
                MockMediator.Object,
                Mock.Of<IHashingService>(),
                mockMapper.Object,
                mockValidator.Object,
                CurrentDateTime.Object,
                new Mock<ILog>().Object,
                new Mock<ICookieStorageService<UpdateApprenticeshipViewModel>>().Object,
                ApprenticeshipFiltersMapper.Object,
                academicYearDateProvider, 
                new AcademicYearValidator(CurrentDateTime.Object, academicYearDateProvider)
                );
        }


        [Test]
        public async Task ShouldValidate()
        {
            var viewModel = new ApprenticeshipViewModel();
            var updateModel = new UpdateApprenticeshipViewModel();

            MockMediator.Setup(m => m.SendAsync(It.IsAny<GetOverlappingApprenticeshipsQueryRequest>()))
                .ReturnsAsync(new GetOverlappingApprenticeshipsQueryResponse { Overlaps = new List<ApprenticeshipOverlapValidationResult>()});

            await Orchestrator.ValidateApprenticeship(viewModel, updateModel);
            
            MockMediator.Verify(m => m.SendAsync(It.IsAny<GetOverlappingApprenticeshipsQueryRequest>()), Times.Once, failMessage: "Should call");
            mockMapper.Verify(m => m.MapOverlappingErrors(It.IsAny<GetOverlappingApprenticeshipsQueryResponse>()), Times.Once, failMessage: "Should verify overlapping apprenticeship");
            mockValidator.Verify(m => m.ValidateToDictionary(It.IsAny<ApprenticeshipViewModel>()), Times.Once, failMessage: "Should validate apprenticeship");
            mockValidator.Verify(m => m.ValidateAcademicYear(It.IsAny<UpdateApprenticeshipViewModel>()), Times.Once, failMessage: "Should validate academic year");

        }
    }
}
