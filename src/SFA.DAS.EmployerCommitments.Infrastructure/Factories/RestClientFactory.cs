using System;
using RestSharp;
using RestSharp.Deserializers;
using SFA.DAS.EmployerCommitments.Infrastructure.Interfaces.REST;

namespace SFA.DAS.EmployerCommitments.Infrastructure.Factories
{
    public class RestClientFactory : IRestClientFactory
    {
        public IRestClient Create(Uri baseUrl)
        {
            var client = new RestClient { BaseUrl = baseUrl };
            client.AddHandler("application/json", new JsonDeserializer());

            return client;
        }
    }
}
