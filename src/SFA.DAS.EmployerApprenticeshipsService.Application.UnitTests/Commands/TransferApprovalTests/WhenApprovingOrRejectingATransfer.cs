using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.Commitments.Api.Types.Commitment.Types;
using SFA.DAS.EmployerCommitments.Application.Commands.SendNotification;
using SFA.DAS.EmployerCommitments.Application.Commands.TransferApprovalStatus;
using SFA.DAS.EmployerCommitments.Application.Exceptions;
using SFA.DAS.EmployerCommitments.Domain.Configuration;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;
using SFA.DAS.EmployerCommitments.Domain.Models.Notification;
using SFA.DAS.HashingService;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerCommitments.Application.UnitTests.Commands.TransferApprovalTests
{
    [TestFixture]
    public sealed class WhenApprovingOrRejectingATransfer
    {
        #region Setup

        private const long ProviderId = 987654321L;
        private const string ProviderLastUpdatedByEmail = "providerbob@example.com";
        private const string CommitmentReference = "COMREF";

        private const string CohortReferenceToken = "cohort_reference";
        private const string UkprnToken = "ukprn";
        private const string EmployerNameToken = "employer_name";
        private const string SenderNameToken = "sender_name";
        private const string EmployerHashedAccountToken = "employer_hashed_account";

        private const string EmployerApprovedTemplateId = "SenderApprovedCommitmentEmployerNotification";
        private const string EmployerRejectedTemplateId = "SenderRejectedCommitmentEmployerNotification";
        private const string ProviderApprovedTemplateId = "SenderApprovedCommitmentProviderNotification";
        private const string ProviderRejectedTemplateId = "SenderRejectedCommitmentProviderNotification";

        private Mock<IEmployerCommitmentApi> _mockCommitmentApi;
        private Mock<IMediator> _mockMediator;
        private Mock<IProviderEmailService> _mockProviderEmailService;
        private Mock<IHashingService> _hashingService;

        private CommitmentView _repositoryCommitment;
        private TransferApprovalCommandHandler _sut;
        private TransferApprovalCommand _command;

        [SetUp]
        public void Setup()
        {
            _command = new TransferApprovalCommand
            {
                CommitmentId = 876,
                TransferSenderId = 676,
                TransferStatus = TransferApprovalStatus.Rejected
            };
            _repositoryCommitment = new CommitmentView
            {
                Reference = CommitmentReference,
                ProviderId = ProviderId,
                ProviderLastUpdateInfo = new LastUpdateInfo {EmailAddress = ProviderLastUpdatedByEmail},
                TransferSender = new TransferSender
                {
                    Id = _command.TransferSenderId,
                    TransferApprovalStatus = TransferApprovalStatus.Pending
                }
            };

            _mockCommitmentApi = new Mock<IEmployerCommitmentApi>();
            _mockCommitmentApi.Setup(x => x.GetTransferSenderCommitment(It.IsAny<long>(), It.IsAny<long>()))
                .ReturnsAsync(_repositoryCommitment);

            _mockMediator = new Mock<IMediator>();
            var config = new EmployerCommitmentsServiceConfiguration
            {
                CommitmentNotification = new CommitmentNotificationConfiguration {SendEmail = true}
            };

            _mockProviderEmailService = new Mock<IProviderEmailService>();

            _hashingService = new Mock<IHashingService>();

            _sut = new TransferApprovalCommandHandler(_mockCommitmentApi.Object, _mockMediator.Object, config,
                Mock.Of<ILog>(), _mockProviderEmailService.Object, _hashingService.Object);
        }

        #endregion Setup

        #region Core Functionality

        [Test]
        public async Task ThenPatchTransferApprovalInterfaceIsCalledCorrectly()
        {
            await _sut.Handle(_command);

            _mockCommitmentApi.Verify(x => x.PatchTransferApprovalStatus(_command.TransferSenderId,
                _command.CommitmentId,
                It.Is<TransferApprovalRequest>(p =>
                    p.TransferApprovalStatus == _command.TransferStatus &&
                    p.UserEmail == _command.UserEmail && p.UserName == _command.UserName)));
        }

        [Test]
        public async Task ThenPatchTransferRequestApprovalInterfaceIsCalledCorrectly()
        {
            _command.TransferRequestId = 10088;

            await _sut.Handle(_command);

            _mockCommitmentApi.Verify(x => x.PatchTransferApprovalStatus(_command.TransferSenderId,
                _command.CommitmentId,
                _command.TransferRequestId,
                It.Is<TransferApprovalRequest>(p =>
                    p.TransferApprovalStatus == _command.TransferStatus &&
                    p.UserEmail == _command.UserEmail && p.UserName == _command.UserName)));
        }

        [Test]
        public void ThenThrowErrorIfTranferSenderDoesNotMatchTransferSenderOnCommitment()
        {
            _command.TransferSenderId = 9877;
            Assert.CatchAsync<InvalidRequestException>(async () => await _sut.Handle(_command));
        }

        [TestCase(TransferApprovalStatus.Approved)]
        [TestCase(TransferApprovalStatus.Rejected)]
        public void ThenThrowErrorIfTranferIsAlreadyApprovedOrRejected(TransferApprovalStatus status)
        {
            _repositoryCommitment.TransferSender.TransferApprovalStatus = status;
            Assert.CatchAsync<InvalidRequestException>(async () => await _sut.Handle(_command));
        }

        #endregion Core Functionality

        #region Notifications

        [TestCase(TransferApprovalStatus.Approved, ProviderApprovedTemplateId)]
        [TestCase(TransferApprovalStatus.Rejected, ProviderRejectedTemplateId)]
        public async Task ThenProviderIsNotifiedWhenEmailEnabled(TransferApprovalStatus transferApprovalStatus,
            string expectedTemplateId)
        {
            _command.TransferStatus = transferApprovalStatus;

            EmailMessage sentEmailMessage = null;
            _mockProviderEmailService.Setup(x =>
                    x.SendEmailToAllProviderRecipients(It.IsAny<long>(), It.IsAny<string>(), It.IsAny<EmailMessage>()))
                .Callback<long, string, EmailMessage>((l, s, m) => sentEmailMessage = m)
                .Returns(Task.CompletedTask);

            await _sut.Handle(_command);

            _mockProviderEmailService.Verify(x => x.SendEmailToAllProviderRecipients(
                    It.Is<long>(l => l == ProviderId),
                    It.Is<string>(s => s == ProviderLastUpdatedByEmail),
                    It.IsAny<EmailMessage>()),
                Times.Once);

            Assert.AreEqual(sentEmailMessage.TemplateId, expectedTemplateId);

            // we pass more tokens than are required for any individual email (an implementation detail),
            // which means we don't want to test for the presence of all the tokens we actually pass (e.g. with CollectionAssert)
            // we only want to check for the tokens that are necessary

            AssertToken(sentEmailMessage.Tokens, CohortReferenceToken, CommitmentReference);
            AssertToken(sentEmailMessage.Tokens, UkprnToken, $"{ProviderId}");
        }

        [TestCase(TransferApprovalStatus.Approved, EmployerApprovedTemplateId)]
        [TestCase(TransferApprovalStatus.Rejected, EmployerRejectedTemplateId)]
        public async Task ThenEmployerIsNotifiedWhenEmailEnabled(TransferApprovalStatus transferApprovalStatus,
            string expectedTemplateId)
        {
            const string legalEntityName = "Receiver Name";
            const string transferSenderName = "Sender Name";
            const long employerAccountId = 5L;
            const string hashedEmployerAccountId = "6GVW8M";

            _command.TransferStatus = transferApprovalStatus;

            _repositoryCommitment.LegalEntityName = legalEntityName;
            _repositoryCommitment.TransferSender.Name = transferSenderName;
            _repositoryCommitment.EmployerAccountId = employerAccountId;

            SendNotificationCommand sentCommand = null;
            _mockMediator.Setup(x => x.SendAsync(It.IsAny<SendNotificationCommand>()))
                .Callback<IAsyncRequest<Unit>>(c => sentCommand = c as SendNotificationCommand)
                .Returns(Task.FromResult(new Unit()));

            _hashingService.Setup(h => h.HashValue(employerAccountId)).Returns(hashedEmployerAccountId);

            await _sut.Handle(_command);

            _mockMediator.Verify(x => x.SendAsync(It.IsAny<SendNotificationCommand>()), Times.Once);

            Assert.IsNotNull(sentCommand.Email);
            Assert.AreEqual(sentCommand.Email.TemplateId, expectedTemplateId);

            // we pass more tokens than are required for any individual email (an implementation detail),
            // which means we don't want to test for the presence of all the tokens we actually pass (e.g. with CollectionAssert)
            // we only want to check for the tokens that are necessary

            AssertToken(sentCommand.Email.Tokens, CohortReferenceToken, CommitmentReference);
            AssertToken(sentCommand.Email.Tokens, EmployerNameToken, legalEntityName);
            AssertToken(sentCommand.Email.Tokens, SenderNameToken, transferSenderName);
            AssertToken(sentCommand.Email.Tokens, EmployerHashedAccountToken, hashedEmployerAccountId);
        }

        private void AssertToken(Dictionary<string, string> actualTokens, string tokenName, string expectedValue)
        {
            Assert.IsTrue(actualTokens.ContainsKey(tokenName));
            Assert.AreEqual(expectedValue, actualTokens[tokenName]);
        }

        //todo: unit test template substitution

        #endregion Notifications
    }
}