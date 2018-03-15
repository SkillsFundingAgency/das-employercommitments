using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.EmployerCommitments.Application.Commands.TransferApprovalStatus;

namespace SFA.DAS.EmployerCommitments.Application.UnitTests.Commands.TransferApprovalTests
{
    [TestFixture]
    public sealed class WhenApprovingOrRejectingATransfer
    {
        private Mock<IEmployerCommitmentApi> _mockCommitmentApi;
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
                TransferSenderInfo = new TransferSenderInfo
                {
                    TransferSenderId = _command.TransferSenderId,
                    TransferApprovalStatus = TransferApprovalStatus.Pending
                }
            };

            _mockCommitmentApi = new Mock<IEmployerCommitmentApi>();
            _mockCommitmentApi.Setup(x => x.GetTransferSenderCommitment(It.IsAny<long>(), It.IsAny<long>()))
                .ReturnsAsync(_repositoryCommitment);

            _sut = new TransferApprovalCommandHandler(_mockCommitmentApi.Object);
        }

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
        public async Task ThenThrowErrorIfTranferSenderDoesNotMatchTransferSenderOnCommitment()
        {
            _command.TransferSenderId = 9877;
            Assert.CatchAsync<InvalidRequestException>(() => _sut.Handle(_command));
        }

        [TestCase(TransferApprovalStatus.Approved)]
        [TestCase(TransferApprovalStatus.Rejected)]
        public async Task ThenThrowErrorIfTranferIsAlreadyApprovedOrRejected(TransferApprovalStatus status)
        {
            _repositoryCommitment.TransferSenderInfo.TransferApprovalStatus = status;
            Assert.CatchAsync<InvalidRequestException>(() => _sut.Handle(_command));
        }

    }
}

