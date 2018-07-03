using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.Commitments.Api.Types.Commitment.Types;
using SFA.DAS.EmployerCommitments.Application.Commands.TransferApprovalStatus;
using SFA.DAS.EmployerCommitments.Application.Exceptions;
using SFA.DAS.EmployerCommitments.Domain.Configuration;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;
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

        private Mock<IEmployerCommitmentApi> _mockCommitmentApi;
        private Mock<IHashingService> _hashingService;
        private Mock<IProviderEmailNotificationService> _providerEmailNotificationService;
        private Mock<IEmployerEmailNotificationService> _employerEmailNotificationService;

        private CommitmentView _repositoryCommitment;
        private TransferApprovalCommandHandler _sut;
        private TransferApprovalCommand _command;

        private Dictionary<string, string> _tokens;

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

            var config = new EmployerCommitmentsServiceConfiguration
            {
                CommitmentNotification = new CommitmentNotificationConfiguration {SendEmail = true}
            };

            _hashingService = new Mock<IHashingService>();

            _providerEmailNotificationService = new Mock<IProviderEmailNotificationService>();
            _providerEmailNotificationService
                .Setup(x => x.SendSenderApprovedOrRejectedCommitmentNotification(It.IsAny<CommitmentView>(), It.IsAny<TransferApprovalStatus>(), It.IsAny<Dictionary<string, string>>()))
                .Callback<CommitmentView, TransferApprovalStatus, Dictionary<string, string>>((c, s, t) => _tokens = t)
                .Returns(() => Task.CompletedTask);

            _employerEmailNotificationService = new Mock<IEmployerEmailNotificationService>();
            _employerEmailNotificationService
                .Setup(x => x.SendSenderApprovedOrRejectedCommitmentNotification(It.IsAny<CommitmentView>(), It.IsAny<TransferApprovalStatus>(), It.IsAny<Dictionary<string, string>>()))
                .Callback<CommitmentView, TransferApprovalStatus, Dictionary<string, string>>((c, s, t) => _tokens = t)
                .Returns(() => Task.CompletedTask);

            _sut = new TransferApprovalCommandHandler(_mockCommitmentApi.Object, config,
                Mock.Of<ILog>(), _hashingService.Object, _providerEmailNotificationService.Object, _employerEmailNotificationService.Object);
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

        [TestCase(TransferApprovalStatus.Approved)]
        [TestCase(TransferApprovalStatus.Rejected)]
        public async Task ThenProviderIsNotifiedWhenEmailEnabled(TransferApprovalStatus transferApprovalStatus)
        {
            _command.TransferStatus = transferApprovalStatus;

            await _sut.Handle(_command);

            _providerEmailNotificationService.Verify(pens =>
                pens.SendSenderApprovedOrRejectedCommitmentNotification(
                    It.IsAny<CommitmentView>(), It.Is<TransferApprovalStatus>(s => s == transferApprovalStatus), It.IsAny<Dictionary<string, string>>()),
                    Times.Once);

            // we pass more tokens than are required for any individual email (an implementation detail),
            // which means we don't want to test for the presence of all the tokens we actually pass (e.g. with CollectionAssert)
            // we only want to check for the tokens that are necessary

            AssertToken(_tokens, CohortReferenceToken, CommitmentReference);
            AssertToken(_tokens, UkprnToken, $"{ProviderId}");
        }

        [TestCase(TransferApprovalStatus.Approved)]
        [TestCase(TransferApprovalStatus.Rejected)]
        public async Task ThenEmployerIsNotifiedWhenEmailEnabled(TransferApprovalStatus transferApprovalStatus)
        {
            const string legalEntityName = "Receiver Name";
            const string transferSenderName = "Sender Name";
            const long employerAccountId = 5L;
            const string hashedEmployerAccountId = "6GVW8M";

            _command.TransferStatus = transferApprovalStatus;

            _repositoryCommitment.LegalEntityName = legalEntityName;
            _repositoryCommitment.TransferSender.Name = transferSenderName;
            _repositoryCommitment.EmployerAccountId = employerAccountId;

            _hashingService.Setup(h => h.HashValue(employerAccountId)).Returns(hashedEmployerAccountId);

            await _sut.Handle(_command);

            _employerEmailNotificationService.Verify(pens =>
                    pens.SendSenderApprovedOrRejectedCommitmentNotification(
                        It.IsAny<CommitmentView>(), It.Is<TransferApprovalStatus>(s => s == transferApprovalStatus), It.IsAny<Dictionary<string, string>>()),
                Times.Once);

            // we pass more tokens than are required for any individual email (an implementation detail),
            // which means we don't want to test for the presence of all the tokens we actually pass (e.g. with CollectionAssert)
            // we only want to check for the tokens that are necessary

            AssertToken(_tokens, CohortReferenceToken, CommitmentReference);
            AssertToken(_tokens, EmployerNameToken, legalEntityName);
            AssertToken(_tokens, SenderNameToken, transferSenderName);
            AssertToken(_tokens, EmployerHashedAccountToken, hashedEmployerAccountId);
        }

        private void AssertToken(Dictionary<string, string> actualTokens, string tokenName, string expectedValue)
        {
            Assert.IsTrue(actualTokens.ContainsKey(tokenName));
            Assert.AreEqual(expectedValue, actualTokens[tokenName]);
        }

        #endregion Notifications
    }
}