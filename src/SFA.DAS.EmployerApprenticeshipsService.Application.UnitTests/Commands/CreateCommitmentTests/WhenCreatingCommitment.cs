using System;
using System.Threading.Tasks;
using KellermanSoftware.CompareNetObjects;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.Commitments.Api.Types.Commitment.Types;
using SFA.DAS.EmployerCommitments.Application.Commands.CreateCommitment;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;

namespace SFA.DAS.EmployerCommitments.Application.UnitTests.Commands.CreateCommitmentTests
{
    [TestFixture]
    public class WhenCreatingCommitment
    {
        private CreateCommitmentCommandHandler _commandHandler;

        private Mock<IEmployerCommitmentApi> _employerCommitmentApi;
        private Mock<IProviderEmailNotificationService> _providerEmailNotificationService;

        private CommitmentView _commitment;

        private CreateCommitmentCommand _payload;
        private CreateCommitmentCommand _validCommand;

        private CommitmentRequest _capturedCommitmentRequest;

        private Func<Task> _act;

        [SetUp]
        public void Arrange()
        {
            _employerCommitmentApi = new Mock<IEmployerCommitmentApi>();

            _employerCommitmentApi.Setup(x => x.CreateEmployerCommitment(It.IsAny<long>(),It.IsAny<CommitmentRequest>()))
                .ReturnsAsync(new CommitmentView())
                .Callback<long, CommitmentRequest>((id, req) => _capturedCommitmentRequest = req);

            _commitment = new CommitmentView();

            _employerCommitmentApi.Setup(x => x.GetEmployerCommitment(It.IsAny<long>(), It.IsAny<long>()))
                .ReturnsAsync(_commitment);

            _providerEmailNotificationService = new Mock<IProviderEmailNotificationService>();
            _providerEmailNotificationService.Setup(x => x.SendCreateCommitmentNotification(It.IsAny<CommitmentView>()))
                .Returns(Task.CompletedTask);

            _payload = new CreateCommitmentCommand
            {
                Commitment = new Commitment
                {
                    EmployerAccountId = 99
                },
                Message = string.Empty,
                UserId = string.Empty
            };

            //Store a copy of the original payload for assertions, to guard against handler modification
            _validCommand = TestHelpers.Clone(_payload);

            _commandHandler = new CreateCommitmentCommandHandler(
                _employerCommitmentApi.Object,
                _providerEmailNotificationService.Object
            );

            _act = async () => await _commandHandler.Handle(_payload);
        }

        [Test]
        public async Task ThenCommitmentsApiIsCalledToCreateCommitment()
        {
            await _act();

            _employerCommitmentApi.Verify(x => x.CreateEmployerCommitment(
                It.Is<long>(employeraccountId => employeraccountId == _validCommand.Commitment.EmployerAccountId),
                It.IsAny<CommitmentRequest>()
                ),Times.Once);

            _capturedCommitmentRequest.Commitment.ShouldCompare(_validCommand.Commitment);
        }

        [TestCase(CommitmentStatus.Active, true)]
        [TestCase(CommitmentStatus.New, false)]
        public async Task ThenProviderIsSentANotificationIfCommitmentIsAssignedToThem(CommitmentStatus status, bool expectSendNotification)
        {
            //Arrange
            _payload.Commitment.CommitmentStatus = status;

            //Act
            await _act();

            //Assert
            _providerEmailNotificationService.Verify(x => x.SendCreateCommitmentNotification(It.IsAny<CommitmentView>()),
                Times.Exactly(expectSendNotification ? 1 : 0));
        }
    }
}
