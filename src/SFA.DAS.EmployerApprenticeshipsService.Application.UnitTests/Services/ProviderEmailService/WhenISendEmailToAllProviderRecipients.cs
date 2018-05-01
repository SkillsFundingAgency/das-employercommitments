using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;
using SFA.DAS.Notifications.Api.Client;
using SFA.DAS.Notifications.Api.Types;

namespace SFA.DAS.EmployerCommitments.Application.UnitTests.Services.ProviderEmailService
{
    [TestFixture]
    public class WhenISendEmailToAllProviderRecipients
    {
        private Application.Services.ProviderEmailService _providerEmailService;
        private Mock<IProviderEmailLookupService> _providerEmailLookupService;
        private Mock<INotificationsApi> _notificationsApi;

        private long _providerId;
        private string _providerLastUpdateEmailAddress;
        private List<string> _exampleRecipients;
        private Email _exampleValidEmail;

        private Func<Task> _act;

        [SetUp]
        public void Arrange()
        {
            _providerId = 1;
            _providerLastUpdateEmailAddress = "AlternativeEmailAddress";

            _exampleRecipients = new List<string> { "Recipient1", "Recipient2", "Recipient3" };
            _providerEmailLookupService = new Mock<IProviderEmailLookupService>();
            _providerEmailLookupService.Setup(x => x.GetEmailsAsync(It.IsAny<long>(), It.IsAny<string>()))
                .Returns(() => new Task<List<string>>(() => _exampleRecipients));

            _notificationsApi = new Mock<INotificationsApi>();
            _notificationsApi.Setup(x => x.SendEmail(It.IsAny<Email>()))
                .Returns(() => Task.CompletedTask);

            _providerEmailService = new Application.Services.ProviderEmailService(_providerEmailLookupService.Object, _notificationsApi.Object);

            _exampleValidEmail = new Email
            {
                TemplateId = "TemplateId",
                Subject ="SubjectId",
                ReplyToAddress = "ReplyToAddress",
                Tokens = new Dictionary<string, string>
                {
                    { "Key1", "Value1" },
                    { "Key2", "Value2" }
                }
            };

            _act = () => _providerEmailService.SendEmailToAllProviderRecipients(
                _providerId, _providerLastUpdateEmailAddress, _exampleValidEmail);
        }

        [Test]
        public async Task ThenTheProviderEmailLookupServiceIsCalledToRetrieveRecipientAddresses()
        {
            await _act.Invoke();

            _providerEmailLookupService.Verify(x => x.GetEmailsAsync(
                    It.Is<long>(l =>  l == _providerId),
                    It.Is<string>(s => s == _providerLastUpdateEmailAddress)
                ), Times.Once);
        }



        [Test]
        public async Task ThenTheNotificationsApiIsCalledToSendAnEmailForEachRecipient()
        {
            await _act.Invoke();

            _notificationsApi.Verify(x => x.SendEmail(It.IsAny<Email>())
                , Times.Exactly(_exampleRecipients.Count));

        }


    }
}
