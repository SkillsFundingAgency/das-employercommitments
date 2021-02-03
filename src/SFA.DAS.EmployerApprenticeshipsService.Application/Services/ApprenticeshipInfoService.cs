using System;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;
using SFA.DAS.EmployerCommitments.Domain.Models.ApprenticeshipCourse;
using SFA.DAS.EmployerCommitments.Domain.Models.ApprenticeshipProvider;

namespace SFA.DAS.EmployerCommitments.Application.Services
{
    public class ApprenticeshipInfoService : IApprenticeshipInfoService
    {
        private const string StandardsKey = "Standards";

        private readonly ICache _cache;
        private readonly IProviderCommitmentsApi _providerCommitmentsApi;
        private readonly ITrainingProgrammeApi _trainingProgrammeApi;
        
        public ApprenticeshipInfoService(ICache cache,
            IProviderCommitmentsApi providerCommitmentsApi,
            ITrainingProgrammeApi trainingProgrammeApi)
        {
            _cache = cache;
            _providerCommitmentsApi = providerCommitmentsApi;
            _trainingProgrammeApi = trainingProgrammeApi;
        }

        public async Task<StandardsView> GetStandards(bool refreshCache = false)
        {
            if (!await _cache.ExistsAsync(StandardsKey) || refreshCache)
            {
                var standards = (await _trainingProgrammeApi.GetAllStandards()).TrainingProgrammes.ToList().OrderBy(x => x.Name).ToList();

                await _cache.SetCustomValueAsync(StandardsKey, new StandardsView
                {
                    CreationDate = DateTime.UtcNow,
                    Standards = standards
                });
            }

            return await _cache.GetCustomValueAsync<StandardsView>(StandardsKey);
        }
        
        public async Task<AllTrainingProgrammesView> GetAll(bool refreshCache = false)
        {
            if (!await _cache.ExistsAsync(nameof(AllTrainingProgrammesView)) || refreshCache)
            {
                var trainingProgrammes = (await _trainingProgrammeApi.GetAll()).TrainingProgrammes.ToList().OrderBy(x => x.Name).ToList();

                await _cache.SetCustomValueAsync(nameof(AllTrainingProgrammesView), new AllTrainingProgrammesView
                {
                    CreatedDate = DateTime.UtcNow,
                    TrainingProgrammes = trainingProgrammes
                });
            }

            return await _cache.GetCustomValueAsync<AllTrainingProgrammesView>(nameof(AllTrainingProgrammesView));
        }

        public async Task<ProvidersView> GetProvider(long ukprn)
        {
            try
            {
                var api = await _providerCommitmentsApi.GetProvider(ukprn);
                return new ProvidersView
                {
                    CreatedDate = DateTime.UtcNow,
                    Provider = new EmployerCommitments.Domain.Models.ApprenticeshipProvider.Provider
                    {
                        Name = api.Provider.Name,
                        Ukprn = api.Provider.Ukprn,
                        ProviderName = api.Provider.Name
                    }
                };
            }
            catch (Exception)
            {
                return null;
            }
        }

    }
}