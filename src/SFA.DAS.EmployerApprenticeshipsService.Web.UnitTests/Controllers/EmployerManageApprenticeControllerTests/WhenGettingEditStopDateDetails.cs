using System.Threading.Tasks;
using System.Web.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;
using SFA.DAS.EmployerCommitments.Domain.Models.UserProfile;
using SFA.DAS.EmployerCommitments.Web.Authentication;
using SFA.DAS.EmployerCommitments.Web.Controllers;
using SFA.DAS.EmployerCommitments.Web.Orchestrators;
using SFA.DAS.EmployerCommitments.Web.ViewModels;
using SFA.DAS.EmployerCommitments.Web.ViewModels.ManageApprenticeships;

namespace SFA.DAS.EmployerCommitments.Web.UnitTests.Controllers.EmployerManageApprenticeControllerTests
{
    [TestFixture]
    public class WhenGettingEditStopDateDetails
    {
        private Mock<IEmployerManageApprenticeshipsOrchestrator> _orchestrator;

        private EmployerManageApprenticesController _controller;

        private const string AccountId = "123";

        private const string ApprenticeshipId = "456";

        [SetUp]
        public void Setup()
        {
            _orchestrator = new Mock<IEmployerManageApprenticeshipsOrchestrator>();

            _controller = new EmployerManageApprenticesController(_orchestrator.Object, Mock.Of<IOwinWrapper>(), Mock.Of<IMultiVariantTestingService>(),
                Mock.Of<ICookieStorageService<FlashMessageViewModel>>());
        }

        [Test]
        public async Task ThenAccessDeniedViewIsReturnedWhenUserNotAuthorised()
        {
            _orchestrator.Setup(o => o.AuthorizeRole(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Role[]>())).Returns(() => Task.FromResult(false));

            var result = await _controller.EditStopDate(AccountId, ApprenticeshipId);

            Assert.AreEqual("AccessDenied", (result as ViewResult)?.ViewName);
        }

        [Test]
        public async Task ThenOrchestratorIsCalled()
        {
            _orchestrator.Setup(o => o.AuthorizeRole(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Role[]>())).Returns(() => Task.FromResult(true));

            var response = new OrchestratorResponse<EditApprenticeshipStopDateViewModel>();
            _orchestrator.Setup(o => o.GetApprenticeshipStopDateDetails(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(response)).Verifiable();

            await _controller.EditStopDate(AccountId, ApprenticeshipId);

            _orchestrator.Verify(o => o.GetApprenticeshipStopDateDetails(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));
        }
    }
}
