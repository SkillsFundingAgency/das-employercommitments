using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.EmployerCommitments.Application.Commands.UpdateApprenticeshipStatus;
using SFA.DAS.EmployerCommitments.Application.Exceptions;
using SFA.DAS.EmployerCommitments.Application.Queries.GetApprenticeship;
using SFA.DAS.EmployerCommitments.Application.Validation;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;
using SFA.DAS.EmployerCommitments.Domain.Models.AcademicYear;
using SFA.DAS.EmployerCommitments.Domain.Models.Apprenticeship;

namespace SFA.DAS.EmployerCommitments.Application.UnitTests.Commands.UpdateApprenticeshipStatusTests
{
    [TestFixture]
    public sealed class WhenUpdatingApprenticeshipStatusToStopped
    {
        private UpdateApprenticeshipStatusCommandHandler _handler;
        private Mock<IEmployerCommitmentApi> _mockCommitmentApi;
        private Mock<ICurrentDateTime> _mockCurrentDateTime;
        private IValidator<UpdateApprenticeshipStatusCommand> _validator = new UpdateApprenticeshipStatusCommandValidator();
        private UpdateApprenticeshipStatusCommand _validCommand;
        private Apprenticeship _testApprenticeship;
        private Mock<IAcademicYearDateProvider> _academicYearDateProvider;
        private Mock<IAcademicYearValidator> _academicYearValidator;
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
                DateOfChange = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1)
            };

            _testApprenticeship = new Apprenticeship { StartDate = DateTime.UtcNow.AddMonths(-2).Date };

            _mockCommitmentApi = new Mock<IEmployerCommitmentApi>();
            _mockCommitmentApi.Setup(x => x.GetEmployerCommitment(It.IsAny<long>(), It.IsAny<long>()))
                .ReturnsAsync(new CommitmentView { ProviderId = 456L });
            _mockCommitmentApi.Setup(x => x.GetEmployerApprenticeship(It.IsAny<long>(), It.IsAny<long>()))
                .ReturnsAsync(_testApprenticeship);

            _mockCurrentDateTime = new Mock<ICurrentDateTime>();
            _mockCurrentDateTime.SetupGet(x => x.Now).Returns(DateTime.UtcNow);

            _academicYearDateProvider = new Mock<IAcademicYearDateProvider>();
            _academicYearDateProvider.Setup(x => x.CurrentAcademicYearStartDate).Returns(new DateTime(2016, 8, 1));

            _academicYearValidator = new Mock<IAcademicYearValidator>();

            _providerEmailNotificationService = new Mock<IProviderEmailNotificationService>();
            _providerEmailNotificationService.Setup(x =>
                x.SendProviderApprenticeshipStopNotification(It.IsAny<Apprenticeship>(),
                        It.IsAny<DateTime>()))
                .Returns(Task.CompletedTask);

            _handler = new UpdateApprenticeshipStatusCommandHandler(
                _mockCommitmentApi.Object,
                _mockCurrentDateTime.Object,
                _validator,
                _providerEmailNotificationService.Object
                );
        }

        [Test]
        public void ShouldThrowValidationErrorIfChangeDateGreaterThanTodayForLiveApprenticeship()
        {
            _validCommand.DateOfChange = DateTime.UtcNow.AddMonths(1); // Change date in the future

            Func<Task> act = async () => await _handler.Handle(_validCommand);

            act.ShouldThrow<InvalidRequestException>().Which.Message.Contains("Date must be a date in the past");
        }

        [Test]
        public void ShouldThrowValidationErrorIfChangeDateEarlierThanStartDateForLiveApprenticeship()
        {
            _validCommand.DateOfChange = DateTime.UtcNow.AddMonths(-3); // Change before start date

            Func<Task> act = async () => await _handler.Handle(_validCommand);

            act.ShouldThrow<InvalidRequestException>().Which.Message.Contains("Date cannot be earlier than training start date");
        }        

        [Test]
        public async Task ShouldRetrieveApprenticeship()
        {
            await _handler.Handle(_validCommand);

            _mockCommitmentApi.Verify(x =>
                x.GetEmployerApprenticeship(It.Is<long>(accountId => accountId == _validCommand.EmployerAccountId),
                    It.Is<long>(apprenticeshipId => apprenticeshipId == _validCommand.ApprenticeshipId)));
        }

        [Test]
        public async Task ShouldSendProviderNotification()
        {
            await _handler.Handle(_validCommand);
            
            _providerEmailNotificationService.Verify(x =>
                    x.SendProviderApprenticeshipStopNotification(It.Is<Apprenticeship>(a => a == _testApprenticeship),
                        It.Is<DateTime>(d => d == _validCommand.DateOfChange))
                ,Times.Once);
        }
    }
}
