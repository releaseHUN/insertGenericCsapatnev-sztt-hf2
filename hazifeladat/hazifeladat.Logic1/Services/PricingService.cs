using hazifeladat.DAL1.Models;
using hazifeladat.DAL1.Models.Enums;
using hazifeladat.DAL1.Repositories.Interfaces;
using hazifeladat.Logic1.Dto;
using hazifeladat.Logic1.Interfaces;

namespace hazifeladat.Logic1.Services
{
    public class PricingService : IPricingService
    {
        private readonly IPlacesRepository _placesRepository;
        private readonly ISeasonRulesRepository _rulesRepo;

        public PricingService(
            IPlacesRepository placesRepository,
            ISeasonRulesRepository rulesRepo)
        {
            _placesRepository = placesRepository;
            _rulesRepo = rulesRepo;
        }

        public async Task InitializeAsync()
        {
            await _placesRepository.LoadAsync();
            await _rulesRepo.LoadAsync();
        }

        public async Task<PriceQuote> CalculatePriceAsync(
            int placeId,
            DateTime arrival,
            DateTime departure)
        {
            if (arrival.Date >= departure.Date)
                throw new ArgumentException("Érvénytelen időszak.");

            var place = await _placesRepository.GetByIdAsync(placeId)
                        ?? throw new InvalidOperationException("Hely nem található.");

            int nights = (int)(departure.Date - arrival.Date).TotalDays;

            decimal basePrice = place.PricePerNight;

            // összes szezon szabály lekérése
            var rules = await _rulesRepo.GetAllAsync();

            decimal finalPricePerNight = ApplySeasonRules(
                basePrice, place.Type, arrival, departure, rules);

            decimal total = finalPricePerNight * nights;

            return new PriceQuote
            {
                Nights = nights,
                PricePerNight = finalPricePerNight,
                TotalPrice = total,
                Description = $"Alapár: {basePrice}, szezon szorzóval: {finalPricePerNight}"
            };
        }

        public async Task<PriceQuote> CalculatePriceForPlaceTypeAsync(
            PlaceTypes placeType,
            DateTime arrival,
            DateTime departure)
        {
            var places = await _placesRepository.GetAllAsync();
            var candidate = places.FirstOrDefault(p => p.Type == placeType);

            if (candidate == null)
                throw new InvalidOperationException("Nincs ilyen típusú hely.");

            return await CalculatePriceAsync(candidate.Id, arrival, departure);
        }

        public async Task SetBasePricePerNightForPlaceAsync(int placeId, decimal pricePerNight)
        {
            var place = await _placesRepository.GetByIdAsync(placeId)
                ?? throw new InvalidOperationException("Hely nem található.");

            place.PricePerNight = pricePerNight;

            await _placesRepository.UpdateAsync(place);
            await _placesRepository.SaveAsync();
        }

        public async Task AddOrUpdateSeasonRuleAsync(SeasonalRules rule)
        {
            await _rulesRepo.AddOrUpdateAsync(rule);
            await _rulesRepo.SaveAsync();
        }

        public async Task<IReadOnlyList<SeasonalRules>> GetSeasonRulesAsync()
            => await _rulesRepo.GetAllAsync();

        private decimal ApplySeasonRules(
            decimal basePricePerNight,
            PlaceTypes placeType,
            DateTime arrival,
            DateTime departure,
            IReadOnlyList<SeasonalRules> rules)
        {
            var applicableRules = rules
                .Where(r =>
                    (r.AppliesToType == null || r.AppliesToType.Contains(placeType)) &&
                    arrival < r.To &&
                    departure > r.From)
                .ToList();

            decimal multiplier = 1m;

            foreach (var rule in applicableRules)
            {
                multiplier *= rule.Multiplier;
            }

            return basePricePerNight * multiplier;
        }
    }
}
