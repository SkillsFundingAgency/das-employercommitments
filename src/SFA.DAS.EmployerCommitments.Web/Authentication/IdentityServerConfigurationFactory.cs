using SFA.DAS.EmployerCommitments.Domain.Configuration;
using SFA.DAS.EmployerUsers.WebClientComponents;

namespace SFA.DAS.EmployerCommitments.Web.Authentication
{
    public class IdentityServerConfigurationFactory : ConfigurationFactory
    {
        private readonly EmployerCommitmentsServiceConfiguration _configuration;

        public IdentityServerConfigurationFactory(EmployerCommitmentsServiceConfiguration configuration)
        {
            _configuration = configuration;
        }

        public override ConfigurationContext Get()
        {
            return new ConfigurationContext {AccountActivationUrl = _configuration.Identity.BaseAddress.Replace("/identity","") + _configuration.Identity.AccountActivationUrl};
        }
    }
}