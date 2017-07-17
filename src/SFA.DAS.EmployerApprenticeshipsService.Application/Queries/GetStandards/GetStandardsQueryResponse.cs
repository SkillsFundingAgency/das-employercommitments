using System.Collections.Generic;
using SFA.DAS.EmployerCommitments.Domain.Models.ApprenticeshipCourse;

namespace SFA.DAS.EmployerCommitments.Application.Queries.GetStandards
{
    public class GetStandardsQueryResponse
    {
        public List<Standard> Standards { get; set; }
    }
}