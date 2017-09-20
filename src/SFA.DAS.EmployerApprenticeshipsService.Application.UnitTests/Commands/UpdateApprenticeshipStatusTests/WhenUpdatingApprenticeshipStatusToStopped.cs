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
using SFA.DAS.EmployerCommitments.Application.Queries.GetApprenticeship;
using SFA.DAS.EmployerCommitments.Application.Validation;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;
using SFA.DAS.EmployerCommitments.Domain.Models.AcademicYear;
using SFA.DAS.EmployerCommitments.Domain.Models.Apprenticeship;
using SFA.DAS.EmployerCommitments.Infrastructure.Services;

namespace SFA.DAS.EmployerCommitments.Application.UnitTests.Commands.UpdateApprenticeshipStatusTests
{
    [TestFixture]
    public sealed class WhenUpdatingApprenticeshipStatusToStopped
    {
        private UpdateApprenticeshipStatusCommandHandler _handler;
        private Mock<IEmployerCommitmentApi> _mockCommitmentApi;
        private Mock<IMediator> _mockMediator;
        private Mock<ICurrentDateTime> _mockCurrentDateTime;
        private IValidator<UpdateApprenticeshipStatusCommand> _validator = new UpdateApprenticeshipStatusCommandValidator();
        private UpdateApprenticeshipStatusCommand _validCommand;
        private Apprenticeship _testApprenticeship;
        private Mock<IAcademicYearDateProvider> _academicYearDateProvider;
        private Mock<IAcademicYearValidator> _academicYearValidator;

        [SetUp]
        public void Setup()
        {
            _validCommand = new UpdateApprenticeshipStatusCommand
            {
                EmployerAccountId = 12L,
                ApprenticeshipId = 4L,
                UserId = "externalUserId",
                ChangeType = ChangeStatusType.Stop,
                DateOfChange = DateTime.UtcNow.Date
            };

            _testApprenticeship = new Apprenticeship { StartDate = DateTime.UtcNow.AddMonths(-2).Date };

            _mockCommitmentApi = new Mock<IEmployerCommitmentApi>();
            _mockCommitmentApi.Setup(x => x.GetEmployerCommitment(It.IsAny<long>(), It.IsAny<long>())).ReturnsAsync(new CommitmentView { ProviderId = 456L });
            _mockMediator = new Mock<IMediator>();

            var apprenticeshipGetResponse = new GetApprenticeshipQueryResponse { Apprenticeship = _testApprenticeship };
            _mockMediator.Setup(x => x.SendAsync(It.IsAny<GetApprenticeshipQueryRequest>())).ReturnsAsync(apprenticeshipGetResponse);
            _mockCurrentDateTime = new Mock<ICurrentDateTime>();
            _mockCurrentDateTime.SetupGet(x => x.Now).Returns(DateTime.UtcNow);

            _academicYearDateProvider = new Mock<IAcademicYearDateProvider>();
            _academicYearDateProvider.Setup(x => x.CurrentAcademicYearStartDate).Returns(new DateTime(2016, 8, 1));

            _academicYearValidator = new Mock<IAcademicYearValidator>();

            _handler = new UpdateApprenticeshipStatusCommandHandler(
                _mockCommitmentApi.Object,
                _mockMediator.Object,
                _mockCurrentDateTime.Object,
                _validator,
                _academicYearDateProvider.Object,
                _academicYearValidator.Object
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
        public void ShouldThrowValidationErrorIfChangeDateIsntTheSameAsStartDateForApprenticeshipWaitingToStart()
        {
            _testApprenticeship.StartDate = DateTime.UtcNow.AddMonths(2).Date;
            _validCommand.DateOfChange = DateTime.UtcNow.AddMonths(2).AddDays(1).Date;

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
            _validCommand.DateOfChange = _testApprenticeship.StartDate.Value.AddMonths(-1);
            _academicYearValidator.Setup(x => x.Validate(It.IsAny<DateTime>()))
                .Returns(AcademicYearValidationResult.NotWithinFundingPeriod);

            Func<Task> act = async () => await _handler.Handle(_validCommand);

            act.ShouldThrow<InvalidRequestException>().Where(x => x.ErrorMessages.Values.Contains("Date cannot be earlier than training start date"));
        }
    }
}
