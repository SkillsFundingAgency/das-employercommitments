using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.Commitments.Api.Types.Commitment.Types;
using SFA.DAS.EmployerCommitments.Application.Services;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;
using SFA.DAS.EmployerCommitments.Domain.Models.Notification;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerCommitments.Application.UnitTests.Services.ProviderEmailNotificationServiceTests
{
    [TestFixture]
    public class WhenSendingSenderApprovedOrRejectedCommitmentNotification
    {
        private const string ProviderApprovedTemplateId = "SenderApprovedCommitmentProviderNotification";
        private const string ProviderRejectedTemplateId = "SenderRejectedCommitmentProviderNotification";

        private ProviderEmailNotificationService _providerEmailNotificationService;
        private Mock<IProviderEmailService> _providerEmailService;
        private CommitmentView _exampleCommitmentView;
        private TransferApprovalStatus _transferApprovalStatus;
        private Dictionary<string, string> _tokens;
        private EmailMessage _sentEmailMessage;
        private Func<Task> _act;

        //todo: move to base?
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
                ProviderLastUpdateInfo = new LastUpdateInfo { EmailAddress = "LastUpdateEmail" },
                LegalEntityName = "Legal Entity Name",
                Reference = "Cohort Reference"
            };

            _transferApprovalStatus = TransferApprovalStatus.Approved;

            _tokens = new Dictionary<string, string>
            {
                {"cohort_reference", "COREF"},
                {"ukprn", "UKPRN"},
                {"employer_name", "Employer Name" },
                {"sender_name", "Sender Name" },
                {"employer_hashed_account", "EMPHASH" },
            };

            _act = async () =>
                await _providerEmailNotificationService.SendSenderApprovedOrRejectedCommitmentNotification(_exampleCommitmentView, _transferApprovalStatus, _tokens);
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
                It.Is<long>(l => l == _exampleCommitmentView.ProviderId),
                It.Is<string>(s => s == _exampleCommitmentView.ProviderLastUpdateInfo.EmailAddress),
                It.IsAny<EmailMessage>()),
                Times.Once);
        }

        [TestCase(ProviderApprovedTemplateId, TransferApprovalStatus.Approved)]
        [TestCase(ProviderRejectedTemplateId, TransferApprovalStatus.Rejected)]
        public async Task ThenEmailMessageTemplateIdIsSetCorrectly(string expectedTemplateId, TransferApprovalStatus transferApprovalStatus)
        {
            _transferApprovalStatus = transferApprovalStatus;

            await _act();

            Assert.AreEqual(expectedTemplateId, _sentEmailMessage.TemplateId);
        }

        [Test]
        public async Task ThenSuppliedTokensArePassedInEmailMessage()
        {
            var expectedTokens = TestHelpers.Clone(_tokens);

            await _act();

            CollectionAssert.AreEqual(expectedTokens, _sentEmailMessage.Tokens);
        }
    }
}
