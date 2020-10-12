using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;
using SFA.DAS.EmployerCommitments.Infrastructure.Services;
using SFA.DAS.EmployerCommitments.Web.Authentication;
using SFA.DAS.EmployerCommitments.Web.Controllers;
using SFA.DAS.EmployerCommitments.Web.Orchestrators;
using SFA.DAS.EmployerCommitments.Web.ViewModels;
using SFA.DAS.EmployerCommitments.Web.ViewModels.ManageApprenticeships;
using SFA.DAS.EmployerCommitments.Domain.Models.UserProfile;
using SFA.DAS.EmployerUrlHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SFA.DAS.EmployerCommitments.Web.UnitTests.Controllers.EmployerManageApprenticeControllerTests
{
    public class WhenSelectingToPauseAnApprentice
    {
        private Mock<IEmployerManageApprenticeshipsOrchestrator> _orchestrator;
        private Mock<IOwinWrapper> _owinWrapper;
        private Mock<IMultiVariantTestingService> _multiVariantTestingService;
        private Mock<ICookieStorageService<FlashMessageViewModel>> _cookieService;
        private Mock<ILinkGenerator> _linkGenerator;

        private const string AccountId = "ACC123";
        private const string ApprenticeshipId = "APP123";
        private const string CommitmentsV2PauseUrl = "https://commitments.apprenticeships.gov.uk/pause";

        private EmployerManageApprenticesController _controller;

        [SetUp]
        public void Setup()
        {
            _orchestrator = new Mock<IEmployerManageApprenticeshipsOrchestrator>();
            _owinWrapper = new Mock<IOwinWrapper>();
            _multiVariantTestingService = new Mock<IMultiVariantTestingService>();
            _cookieService = new Mock<ICookieStorageService<FlashMessageViewModel>>();
            _linkGenerator = new Mock<ILinkGenerator>();

            _controller = new EmployerManageApprenticesController(
                _orchestrator.Object, _owinWrapper.Object, _multiVariantTestingService.Object, _cookieService.Object, _linkGenerator.Object);
        }

        [Test]
        public async Task WhenRequestingChangeStatusPage_AndStatusIsNotCurrentlyPaused_ThenRedirectToV2ChangeStatusPage()
        {
            _owinWrapper.Setup(o => o.GetClaimValue(@"sub"))
                .Returns("ClaimValue");
            _orchestrator.Setup(a => a.AuthorizeRole(AccountId, It.IsAny<string>(), new Role[] { Role.Owner, Role.Transactor }))
                .ReturnsAsync(true);
            _orchestrator.Setup(r => r.GetChangeStatusChoiceNavigation(AccountId, ApprenticeshipId, It.IsAny<string>()))
                .ReturnsAsync(GetOrchestratorResponse());
            _linkGenerator.Setup(l => l.CommitmentsV2Link(It.IsAny<string>())).Returns(CommitmentsV2PauseUrl);

            var response = await _controller.ChangeStatus(AccountId, ApprenticeshipId);
            RedirectResult result = (RedirectResult)response;
            
            Assert.AreEqual(CommitmentsV2PauseUrl, result.Url);
        }

        private OrchestratorResponse<ChangeStatusChoiceViewModel> GetOrchestratorResponse()
        {
            return new OrchestratorResponse<ChangeStatusChoiceViewModel>
            {
                Data = new ChangeStatusChoiceViewModel
                {
                    IsCurrentlyPaused = false
                }
            };
        }
    }
}
