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

namespace SFA.DAS.EmployerCommitments.Web.UnitTests.Controllers.EmployerManageApprenticeControllerTests
{
    [TestFixture]
    public class WhenConfirmingTheStatusChangedToStop
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
                Mock.Of<ICookieStorageService<FlashMessageViewModel>>(), Mock.Of<ILinkGenerator>());
        }

        [Test]
        public async Task ThenAccessDeniedViewIsReturnedWhenUserNotAuthorised()
        {
            //Arrange
            _orchestrator.Setup(o => o.AuthorizeRole(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Role[]>())).Returns(() => Task.FromResult(false));

            //Act
            var result = await _controller.StatusChangeConfirmation(AccountId, ApprenticeshipId, ChangeStatusType.Stop, WhenToMakeChangeOptions.Immediately, DateTime.Now,null);

            //Assert
            Assert.AreEqual("AccessDenied", (result as ViewResult)?.ViewName);
        }

        [Test]
        public async Task ThenItWillAllowToStopIfTheApprenticeStatusIsWaitingToStart()
        {
            //Arrange
            _owinWrapper.Setup(x => x.GetClaimValue(It.IsAny<string>())).Returns(string.Empty);
            _orchestrator.Setup(o => o.AuthorizeRole(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Role[]>())).Returns(() => Task.FromResult(true));
            _orchestrator.Setup(o => o.GetChangeStatusConfirmationViewModel(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ChangeStatusType>(), It.IsAny<WhenToMakeChangeOptions>(), It.IsAny<DateTime>(), null,It.IsAny<string>())).Returns(Task.FromResult(new OrchestratorResponse<ConfirmationStateChangeViewModel>()));

            var response = new OrchestratorResponse<ConfirmationStateChangeViewModel> { Data = new ConfirmationStateChangeViewModel() };
            _orchestrator.Setup(o => o.GetChangeStatusConfirmationViewModel(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ChangeStatusType>(), It.IsAny<WhenToMakeChangeOptions>(), It.IsAny<DateTime>(), null,It.IsAny<string>())).Returns(Task.FromResult(response)).Verifiable();

            //Act
            await _controller.StatusChangeConfirmation(AccountId, ApprenticeshipId, ChangeStatusType.Stop, WhenToMakeChangeOptions.Immediately, DateTime.Now,null);

            //Assert
            _orchestrator.Verify(x => x.GetChangeStatusConfirmationViewModel(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ChangeStatusType>(), It.IsAny<WhenToMakeChangeOptions>(), It.IsAny<DateTime>(), null,It.IsAny<string>()));
        }

        [Test]
        public async Task ThenItWillAllowToStopIfTheApprenticeStatusIsWaitingToStart_Redundancy_True()
        {
            //Arrange
            _owinWrapper.Setup(x => x.GetClaimValue(It.IsAny<string>())).Returns(string.Empty);
            _orchestrator.Setup(o => o.AuthorizeRole(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Role[]>())).Returns(() => Task.FromResult(true));
            _orchestrator.Setup(o => o.GetChangeStatusConfirmationViewModel(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ChangeStatusType>(), It.IsAny<WhenToMakeChangeOptions>(), It.IsAny<DateTime>(), true, It.IsAny<string>())).Returns(Task.FromResult(new OrchestratorResponse<ConfirmationStateChangeViewModel>()));

            var response = new OrchestratorResponse<ConfirmationStateChangeViewModel> { Data = new ConfirmationStateChangeViewModel() };
            _orchestrator.Setup(o => o.GetChangeStatusConfirmationViewModel(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ChangeStatusType>(), It.IsAny<WhenToMakeChangeOptions>(), It.IsAny<DateTime>(), true, It.IsAny<string>())).Returns(Task.FromResult(response)).Verifiable();

            //Act
            await _controller.StatusChangeConfirmation(AccountId, ApprenticeshipId, ChangeStatusType.Stop, WhenToMakeChangeOptions.Immediately, DateTime.Now, true);

            //Assert
            _orchestrator.Verify(x => x.GetChangeStatusConfirmationViewModel(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ChangeStatusType>(), It.IsAny<WhenToMakeChangeOptions>(), It.IsAny<DateTime>(), true, It.IsAny<string>()));
        }

        [Test]
        public async Task ThenItWillAllowToStopIfTheApprenticeStatusIsWaitingToStart_Redundancy_False()
        {
            //Arrange
            _owinWrapper.Setup(x => x.GetClaimValue(It.IsAny<string>())).Returns(string.Empty);
            _orchestrator.Setup(o => o.AuthorizeRole(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Role[]>())).Returns(() => Task.FromResult(true));
            _orchestrator.Setup(o => o.GetChangeStatusConfirmationViewModel(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ChangeStatusType>(), It.IsAny<WhenToMakeChangeOptions>(), It.IsAny<DateTime>(), false, It.IsAny<string>())).Returns(Task.FromResult(new OrchestratorResponse<ConfirmationStateChangeViewModel>()));

            var response = new OrchestratorResponse<ConfirmationStateChangeViewModel> { Data = new ConfirmationStateChangeViewModel() };
            _orchestrator.Setup(o => o.GetChangeStatusConfirmationViewModel(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ChangeStatusType>(), It.IsAny<WhenToMakeChangeOptions>(), It.IsAny<DateTime>(), false, It.IsAny<string>())).Returns(Task.FromResult(response)).Verifiable();

            //Act
            await _controller.StatusChangeConfirmation(AccountId, ApprenticeshipId, ChangeStatusType.Stop, WhenToMakeChangeOptions.Immediately, DateTime.Now, false);

            //Assert
            _orchestrator.Verify(x => x.GetChangeStatusConfirmationViewModel(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ChangeStatusType>(), It.IsAny<WhenToMakeChangeOptions>(), It.IsAny<DateTime>(), false, It.IsAny<string>()));
        }
    }
}
