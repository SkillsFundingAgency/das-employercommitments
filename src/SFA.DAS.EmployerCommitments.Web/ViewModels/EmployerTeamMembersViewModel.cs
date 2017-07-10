using System.Collections.Generic;
using SFA.DAS.EmployerCommitments.Domain.Models.AccountTeam;

namespace SFA.DAS.EmployerCommitments.Web.ViewModels
{
    public class EmployerTeamMembersViewModel
    {
        public List<TeamMember> TeamMembers { get; set; }
        public string HashedAccountId { get; set; }
        public string SuccessMessage { get; set; }
    }
}