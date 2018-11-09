using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;

namespace SFA.DAS.EmployerCommitments.Application.Extensions
{
    public static class PriceEpisodeExtensions
    {
        public static decimal GetCostOn(this List<PriceHistory> priceEpisodes, DateTime effectiveDate)
        {
            if (priceEpisodes.Count == 0)
            {
                return 0;
            }

            var effectiveEpisode = priceEpisodes.OrderBy(x => x.FromDate).FirstOrDefault(x =>
                effectiveDate >= x.FromDate && (effectiveDate <= x.ToDate || x.ToDate == null));

            if (effectiveEpisode != null)
            {
                return effectiveEpisode.Cost;
            }

            var firstEpisode = priceEpisodes.OrderBy(x => x.FromDate).FirstOrDefault(x => x.FromDate > effectiveDate);
            if(firstEpisode != null)
            {
                return firstEpisode.Cost;
            }

            var lastEpisode = priceEpisodes.OrderByDescending(x => x.FromDate)
                .FirstOrDefault(x => x.ToDate < effectiveDate || x.ToDate == null);
            if (lastEpisode != null)
            {
                return lastEpisode.Cost;
            }

            return 0;
        }
    }
}
