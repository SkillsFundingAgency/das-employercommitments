using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerCommitments.Application.Services;
using SFA.DAS.EmployerCommitments.Domain.Configuration;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;
using SFA.DAS.EmployerCommitments.Domain.Models.Notification;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Notifications.Api.Client;
using SFA.DAS.Notifications.Api.Types;

namespace SFA.DAS.EmployerCommitments.Application.UnitTests.Services.ProviderEmailServiceTests
{
    [TestFixture]
    public class WhenISendEmailToAllProviderRecipients
    {
        private ProviderEmailService _providerEmailService;
        private Mock<IProviderEmailLookupService> _providerEmailLookupService;
        private Mock<INotificationsApi> _notificationsApi;

        private long _providerId;
        private string _providerLastUpdateEmailAddress;
        private CommitmentNotificationConfiguration _commitmentNotificationConfiguration;
        private List<string> _exampleRecipients;
        private EmailMessage _exampleValidEmail;

        private List<Email> _sentEmails;

        private Func<Task> _act;

        [SetUp]
        public void Arrange()
        {
            _providerId = 1;
            _providerLastUpdateEmailAddress = "AlternativeEmailAddress";

            _commitmentNotificationConfiguration = new CommitmentNotificationConfiguration {SendEmail = true};

            _exampleRecipients = new List<string> { "Recipient1", "Recipient2", "Recipient3" };
            _providerEmailLookupService = new Mock<IProviderEmailLookupService>();
            _providerEmailLookupService.Setup(x => x.GetEmailsAsync(It.IsAny<long>(), It.IsAny<string>()))
                .ReturnsAsync(_exampleRecipients);

            _sentEmails = new List<Email>();
            _notificationsApi = new Mock<INotificationsApi>();
            _notificationsApi.Setup(x => x.SendEmail(It.IsAny<Email>()))
                .Callback<Email>(email => _sentEmails.Add(email))
                .Returns(() => Task.CompletedTask);

            _providerEmailService = new ProviderEmailService(_providerEmailLookupService.Object,
                _notificationsApi.Object, Mock.Of<ILog>(),
                new EmployerCommitmentsServiceConfiguration
                {
                    CommitmentNotification = _commitmentNotificationConfiguration
                });

            _exampleValidEmail = new EmailMessage
            {
                TemplateId = "TemplateId",
                Tokens = new Dictionary<string, string>
                {
                    { "Key1", "Value1" },
                    { "Key2", "Value2" }
                }
            };

            _act = async () =>  await _providerEmailService.SendEmailToAllProviderRecipients(
                _providerId, _providerLastUpdateEmailAddress, _exampleValidEmail);
        }

        [TearDown]
        public void TearDown()
        {
            _sentEmails.Clear();
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

            CollectionAssert.AreEquivalent(_exampleRecipients, _sentEmails.Select(e => e.RecipientsAddress));
        }

        [Test]
        public async Task ThenIfSendingEmailIsDisabledInConfigurationThenNoEmailsAreSent()
        {
            //Arrange
            _commitmentNotificationConfiguration.SendEmail = false;

            //Act
            await _act.Invoke();

            //Assert
            Assert.IsEmpty(_sentEmails);
        }

        [Test]
        public async Task ThenEmailMessagePropertiesAreMappedCorrectly()
        {
            await _act.Invoke();

            Assert.AreEqual(_exampleValidEmail.TemplateId, _sentEmails[0].TemplateId);
            CollectionAssert.AreEqual(_exampleValidEmail.Tokens, _sentEmails[0].Tokens);
        }
    }
}
