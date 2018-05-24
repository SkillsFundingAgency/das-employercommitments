using System;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.Apprenticeship.Types;
using SFA.DAS.EmployerCommitments.Application.Exceptions;
using SFA.DAS.EmployerCommitments.Application.Queries.GetApprenticeship;
using SFA.DAS.EmployerCommitments.Application.Queries.GetApprenticeshipUpdate;

namespace SFA.DAS.EmployerCommitments.Web.UnitTests.Orchestrators.EmployerManageApprenticeshipsOrchestratorTests
{
    [TestFixture]
    public class WhenGettingViewChanges : ManageApprenticeshipsOrchestratorTestBase
    {
        [Test]
        public void ThenInvalidStateExceptionOccursWhenNoViewModelReturned()
        {
            MockDateTime.Setup(m => m.Now).Returns(new DateTime(1998, 1, 1));
            MockMediator.Setup(m => m.SendAsync(It.IsAny<GetApprenticeshipQueryRequest>()))
                .ReturnsAsync(new GetApprenticeshipQueryResponse
                {
                    Apprenticeship =
                        new Apprenticeship
                        {
                            PaymentStatus = PaymentStatus.Active,
                            StartDate = new DateTime(1998, 1, 1)
                        }
                });
            MockMediator.Setup(m => m.SendAsync(It.IsAny<GetApprenticeshipUpdateRequest>()))
                .ReturnsAsync(new GetApprenticeshipUpdateResponse());

            Assert.ThrowsAsync<InvalidStateException>(() => Orchestrator.GetViewChangesViewModel("hashedAccountId", "hashedApprenticeshipId", "UserId"));
        }
    }
}