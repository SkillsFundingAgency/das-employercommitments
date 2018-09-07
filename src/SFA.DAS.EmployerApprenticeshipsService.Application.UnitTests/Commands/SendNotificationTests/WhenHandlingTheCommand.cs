using System;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerCommitments.Application.Commands.SendNotification;
using SFA.DAS.EmployerCommitments.Application.Exceptions;
using SFA.DAS.EmployerCommitments.Application.Validation;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;

namespace SFA.DAS.EmployerCommitments.Application.UnitTests.Commands.SendNotificationTests
{
    [TestFixture]
    public class WhenHandlingTheCommand
    {
        [Test, EmployerCommitmentsAutoData]
        public async Task ThenItValidatesTheCommand(
            SendNotificationCommand command,
            [Frozen] Mock<IValidator<SendNotificationCommand>> mockValidator,
            SendNotificationCommandHandler sut)
        {
            await sut.Handle(command);

            mockValidator.Verify(validator => validator.Validate(command), Times.Once);
        }

        [Test, EmployerCommitmentsAutoData]
        public void ThenThrowsInvalidRequestExceptionIfCommandInvalid(
            SendNotificationCommand command,
            string propertyName,
            ValidationResult validationResult,
            InvalidRequestException validationException,
            [Frozen] Mock<IValidator<SendNotificationCommand>> mockValidator,
            SendNotificationCommandHandler sut)
        {
            validationResult.AddError(propertyName);

            mockValidator
                .Setup(validator => validator.Validate(command))
                .Returns(validationResult);

            Func<Task> act = async () => { await sut.Handle(command); };

            act.ShouldThrowExactly<InvalidRequestException>()
                .Which.ErrorMessages.ContainsKey(propertyName).Should().BeTrue();
        }

        [Test, EmployerCommitmentsAutoData]
        public async Task ThenSendsEmailToNotificationApi(
            SendNotificationCommand command,
            [Frozen] Mock<IBackgroundNotificationService> mockNotificationsApi,
            SendNotificationCommandHandler sut)
        {
            await sut.Handle(command);

            mockNotificationsApi.Verify(api => api.SendEmail(command.Email), Times.Once);
        }
    }
}