using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;
using SFA.DAS.EmployerCommitments.Domain.Models.UserProfile;
using SFA.DAS.EmployerCommitments.Web.Authentication;
using SFA.DAS.EmployerCommitments.Web.Controllers;
using SFA.DAS.EmployerCommitments.Web.Orchestrators;
using SFA.DAS.EmployerCommitments.Web.ViewModels;
using SFA.DAS.EmployerUrlHelper;

namespace SFA.DAS.EmployerCommitments.Web.UnitTests.Controllers.EmployerCommitmentsControllerTests
{
    public class EmployerCommitmentsControllerTest : ControllerTest
    {
        protected Mock<IEmployerCommitmentsOrchestrator> Orchestrator;
        protected EmployerCommitmentsController Controller;

        [SetUp]
        public void EmployerCommitmentsControllerSetup()
        {
            Orchestrator = new Mock<IEmployerCommitmentsOrchestrator>();

            Orchestrator.Setup(o => o.AuthorizeRole(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Role[]>())).ReturnsAsync(true);

            Controller = new EmployerCommitmentsController(Orchestrator.Object, Mock.Of<IOwinWrapper>(), Mock.Of<IMultiVariantTestingService>(),
                Mock.Of<ICookieStorageService<FlashMessageViewModel>>(), Mock.Of<ICookieStorageService<string>>(), Mock.Of<IFeatureToggleService>(), Mock.Of<ILinkGenerator
                >());
        }
    }
}
