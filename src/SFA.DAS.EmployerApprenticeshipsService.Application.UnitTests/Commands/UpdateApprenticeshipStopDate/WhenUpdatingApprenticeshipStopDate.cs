using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.EmployerCommitments.Application.Commands.UpdateApprenticeshipStopDate;
using SFA.DAS.EmployerCommitments.Application.Exceptions;
using SFA.DAS.EmployerCommitments.Application.Queries.GetApprenticeship;
using SFA.DAS.EmployerCommitments.Application.Validation;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;
using SFA.DAS.EmployerCommitments.Domain.Models.AcademicYear;

namespace SFA.DAS.EmployerCommitments.Application.UnitTests.Commands.UpdateApprenticeshipStopDate
{
    [TestFixture]
    public sealed class WhenUpdatingApprenticeshipStopDate
    {
        private UpdateApprenticeshipStopDateCommandHandler _handler;
        private Mock<IEmployerCommitmentApi> _mockCommitmentApi;
        private Mock<IMediator> _mockMediator;
        private Mock<ICurrentDateTime> _mockCurrentDateTime;
        private IValidator<UpdateApprenticeshipStopDateCommand> _validator = new UpdateApprenticeshipStopDateCommandValidator();
        private UpdateApprenticeshipStopDateCommand _validCommand;
        private Apprenticeship _testApprenticeship;
        private Mock<IAcademicYearDateProvider> _academicYearDateProvider;
        private Mock<IAcademicYearValidator> _academicYearValidator;
        private Mock<IProviderEmailNotificationService> _providerEmailNotificationService;
        private DateTime _newStopDate;

        [SetUp]
        public void Setup()
        {
            _newStopDate = DateTime.UtcNow.AddMonths(-2).Date;

            _validCommand = new UpdateApprenticeshipStopDateCommand
            {
                EmployerAccountId = 12L,
                ApprenticeshipId = 4L,
                UserId = "externalUserId",
                NewStopDate = _newStopDate,
                CommitmentId = 123
            };

            _testApprenticeship = new Apprenticeship
            {
                Id = 4L,
                FirstName = "TEST",
                LastName = "APPRENTICE",
                StartDate = DateTime.UtcNow.AddMonths(-2).Date,
                StopDate = DateTime.UtcNow.AddMonths(6).Date
            };

            _mockCommitmentApi = new Mock<IEmployerCommitmentApi>();
            _mockCommitmentApi.Setup(x => x.GetEmployerCommitment(It.IsAny<long>(), It.IsAny<long>())).ReturnsAsync(new CommitmentView { ProviderId = 456L });
            _mockMediator = new Mock<IMediator>();

            //todo: can this mediator call be removed?
            var apprenticeshipGetResponse = new GetApprenticeshipQueryResponse { Apprenticeship = _testApprenticeship };
            _mockMediator.Setup(x => x.SendAsync(It.IsAny<GetApprenticeshipQueryRequest>())).ReturnsAsync(apprenticeshipGetResponse);
            _mockCurrentDateTime = new Mock<ICurrentDateTime>();
            _mockCurrentDateTime.SetupGet(x => x.Now).Returns(DateTime.UtcNow);

            _mockCommitmentApi.Setup(x => x.GetEmployerApprenticeship(It.IsAny<long>(), It.IsAny<long>()))
                .ReturnsAsync(_testApprenticeship);

            _academicYearDateProvider = new Mock<IAcademicYearDateProvider>();
            _academicYearDateProvider.Setup(x => x.CurrentAcademicYearStartDate).Returns(new DateTime(2016, 8, 1));

            _academicYearValidator = new Mock<IAcademicYearValidator>();

            _providerEmailNotificationService = new Mock<IProviderEmailNotificationService>();
            _providerEmailNotificationService.Setup(x =>
                x.SendProviderApprenticeshipStopEditNotification(It.IsAny<Apprenticeship>(), It.IsAny<DateTime>()))
                .Returns(Task.CompletedTask);

            _handler = new UpdateApprenticeshipStopDateCommandHandler(
                _mockCommitmentApi.Object,
                _mockMediator.Object,
                _mockCurrentDateTime.Object,
                _validator,
                _academicYearDateProvider.Object,
                _academicYearValidator.Object,
                _providerEmailNotificationService.Object
                );
        }

        [Test]
        public void ShouldThrowValidationErrorIfStopDateGreaterThanTodayForLiveApprenticeship()
        {
            _validCommand.NewStopDate = DateTime.UtcNow.AddMonths(1); // Change date in the future

            Func<Task> act = async () => await _handler.Handle(_validCommand);

            act.ShouldThrow<InvalidRequestException>().Which.Message.Contains("Date must be a date in the past");
        }

        [Test]
        public void ShouldThrowValidationErrorIfStopDateEarlierThanStartDateForLiveApprenticeship()
        {
            _validCommand.NewStopDate = DateTime.UtcNow.AddMonths(-3); // Change before start date

            Func<Task> act = async () => await _handler.Handle(_validCommand);

            act.ShouldThrow<InvalidRequestException>().Which.Message.Contains("Date cannot be earlier than training start date");
        }

        [Test]
        public void ShouldThrowValidationErrorIfStopDateIsntTheSameAsStartDateForApprenticeshipWaitingToStart()
        {
            _testApprenticeship.StartDate = DateTime.UtcNow.AddMonths(2).Date;
            _validCommand.NewStopDate = DateTime.UtcNow.AddMonths(2).AddDays(1).Date;

            Func<Task> act = async () => await _handler.Handle(_validCommand);

            act.ShouldThrow<InvalidRequestException>().Which.Message.Contains("Date must the same as start date if training hasn't started");
        }

        [TestCase(AcademicYearValidationResult.NotWithinFundingPeriod, false, Description = "Validation fails if date of change is in the previous academic year - if the R14 date has passed")]
        [TestCase(AcademicYearValidationResult.Success, true)]
        public void ShouldThrowValidationErrorAfterR14Close(AcademicYearValidationResult academicYearValidationResult, bool expectPassValidation)
        {
            _testApprenticeship.StartDate = new DateTime(2016, 3, 1); //early last academic year
            _academicYearValidator.Setup(x => x.Validate(It.IsAny<DateTime>())).Returns(academicYearValidationResult);

            Func<Task> act = async () => await _handler.Handle(_validCommand);

            if (expectPassValidation)
            {
                act.ShouldNotThrow<InvalidRequestException>();
            }
            else
            {
                act.ShouldThrow<InvalidRequestException>().Where(x => x.ErrorMessages.Values.Contains("The earliest date you can stop this apprentice is 01 08 2016"));
            }
        }

        [Test(Description = "Validation fails for both R14 having passed and change date before Start Date - Start Date error takes precedence")]
        public void ShouldThrowStartDateValidationErrorAfterR14CloseAndStopDateBeforeStartDate()
        {
            _testApprenticeship.StartDate = new DateTime(2016, 3, 1);
            _validCommand.NewStopDate = _testApprenticeship.StartDate.Value.AddMonths(-1);
            _academicYearValidator.Setup(x => x.Validate(It.IsAny<DateTime>()))
                .Returns(AcademicYearValidationResult.NotWithinFundingPeriod);

            Func<Task> act = async () => await _handler.Handle(_validCommand);

            act.ShouldThrow<InvalidRequestException>().Where(x => x.ErrorMessages.Values.Contains("Date cannot be earlier than training start date"));
        }

        [Test]
        public async Task ShouldSendProviderApprenticeshipStopEditNotification()
        {
            await _handler.Handle(_validCommand);

            _providerEmailNotificationService.Verify(x => x.SendProviderApprenticeshipStopEditNotification(
                It.Is<Apprenticeship>(a => a.Id == _testApprenticeship.Id
                                           && a.StopDate == _testApprenticeship.StopDate
                                           && a.FirstName == _testApprenticeship.FirstName
                                           && a.LastName == _testApprenticeship.LastName
                                           ),
                It.Is<DateTime>(d => d == _newStopDate)),Times.Once);
        }
    }
}
