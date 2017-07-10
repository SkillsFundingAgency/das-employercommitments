using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.EmployerCommitments.Domain.Models.Employer;

namespace SFA.DAS.EmployerCommitments.Domain.Interfaces
{
    public interface IAddressLookupService
    {
        Task<ICollection<Address>> GetAddressesByPostcode(string postcode);
    }
}
