using System.Threading.Tasks;
using SFA.DAS.EmployerCommitments.Domain.Models.ApprenticeshipCourse;
using SFA.DAS.EmployerCommitments.Domain.Models.ApprenticeshipProvider;

namespace SFA.DAS.EmployerCommitments.Domain.Interfaces
{
    public interface IApprenticeshipInfoService
    {
        Task<StandardsView> GetStandards(bool refreshCache = false);
        Task<ProvidersView> GetProvider(long ukprn);
        Task<AllTrainingProgrammesView> GetAll(bool refreshCache = false);
    }
}