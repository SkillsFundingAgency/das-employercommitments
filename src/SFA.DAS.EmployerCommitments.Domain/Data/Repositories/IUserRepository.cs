using System.Threading.Tasks;
using SFA.DAS.EmployerCommitments.Domain.Models.UserProfile;

namespace SFA.DAS.EmployerCommitments.Domain.Data.Repositories
{
    public interface IUserRepository
    {
        Task<User> GetUserById(long id);
        Task<User> GetByUserRef(string id);
        Task<User> GetByEmailAddress(string emailAddress);
        Task Create(User registerUser);
        Task Update(User user);
        Task Upsert(User user);
        Task<Users> GetAllUsers();
    }
}
