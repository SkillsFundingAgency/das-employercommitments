using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;

namespace SFA.DAS.EmployerCommitments.Infrastructure.Services
{
    public class AzureAdAuthenticationService : IAzureAdAuthenticationService
    {
        public async Task<string> GetAuthenticationResult(string clientId, string appKey, string resourceId, string tenant)
        {
            var authority = $"https://login.microsoftonline.com/{tenant}";
            var clientCredential = new ClientCredential(clientId, appKey);
            var context = new AuthenticationContext(authority, true);
            var result = await context.AcquireTokenAsync(resourceId, clientCredential);
            return result.AccessToken;
        }
    }
}