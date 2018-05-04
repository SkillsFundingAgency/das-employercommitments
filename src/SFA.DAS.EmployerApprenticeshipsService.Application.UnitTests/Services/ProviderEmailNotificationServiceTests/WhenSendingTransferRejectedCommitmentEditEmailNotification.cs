using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.Commitments.Api.Types.Commitment.Types;
using SFA.DAS.EmployerCommitments.Application.Services;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;
using SFA.DAS.EmployerCommitments.Domain.Models.Notification;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerCommitments.Application.UnitTests.Services.ProviderEmailNotificationServiceTests
{
    [TestFixture]
    public class WhenSendingTransferRejectedCommitmentEditEmailNotification
    {
        private ProviderEmailNotificationService _providerEmailNotificationService;

        private Mock<IProviderEmailService> _providerEmailService;

        private CommitmentView _exampleCommitmentView;

        private EmailMessage _sentEmailMessage;

        private Func<Task> _act;

        [SetUp]
        public void Arrange()
        {
            _providerEmailService = new Mock<IProviderEmailService>();
            _providerEmailService.Setup(x =>
                x.SendEmailToAllProviderRecipients(It.IsAny<long>(), It.IsAny<string>(), It.IsAny<EmailMessage>()))
                .Callback<long, string, EmailMessage>((l, s, m) => _sentEmailMessage = m)
                .Returns(Task.CompletedTask);

            _providerEmailNotificationService =
                new ProviderEmailNotificationService(_providerEmailService.Object, Mock.Of<ILog>());

            _exampleCommitmentView = new CommitmentView
            {
                ProviderId = 1,
                ProviderLastUpdateInfo = new LastUpdateInfo{ EmailAddress = "LastUpdateEmail" },
                LegalEntityName = "Legal Entity Name",
                Reference = "Cohort Reference"
            };

            _act = async () =>
                await _providerEmailNotificationService.SendProviderTransferRejectedCommitmentEditNotification(
                    _exampleCommitmentView);
        }

        [TearDown]
        public void TearDown()
        {
            _sentEmailMessage = null;
        }

        [Test]
        public async Task ThenProviderEmailServiceIsCalledToSendEmail()
        {
            await _act.Invoke();

            _providerEmailService.Verify(x => x.SendEmailToAllProviderRecipients(
                It.Is<long>(l => l == _exampleCommitmentView.ProviderId),
                It.Is<string>((s => s == _exampleCommitmentView.ProviderLastUpdateInfo.EmailAddress)),
                It.IsAny<EmailMessage>()),
                Times.Once);
        }

        [Test]
        public async Task ThenEmailMessageTemplateIdIsSetCorrectly()
        {
            await _act.Invoke();

            Assert.AreEqual("ProviderTransferRejectedCommitmentEditNotification", _sentEmailMessage.TemplateId);
        }

        [Test]
        public async Task ThenEmailMessageIsTokenisedCorrectly()
        {
            await _act.Invoke();

            //Assert

            var expectedTokens = new Dictionary<string, string>
            {
                {"EmployerName", _exampleCommitmentView.LegalEntityName},
                {"CohortRef", _exampleCommitmentView.Reference}
            };

            CollectionAssert.AreEqual(expectedTokens, _sentEmailMessage.Tokens);
        }
    }
}
