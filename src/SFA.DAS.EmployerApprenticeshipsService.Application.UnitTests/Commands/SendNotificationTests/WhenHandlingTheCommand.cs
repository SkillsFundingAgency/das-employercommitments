using System.Threading.Tasks;
using AutoFixture.NUnit3;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerCommitments.Application.Commands.SendNotification;
using SFA.DAS.EmployerCommitments.Application.Validation;

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
        // ThenThrowsInvalidRequestExceptionIfInvalid
        // then sends email to notification

        // then catches exception if api throws
    }
}