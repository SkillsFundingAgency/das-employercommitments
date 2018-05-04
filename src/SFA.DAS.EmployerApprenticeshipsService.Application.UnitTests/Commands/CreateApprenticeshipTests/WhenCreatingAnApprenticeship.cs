using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.EmployerCommitments.Application.Commands.CreateApprenticeship;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;

namespace SFA.DAS.EmployerCommitments.Application.UnitTests.Commands.CreateApprenticeshipTests
{
    [TestFixture]
    public sealed class WhenCreatingAnApprenticeship
    {
        private CreateApprenticeshipCommand _validCommand;
        private CreateApprenticeshipCommandHandler _handler;
        private Mock<IProviderEmailNotificationService> _providerEmailNotificationService;
        private Mock<IEmployerCommitmentApi> _commitmentApi;
        private CommitmentView _commitmentView;

        [SetUp]
        public void Setup()
        {
            _commitmentView = new CommitmentView();

            _commitmentApi = new Mock<IEmployerCommitmentApi>();
            _commitmentApi.Setup(x => x.GetEmployerCommitment(It.IsAny<long>(), It.IsAny<long>()))
                .ReturnsAsync(_commitmentView);

            _providerEmailNotificationService = new Mock<IProviderEmailNotificationService>();
            _providerEmailNotificationService
                .Setup(x => x.SendProviderTransferRejectedCommitmentEditEmailNotification(It.IsAny<CommitmentView>()))
                .Returns(() => Task.CompletedTask);

            _handler = new CreateApprenticeshipCommandHandler(_commitmentApi.Object,
                _providerEmailNotificationService.Object);

            _validCommand = new CreateApprenticeshipCommand
            {
                AccountId = 123,
                Apprenticeship = new Apprenticeship { CommitmentId = 5634 },
                UserId = "ABC123",
                UserDisplayName = "Bob",
                UserEmailAddress = "test@email.com"
            };
        }

        [Test]
        public async Task ThenTheApprenticeshipIsCreated()
        {
            await _handler.Handle(_validCommand);

            _commitmentApi.Verify(
                x =>
                    x.CreateEmployerApprenticeship(_validCommand.AccountId, _validCommand.Apprenticeship.CommitmentId,
                        It.Is<ApprenticeshipRequest>(
                            y =>
                                y.Apprenticeship == _validCommand.Apprenticeship && y.UserId == _validCommand.UserId && y.LastUpdatedByInfo.Name == _validCommand.UserDisplayName &&
                                y.LastUpdatedByInfo.EmailAddress == _validCommand.UserEmailAddress)));
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
                x.SendProviderTransferRejectedCommitmentEditEmailNotification(
                    It.Is<CommitmentView>(c => c == _commitmentView)));
        }
    }
}
