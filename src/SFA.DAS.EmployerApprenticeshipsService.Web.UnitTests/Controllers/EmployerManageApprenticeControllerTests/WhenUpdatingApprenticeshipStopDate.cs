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
    public class WhenUpdatingApprenticeshipStopDate
    {
        private Mock<IEmployerManageApprenticeshipsOrchestrator> _orchestrator;
        private EmployerManageApprenticesController _controller;
        private Mock<IOwinWrapper> _owinWrapper;
        private Mock<ICookieStorageService<FlashMessageViewModel>> _cookieStorage;
        private Mock<ILinkGenerator> _linkGenerator;
        private const string AccountId = "123";
        private const string ApprenticeshipId = "456";
        private const string CommitmentsV2DetailsUrl = "https://commitments.apprenticeships.gov.uk/details";

        [SetUp]
        public void Setup()
        {
            _orchestrator = new Mock<IEmployerManageApprenticeshipsOrchestrator>();
            _owinWrapper = new Mock<IOwinWrapper>();
            _cookieStorage = new Mock<ICookieStorageService<FlashMessageViewModel>>();
            _linkGenerator = new Mock<ILinkGenerator>();
            _linkGenerator.Setup(l => l.CommitmentsV2Link(It.IsAny<string>())).Returns(CommitmentsV2DetailsUrl);

            _controller = new EmployerManageApprenticesController(_orchestrator.Object, _owinWrapper.Object, Mock.Of<IMultiVariantTestingService>(),
                _cookieStorage.Object, _linkGenerator.Object, Mock.Of<ILog>());
        }

       
       
      
    }
}