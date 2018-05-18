using System.Collections.Generic;
using System.Threading.Tasks;
using FeatureToggle;
using FluentAssertions;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types.ProviderPayment;
using SFA.DAS.EmployerCommitments.Application.Queries.GetProviderPaymentPriority;
using SFA.DAS.EmployerCommitments.Application.Queries.GetUserAccountRole;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;
using SFA.DAS.EmployerCommitments.Domain.Models.AccountTeam;
using SFA.DAS.EmployerCommitments.Domain.Models.FeatureToggles;
using SFA.DAS.EmployerCommitments.Web.Orchestrators;
using SFA.DAS.EmployerCommitments.Web.Orchestrators.Mappers;
using SFA.DAS.EmployerCommitments.Web.PublicHashingService;
using SFA.DAS.NLog.Logger;
using SFA.DAS.HashingService;

namespace SFA.DAS.EmployerCommitments.Web.UnitTests.Orchestrators.EmployerCommitmentOrchestrator
{
    [TestFixture]
    public sealed class WhenGettingIndexView
    {
        private Mock<IMediator> _mediator;
        private Mock<IFeatureToggleService> _featureToggleService;
        private EmployerCommitmentsOrchestrator _orchestrator;

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetUserAccountRoleQuery>()))
                .ReturnsAsync(new GetUserAccountRoleResponse { User = new TeamMember() });
            var logger = new Mock<ILog>();
            var hashingService = new Mock<IHashingService>();
            _featureToggleService = new Mock<IFeatureToggleService>();

            _featureToggleService.Setup(x => x.Get<PublicSectorReporting>()).Returns(new Mock<IFeatureToggle>().Object);

            hashingService.Setup(x => x.DecodeValue("ABC123")).Returns(123L);
    
            _orchestrator = new EmployerCommitmentsOrchestrator(
                _mediator.Object,
                hashingService.Object,
                Mock.Of<IPublicHashingService>(),
                Mock.Of<IApprenticeshipMapper>(), 
                Mock.Of<ICommitmentMapper>(),
                logger.Object,
                _featureToggleService.Object);
        }

        [Test]
        public async Task ShouldIndicateToHidePaymentPriorityLinkWhenLessThan2ValidProviders()
        {
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetProviderPaymentPriorityRequest>()))
                .ReturnsAsync(new GetProviderPaymentPriorityResponse
                {
                    Data = new List<ProviderPaymentPriorityItem>
                    {
                        new ProviderPaymentPriorityItem { PriorityOrder = 1, ProviderId = 11, ProviderName = "BBB" }
                    }
                });

            var response = await _orchestrator.GetIndexViewModel("123", "user123");

            response.Data.ShowSetPaymentPriorityLink.Should().BeFalse();
        }

        [Test]
        public async Task ShouldIndicateToShowPaymentPriorityLinkWhen2OrMoreValidProviders()
        {
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetProviderPaymentPriorityRequest>()))
                .ReturnsAsync(new GetProviderPaymentPriorityResponse
                {
                    Data = new List<ProviderPaymentPriorityItem>
                    {
                        new ProviderPaymentPriorityItem { PriorityOrder = 1, ProviderId = 11, ProviderName = "BBB" },
                        new ProviderPaymentPriorityItem { PriorityOrder = 2, ProviderId = 22, ProviderName = "CCC" },
                    }
                });

            var response = await _orchestrator.GetIndexViewModel("123", "user123");

            response.Data.ShowSetPaymentPriorityLink.Should().BeTrue();
        }

        [Test]
        public async Task ShouldIndicateToShowPublicSectorReportingLinkWhenTheFeatureIsEnabled()
        {
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetProviderPaymentPriorityRequest>())).ReturnsAsync(new GetProviderPaymentPriorityResponse { Data = new List<ProviderPaymentPriorityItem>() });

            var feature = new Mock<IFeatureToggle>();
            feature.SetupGet(x => x.FeatureEnabled).Returns(true);
            _featureToggleService.Setup(x => x.Get<PublicSectorReporting>()).Returns(feature.Object);

            var response = await _orchestrator.GetIndexViewModel("123", "user123");

            response.Data.ShowPublicSectorReportingLink.Should().BeTrue();
        }

        [Test]
        public async Task ShouldIndicateToHidePublicSectorReportingLinkWhenTheFeatureIsNotEnabled()
        {
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetProviderPaymentPriorityRequest>())).ReturnsAsync(new GetProviderPaymentPriorityResponse { Data = new List<ProviderPaymentPriorityItem>() });

            var feature = new Mock<IFeatureToggle>();
            feature.SetupGet(x => x.FeatureEnabled).Returns(false);
            _featureToggleService.Setup(x => x.Get<PublicSectorReporting>()).Returns(feature.Object);

            var response = await _orchestrator.GetIndexViewModel("123", "user123");

            response.Data.ShowPublicSectorReportingLink.Should().BeFalse();
        }
    }
}
