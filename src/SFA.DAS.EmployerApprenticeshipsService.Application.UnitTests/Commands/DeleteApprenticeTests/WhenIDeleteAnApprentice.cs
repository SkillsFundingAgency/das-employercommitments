using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.EmployerCommitments.Application.Commands.DeleteApprentice;
using SFA.DAS.EmployerCommitments.Application.Exceptions;
using SFA.DAS.EmployerCommitments.Application.Validation;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;

namespace SFA.DAS.EmployerCommitments.Application.UnitTests.Commands.DeleteApprenticeTests
{
    [TestFixture]
    public class WhenIDeleteAnApprentice
    {
        private Mock<IValidator<DeleteApprenticeshipCommand>> _validator;
        private DeleteApprenticeshipCommandHandler _handler;
        private Mock<IProviderEmailNotificationService> _providerEmailNotificationService;
        private Mock<IEmployerCommitmentApi> _commitmentsApi;
        private CommitmentView _commitmentView;

        [SetUp]
        public void Arrange()
        {
            _commitmentView = new CommitmentView();

            _commitmentsApi = new Mock<IEmployerCommitmentApi>();
            _commitmentsApi.Setup(x => x.GetEmployerCommitment(It.IsAny<long>(), It.IsAny<long>()))
                .ReturnsAsync(_commitmentView);
            _commitmentsApi.Setup(x => x.DeleteEmployerApprenticeship(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<DeleteRequest>()))
                .Returns(Task.FromResult<object>(null));

            _providerEmailNotificationService = new Mock<IProviderEmailNotificationService>();
            _providerEmailNotificationService
                .Setup(x => x.SendProviderTransferRejectedCommitmentEditNotification(It.IsAny<CommitmentView>()))
                .Returns(() => Task.CompletedTask);

            _validator = new Mock<IValidator<DeleteApprenticeshipCommand>>();
            _validator.Setup(x => x.Validate(It.IsAny<DeleteApprenticeshipCommand>()))
                .Returns(new ValidationResult());

            _handler = new DeleteApprenticeshipCommandHandler(_commitmentsApi.Object,
                _validator.Object,
                _providerEmailNotificationService.Object);
        }

        [Test]
        public async Task TheCommitmentsServiceShouldBeCalledIfTheRequestIsValid()
        {
            //Arrange
            var command = new DeleteApprenticeshipCommand
            {
                AccountId = 1,
                ApprenticeshipId = 2,
                UserId = "ABC123",
                UserDisplayName = "Bob",
                UserEmailAddress = "test@email.com"
            };

            //Act
            await _handler.Handle(command);

            //Assert
            _commitmentsApi.Verify(
                x =>
                    x.DeleteEmployerApprenticeship(command.AccountId, command.ApprenticeshipId,
                        It.Is<DeleteRequest>(r => r.UserId == command.UserId && r.LastUpdatedByInfo.Name == command.UserDisplayName && r.LastUpdatedByInfo.EmailAddress == command.UserEmailAddress)),
                Times.Once);
        }

        [Test]
        public void ThenIShouldGetAInvalidRequestExceptionIfValidationFails()
        {
            //Arrange
            var command = new DeleteApprenticeshipCommand();
            _validator = new Mock<IValidator<DeleteApprenticeshipCommand>>();
            _validator.Setup(x => x.Validate(It.IsAny<DeleteApprenticeshipCommand>()))
                .Returns(new ValidationResult
                {
                    ValidationDictionary = new Dictionary<string, string>
                    {
                        { "test", "test validation error"}
                    }
                });

            _handler = new DeleteApprenticeshipCommandHandler(_commitmentsApi.Object, _validator.Object,
                _providerEmailNotificationService.Object);

            //Act + Assert
            Assert.ThrowsAsync<InvalidRequestException>(async () => await _handler.Handle(command));
        }

        [Test]
        public async Task ThenIfTheCohortWasRejectedByTransferSenderThenNotifyProviderOfEdit()
        {
            //Arrange
            var command = new DeleteApprenticeshipCommand
            {
                AccountId = 1,
                ApprenticeshipId = 2,
                UserId = "ABC123",
                UserDisplayName = "Bob",
                UserEmailAddress = "test@email.com"
            };

            _commitmentView.TransferSender = new TransferSender
            {
                TransferApprovalStatus = TransferApprovalStatus.Rejected
            };

            //Act
            await _handler.Handle(command);

            //Assert
            _providerEmailNotificationService.Verify(x =>
                x.SendProviderTransferRejectedCommitmentEditNotification(
                    It.Is<CommitmentView>(c => c == _commitmentView)), Times.Once);
        }
    }
}
