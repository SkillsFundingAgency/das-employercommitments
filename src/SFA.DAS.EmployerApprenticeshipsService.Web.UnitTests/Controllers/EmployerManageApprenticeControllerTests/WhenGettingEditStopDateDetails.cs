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
                Mock.Of<ICookieStorageService<FlashMessageViewModel>>(), Mock.Of<ILinkGenerator>(), Mock.Of<ILog>());
        }

      
    }
}
