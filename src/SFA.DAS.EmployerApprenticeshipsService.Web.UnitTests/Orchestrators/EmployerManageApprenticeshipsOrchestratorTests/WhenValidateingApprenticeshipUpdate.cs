using System;
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

namespace SFA.DAS.EmployerCommitments.Web.UnitTests.Orchestrators.EmployerManageApprenticeshipsOrchestratorTests
{
    [TestFixture]
    public class WhenValidateingApprenticeshipUpdate : ManageApprenticeshipsOrchestratorTestBase
    {
        public Mock<IValidateApprenticeship> mockValidator;

        private Mock<IApprenticeshipMapper> mockMapper;

        [SetUp]
        public void SetUp()
        {
            mockValidator = new Mock<IValidateApprenticeship>();
            mockValidator.Setup(m => m.ValidateToDictionary(It.IsAny<ApprenticeshipViewModel>()))
                .Returns(new Dictionary<string, string>());
            mockValidator.Setup(m => m.ValidateAcademicYear(It.IsAny<DateTime?>()))
                .Returns(new Dictionary<string, string>());

            mockMapper = new Mock<IApprenticeshipMapper>();
            mockMapper.Setup(m => m.MapOverlappingErrors(It.IsAny<GetOverlappingApprenticeshipsQueryResponse>()))
                .Returns(new Dictionary<string, string>());

            Orchestrator = new EmployerManageApprenticeshipsOrchestrator(
                MockMediator.Object,
                Mock.Of<IHashingService>(),
                mockMapper.Object,
                mockValidator.Object,
                new CurrentDateTime(),
                new Mock<ILog>().Object, new Mock<ICookieStorageService<UpdateApprenticeshipViewModel>>().Object,
                ApprenticeshipFiltersMapper.Object);
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
            mockValidator.Verify(m => m.ValidateAcademicYear(It.IsAny<DateTime?>()), Times.Once, failMessage: "Should validate academic year");
        }
    }
}
