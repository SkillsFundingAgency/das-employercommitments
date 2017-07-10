using System;
using SFA.DAS.EmployerCommitments.Infrastructure.Interfaces.REST;
using SFA.DAS.EmployerCommitments.Infrastructure.Services;

namespace SFA.DAS.EmployerCommitments.Infrastructure.Factories
{
    public class RestServiceFactory : IRestServiceFactory
    {
        private readonly IRestClientFactory _clientFactory;

        public RestServiceFactory(IRestClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public IRestService Create(string baseUrl)
        {
            var client = _clientFactory.Create(new Uri(baseUrl));

            return new RestService(client);
        }
    }
}
