using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types.ProviderPayment;
using SFA.DAS.EmployerCommitments.Application.Queries.GetProviderPaymentPriority;
using SFA.DAS.EmployerCommitments.Web.UnitTests.Orchestrators.EmployerCommitmentOrchestrator;

namespace SFA.DAS.EmployerCommitments.Web.UnitTests.Orchestrators.EmployerManageApprenticeshipsOrchestratorTests
{
    [TestFixture]
    public sealed class WhenGettingProviderPriorityList : EmployerManageApprenticeshipsOrchestratorTestBase
    {
        [Test]
        public async Task ReturnListOfProvidersInAlphabeticalOrder()
        {
            MockMediator.Setup(x => x.SendAsync(It.IsAny<GetProviderPaymentPriorityRequest>()))
                .ReturnsAsync(new GetProviderPaymentPriorityResponse
                {
                    Data = new List<ProviderPaymentPriorityItem>
                    {
                        new ProviderPaymentPriorityItem { PriorityOrder = 1, ProviderId = 11, ProviderName = "BBB" },
                        new ProviderPaymentPriorityItem { PriorityOrder = 2, ProviderId = 22, ProviderName = "CCC" },
                        new ProviderPaymentPriorityItem { PriorityOrder = 3, ProviderId = 33, ProviderName = "AAA" },
                    }
                });

            var response = await EmployerManageApprenticeshipsOrchestrator.GetPaymentOrder("123", "user123");

            var list = response.Data.Items.ToList();
            list[0].ProviderName.Should().Be("AAA");
            list[1].ProviderName.Should().Be("BBB");
            list[2].ProviderName.Should().Be("CCC");
        }

        [Test]
        public async Task ReturnNotFoundIfLessThan2ProvidersInTheList()
        {
            MockMediator.Setup(x => x.SendAsync(It.IsAny<GetProviderPaymentPriorityRequest>()))
                .ReturnsAsync(new GetProviderPaymentPriorityResponse
                {
                    Data = new List<ProviderPaymentPriorityItem> { new ProviderPaymentPriorityItem() }
                });

            var response = await EmployerManageApprenticeshipsOrchestrator.GetPaymentOrder("123", "user123");

            response.Status.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
