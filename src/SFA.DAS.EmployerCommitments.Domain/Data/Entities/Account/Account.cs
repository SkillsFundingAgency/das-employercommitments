using SFA.DAS.EmployerCommitments.Domain.Models.UserProfile;

namespace SFA.DAS.EmployerCommitments.Domain.Data.Entities.Account
{
    public class Account
    {
        public string Name { get; set; }
        public long Id { get; set; }
        public string HashedId { get; set; }
        public int RoleId { get; set; }
        public string RoleName => ((Role)RoleId).ToString();
    }
}
