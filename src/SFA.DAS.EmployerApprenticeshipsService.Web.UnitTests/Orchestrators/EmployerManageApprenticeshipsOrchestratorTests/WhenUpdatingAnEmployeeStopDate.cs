using System;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.Apprenticeship.Types;
using SFA.DAS.EmployerCommitments.Application.Commands.UpdateApprenticeshipStatus;
using SFA.DAS.EmployerCommitments.Application.Queries.GetApprenticeship;
using SFA.DAS.EmployerCommitments.Web.ViewModels;
using SFA.DAS.EmployerCommitments.Web.ViewModels.ManageApprenticeships;

namespace SFA.DAS.EmployerCommitments.Web.UnitTests.Orchestrators.EmployerManageApprenticeshipsOrchestratorTests
{
    [TestFixture]
    public class WhenUpdatingAnEmployeeStopDate : ManageApprenticeshipsOrchestratorTestBase
    {
        [Test]
        public async Task ThenNewStopDateIsApplied()
        {
            var updateDate = DateTime.Today;

            MockMediator.Setup(x => x.SendAsync(It.IsAny<GetApprenticeshipQueryRequest>())).Returns(Task.FromResult(new GetApprenticeshipQueryResponse {Apprenticeship = new Apprenticeship {PaymentStatus = PaymentStatus.Withdrawn} }));

            MockMediator.Setup(x => x.SendAsync(It.IsAny<UpdateApprenticeshipStatusCommand>())).Callback<IAsyncRequest<Unit>>(u =>
            {
                var command = u as UpdateApprenticeshipStatusCommand;
                Assert.AreEqual(updateDate, command.DateOfChange);
                Assert.AreEqual((int)ChangeStatusType.Stop, (int)command.ChangeType);
            }).Returns(Task.FromResult(new Unit()));

            const string accountId = "accountid";
            const string apprenticeId = "apprenticeId";

            var updateDateModel = new DateTimeViewModel {Day = updateDate.Day, Month = updateDate.Month, Year = updateDate.Year};
            
            var newStopDate = new ChangeStatusViewModel
            {
                ChangeType = ChangeStatusType.Stop,
                DateOfChange = updateDateModel,
                WhenToMakeChange = WhenToMakeChangeOptions.SpecificDate,
                ChangeConfirmed = true
            };

            await Orchestrator.UpdateStatus(accountId, apprenticeId, newStopDate, string.Empty, string.Empty, string.Empty);
        }
    }
}
