using System.Threading.Tasks;
using SFA.DAS.EmployerCommitments.Domain.Models.ApprenticeshipCourse;
using SFA.DAS.EmployerCommitments.Domain.Models.ApprenticeshipProvider;

namespace SFA.DAS.EmployerCommitments.Domain.Interfaces
{
    public interface IApprenticeshipInfoServiceWrapper
    {
        Task<StandardsView> GetStandardsAsync(bool refreshCache = false);
        Task<FrameworksView> GetFrameworksAsync(bool refreshCache = false);
        ProvidersView GetProvider(long ukPrn);
    }
}