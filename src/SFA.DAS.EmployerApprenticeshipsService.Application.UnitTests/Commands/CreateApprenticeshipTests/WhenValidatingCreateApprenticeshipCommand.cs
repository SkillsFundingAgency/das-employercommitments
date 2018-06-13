using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.EmployerCommitments.Application.Commands.CreateApprenticeship;
using SFA.DAS.EmployerCommitments.Application.Exceptions;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;

namespace SFA.DAS.EmployerCommitments.Application.UnitTests.Commands.CreateApprenticeshipTests
{
    [TestFixture]
    public sealed class WhenValidatingCreateApprenticeshipCommand
    {
        private CreateApprenticeshipCommand _validCommand;
        private CreateApprenticeshipCommandHandler _handler;
        private Mock<IEmployerCommitmentApi> _commitmentApi;

        [SetUp]
        public void Setup()
        {
            _commitmentApi = new Mock<IEmployerCommitmentApi>();
            _commitmentApi.Setup(x => x.GetEmployerCommitment(It.IsAny<long>(), It.IsAny<long>()))
                .ReturnsAsync(new CommitmentView());

            _handler = new CreateApprenticeshipCommandHandler(_commitmentApi.Object,
                Mock.Of<IProviderEmailNotificationService>());

            _validCommand = new CreateApprenticeshipCommand
            {
                AccountId = 123,
                Apprenticeship = new Apprenticeship()
            };
        }

        [TestCase(0)]
        [TestCase(-5)]
        public void AccountIdMustBeGreaterThanZeroElseExceptionThrown(long accountId)
        {
            var command = new CreateApprenticeshipCommand { AccountId = accountId, Apprenticeship = new Apprenticeship { CommitmentId = 123 } };

            Assert.ThrowsAsync<InvalidRequestException>(() => _handler.Handle(command));
        }

        [TestCase(0)]
        [TestCase(-5)]
        public void CommitmentIdMustBeGreaterThanZeroElseExceptionThrown(long commitmentId)
        {
            var command = new CreateApprenticeshipCommand { AccountId = 123, Apprenticeship = new Apprenticeship { CommitmentId = commitmentId } };

            Assert.ThrowsAsync<InvalidRequestException>(() => _handler.Handle(command));
        }

        [Test]
        public void OnlyValidCommitmentIdAndAccountIdNeedsToBePopulated()
        {
            var command = new CreateApprenticeshipCommand
                { AccountId = 123, Apprenticeship = new Apprenticeship { CommitmentId = 321 }, UserId = "externalUserId"};

            Assert.DoesNotThrowAsync(() => _handler.Handle(command));
        }

        [Test]
        public void ThenValidationErrorsShouldThrowAnExceptionWhenUserIdMissing()
        {
            var command = new CreateApprenticeshipCommand
                { AccountId = 123, Apprenticeship = new Apprenticeship { CommitmentId = 321 }, UserId = string.Empty };

            Assert.ThrowsAsync<InvalidRequestException>(async () => await _handler.Handle(command));
        }
    }
}
