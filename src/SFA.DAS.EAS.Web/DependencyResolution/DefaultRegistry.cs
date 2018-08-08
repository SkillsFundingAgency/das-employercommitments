// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultRegistry.cs" company="Web Advanced">
// Copyright 2012 Web Advanced (www.webadvanced.com)
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0

// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Web;
using AutoMapper;
using FeatureToggle;
using MediatR;
using Microsoft.Azure;
using SFA.DAS.Audit.Client;
using SFA.DAS.Commitments.Api.Client;
using SFA.DAS.Commitments.Api.Client.Configuration;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Configuration;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.Configuration.FileStorage;
using SFA.DAS.CookieService;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.EmployerCommitments.Application.Validation;
using SFA.DAS.EmployerCommitments.Domain.Configuration;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;
using SFA.DAS.EmployerCommitments.Infrastructure.Caching;
using SFA.DAS.EmployerCommitments.Infrastructure.Services;
using SFA.DAS.EmployerCommitments.Web.PublicHashingService;
using SFA.DAS.EmployerCommitments.Web.Validators;
using SFA.DAS.EmployerCommitments.Web.Validators.Messages;
using SFA.DAS.EmployerCommitments.Web.ViewModels;
using SFA.DAS.Http;
using SFA.DAS.Notifications.Api.Client;
using SFA.DAS.Http.TokenGenerators;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Notifications.Api.Client.Configuration;
using StructureMap;
using StructureMap.Graph;
using StructureMap.TypeRules;
using IConfiguration = SFA.DAS.EmployerCommitments.Domain.Interfaces.IConfiguration;
using NotificationsApiClientConfiguration = SFA.DAS.EmployerCommitments.Domain.Configuration.NotificationsApiClientConfiguration;
using SFA.DAS.HashingService;

namespace SFA.DAS.EmployerCommitments.Web.DependencyResolution
{
    public class DefaultRegistry : Registry
    {
        private const string ServiceName = "SFA.DAS.EmployerCommitments";
        private const string ServiceNamespace = "SFA.DAS";

        public DefaultRegistry()
        {

            Scan(
                scan =>
                {
                    scan.AssembliesFromApplicationBaseDirectory(a => a.GetName().Name.StartsWith(ServiceNamespace));
                    scan.RegisterConcreteTypesAgainstTheFirstInterface();
                    scan.ConnectImplementationsToTypesClosing(typeof(IValidator<>)).OnAddedPluginTypes(t => t.Singleton());
                });

            For<HttpContextBase>().Use(() => new HttpContextWrapper(HttpContext.Current));
            For(typeof(ICookieService<>)).Use(typeof(HttpCookieService<>));
            For(typeof(ICookieStorageService<>)).Use(typeof(CookieStorageService<>));
            For<IApprenticeshipValidationErrorText>().Use<WebApprenticeshipValidationText>();
            For<IConfiguration>().Use<EmployerCommitmentsServiceConfiguration>();

            var config = this.GetConfiguration();
            
            For<ICache>().Use<InMemoryCache>(); //RedisCache

            For<IApprenticeshipInfoServiceConfiguration>().Use(config.ApprenticeshipInfoService);
            For<IApprenticeshipCoreValidator>().Use<ApprenticeshipCoreValidator>().Singleton();
            For<IApprenticeshipViewModelValidator>().Use<ApprenticeshipViewModelValidator>().Singleton();
            For<IValidateApprovedApprenticeship>().Use<ApprovedApprenticeshipViewModelValidator>().Singleton();

            ConfigureHashingService(config);
            SetUpCommitmentApi(config);

            For<IBooleanToggleValueProvider>().Use<CloudConfigurationBooleanValueProvider>();
            For<IFeatureToggleService>().Use<FeatureToggleService>();
          
            ConfigureNotificationsApi();

            RegisterMapper();

            RegisterMediator();

            RegisterAuditService();

            RegisterExecutionPolicies();

            RegisterLogger();

            RegisterAccountApi();
        }

        private void RegisterAccountApi()
        {
            For<IAccountApiClient>().Use<AccountApiClient>();

            var useStubSetting = CloudConfigurationManager.GetSetting("UseAccountApiTransfersStub") ?? "False";

            if (Boolean.TryParse(useStubSetting, out bool useStub))
            {
                if (useStub)
                {
                    For<IAccountApiClient>().DecorateAllWith<Decorators.AccountApiClientDecorator>();  
                }
            }
        }

        private void SetUpCommitmentApi(EmployerCommitmentsServiceConfiguration config)
        {
            var bearerToken = (IGenerateBearerToken)new JwtBearerTokenGenerator(config.CommitmentsApi);

            var httpClient = new HttpClientBuilder()
                .WithBearerAuthorisationHeader(bearerToken)
                .WithHandler(new NLog.Logger.Web.MessageHandlers.RequestIdMessageRequestHandler())
                .WithHandler(new NLog.Logger.Web.MessageHandlers.SessionIdMessageRequestHandler())
                .WithDefaultHeaders()
                .Build();

            For<IEmployerCommitmentApi>().Use<EmployerCommitmentApi>()
                .Ctor<ICommitmentsApiClientConfiguration>().Is(config.CommitmentsApi)
                .Ctor<HttpClient>().Is(httpClient);

            For<IValidationApi>().Use<ValidationApi>()
                .Ctor<ICommitmentsApiClientConfiguration>().Is(config.CommitmentsApi)
                .Ctor<HttpClient>().Is(httpClient);

            For<IValidationApi>().Use<ValidationApi>()
                .Ctor<ICommitmentsApiClientConfiguration>().Is(config.CommitmentsApi)
                .Ctor<HttpClient>().Is(httpClient);
        }

        private void ConfigureNotificationsApi()
        {
            var config = Infrastructure.DependencyResolution.ConfigurationHelper.GetConfiguration
                <NotificationsApiClientConfiguration>($"{ServiceName}.Notifications");

            HttpClient httpClient;

            if (string.IsNullOrWhiteSpace(config.ClientId))
            {
                httpClient = new Http.HttpClientBuilder()
                .WithBearerAuthorisationHeader(new JwtBearerTokenGenerator(config))
                .Build();
            }
            else
            {
                httpClient = new Http.HttpClientBuilder()
                .WithBearerAuthorisationHeader(new AzureADBearerTokenGenerator(config))
                .Build();
            }

            For<INotificationsApi>().Use<NotificationsApi>().Ctor<HttpClient>().Is(httpClient);

            For<INotificationsApiClientConfiguration>().Use(config);
        }

        private void RegisterExecutionPolicies()
        {
            For<Infrastructure.ExecutionPolicies.ExecutionPolicy>()
                .Use<Infrastructure.ExecutionPolicies.IdamsExecutionPolicy>()
                .Named(Infrastructure.ExecutionPolicies.IdamsExecutionPolicy.Name);
        }

        private void RegisterAuditService()
        {
            var environment = Environment.GetEnvironmentVariable("DASENV");
            if (string.IsNullOrEmpty(environment))
            {
                environment = CloudConfigurationManager.GetSetting("EnvironmentName");
            }

            For<IAuditMessageFactory>().Use<AuditMessageFactory>().Singleton();

            if (environment.Equals("LOCAL"))
            {
                For<IAuditApiClient>().Use<StubAuditApiClient>();
            }
            else
            {
                For<IAuditApiClient>().Use<AuditApiClient>();
            }
        }

        private void RegisterMapper()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(a => a.FullName.StartsWith("SFA.DAS.EmployerCommitments"));

            var mappingProfiles = new List<Profile>();

            foreach (var assembly in assemblies)
            {
                var profiles = Assembly.Load(assembly.FullName).GetTypes()
                                       .Where(t => typeof(Profile).IsAssignableFrom(t))
                                       .Where(t => t.IsConcrete() && t.HasConstructors())
                                       .Select(t => (Profile)Activator.CreateInstance(t));

                mappingProfiles.AddRange(profiles);
            }

            var config = new MapperConfiguration(cfg =>
            {
                mappingProfiles.ForEach(cfg.AddProfile);
            });

            var mapper = config.CreateMapper();

            For<IConfigurationProvider>().Use(config).Singleton();
            For<IMapper>().Use(mapper).Singleton();
        }

        private EmployerCommitmentsServiceConfiguration GetConfiguration()
        {
            var environment = Environment.GetEnvironmentVariable("DASENV");
            if (string.IsNullOrEmpty(environment))
            {
                environment = CloudConfigurationManager.GetSetting("EnvironmentName");
            }
            if (environment.Equals("LOCAL") || environment.Equals("AT") || environment.Equals("TEST"))
            {
                PopulateSystemDetails(environment);
            }

            var configurationRepository = GetConfigurationRepository();
            var configurationService = new ConfigurationService(configurationRepository,
                new ConfigurationOptions(ServiceName, environment, "1.0"));

            var result = configurationService.Get<EmployerCommitmentsServiceConfiguration>();

            return result;
        }

        private static IConfigurationRepository GetConfigurationRepository()
        {
            IConfigurationRepository configurationRepository;
            if (bool.Parse(ConfigurationManager.AppSettings["LocalConfig"]))
            {
                configurationRepository = new FileStorageConfigurationRepository();
            }
            else
            {
                configurationRepository = new AzureTableStorageConfigurationRepository(CloudConfigurationManager.GetSetting("ConfigurationStorageConnectionString"));
            }
            return configurationRepository;
        }

        private void RegisterMediator()
        {
            For<SingleInstanceFactory>().Use<SingleInstanceFactory>(ctx => t => ctx.GetInstance(t));
            For<MultiInstanceFactory>().Use<MultiInstanceFactory>(ctx => t => ctx.GetAllInstances(t));
            For<IMediator>().Use<Mediator>();
        }

        private void PopulateSystemDetails(string envName)
        {
            SystemDetailsViewModel.EnvironmentName = envName;
            SystemDetailsViewModel.VersionNumber = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        private void RegisterLogger()
        {
            For<IRequestContext>().Use(x => new RequestContext(new HttpContextWrapper(HttpContext.Current)));
            For<ILog>().Use(x => new NLogLogger(
                x.ParentType,
                x.GetInstance<IRequestContext>(),
                null)).AlwaysUnique();
        }

        private void ConfigureHashingService(EmployerCommitmentsServiceConfiguration config)
        {
            For<IHashingService>().Use(x => new HashingService.HashingService(config.AllowedHashstringCharacters, config.Hashstring));
            For<IPublicHashingService>().Use(x => new PublicHashingService.PublicHashingService(config.PublicAllowedHashstringCharacters, config.PublicHashstring));
        }

    }
}