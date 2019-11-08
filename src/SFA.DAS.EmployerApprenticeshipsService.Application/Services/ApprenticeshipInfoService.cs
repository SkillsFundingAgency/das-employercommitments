using System.Linq;
using System.Net;
using System.Threading.Tasks;
using SFA.DAS.Apprenticeships.Api.Client;
using SFA.DAS.Apprenticeships.Api.Types.Exceptions;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;
using SFA.DAS.EmployerCommitments.Domain.Models.ApprenticeshipCourse;
using SFA.DAS.EmployerCommitments.Domain.Models.ApprenticeshipProvider;

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

        public ProvidersView GetProvider(long ukPrn)
        {
            try
            {
                var api = new Providers.Api.Client.ProviderApiClient(_configuration.BaseUrl);
                var provider = api.Get(ukPrn);
                return _mapper.MapFrom(provider);
            }
            catch (EntityNotFoundException)
            {
                return null;
            }
        }

    }
}