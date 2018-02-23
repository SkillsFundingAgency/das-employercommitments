using System.Collections.Generic;
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
    public class WhenUpdatingApprenticeshipStopDate
    {
        private Mock<IEmployerManageApprenticeshipsOrchestrator> _orchestrator;

        private EmployerManageApprenticesController _controller;

        private Mock<IOwinWrapper> _owinWrapper;

        private Mock<ICookieStorageService<FlashMessageViewModel>> _cookieStorage;

        private const string AccountId = "123";

        private const string ApprenticeshipId = "456";

        [SetUp]
        public void Setup()
        {
            _orchestrator = new Mock<IEmployerManageApprenticeshipsOrchestrator>();
            _owinWrapper = new Mock<IOwinWrapper>();
            _cookieStorage = new Mock<ICookieStorageService<FlashMessageViewModel>>();

            _controller = new EmployerManageApprenticesController(_orchestrator.Object, _owinWrapper.Object, Mock.Of<IMultiVariantTestingService>(),
                _cookieStorage.Object);
        }

        [Test]
        public async Task ThenAccessDeniedViewIsReturnedWhenUserNotAuthorised()
        {
            _orchestrator.Setup(o => o.AuthorizeRole(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Role[]>())).Returns(() => Task.FromResult(false));

            var result = await _controller.UpdateApprenticeshipStopDate(AccountId, ApprenticeshipId, null);

            Assert.AreEqual("AccessDenied", (result as ViewResult)?.ViewName);
        }

        [Test]
        public async Task ThenOrchestratorWillValidateApprenticeshipStatus()
        {
            _owinWrapper.Setup(x => x.GetClaimValue(It.IsAny<string>())).Returns(string.Empty);
            _orchestrator.Setup(o => o.AuthorizeRole(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Role[]>())).Returns(() => Task.FromResult(true));
            _orchestrator.Setup(o => o.GetApprenticeshipStopDateDetails(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(new OrchestratorResponse<EditApprenticeshipStopDateViewModel>()));

            var validationResult = new Dictionary<string, string> {{"error", "error"}};
            _orchestrator.Setup(o => o.ValidateApprenticeshipStopDate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<EditStopDateViewModel>())).Returns(Task.FromResult(validationResult)).Verifiable();

            var response = new OrchestratorResponse<EditApprenticeshipStopDateViewModel> {Data = new EditApprenticeshipStopDateViewModel()};
            _orchestrator.Setup(o => o.GetApprenticeshipStopDateDetails(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(response)).Verifiable();

            await _controller.UpdateApprenticeshipStopDate(AccountId, ApprenticeshipId, null);

            _orchestrator.Verify(o => o.ValidateApprenticeshipStopDate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<EditStopDateViewModel>()));
        }

        [Test]
        public async Task ThenOrchestratorWillReturnValidationErrors()
        {
            _owinWrapper.Setup(x => x.GetClaimValue(It.IsAny<string>())).Returns(string.Empty);
            _orchestrator.Setup(o => o.AuthorizeRole(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Role[]>())).Returns(() => Task.FromResult(true));
            _orchestrator.Setup(o => o.GetApprenticeshipStopDateDetails(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(new OrchestratorResponse<EditApprenticeshipStopDateViewModel>()));

            var validationResult = new Dictionary<string, string> { { "error", "error" } };
            _orchestrator.Setup(o => o.ValidateApprenticeshipStopDate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<EditStopDateViewModel>())).Returns(Task.FromResult(validationResult)).Verifiable();

            var response = new OrchestratorResponse<EditApprenticeshipStopDateViewModel> {Data = new EditApprenticeshipStopDateViewModel()};
            _orchestrator.Setup(o => o.GetApprenticeshipStopDateDetails(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(response)).Verifiable();

           var result =  await _controller.UpdateApprenticeshipStopDate(AccountId, ApprenticeshipId, null);

            var resultModel = result as ViewResult;

            Assert.AreEqual(1, ((OrchestratorResponse<EditApprenticeshipStopDateViewModel>)resultModel.Model).Data.ErrorDictionary.Count);
        }

        [Test]
        public async Task ThenOrchestratorWillUpdateStopDate()
        {
            _owinWrapper.Setup(x => x.GetClaimValue(It.IsAny<string>())).Returns(string.Empty);
            _orchestrator.Setup(o => o.AuthorizeRole(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Role[]>())).Returns(() => Task.FromResult(true));
            _orchestrator.Setup(o => o.GetApprenticeshipStopDateDetails(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(new OrchestratorResponse<EditApprenticeshipStopDateViewModel>()));

            var validationResult = new Dictionary<string, string>();
            _orchestrator.Setup(o => o.ValidateApprenticeshipStopDate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<EditStopDateViewModel>())).Returns(Task.FromResult(validationResult)).Verifiable();

            var response = new OrchestratorResponse<EditApprenticeshipStopDateViewModel> { Data = new EditApprenticeshipStopDateViewModel() };
            _orchestrator.Setup(o => o.GetApprenticeshipStopDateDetails(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(response)).Verifiable();

            var model = new EditStopDateViewModel();

            await _controller.UpdateApprenticeshipStopDate(AccountId, ApprenticeshipId, model);

            _orchestrator.Verify(x => x.UpdateStopDate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<EditStopDateViewModel>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));
        }

        [Test]
        public async Task ThenCookieWillBeUpdated()
        {
            _owinWrapper.Setup(x => x.GetClaimValue(It.IsAny<string>())).Returns(string.Empty);
            _orchestrator.Setup(o => o.AuthorizeRole(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Role[]>())).Returns(() => Task.FromResult(true));
            _orchestrator.Setup(o => o.GetApprenticeshipStopDateDetails(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(new OrchestratorResponse<EditApprenticeshipStopDateViewModel>()));

            var validationResult = new Dictionary<string, string>();
            _orchestrator.Setup(o => o.ValidateApprenticeshipStopDate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<EditStopDateViewModel>())).Returns(Task.FromResult(validationResult)).Verifiable();

            var response = new OrchestratorResponse<EditApprenticeshipStopDateViewModel> { Data = new EditApprenticeshipStopDateViewModel() };
            _orchestrator.Setup(o => o.GetApprenticeshipStopDateDetails(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(response)).Verifiable();

            _cookieStorage.Setup(x => x.Delete(It.IsAny<string>())).Verifiable();
            _cookieStorage.Setup(x => x.Create(It.IsAny<FlashMessageViewModel>(), It.IsAny<string>(), It.IsAny<int>())).Verifiable();

            var model = new EditStopDateViewModel();

            await _controller.UpdateApprenticeshipStopDate(AccountId, ApprenticeshipId, model);

            _cookieStorage.Verify(x => x.Delete(It.IsAny<string>()));
            _cookieStorage.Verify(x => x.Create(It.IsAny<FlashMessageViewModel>(), It.IsAny<string>(), It.IsAny<int>()));
        }
    }
}