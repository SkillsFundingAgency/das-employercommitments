using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.EmployerCommitments.Application.Commands.TransferApprovalStatus;
using SFA.DAS.EmployerCommitments.Application.Exceptions;
using SFA.DAS.EmployerCommitments.Domain.Configuration;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerCommitments.Application.UnitTests.Commands.TransferApprovalTests
{
    [TestFixture]
    public sealed class WhenApprovingOrRejectingATransfer
    {
        #region Setup

        private Mock<IEmployerCommitmentApi> _mockCommitmentApi;
        private Mock<IProviderEmailNotificationService> _providerEmailNotificationService;
        private Mock<IEmployerEmailNotificationService> _employerEmailNotificationService;

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

            _providerEmailNotificationService = new Mock<IProviderEmailNotificationService>();
            _providerEmailNotificationService
                .Setup(x => x.SendSenderApprovedOrRejectedCommitmentNotification(It.IsAny<CommitmentView>(), It.IsAny<TransferApprovalStatus>()))
                .Returns(() => Task.CompletedTask);

            _employerEmailNotificationService = new Mock<IEmployerEmailNotificationService>();
            _employerEmailNotificationService
                .Setup(x => x.SendSenderApprovedOrRejectedCommitmentNotification(It.IsAny<CommitmentView>(), It.IsAny<TransferApprovalStatus>()))
                .Returns(() => Task.CompletedTask);

            _sut = new TransferApprovalCommandHandler(_mockCommitmentApi.Object, config,
                Mock.Of<ILog>(), _providerEmailNotificationService.Object, _employerEmailNotificationService.Object);
        }

        #endregion Setup

        #region Core Functionality

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
                    It.IsAny<CommitmentView>(), It.Is<TransferApprovalStatus>(s => s == transferApprovalStatus)),
                    Times.Once);
        }

        [TestCase(TransferApprovalStatus.Approved)]
        [TestCase(TransferApprovalStatus.Rejected)]
        public async Task ThenEmployerIsNotifiedWhenEmailEnabled(TransferApprovalStatus transferApprovalStatus)
        {
            _command.TransferStatus = transferApprovalStatus;

            await _sut.Handle(_command);

            _employerEmailNotificationService.Verify(pens =>
                    pens.SendSenderApprovedOrRejectedCommitmentNotification(
                        It.IsAny<CommitmentView>(), It.Is<TransferApprovalStatus>(s => s == transferApprovalStatus)),
                Times.Once);
        }

        #endregion Notifications
    }
}