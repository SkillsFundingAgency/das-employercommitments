using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.EmployerCommitments.Domain.Models.UserProfile;

namespace SFA.DAS.EmployerCommitments.Domain.Models.AccountTeam
{
    public class Account
    {
        public long Id { get; set; }
        public ApprenticeshipEmployerType ApprenticeshipEmployerType { get; set; }
    }
}
