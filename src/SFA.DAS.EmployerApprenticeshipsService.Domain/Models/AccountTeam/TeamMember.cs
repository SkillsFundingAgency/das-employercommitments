using SFA.DAS.EmployerCommitments.Domain.Models.UserProfile;

namespace SFA.DAS.EmployerCommitments.Domain.Models.AccountTeam
{
    public class TeamMember
    {
        public long AccountId { get; set; }
        public string HashedAccountId { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string UserRef { get; set; }
        public Role Role { get; set; }
        public bool CanReceiveNotifications { get; set; }
    }
}
