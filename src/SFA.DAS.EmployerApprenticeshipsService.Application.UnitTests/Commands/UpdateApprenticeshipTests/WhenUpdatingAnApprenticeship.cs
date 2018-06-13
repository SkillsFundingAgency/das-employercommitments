using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.EmployerCommitments.Application.Commands.UpdateApprenticeship;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;

namespace SFA.DAS.EmployerCommitments.Application.UnitTests.Commands.UpdateApprenticeshipTests
{
    [TestFixture]
    public class WhenUpdatingAnApprenticeship
    {
        private UpdateApprenticeshipCommand _validCommand;
        private UpdateApprenticeshipCommandHandler _handler;
        private Mock<IProviderEmailNotificationService> _providerEmailNotificationService;
        private Mock<IEmployerCommitmentApi> _commitmentApi;
        private CommitmentView _commitmentView;

        [SetUp]
        public void Arrange()
        {
            _commitmentView = new CommitmentView();

            _commitmentApi = new Mock<IEmployerCommitmentApi>();
            _commitmentApi.Setup(x => x.GetEmployerCommitment(It.IsAny<long>(), It.IsAny<long>()))
                .ReturnsAsync(_commitmentView);

            _providerEmailNotificationService = new Mock<IProviderEmailNotificationService>();
            _providerEmailNotificationService
                .Setup(x => x.SendProviderTransferRejectedCommitmentEditNotification(It.IsAny<CommitmentView>()))
                .Returns(() => Task.CompletedTask);

            _handler = new UpdateApprenticeshipCommandHandler(_commitmentApi.Object,
                _providerEmailNotificationService.Object);

            _validCommand = new UpdateApprenticeshipCommand
            {
                AccountId = 123,
                Apprenticeship = new Apprenticeship { CommitmentId = 5634 },
                UserId = "ABC123",
                UserName = "Bob",
                UserEmail = "test@email.com"
            };
        }

        [Test]
        public async Task ThenIfTheCohortWasRejectedByTransferSenderThenNotifyProviderOfEdit()
        {
            _commitmentView.TransferSender = new TransferSender
            {
                TransferApprovalStatus = TransferApprovalStatus.Rejected
            };

            await _handler.Handle(_validCommand);

            _providerEmailNotificationService.Verify(x =>
                x.SendProviderTransferRejectedCommitmentEditNotification(
                    It.Is<CommitmentView>(c => c == _commitmentView)), Times.Once);
        }
    }
}