using System.Threading.Tasks;
using SFA.DAS.EmployerCommitments.Domain.Models.Employer;

namespace SFA.DAS.EmployerCommitments.Domain.Interfaces
{
    public interface IEmployerVerificationService
    {
        Task<EmployerInformation> GetInformation(string id);
    }
}