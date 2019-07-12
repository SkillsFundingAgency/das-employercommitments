using System;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.EmployerCommitments.Application.Queries.ValidateStatusChangeDate;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;
using SFA.DAS.EmployerCommitments.Domain.Models.AcademicYear;
using SFA.DAS.EmployerCommitments.Domain.Models.Apprenticeship;

namespace SFA.DAS.EmployerCommitments.Application.UnitTests.Queries.ValidateStatusChangeDateQuery
{
    [TestFixture]
    public sealed class WhenValidatingChangeDate
    {
        private Application.Queries.ValidateStatusChangeDate.ValidateStatusChangeDateQuery _testQuery;
        private ValidateStatusChangeDateQueryHandler _handler;
        private Mock<IEmployerCommitmentApi> _commitmentsApi;
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

            _commitmentsApi = new Mock<IEmployerCommitmentApi>();
            _commitmentsApi.Setup(x => x.GetEmployerApprenticeship(It.IsAny<long>(), It.IsAny<long>()))
                .ReturnsAsync(_apprenticeship);

            _handler = new ValidateStatusChangeDateQueryHandler(new ValidateStatusChangeDateQueryValidator(),
                _mockCurrentDate.Object,
                _academicYearValidator.Object,
                _commitmentsApi.Object);
        }

        [Test]
        public async Task WhenDateIsInTheFutureAnValidationErrorReturned()
        {
            _testQuery.DateOfChange = new DateTime(2017, 7, 10); // Change date in the future
            _mockCurrentDate.SetupGet(x => x.Now).Returns(DateTime.UtcNow.AddDays(-2)); // Started training
            _apprenticeship.StartDate = DateTime.UtcNow.AddDays(2);

            var response = await _handler.Handle(_testQuery);

            response.ValidationResult.IsValid().Should().BeFalse();
            response.ValidationResult.ValidationDictionary.Should().ContainValue("The stop date cannot be in the future");
        }

        [Test]
        public async Task WhenDateIsInEarlierMonthThanApprenticeshipStartDateAValidationErrorReturned()
        {
            _apprenticeship.StartDate = new DateTime(2017, 5, 1);
            _testQuery.DateOfChange = _apprenticeship.StartDate.Value.AddMonths(-1); // Change date before apprenticeship start date.

            var response = await _handler.Handle(_testQuery);

            response.ValidationResult.IsValid().Should().BeFalse();
            response.ValidationResult.ValidationDictionary.Should().ContainValue("The stop month cannot be before the apprenticeship started");
        }
    }
}
