using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Reservations.Api.Types;

namespace SFA.DAS.EmployerCommitments.Application.Queries.GetReservationValidation
{
    public class GetReservationValidationHandler : IAsyncRequestHandler<GetReservationValidationRequest, GetReservationValidationResponse>
    {
        private readonly IReservationsApiClient _reservationClient;

        public GetReservationValidationHandler(IReservationsApiClient reservationClient)
        {
            _reservationClient = reservationClient;
        }

        public async Task<GetReservationValidationResponse> Handle(GetReservationValidationRequest request)
        {
            var validationReservationMessage = new ReservationValidationMessage
            {
                StartDate = request.StartDate,
                CourseCode = request.TrainingCode,
                ReservationId = request.ReservationId
            };

            var result = await _reservationClient.ValidateReservation(validationReservationMessage, CancellationToken.None);


            return new GetReservationValidationResponse
            {
                Data = result
            };
        }
    }
}