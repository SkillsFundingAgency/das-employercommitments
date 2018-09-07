using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Apprenticeships.Api.Types;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;
using SFA.DAS.EmployerCommitments.Domain.Models.ApprenticeshipCourse;
using SFA.DAS.EmployerCommitments.Domain.Models.ApprenticeshipProvider;
using Framework = SFA.DAS.EmployerCommitments.Domain.Models.ApprenticeshipCourse.Framework;
using Standard = SFA.DAS.EmployerCommitments.Domain.Models.ApprenticeshipCourse.Standard;
using FundingPeriod = SFA.DAS.EmployerCommitments.Domain.Models.ApprenticeshipCourse.FundingPeriod;

namespace SFA.DAS.EmployerCommitments.Application.Mappers
{
    public class ApprenticeshipInfoServiceMapper : IApprenticeshipInfoServiceMapper
    {
       public FrameworksView MapFrom(List<FrameworkSummary> frameworks)
        {
            return new FrameworksView
            {
                CreatedDate = DateTime.UtcNow,
                Frameworks = frameworks.Select(x => new Framework
                {
                    Id = x.Id,
                    Title = GetTitle(x.FrameworkName.Trim() == x.PathwayName.Trim() ? x.FrameworkName : x.Title, x.Level),
                    FrameworkCode = x.FrameworkCode,
                    FrameworkName = x.FrameworkName,
                    ProgrammeType = x.ProgType,
                    Level = x.Level,
                    PathwayCode = x.PathwayCode,
                    PathwayName = x.PathwayName,
                    Duration = x.Duration,
                    MaxFunding = x.CurrentFundingCap,
                    EffectiveFrom = x.EffectiveFrom,
                    EffectiveTo =  x.EffectiveTo,
                    FundingPeriods = MapFundingPeriods(x.FundingPeriods)
                }).ToList()
            };
        }

        public  ProvidersView MapFrom(Apprenticeships.Api.Types.Providers.Provider provider)
        {
            return new ProvidersView
            {
                CreatedDate = DateTime.UtcNow,
                Provider = new EmployerCommitments.Domain.Models.ApprenticeshipProvider.Provider
                {
                    Ukprn = provider.Ukprn,
                    ProviderName = provider.ProviderName,
                    Email = provider.Email,
                    Phone = provider.Phone,
                    NationalProvider = provider.NationalProvider
                }
            };
        }

        public StandardsView MapFrom(List<StandardSummary> standards)
        {
            return new StandardsView
            {
                CreationDate = DateTime.UtcNow,
                Standards = standards.Select(x => new Standard
                {
                    Id = x.Id,
                    Code = long.Parse(x.Id),
                    Level = x.Level,
                    Title = GetTitle(x.Title, x.Level) + " (Standard)",
                    CourseName = x.Title,
                    Duration = x.Duration,
                    MaxFunding = x.CurrentFundingCap,
                    EffectiveFrom = x.EffectiveFrom,
                    EffectiveTo = x.LastDateForNewStarts,
                    FundingPeriods = MapFundingPeriods(x.FundingPeriods)
                }).ToList()
            };
        }

        private static IEnumerable<FundingPeriod> MapFundingPeriods(IEnumerable<Apprenticeships.Api.Types.FundingPeriod> source)
        {
            if (source == null)
                return Enumerable.Empty<FundingPeriod>();

            return source.Select(x => new FundingPeriod
            {
                EffectiveFrom = x.EffectiveFrom,
                EffectiveTo = x.EffectiveTo,
                FundingCap = x.FundingCap
            }).OrderBy(y => y.EffectiveFrom ?? DateTime.MinValue);
        }

        private static string GetTitle(string title, int level)
        {
            return $"{title}, Level: {level}";
        }
    }
}
