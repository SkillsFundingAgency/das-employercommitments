using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.Commitments.Api.Types.Commitment.Types;
using SFA.DAS.EmployerCommitments.Application.Commands.SendNotification;
using SFA.DAS.EmployerCommitments.Application.Services;
using SFA.DAS.HashingService;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerCommitments.Application.UnitTests.Services.EmployerEmailNotificationServiceTests
{
    [TestFixture]
    public class WhenSendingSenderApprovedOrRejectedCommitmentNotification
    {
        private const string EmployerApprovedTemplateId = "SenderApprovedCommitmentEmployerNotification";
        private const string EmployerRejectedTemplateId = "SenderRejectedCommitmentEmployerNotification";

        private const string EmployersEmailAddress = "LastUpdateEmail@example.com";
        private const string CohortReference = "COREF";
        private const string EmployerName = "Employer Name";
        private const string SenderName = "Sender Name";
        const long EmployerAccountId = 5L;
        const string HashedEmployerAccountId = "6GVW8M";

        private EmployerEmailNotificationService _employermailNotificationService;
        private Mock<IMediator> _mediator;
        private Mock<IHashingService> _hashingService;
        private CommitmentView _exampleCommitmentView;
        private TransferApprovalStatus _transferApprovalStatus;
        private Dictionary<string, string> _tokens;
        private SendNotificationCommand _sendNotificationCommand;
        private Func<Task> _act;

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _mediator.Setup(m => m.SendAsync(It.IsAny<SendNotificationCommand>()))
                .Callback<IAsyncRequest<Unit>>(c => _sendNotificationCommand = c as SendNotificationCommand)
                .Returns(Task.FromResult(new Unit()));

            _hashingService = new Mock<IHashingService>();

            _employermailNotificationService =
                new EmployerEmailNotificationService(_mediator.Object, Mock.Of<ILog>(), _hashingService.Object);

            _exampleCommitmentView = new CommitmentView
            {
                EmployerLastUpdateInfo = new LastUpdateInfo { EmailAddress = EmployersEmailAddress },
                EmployerAccountId = EmployerAccountId,
                LegalEntityName = EmployerName,
                Reference = CohortReference,
                TransferSender = new TransferSender
                {
                    Name = SenderName
                }
            };

            _transferApprovalStatus = TransferApprovalStatus.Approved;

            _tokens = new Dictionary<string, string>
            {
                {"cohort_reference", CohortReference},
                {"employer_name", EmployerName },
                {"sender_name", SenderName },
                {"employer_hashed_account", HashedEmployerAccountId },
            };

            _act = async () =>
                await _employermailNotificationService.SendSenderApprovedOrRejectedCommitmentNotification(_exampleCommitmentView, _transferApprovalStatus);
        }

        [TearDown]
        public void TearDown()
        {
            _sendNotificationCommand = null;
        }

        [Test]
        public async Task ThenNotificationCommandIsCalledToSendEmail()
        {
            await _act();

            _mediator.Verify(x => x.SendAsync(It.IsAny<SendNotificationCommand>()), Times.Once);
        }

        [Test]
        public async Task ThenNoNotificationIsSentIfNoEmployerLastUpdateInfo()
        {
            _exampleCommitmentView.EmployerLastUpdateInfo = null;

            await _act();

            _mediator.Verify(x => x.SendAsync(It.IsAny<SendNotificationCommand>()), Times.Never);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public async Task ThenNoNotificationIsSentIfNoEmployerLastUpdateEmailAddress(string emailAddress)
        {
            _exampleCommitmentView.EmployerLastUpdateInfo.EmailAddress = emailAddress;

            await _act();

            _mediator.Verify(x => x.SendAsync(It.IsAny<SendNotificationCommand>()), Times.Never);
        }

        [Test]
        public async Task ThenEmployersEmailAddressIsPassedInEmailMessage()
        {
            await _act();

            Assert.AreEqual(EmployersEmailAddress, _sendNotificationCommand.Email.RecipientsAddress);
        }

        [TestCase(EmployerApprovedTemplateId, TransferApprovalStatus.Approved)]
        [TestCase(EmployerRejectedTemplateId, TransferApprovalStatus.Rejected)]
        public async Task ThenEmailMessageTemplateIdIsSetCorrectly(string expectedTemplateId, TransferApprovalStatus transferApprovalStatus)
        {
            _transferApprovalStatus = transferApprovalStatus;

            await _act();

            Assert.AreEqual(expectedTemplateId, _sendNotificationCommand.Email.TemplateId);
        }

        [Test]
        public async Task ThenCorrectTokensArePassedInEmailMessage()
        {
            _hashingService.Setup(h => h.HashValue(EmployerAccountId)).Returns(HashedEmployerAccountId);

            await _act();

            CollectionAssert.AreEqual(_tokens, _sendNotificationCommand.Email.Tokens);
        }

        [Test]
        public void AndSenderHasApprovedThenTemplateSubstitutionIsCorrect()
        {
            const string template = @"Dear ((employer_name)),

Your transfer request for cohort ((cohort_reference)) has been approved by ((sender_name)).

This is an automated message. Please do not reply to this email.

Kind regards,

Apprenticeship service team";

            string expectedEmailBody = $@"Dear {EmployerName},

Your transfer request for cohort {CohortReference} has been approved by {SenderName}.

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
            const string template = @"Dear ((employer_name)),

((sender_name)) has rejected your transfer request for cohort ((cohort_reference)).

What happens next?

Contact ((sender_name)) to agree what steps to take next.

To review and edit cohort ((cohort_reference)), follow the link below.
https://manage-apprenticeships.service.gov.uk/commitments/accounts/((employer_hashed_account))/apprentices/cohorts/transferFunded

This is an automated message. Please do not reply to this email.

Kind regards,

Apprenticeship service team";

            string expectedEmailBody = $@"Dear {EmployerName},

{SenderName} has rejected your transfer request for cohort {CohortReference}.

What happens next?

Contact {SenderName} to agree what steps to take next.

To review and edit cohort {CohortReference}, follow the link below.
https://manage-apprenticeships.service.gov.uk/commitments/accounts/{HashedEmployerAccountId}/apprentices/cohorts/transferFunded

This is an automated message. Please do not reply to this email.

Kind regards,

Apprenticeship service team";

            var emailBody = TestHelpers.PopulateTemplate(template, _tokens);

            TestContext.WriteLine(emailBody);
            Assert.AreEqual(expectedEmailBody, emailBody);
        }
    }
}
