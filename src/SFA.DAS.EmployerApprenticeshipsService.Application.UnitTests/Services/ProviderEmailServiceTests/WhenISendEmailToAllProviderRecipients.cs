﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerCommitments.Application.Services;
using SFA.DAS.EmployerCommitments.Domain.Configuration;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;
using SFA.DAS.EmployerCommitments.Domain.Models.Notification;
using SFA.DAS.EmployerCommitments.Infrastructure.Services;
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
        private Mock<IBackgroundNotificationService> _backgroundNotificationService;

        private long _providerId;
        private string _providerLastUpdateEmailAddress;
        private EmployerCommitmentsServiceConfiguration _configuration;
        private List<string> _exampleRecipients;
        private EmailMessage _exampleValidEmail;

        private List<Email> _sentEmails;

        private Func<Task> _act;

        [SetUp]
        public void Arrange()
        {
            _providerId = 1;
            _providerLastUpdateEmailAddress = "AlternativeEmailAddress";

            _configuration = new EmployerCommitmentsServiceConfiguration
            {
                CommitmentNotification = new CommitmentNotificationConfiguration {SendEmail = true}
            };

            _exampleRecipients = new List<string> { "Recipient1@example.com", "Recipient2@example.com", "Recipient3@example.com" };
            _providerEmailLookupService = new Mock<IProviderEmailLookupService>();
            _providerEmailLookupService.Setup(x => x.GetEmailsAsync(It.IsAny<long>(), It.IsAny<string>()))
                .ReturnsAsync(_exampleRecipients);

            _sentEmails = new List<Email>();
            _backgroundNotificationService = new Mock<IBackgroundNotificationService>();
            _backgroundNotificationService.Setup(x => x.SendEmail(It.IsAny<Email>()))
                .Callback<Email>(email => _sentEmails.Add(email))
                .Returns(Task.CompletedTask);

            _providerEmailService = new ProviderEmailService(Mock.Of<ILog>(),
                _configuration, Mock.Of<ProviderNotifyService>());

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
            await _act();

            _providerEmailLookupService.Verify(x => x.GetEmailsAsync(
                    It.Is<long>(l =>  l == _providerId),
                    It.Is<string>(s => s == _providerLastUpdateEmailAddress)
                ), Times.Once);
        }

        [Test]
        public async Task ThenTheNotificationsApiIsCalledToSendAnEmailForEachRecipient()
        {
            await _act();

            CollectionAssert.AreEquivalent(_exampleRecipients, _sentEmails.Select(e => e.RecipientsAddress));
        }

        [Test]
        public async Task ThenIfSendingEmailIsDisabledInConfigurationThenNoEmailsAreSent()
        {
            //Arrange
            _configuration.CommitmentNotification.SendEmail = false;

            //Act
            await _act();

            //Assert
            Assert.IsEmpty(_sentEmails);
        }

        [Test]
        public async Task ThenEmailMessagePropertiesAreMappedCorrectly()
        {
            await _act();

            Assert.AreEqual(_exampleValidEmail.TemplateId, _sentEmails[0].TemplateId);
            CollectionAssert.AreEquivalent(_exampleValidEmail.Tokens, _sentEmails[0].Tokens);
        }
    }
}
