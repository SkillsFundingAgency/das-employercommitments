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
using SFA.DAS.HashingService;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerCommitments.Application.UnitTests.Services.ProviderEmailNotificationServiceTests
{
    [TestFixture]
    public class WhenSendingSenderApprovedOrRejectedCommitmentNotification
    {
        private const string ProviderApprovedTemplateId = "SenderApprovedCommitmentProviderNotification";
        private const string ProviderRejectedTemplateId = "SenderRejectedCommitmentProviderNotification";

        private const string ProviderLastUpdatedByEmail = "providerbob@example.com";
        private const long ProviderId = 987654321L;
        private const string CohortReference = "COREF";

        private ProviderEmailNotificationService _providerEmailNotificationService;
        private Mock<IProviderEmailService> _providerEmailService;
        private CommitmentView _exampleCommitmentView;
        private TransferApprovalStatus _transferApprovalStatus;
        private Dictionary<string, string> _tokens;
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
                new ProviderEmailNotificationService(_providerEmailService.Object, Mock.Of<ILog>(), Mock.Of<IHashingService>());

            _exampleCommitmentView = new CommitmentView
            {
                Reference = CohortReference,
                ProviderId = ProviderId,
                ProviderLastUpdateInfo = new LastUpdateInfo { EmailAddress = ProviderLastUpdatedByEmail }
            };

            _transferApprovalStatus = TransferApprovalStatus.Approved;

            _tokens = new Dictionary<string, string>
            {
                {"cohort_reference", CohortReference },
                {"ukprn", ProviderId.ToString() }
            };

            _act = async () =>
                await _providerEmailNotificationService.SendSenderApprovedOrRejectedCommitmentNotification(_exampleCommitmentView, _transferApprovalStatus);
        }

        [TearDown]
        public void TearDown()
        {
            _sentEmailMessage = null;
        }

        [Test]
        public async Task ThenProviderEmailServiceIsCalledToSendEmailWithCorrectEmailAddressAndProviderId()
        {
            await _act();

            _providerEmailService.Verify(x => x.SendEmailToAllProviderRecipients(
                It.Is<long>(l => l == ProviderId),
                It.Is<string>(s => s == ProviderLastUpdatedByEmail),
                It.IsAny<EmailMessage>()),
                Times.Once);
        }

        [Test]
        public async Task AndNullProviderLastUpdateInfoThenProviderEmailServiceIsCalledWithEmptyEmail()
        {
            _exampleCommitmentView.ProviderLastUpdateInfo = null;

            await _act();

            _providerEmailService.Verify(x => x.SendEmailToAllProviderRecipients(
                    It.IsAny<long>(),
                    It.Is<string>(s => s == ""),
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
        public async Task ThenCorrectTokensArePassedInEmailMessage()
        {
            await _act();

            CollectionAssert.AreEqual(_tokens, _sentEmailMessage.Tokens);
        }

        [Test]
        public void AndSenderHasApprovedThenTemplateSubstitutionIsCorrect()
        {
            const string template = @"The transfer of funds for cohort ((cohort_reference)) has been approved.

This is an automated message. Please do not reply to this email.

Kind regards,

Apprenticeship service team";

            string expectedEmailBody = $@"The transfer of funds for cohort {CohortReference} has been approved.

This is an automated message. Please do not reply to this email.

Kind regards,

Apprenticeship service team";

            var emailBody = TestHelpers.PopulateTemplate(template, _tokens);

            TestContext.WriteLine(emailBody);
            Assert.AreEqual(expectedEmailBody, emailBody);
        }

        [Test]
        public void AndSenderHasRejectedThenTemplateSubstitutionIsCorrect()
        {
            const string template = @"The sending employer rejected the transfer request for cohort ((cohort_reference)).

What happens next?

Funds for apprenticeship training won’t be available until the receiving employer, sending employer and yourself agree the cohort details.

You need to contact the receiving employer to discuss what steps to take next. To view cohort ((cohort_reference)), follow the link below.

https://providers.apprenticeships.sfa.bis.gov.uk/((ukprn))/apprentices/cohorts/transferfunded

This is an automated message. Please do not reply to this email.

Kind regards,

Apprenticeship service team";

            string expectedEmailBody = $@"The sending employer rejected the transfer request for cohort {CohortReference}.

What happens next?

Funds for apprenticeship training won’t be available until the receiving employer, sending employer and yourself agree the cohort details.

You need to contact the receiving employer to discuss what steps to take next. To view cohort {CohortReference}, follow the link below.

https://providers.apprenticeships.sfa.bis.gov.uk/{ProviderId}/apprentices/cohorts/transferfunded

This is an automated message. Please do not reply to this email.

Kind regards,

Apprenticeship service team";

            var emailBody = TestHelpers.PopulateTemplate(template, _tokens);

            TestContext.WriteLine(emailBody);
            Assert.AreEqual(expectedEmailBody, emailBody);
        }
    }
}
