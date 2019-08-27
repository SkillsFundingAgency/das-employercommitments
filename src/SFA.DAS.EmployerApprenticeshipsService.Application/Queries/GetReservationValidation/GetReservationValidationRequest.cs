using System;
using MediatR;

namespace SFA.DAS.EmployerCommitments.Application.Queries.GetReservationValidation
{
    public class GetReservationValidationRequest : IAsyncRequest<GetReservationValidationResponse>
    {
        public string TrainingCode { get; set; }
        public DateTime StartDate { get; set; }
        public Guid ReservationId { get; set; }
    }
}
