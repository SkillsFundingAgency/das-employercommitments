using System.Web.Http.Results;
using AutoFixture.NUnit3;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;
using SFA.DAS.EmployerCommitments.Domain.Models.FeatureToggles;
using SFA.DAS.EmployerCommitments.Web.Authentication;
using SFA.DAS.EmployerCommitments.Web.Controllers;
using SFA.DAS.EmployerCommitments.Web.Orchestrators;
using SFA.DAS.EmployerCommitments.Web.ViewModels;
using SFA.DAS.EmployerCommitments.Web.ViewModels.ManageApprenticeships;
using SFA.DAS.EmployerUrlHelper;
using SFA.DAS.Testing.AutoFixture;
using RedirectResult = System.Web.Mvc.RedirectResult;

namespace SFA.DAS.EmployerCommitments.Web.UnitTests.Controllers.EmployerManageApprenticeControllerTests
{
    [TestFixture]
    public class WhenGettingListAll
    {
        [Test, MoqAutoData]
        public void AndEmployerManageApprenticesV2ToggleIsEnabledThanRoutesToEmployerCommitmentsV2(
        EmployerManageApprenticeshipsOrchestrator orchestrator,
        [Frozen]Mock<IOwinWrapper> owinWrapper,
        [Frozen]Mock<IMultiVariantTestingService> multiVariantTestingService,
        [Frozen]Mock<ICookieStorageService<FlashMessageViewModel>> flashMessage,
        [Frozen]Mock<IFeatureToggleService> featureToggleService,
        [Frozen]Mock<ILinkGenerator> linkGenerator)
        {
            //Arrange
            var controller = new EmployerManageApprenticesController(
                orchestrator,
                owinWrapper.Object,
                multiVariantTestingService.Object,
                flashMessage.Object,
                linkGenerator.Object,
                featureToggleService.Object);

            featureToggleService.Setup(x => x.Get<EmployerManageApprenticesV2>().FeatureEnabled).Returns(true);

            //Act
            var actual = controller.ListAll("", new ApprenticeshipFiltersViewModel());

            //Assert
            Assert.IsInstanceOf<RedirectResult>(actual.Result);
        }
    }
}