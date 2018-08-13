using System;
using MediatR;

namespace SFA.DAS.EmployerCommitments.Application.Queries.GetTrainingProgrammes
{
    public sealed class GetTrainingProgrammesQueryRequest : IAsyncRequest<GetTrainingProgrammesQueryResponse>
    {
        public GetTrainingProgrammesQueryRequest()
        {
            IncludeFrameworks = true;
            EffectiveDate = null;
        }

        public bool IncludeFrameworks { get; set; }
        public DateTime? EffectiveDate { get; set; }
    }
}