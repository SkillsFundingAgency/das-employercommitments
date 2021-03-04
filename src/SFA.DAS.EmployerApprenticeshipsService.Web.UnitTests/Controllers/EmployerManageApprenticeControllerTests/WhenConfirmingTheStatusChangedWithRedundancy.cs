using System;
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
using SFA.DAS.EmployerUrlHelper;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerCommitments.Web.UnitTests.Controllers.EmployerManageApprenticeControllerTests
{
    [TestFixture]
    public class WhenConfirmingTheStatusChangedWithRedundancy
    {
        private Mock<IEmployerManageApprenticeshipsOrchestrator> _orchestrator;
        private EmployerManageApprenticesController _controller;
        private Mock<IOwinWrapper> _owinWrapper;
        private const string AccountId = "123";
        private const string ApprenticeshipId = "456";

        [SetUp]
        public void Setup()
        {
            _orchestrator = new Mock<IEmployerManageApprenticeshipsOrchestrator>();
            _owinWrapper = new Mock<IOwinWrapper>();
            _controller = new EmployerManageApprenticesController(_orchestrator.Object, _owinWrapper.Object, Mock.Of<IMultiVariantTestingService>(),
                Mock.Of<ICookieStorageService<FlashMessageViewModel>>(), Mock.Of<ILinkGenerator>(), Mock.Of<ILog>());
        }

        [Test]
        public async Task ThenAccessDeniedViewIsReturnedWhenUserNotAuthorised()
        {
            //Arrange
            _orchestrator.Setup(o => o.AuthorizeRole(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Role[]>())).Returns(() => Task.FromResult(false));

            //Act
            var result = await _controller.HasApprenticeBeenMadeRedundant(AccountId, ApprenticeshipId, ChangeStatusType.Stop, DateTime.Now, WhenToMakeChangeOptions.Immediately);

            //Assert
            Assert.AreEqual("AccessDenied", (result as ViewResult)?.ViewName);
        }

        [Test]
        public async Task ThenItWillMakeTheStoppedApprenticeRedundant()
        {
            //Arrange
            _owinWrapper.Setup(x => x.GetClaimValue(It.IsAny<string>())).Returns(string.Empty);
            _orchestrator.Setup(o => o.AuthorizeRole(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Role[]>())).Returns(() => Task.FromResult(true));
            //_orchestrator.Setup(o => o.GetRedundantViewModel(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ChangeStatusType>(), It.IsAny<DateTime>(), It.IsAny<WhenToMakeChangeOptions>(), It.IsAny<string>(), true))
            //    .Returns(Task.FromResult(new OrchestratorResponse<RedundantApprenticeViewModel>()));

            var response = new OrchestratorResponse<RedundantApprenticeViewModel> { Data = new RedundantApprenticeViewModel() };
            _orchestrator.Setup(o => o.GetRedundantViewModel(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ChangeStatusType>(), It.IsAny<DateTime>(), It.IsAny<WhenToMakeChangeOptions>(), It.IsAny<string>(), null))
                .Returns(Task.FromResult(response)).Verifiable();

            //Act
            await _controller.HasApprenticeBeenMadeRedundant(AccountId, ApprenticeshipId, ChangeStatusType.Stop, DateTime.Now, WhenToMakeChangeOptions.Immediately);

            //Assert
            _orchestrator.Verify(x => x.GetRedundantViewModel(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ChangeStatusType>(), It.IsAny<DateTime>(), It.IsAny<WhenToMakeChangeOptions>(), It.IsAny<string>(), null));
        }
    }
}
