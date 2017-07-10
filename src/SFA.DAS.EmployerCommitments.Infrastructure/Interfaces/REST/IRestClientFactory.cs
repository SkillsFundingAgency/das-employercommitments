using System;
using RestSharp;

namespace SFA.DAS.EmployerCommitments.Infrastructure.Interfaces.REST
{
    public interface IRestClientFactory
    {
        IRestClient Create(Uri baseUrl);
    }
}
