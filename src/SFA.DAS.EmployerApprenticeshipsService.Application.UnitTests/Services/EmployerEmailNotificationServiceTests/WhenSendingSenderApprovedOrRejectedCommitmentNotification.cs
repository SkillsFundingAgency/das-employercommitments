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
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerCommitments.Application.UnitTests.Services.EmployerEmailNotificationServiceTests
{
    [TestFixture]
    public class WhenSendingSenderApprovedOrRejectedCommitmentNotification
    {
        private const string EmployerApprovedTemplateId = "SenderApprovedCommitmentEmployerNotification";
        private const string EmployerRejectedTemplateId = "SenderRejectedCommitmentEmployerNotification";

        private EmployerEmailNotificationService _employermailNotificationService;
        private Mock<IMediator> _mediator;
        private CommitmentView _exampleCommitmentView;
        private TransferApprovalStatus _transferApprovalStatus;
        private Dictionary<string, string> _tokens;
        private SendNotificationCommand _sendNotificationCommand;
        private Func<Task> _act;

        //todo: move to base?
        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _mediator.Setup(m => m.SendAsync(It.IsAny<SendNotificationCommand>()))
                .Callback<IAsyncRequest<Unit>>(c => _sendNotificationCommand = c as SendNotificationCommand)
                .Returns(Task.FromResult(new Unit()));

            _employermailNotificationService =
                new EmployerEmailNotificationService(_mediator.Object, Mock.Of<ILog>());

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
                await _employermailNotificationService.SendSenderApprovedOrRejectedCommitmentNotification(_exampleCommitmentView, _transferApprovalStatus, _tokens);
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

            _mediator.Verify(x => x.SendAsync(
                It.IsAny<SendNotificationCommand>()),
                Times.Once);
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
        public async Task ThenSuppliedTokensArePassedInEmailMessage()
        {
            var expectedTokens = TestHelpers.Clone(_tokens);

            await _act();

            CollectionAssert.AreEqual(expectedTokens, _sendNotificationCommand.Email.Tokens);
        }
    }
}
