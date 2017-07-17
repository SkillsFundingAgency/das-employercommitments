using System.Collections.Generic;
using SFA.DAS.EmployerCommitments.Domain.Models.AccountTeam;

namespace SFA.DAS.EmployerCommitments.Web.ViewModels
{
    public class UserInvitationsViewModel
    {
        public List<InvitationView> Invitations { get; set; }
        public bool ShowBreadCrumbs { get; set; }
    }
}
