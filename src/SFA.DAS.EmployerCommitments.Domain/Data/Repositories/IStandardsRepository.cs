using System.Threading.Tasks;
using SFA.DAS.EmployerCommitments.Domain.Models.ApprenticeshipCourse;

namespace SFA.DAS.EmployerCommitments.Domain.Data.Repositories
{
    public interface IStandardsRepository
    {
        Task<Standard[]> GetAllAsync();
        Task<Standard> GetByCodeAsync(string code);
    }
}