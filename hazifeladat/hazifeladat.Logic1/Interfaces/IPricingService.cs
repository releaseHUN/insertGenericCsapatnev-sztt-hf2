using hazifeladat.DAL1.Models;
using hazifeladat.DAL1.Models.Enums;
using hazifeladat.Logic1.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace hazifeladat.Logic1.Interfaces
{
    public interface IPricingService
    {
        Task InitializeAsync();

        Task<PriceQuote> CalculatePriceAsync(
            int placeId,
            DateTime arrival,
            DateTime departure);

        Task<PriceQuote> CalculatePriceForPlaceTypeAsync(
            PlaceTypes placeType,
            DateTime arrival,
            DateTime departure);

        Task SetBasePricePerNightForPlaceAsync(int placeId, decimal pricePerNight);

        Task AddOrUpdateSeasonRuleAsync(SeasonalRules rule);
        Task<IReadOnlyList<SeasonalRules>> GetSeasonRulesAsync();
    }
}
