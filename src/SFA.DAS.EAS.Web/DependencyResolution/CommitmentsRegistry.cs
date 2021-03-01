using System.Net.Http;
using SFA.DAS.AutoConfiguration;
using SFA.DAS.Commitments.Api.Client;
using SFA.DAS.Commitments.Api.Client.Configuration;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.EmployerCommitments.Domain.Configuration;
using SFA.DAS.Http;
using SFA.DAS.Http.TokenGenerators;
using SFA.DAS.NLog.Logger.Web.MessageHandlers;
using StructureMap;

namespace SFA.DAS.EmployerCommitments.Web.DependencyResolution
{
    public class CommitmentsRegistry : Registry
    {
        public CommitmentsRegistry()
        {
            For<CommitmentsApiClientConfiguration>().Use(c => c.GetInstance<IAutoConfigurationService>().Get<CommitmentsApiClientConfiguration>(ConfigurationKeys.CommitmentsApiClient)).Singleton();
            For<ICommitmentsApiClientConfiguration>().Use(c => c.GetInstance<CommitmentsApiClientConfiguration>());

            For<HttpClient>().Use(x => GetHttpClient(x));

            For<ITrainingProgrammeApi>().Use<TrainingProgrammeApi>().Ctor<HttpClient>().Is(c => c.GetInstance<HttpClient>());
            For<IProviderCommitmentsApi>().Use<ProviderCommitmentsApi>().Ctor<HttpClient>().Is(c => c.GetInstance<HttpClient>());
            For<IEmployerCommitmentApi>().Use<EmployerCommitmentApi>().Ctor<HttpClient>().Is(c => c.GetInstance<HttpClient>());

            For<IValidationApi>().Use<ValidationApi>()
                .Ctor<ICommitmentsApiClientConfiguration>()
                .Is(c => c.GetInstance<CommitmentsApiClientConfiguration>())
                .Ctor<HttpClient>()
                .Is(c => c.GetInstance<HttpClient>());
        }

        private HttpClient GetHttpClient(IContext context)
        {
            var config = context.GetInstance<CommitmentsApiClientConfiguration>();

            var httpClientBuilder = string.IsNullOrWhiteSpace(config.ClientId)
                ? new HttpClientBuilder().WithBearerAuthorisationHeader(new JwtBearerTokenGenerator(config))
                : new HttpClientBuilder().WithBearerAuthorisationHeader(new AzureActiveDirectoryBearerTokenGenerator(config));

            return httpClientBuilder
                .WithDefaultHeaders()
                .WithHandler(new RequestIdMessageRequestHandler())
                .WithHandler(new SessionIdMessageRequestHandler())
                .Build();
        }
    }
}