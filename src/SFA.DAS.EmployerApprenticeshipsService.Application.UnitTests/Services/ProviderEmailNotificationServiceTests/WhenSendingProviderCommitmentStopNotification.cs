﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.EmployerCommitments.Application.Services;
using SFA.DAS.EmployerCommitments.Domain.Configuration;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;
using SFA.DAS.EmployerCommitments.Domain.Models.Notification;
using SFA.DAS.HashingService;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerCommitments.Application.UnitTests.Services.ProviderEmailNotificationServiceTests
{
    public class WhenSendingProviderCommitmentStopNotification
    {
        private ProviderEmailNotificationService _providerEmailNotificationService;

        private EmployerCommitmentsServiceConfiguration _configuration;

        private Mock<IProviderEmailService> _providerEmailService;
        private Mock<IHashingService> _hashingService;

        private Apprenticeship _exampleApprenticeship;
        private DateTime _exampleStopDate;

        private EmailMessage _sentEmailMessage;

        private Func<Task> _act;

        [SetUp]
        public void Arrange()
        {
            _configuration = new EmployerCommitmentsServiceConfiguration
            {
                CommitmentNotification = new CommitmentNotificationConfiguration
                {
                    SendEmail = true
                }
            };

            _providerEmailService = new Mock<IProviderEmailService>();
            _providerEmailService.Setup(x =>
                x.SendEmailToAllProviderRecipients(It.IsAny<long>(), It.IsAny<string>(), It.IsAny<EmailMessage>()))
                .Callback<long, string, EmailMessage>((l, s, m) => _sentEmailMessage = m)
                .Returns(Task.CompletedTask);

            _hashingService = new Mock<IHashingService>();
            _hashingService.Setup(x => x.HashValue(It.IsAny<long>())).Returns("HASH");

            _providerEmailNotificationService =
                new ProviderEmailNotificationService(_providerEmailService.Object, Mock.Of<ILog>(), _hashingService.Object, _configuration);

            var payload = new Apprenticeship
            {
                ProviderId = 123L,
                LegalEntityName = "Legal Entity Name",
                ULN = "1234567890",
                FirstName = "John",
                LastName = "Smith",
                StopDate = new DateTime(2018, 05, 01)
            };

            _exampleStopDate = new DateTime(2018, 05, 01);

            //Keep a copy of the payload to assert against, to guard against handler modifications
            _exampleApprenticeship =
                JsonConvert.DeserializeObject<Apprenticeship>(JsonConvert.SerializeObject(payload));

            _act = async () =>
                await _providerEmailNotificationService.SendProviderApprenticeshipStopNotification(
                    payload, _exampleStopDate);
        }

        [TearDown]
        public void TearDown()
        {
            _sentEmailMessage = null;
        }

        [Test]
        public async Task ThenProviderEmailServiceIsCalledToSendEmail()
        {
            await _act();

            _providerEmailService.Verify(x => x.SendEmailToAllProviderRecipients(
                It.Is<long>(l => l == _exampleApprenticeship.ProviderId),
                It.Is<string>((s => s == string.Empty)),
                It.IsAny<EmailMessage>()),
                Times.Once);
        }

        [Test]
        public async Task ThenEmailMessageTemplateIdIsSetCorrectly()
        {
            await _act();

            Assert.AreEqual("ProviderApprenticeshipStopNotification", _sentEmailMessage.TemplateId);
        }

        [Test]
        public async Task ThenEmailMessageIsTokenisedCorrectly()
        {
            await _act();

            //Assert
            var expectedTokens = new Dictionary<string, string>
            {
                {"EMPLOYER", _exampleApprenticeship.LegalEntityName},
                {"APPRENTICE", $"{_exampleApprenticeship.FirstName} {_exampleApprenticeship.LastName}"},
                {"DATE", _exampleStopDate.ToString("dd/MM/yyyy")},
                {"URL", $"{_exampleApprenticeship.ProviderId}/apprentices/manage/HASH/details" }
            };

            CollectionAssert.AreEquivalent(expectedTokens, _sentEmailMessage.Tokens);
        }
    }
}
