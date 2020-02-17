using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerCommitments.Application.Queries.GetReservationValidation;
using SFA.DAS.Reservations.Api.Types;

namespace SFA.DAS.EmployerCommitments.Application.UnitTests.Queries.GetReservationValidation
{
    [TestFixture]
    public class WhenIGetReservationValidation
    {
        private GetReservationValidationFixture _fixture;

        [SetUp]
        public void Arrange()
        {
            _fixture = new GetReservationValidationFixture();
        }

        [Test]
        public async Task ThenVerifyApiMessageIsMappedCorrectly()
        {
            await _fixture.Sut.Handle(_fixture.GetReservationValidationRequest);

            _fixture.VerifyRequestMapsFieldsCorrectlyToApiInputMessage(_fixture.GetReservationValidationRequest);
        }

        [Test]
        public async Task AndNoReservationErrorsAreFoundThenReturnsNoErrors()
        {
            _fixture.SetupApiToReturn(_fixture.ReservationValidationResultWithNoErrors);

            var result = await _fixture.Sut.Handle(_fixture.GetReservationValidationRequest);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Data);
            Assert.IsFalse(result.Data.HasErrors);
        }

        [Test]
        public async Task AndReservationErrorsAreFoundThenReturnsErrors()
        {
            _fixture.SetupApiToReturn(_fixture.ReservationValidationResultWithErrors);

            var result = await _fixture.Sut.Handle(_fixture.GetReservationValidationRequest);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Data);
            Assert.IsTrue(result.Data.HasErrors);
            Assert.AreEqual(_fixture.ReservationValidationResultWithErrors,  result.Data);
        }
    }

    public class GetReservationValidationFixture
    {
        public GetReservationValidationFixture()
        {
            var autoFixture = new Fixture();

            MockReservationApiClient = new Mock<IReservationsApiClient>();
            ReservationValidationResultWithNoErrors =
                new ReservationValidationResult(new List<ReservationValidationError>());
            ReservationValidationResultWithErrors = autoFixture.Create<ReservationValidationResult>();
            GetReservationValidationRequest = autoFixture.Create<GetReservationValidationRequest>();

            Sut = new GetReservationValidationHandler(MockReservationApiClient.Object);
        }

    public GetReservationValidationHandler Sut { get; }
        public Mock<IReservationsApiClient> MockReservationApiClient { get; }
        public ReservationValidationResult ReservationValidationResultWithNoErrors { get; }
        public ReservationValidationResult ReservationValidationResultWithErrors { get; }
        public GetReservationValidationRequest GetReservationValidationRequest { get; }

        public GetReservationValidationFixture SetupApiToReturn(ReservationValidationResult result)
        {
            MockReservationApiClient
                .Setup(x => x.ValidateReservation(It.IsAny<ReservationValidationMessage>(),
                    It.IsAny<CancellationToken>())).ReturnsAsync(result);
            return this;
        }

        public void VerifyRequestMapsFieldsCorrectlyToApiInputMessage(GetReservationValidationRequest input)
        {
            MockReservationApiClient
                .Verify(
                    x => x.ValidateReservation(
                        It.Is<ReservationValidationMessage>(p =>
                            p.StartDate == input.StartDate && p.CourseCode == input.TrainingCode &&
                            p.ReservationId == input.ReservationId), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}