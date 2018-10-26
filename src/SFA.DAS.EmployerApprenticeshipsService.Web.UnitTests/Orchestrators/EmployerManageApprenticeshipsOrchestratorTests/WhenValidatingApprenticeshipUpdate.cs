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
    public class WhenValidatingApprenticeshipUpdate  : ManageApprenticeshipsOrchestratorTestBase
    {
        private Mock<IValidateApprovedApprenticeship> _mockValidator;
        private Mock<IApprenticeshipMapper> _mockMapper;
        private readonly Mock<ICurrentDateTime> _currentDateTime = new Mock<ICurrentDateTime>();

        [SetUp]
        public void SetUp()
        {
            _mockValidator = new Mock<IValidateApprovedApprenticeship>();
            _mockValidator.Setup(m => m.MapOverlappingErrors(It.IsAny<GetOverlappingApprenticeshipsQueryResponse>()))
                .Returns(new Dictionary<string, string>());
            _mockMapper = new Mock<IApprenticeshipMapper>();

            _mockValidator.Setup(m => m.ValidateToDictionary(It.IsAny<ApprenticeshipViewModel>()))
                .Returns(new Dictionary<string, string>());

            _mockValidator.Setup(m => m.ValidateAcademicYear(It.IsAny<UpdateApprenticeshipViewModel>()))
                .Returns(new Dictionary<string, string>());

            _mockValidator.Setup(m => m.ValidateApprovedEndDate(It.IsAny<UpdateApprenticeshipViewModel>()))
                .Returns(new Dictionary<string, string>());

            var academicYearDateProvider = Mock.Of<IAcademicYearDateProvider>();
            
            Orchestrator = new EmployerManageApprenticeshipsOrchestrator(
                MockMediator.Object,
                Mock.Of<IHashingService>(),
                _mockMapper.Object,
                _mockValidator.Object,
                _currentDateTime.Object,
                new Mock<ILog>().Object,
                new Mock<ICookieStorageService<UpdateApprenticeshipViewModel>>().Object,
                Mock.Of<IFiltersCookieManager>(),
                ApprenticeshipFiltersMapper.Object,
                academicYearDateProvider, 
                new AcademicYearValidator(_currentDateTime.Object, academicYearDateProvider),
                Mock.Of<IApprovedApprenticeshipMapper>()
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
            _mockValidator.Verify(m => m.MapOverlappingErrors(It.IsAny<GetOverlappingApprenticeshipsQueryResponse>()), Times.Once, failMessage: "Should verify overlapping apprenticeship");
            _mockValidator.Verify(m => m.ValidateToDictionary(It.IsAny<ApprenticeshipViewModel>()), Times.Once, failMessage: "Should validate apprenticeship");
            _mockValidator.Verify(m => m.ValidateAcademicYear(It.IsAny<UpdateApprenticeshipViewModel>()), Times.Once, failMessage: "Should validate academic year");
            _mockValidator.Verify(m => m.ValidateApprovedEndDate(It.IsAny<UpdateApprenticeshipViewModel>()), Times.Once, failMessage: "Should validate end date");
        }
    }
}
