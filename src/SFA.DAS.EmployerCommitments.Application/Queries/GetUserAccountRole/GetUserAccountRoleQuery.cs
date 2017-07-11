using System.Collections.Generic;
using System.Linq;
using System.Text;
using MediatR;

namespace SFA.DAS.EmployerCommitments.Application.Queries.GetUserAccountRole
{
    public class GetUserAccountRoleQuery : IAsyncRequest<GetUserAccountRoleResponse>
    {
        public string HashedAccountId { get; set; }
        public string UserId { get; set; }
    }
}
