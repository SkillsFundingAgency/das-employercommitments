using System.Collections.Generic;
using SFA.DAS.EmployerCommitments.Domain.Models.ApprenticeshipCourse;

namespace SFA.DAS.EmployerCommitments.Application.Queries.GetFrameworks
{
    public class GetFrameworksQueryResponse
    {
        public List<Framework> Frameworks { get; set; }
    }
}