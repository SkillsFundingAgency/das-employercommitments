using System.Threading.Tasks;
using AutoFixture.NUnit3;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerCommitments.Application.Commands.SendNotification;
using SFA.DAS.Notifications.Api.Client;

namespace SFA.DAS.EmployerCommitments.Infrastructure.UnitTests.Services.BackgroundNotificationTests
{
    [TestFixture]
    public class WhenSendingBackgroundNotification
    {
        [Test, EmployerCommitmentsAutoData, Ignore("todo")]
        public async Task ThenSendsEmailToNotificationApi(
            SendNotificationCommand command,
            [Frozen] Mock<INotificationsApi> mockNotificationsApi,
            SendNotificationCommandHandler sut)
        {
            await sut.Handle(command);

            mockNotificationsApi.Verify(api => api.SendEmail(command.Email), Times.Once);
        }
    }
}