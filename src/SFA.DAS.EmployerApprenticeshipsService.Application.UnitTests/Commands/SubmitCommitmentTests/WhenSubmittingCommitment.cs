using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.Commitments.Api.Types.Commitment.Types;
using SFA.DAS.EmployerCommitments.Application.Commands.SubmitCommitment;
using SFA.DAS.EmployerCommitments.Application.Exceptions;
using SFA.DAS.EmployerCommitments.Domain.Configuration;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;
using SFA.DAS.EmployerCommitments.Domain.Models.Notification;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerCommitments.Application.UnitTests.Commands.SubmitCommitmentTests
{
    [TestFixture]
    public sealed class WhenSubmittingCommitment
    {
        private SubmitCommitmentCommandHandler _handler;
        private Mock<IEmployerCommitmentApi> _mockCommitmentApi;
        private SubmitCommitmentCommand _validCommand;
        private CommitmentView _repositoryCommitment;
        private Mock<IProviderEmailService> _mockEmailService;

        private const string CohortReference = "COREF";

        [SetUp]
        public void Setup()
        {
            _validCommand = new SubmitCommitmentCommand { EmployerAccountId = 12L, CommitmentId = 2L, UserDisplayName = "Test User", UserEmailAddress = "test@test.com", UserId = "externalUserId"};
            _repositoryCommitment = new CommitmentView
            {
                ProviderId = 456L,
                EmployerAccountId = 12L,
                AgreementStatus = AgreementStatus.NotAgreed,
                Reference = CohortReference
            };

            _mockCommitmentApi = new Mock<IEmployerCommitmentApi>();
            _mockCommitmentApi.Setup(x => x.GetEmployerCommitment(It.IsAny<long>(), It.IsAny<long>()))
                .ReturnsAsync(_repositoryCommitment);

            var config = new EmployerCommitmentsServiceConfiguration
                             {
                                 CommitmentNotification = new CommitmentNotificationConfiguration { SendEmail = true }
                             };

            _mockEmailService = new Mock<IProviderEmailService>();

            _handler = new SubmitCommitmentCommandHandler(_mockCommitmentApi.Object, config, Mock.Of<ILog>(), _mockEmailService.Object);
        }

        [Test]
        public async Task ThenIfTheEmployerIsAmendingTheCommitmentTheCommitmentShouldBePatched()
        {
            await _handler.Handle(_validCommand);

            _mockCommitmentApi.Verify(x => x.PatchEmployerCommitment(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<CommitmentSubmission>()));
        }

        [Test]
        public async Task ThenIfTheEmployerIsApprovingTheCommitmentTheCommitmentShouldBeApproved()
        {
            _validCommand.LastAction = LastAction.Approve;

            await _handler.Handle(_validCommand);

            _mockCommitmentApi.Verify(x => x.ApproveCohort(_validCommand.EmployerAccountId, _validCommand.CommitmentId,
                It.Is<CommitmentSubmission>(y =>
                    y.UserId == _validCommand.UserId && y.Message == _validCommand.Message && y.LastUpdatedByInfo.EmailAddress == _validCommand.UserEmailAddress &&
                    y.LastUpdatedByInfo.Name == _validCommand.UserDisplayName)));
        }

        [Test]
        public async Task NotCallSendEmail()
        {
            _validCommand.LastAction = LastAction.None;
            await _handler.Handle(_validCommand);

            _mockEmailService.Verify(x => x.SendEmailToAllProviderRecipients(It.IsAny<long>(), It.IsAny<string>(), It.IsAny<EmailMessage>()), Times.Never);
        }

        [Test]
        public async Task CallGetProviderEmailQueryRequest()
        {
            _validCommand.LastAction = LastAction.Amend;
            await _handler.Handle(_validCommand);

            _mockEmailService.Verify(x => x.SendEmailToAllProviderRecipients(It.IsAny<long>(), It.IsAny<string>(), It.IsAny<EmailMessage>()), Times.Once);
        }

        [Test]
        public void ThenValidationErrorsShouldThrowAnException()
        {
            _validCommand.EmployerAccountId = 0; // Should fail validation

            Assert.ThrowsAsync<InvalidRequestException>(async () => await _handler.Handle(_validCommand));
        }

        [Test]
        public void ThenValidationErrorsShouldThrowAnException2()
        {
            _validCommand.EmployerAccountId = 2;

            Assert.ThrowsAsync<InvalidRequestException>(async () => await _handler.Handle(_validCommand));
        }
    }
}

