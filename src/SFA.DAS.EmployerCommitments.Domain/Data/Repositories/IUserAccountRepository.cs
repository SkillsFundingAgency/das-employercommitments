﻿using System.Threading.Tasks;
using SFA.DAS.EmployerCommitments.Domain.Data.Entities.Account;
using SFA.DAS.EmployerCommitments.Domain.Models.UserProfile;

namespace SFA.DAS.EmployerCommitments.Domain.Data.Repositories
{
    public interface IUserAccountRepository 
    {
        Task<Accounts<Account>> GetAccountsByUserRef(string userRef);
        Task<User> Get(string email);
        Task<User> Get(long id);
    }
}
