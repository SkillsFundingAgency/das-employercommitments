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
        private IAcademicYearValidator _academicYearValidator;

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
            _academicYearValidator = new AcademicYearValidator(_mockCurrentDateTime.Object,_academicYearDateProvider.Object);

            _academicYearDateProvider.Setup(x => x.CurrentAcademicYearStartDate).Returns(new DateTime(2016, 8, 1));
            _academicYearDateProvider.Setup(x => x.CurrentAcademicYearEndDate).Returns(new DateTime(2017, 7, 31));
            _academicYearDateProvider.Setup(x => x.LastAcademicYearFundingPeriod).Returns(new DateTime(2016, 10, 18));

            _handler = new UpdateApprenticeshipStatusCommandHandler(
                _mockCommitmentApi.Object,
                _mockMediator.Object,
                _mockCurrentDateTime.Object,
                _validator,
                _academicYearDateProvider.Object,
                _academicYearValidator
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

        [Test(Description = "Validation fails if date of change is in the previous academic year and the R14 date has passed")]
        public void ShouldThrowValidationErrorAfterR14Close()
        {
            _testApprenticeship.StartDate = new DateTime(2016, 3, 1); //early last academic year
            _validCommand.DateOfChange = new DateTime(2016, 5, 1); //last academic year
            _mockCurrentDateTime.Setup(x => x.Now).Returns(new DateTime(2016, 10, 19)); //after cut-off

            Func<Task> act = async () => await _handler.Handle(_validCommand);

            act.ShouldThrow<InvalidRequestException>().Where(x => x.ErrorMessages.Values.Contains("The earliest date you can stop this apprentice is 01 08 2016"));
        }

        [Test(Description = "Validation passes if date of change is in the previous academic year but the R14 date has not yet passed")]
        public void ShouldNotThrowValidationErrorIfBeforeR14Close()
        {
            _testApprenticeship.StartDate = new DateTime(2016, 3, 1);
            _validCommand.DateOfChange = new DateTime(2016, 5, 1); //last academic year
            _mockCurrentDateTime.Setup(x => x.Now).Returns(new DateTime(2016, 10, 17)); //prior to cut-off

            Func<Task> act = async () => await _handler.Handle(_validCommand);

            act.ShouldNotThrow<InvalidRequestException>();
        }

        [Test(Description = "Validation fails for both R14 having passed and change date before Start Date - Start Date error takes precedence")]
        public void ShouldThrowStartDateValidationErrorAfterR14CloseAndStopDateBeforeStartDate()
        {
            _testApprenticeship.StartDate = new DateTime(2016, 3, 1);
            _validCommand.DateOfChange = new DateTime(2016, 1, 1); //last academic year
            _mockCurrentDateTime.Setup(x => x.Now).Returns(new DateTime(2016, 10, 19)); //after cut-off

            Func<Task> act = async () => await _handler.Handle(_validCommand);

            act.ShouldThrow<InvalidRequestException>().Where(x => x.ErrorMessages.Values.Contains("Date cannot be earlier than training start date"));
        }

        //Story says name must be in error message, but not in design so waiting on this..............
        //[Test]
        //public void ShowThrowValidationErrorContainingApprenticeNameAfterR14Close()
        //{
        //    _testApprenticeship.FirstName = "Test";
        //    _testApprenticeship.LastName = "TestSurname";
        //    _testApprenticeship.StartDate = new DateTime(2016, 3, 1);
        //    _validCommand.DateOfChange = new DateTime(2016, 5, 1); //last academic year
        //    _mockCurrentDateTime.Setup(x => x.Now).Returns(new DateTime(2016, 10, 19)); //after cut-off

        //    Func<Task> act = async () => await _handler.Handle(_validCommand);
        //    act.ShouldThrow<InvalidRequestException>().Where(x => x.ErrorMessages.Values.Contains("Test TestSurname"));
        //}
    }
}
