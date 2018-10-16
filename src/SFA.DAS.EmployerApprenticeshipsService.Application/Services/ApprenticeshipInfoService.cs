using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Apprenticeships.Api.Client;
using SFA.DAS.Apprenticeships.Api.Types.Exceptions;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;
using SFA.DAS.EmployerCommitments.Domain.Models.ApprenticeshipCourse;
using SFA.DAS.EmployerCommitments.Domain.Models.ApprenticeshipProvider;
using static System.String;

namespace SFA.DAS.EmployerCommitments.Application.Services
{
    public class ApprenticeshipInfoService : IApprenticeshipInfoService
    {
        private const string StandardsKey = "Standards";
        private const string FrameworksKey = "Frameworks";

        private readonly ICache _cache;
        private readonly IApprenticeshipInfoServiceConfiguration _configuration;
        private readonly IApprenticeshipInfoServiceMapper _mapper;

        public ApprenticeshipInfoService(ICache cache,
            IApprenticeshipInfoServiceConfiguration configuration,
            IApprenticeshipInfoServiceMapper mapper)
        {
            _cache = cache;
            _configuration = configuration;
            _mapper = mapper;
        }

        public async Task<StandardsView> GetStandardsAsync(bool refreshCache = false)
        {
            if (!await _cache.ExistsAsync(StandardsKey) || refreshCache)
            {
                var api = new StandardApiClient(_configuration.BaseUrl);

                var standards = (await api.GetAllAsync()).OrderBy(x => x.Title).ToList();

                await _cache.SetCustomValueAsync(StandardsKey, _mapper.MapFrom(standards));
            }

            return await _cache.GetCustomValueAsync<StandardsView>(StandardsKey);
        }

        public async Task<FrameworksView> GetFrameworksAsync(bool refreshCache = false)
        {
            if (!await _cache.ExistsAsync(FrameworksKey) || refreshCache)
            {
                var api = new FrameworkApiClient(_configuration.BaseUrl);

                var frameworks = (await api.GetAllAsync()).OrderBy(x => x.Title).ToList();

                await _cache.SetCustomValueAsync(FrameworksKey, _mapper.MapFrom(frameworks));
            }

            return await _cache.GetCustomValueAsync<FrameworksView>(FrameworksKey);
        }

        public ProvidersView GetProvider(long ukprn)
        {
            var deletedUkprns = new List<Apprenticeships.Api.Types.Providers.Provider>
            {
                new Apprenticeships.Api.Types.Providers.Provider
                {
                    Ukprn = 10000534,
                    ProviderName = "BARNFIELD COLLEGE",
                    Email = Empty
                },
                new Apprenticeships.Api.Types.Providers.Provider
                {
                    Ukprn = 10004442,
                    ProviderName = "MOULTON COLLEGE",
                    Email = Empty
                }
            };

            try
            {
                if (deletedUkprns.Exists(provider1 => provider1.Ukprn == ukprn))
                {
                    return _mapper.MapFrom(deletedUkprns.Single(provider1 => provider1.Ukprn == ukprn));
                }

                var api = new Providers.Api.Client.ProviderApiClient(_configuration.BaseUrl);
                var provider = api.Get(ukprn);
                return _mapper.MapFrom(provider);
            }
            catch (EntityNotFoundException)
            {
                return null;
            }
        }
    }
}