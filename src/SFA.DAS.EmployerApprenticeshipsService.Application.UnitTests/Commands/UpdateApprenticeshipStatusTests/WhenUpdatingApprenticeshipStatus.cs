using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.Apprenticeship.Types;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.EmployerCommitments.Application.Commands.UpdateApprenticeshipStatus;
using SFA.DAS.EmployerCommitments.Application.Exceptions;
using SFA.DAS.EmployerCommitments.Application.Validation;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;
using SFA.DAS.EmployerCommitments.Domain.Models.Apprenticeship;

namespace SFA.DAS.EmployerCommitments.Application.UnitTests.Commands.UpdateApprenticeshipStatusTests
{
    [TestFixture]
    public sealed class WhenUpdatingApprenticeshipStatus
    {
        private UpdateApprenticeshipStatusCommandHandler _handler;
        private Mock<IEmployerCommitmentApi> _mockCommitmentApi;
        private Mock<ICurrentDateTime> _mockCurrentDateTime;
        private IValidator<UpdateApprenticeshipStatusCommand> _validator = new UpdateApprenticeshipStatusCommandValidator();
        private UpdateApprenticeshipStatusCommand _validCommand;
        private Mock<IProviderEmailNotificationService> _providerEmailNotificationService;

        [SetUp]
        public void Setup()
        {
            _validCommand = new UpdateApprenticeshipStatusCommand
            {
                EmployerAccountId = 12L,
                ApprenticeshipId = 4L,
                UserId = "externalUserId",
                ChangeType = ChangeStatusType.Stop,
                DateOfChange = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1),
                UserEmailAddress = "test@email.com",
                UserDisplayName = "Bob"
            };

            var apprenticeshipFromApi = new Apprenticeship { StartDate = DateTime.UtcNow.AddMonths(-2).Date };

            _mockCommitmentApi = new Mock<IEmployerCommitmentApi>();
            _mockCommitmentApi.Setup(x => x.GetEmployerCommitment(It.IsAny<long>(), It.IsAny<long>()))
                .ReturnsAsync(new CommitmentView { ProviderId = 456L });
            _mockCommitmentApi.Setup(x => x.GetEmployerApprenticeship(It.IsAny<long>(), It.IsAny<long>()))
                .ReturnsAsync(apprenticeshipFromApi);

            _mockCurrentDateTime = new Mock<ICurrentDateTime>();
            _mockCurrentDateTime.SetupGet(x => x.Now).Returns(DateTime.UtcNow);

            new Mock<IAcademicYearDateProvider>();
            new Mock<IAcademicYearValidator>();

            _providerEmailNotificationService = new Mock<IProviderEmailNotificationService>();
            _providerEmailNotificationService.Setup(x =>
                x.SendProviderApprenticeshipStopNotification(It.IsAny<Apprenticeship>(), It.IsAny<DateTime>())).Returns(Task.CompletedTask);

            _handler = new UpdateApprenticeshipStatusCommandHandler(
                _mockCommitmentApi.Object,
                _mockCurrentDateTime.Object,
                _validator,
                _providerEmailNotificationService.Object
                );
        }

        [TestCase(ChangeStatusType.Stop, PaymentStatus.Withdrawn)]
        [TestCase(ChangeStatusType.Pause, PaymentStatus.Paused)]
        [TestCase(ChangeStatusType.Resume, PaymentStatus.Active)]
        public async Task ThenTheCommitmentApiShouldBeCalledWithCorrectStatus(ChangeStatusType type, PaymentStatus expectedStatus)
        {
            _validCommand.ChangeType = type;            
            await _handler.Handle(_validCommand);

            _mockCommitmentApi.Verify(x => x.PatchEmployerApprenticeship(
                _validCommand.EmployerAccountId, _validCommand.ApprenticeshipId,
                It.Is<ApprenticeshipSubmission>(
                    y => y.PaymentStatus == expectedStatus && y.LastUpdatedByInfo.Name == _validCommand.UserDisplayName && y.LastUpdatedByInfo.EmailAddress == _validCommand.UserEmailAddress)));
        }

        [TestCase(ChangeStatusType.Stop, true)]
        [TestCase(ChangeStatusType.Pause, false)]
        [TestCase(ChangeStatusType.Resume, false)]
        public async Task ThenIfStoppingSendProviderApprenticeshipStopNotification(ChangeStatusType type, bool expectSendNotification)
        {
            //Arrange
            _validCommand.ChangeType = type;

            //Act
            await _handler.Handle(_validCommand);

            //Assert
            _providerEmailNotificationService.Verify(
                x => x.SendProviderApprenticeshipStopNotification(It.IsAny<Apprenticeship>(), It.IsAny<DateTime>()),
                Times.Exactly(expectSendNotification ? 1 : 0)
                );
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task ThenTheCommitmentApiShouldBeCalledWithCorrectStatus_MadeRedundant_True(bool madeRedundant)
        {
            //Arrange
            _validCommand.ChangeType = ChangeStatusType.Stop;
            _validCommand.MadeRedundant = madeRedundant;

            //Act
            await _handler.Handle(_validCommand);

            //Assert
            _mockCommitmentApi.Verify(x => x.PatchEmployerApprenticeship(
                    _validCommand.EmployerAccountId, _validCommand.ApprenticeshipId,
                    It.Is<ApprenticeshipSubmission>(
                        y => y.PaymentStatus == PaymentStatus.Withdrawn && 
                             y.LastUpdatedByInfo.Name == _validCommand.UserDisplayName && 
                             y.LastUpdatedByInfo.EmailAddress == 
                             _validCommand.UserEmailAddress && y.MadeRedundant == madeRedundant)));
        }

        [Test]
        public void ThenValidationErrorsShouldThrowAnException()
        {
            _validCommand.ApprenticeshipId = 0; // Should fail validation

            Assert.ThrowsAsync<InvalidRequestException>(async () => await _handler.Handle(_validCommand));
        }
    }
}
