using hazifeladat.DAL1.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace hazifeladat.DAL1.Repositories.Interfaces
{
    public interface ISeasonRulesRepository
    {

        Task<bool> LoadAsync();
        Task<bool> SaveAsync();

        Task<IReadOnlyList<SeasonalRules>> GetAllAsync();
        Task AddOrUpdateAsync(SeasonalRules rule);
        Task DeleteAsync(int ruleId);
    }
}
