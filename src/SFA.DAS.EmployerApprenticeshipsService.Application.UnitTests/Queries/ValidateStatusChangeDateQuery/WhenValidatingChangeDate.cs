using System;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.EmployerCommitments.Application.Queries.GetApprenticeship;
using SFA.DAS.EmployerCommitments.Application.Queries.ValidateStatusChangeDate;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;
using SFA.DAS.EmployerCommitments.Domain.Models.AcademicYear;
using SFA.DAS.EmployerCommitments.Domain.Models.Apprenticeship;
using SFA.DAS.EmployerCommitments.Infrastructure.Services;

namespace SFA.DAS.EmployerCommitments.Application.UnitTests.Queries.ValidateStatusChangeDateQuery
{
    [TestFixture]
    public sealed class WhenValidatingChangeDate
    {
        private Application.Queries.ValidateStatusChangeDate.ValidateStatusChangeDateQuery _testQuery;
        private ValidateStatusChangeDateQueryHandler _handler;
        private Mock<IMediator> _mockMediator;
        private Mock<ICurrentDateTime> _mockCurrentDate;
        private Mock<IAcademicYearValidator> _academicYearValidator;
        private Apprenticeship _apprenticeship;

        [SetUp]
        public void Setup()
        {
            _testQuery = new Application.Queries.ValidateStatusChangeDate.ValidateStatusChangeDateQuery { AccountId = 123, ApprenticeshipId = 456, ChangeOption = ChangeOption.SpecificDate };
            _apprenticeship = new Apprenticeship { StartDate = DateTime.UtcNow.Date };

            _mockCurrentDate = new Mock<ICurrentDateTime>();
            _mockCurrentDate.SetupGet(x => x.Now).Returns(new DateTime(2017, 6, 20)); // Started training

            _academicYearValidator = new Mock<IAcademicYearValidator>();

            _mockMediator = new Mock<IMediator>();
            _mockMediator.Setup(x => x.SendAsync(It.IsAny<GetApprenticeshipQueryRequest>()))
                .ReturnsAsync(new GetApprenticeshipQueryResponse { Apprenticeship = _apprenticeship });

            _handler = new ValidateStatusChangeDateQueryHandler(new ValidateStatusChangeDateQueryValidator(), _mockMediator.Object, _mockCurrentDate.Object, _academicYearValidator.Object);
        }

        [Test]
        public async Task WhenDateIsInTheFutureAnValidationErrorReturned()
        {
            _testQuery.DateOfChange = new DateTime(2017, 7, 10); // Change date in the future
            _mockCurrentDate.SetupGet(x => x.Now).Returns(DateTime.UtcNow.AddDays(-2)); // Started training
            _apprenticeship.StartDate = DateTime.UtcNow.AddDays(2);
            _mockMediator.Setup(x => x.SendAsync(It.IsAny<GetApprenticeshipQueryRequest>()))
                .ReturnsAsync(new GetApprenticeshipQueryResponse { Apprenticeship = _apprenticeship });

            var response = await _handler.Handle(_testQuery);

            response.ValidationResult.IsValid().Should().BeFalse();
            response.ValidationResult.ValidationDictionary.Should().ContainValue("Date must be a date in the past");
        }

        [Test]
        public async Task WhenDateIsEarlierThanTrainingStartDateAValidationErrorReturned()
        {
            _apprenticeship.StartDate = new DateTime(2017, 5, 1);
            _testQuery.DateOfChange = new DateTime(2017, 4, 28); // Change date before Training start date.

            var response = await _handler.Handle(_testQuery);

            response.ValidationResult.IsValid().Should().BeFalse();
            response.ValidationResult.ValidationDictionary.Should().ContainValue("Date cannot be earlier than training start date");
        }

        [TestCase(AcademicYearValidationResult.Success, true)]
        [TestCase(AcademicYearValidationResult.NotWithinFundingPeriod, false)]
        public async Task WhenDateIsInPreviousAcademicYearAndAfterR14DateThenValidationFails(
            AcademicYearValidationResult academicYearValidationResult, bool expectedPassValidation)
        {
            //Arrange
            _apprenticeship.StartDate = new DateTime(2016, 3, 1);
            _testQuery.DateOfChange = new DateTime(2016, 5, 1);

            _academicYearValidator.Setup(x => x.Validate(It.IsAny<DateTime>())).Returns(academicYearValidationResult);

            //Act
            var response = await _handler.Handle(_testQuery);
            
            //Assert
            response.ValidationResult.IsValid().Should().Be(expectedPassValidation);

            if (!expectedPassValidation)
            {
                response.ValidationResult.ValidationDictionary.Should().ContainValue("Date can't be in previous academic year");
            }
        }
    }
}
