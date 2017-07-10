using System.Threading.Tasks;
using SFA.DAS.EmployerCommitments.Domain.Models.ApprenticeshipProvider;

namespace SFA.DAS.EmployerCommitments.Domain.Data.Repositories
{
    public interface IProviderRepository
    {
        Task<Providers> GetAllProviders();
    }
}