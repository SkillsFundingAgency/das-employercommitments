using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.EmployerCommitments.Domain.Models.Levy;

namespace SFA.DAS.EmployerCommitments.Domain.Data.Repositories
{
    public interface IEnglishFractionRepository
    {
        Task<DateTime> GetLastUpdateDate();
        Task<IEnumerable<DasEnglishFraction>> GetAllEmployerFractions(string employerReference);
        Task CreateEmployerFraction(DasEnglishFraction fractions, string employerReference);
        Task SetLastUpdateDate(DateTime dateUpdated);
        Task<IEnumerable<DasEnglishFraction>> GetCurrentFractionForSchemes(long accountId, IEnumerable<string> employerReferences);
    }
}
