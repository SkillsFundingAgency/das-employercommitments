using SFA.DAS.EmployerCommitments.Domain.Data.Entities.Account;

namespace SFA.DAS.EmployerCommitments.Web.ViewModels
{
    public class UserAccountsViewModel
    {
        public Accounts<Account> Accounts;
        public int Invitations;
        public FlashMessageViewModel FlashMessage;
        public string ErrorMessage;
    }
}