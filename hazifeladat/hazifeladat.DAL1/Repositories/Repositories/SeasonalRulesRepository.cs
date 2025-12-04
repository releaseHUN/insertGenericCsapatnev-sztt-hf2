using hazifeladat.DAL1.Models;
using hazifeladat.DAL1.Repositories.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace hazifeladat.DAL1.Repositories.Repositories
{
    public class SeasonalRulesRepository : ISeasonRulesRepository
    {
        private readonly string _filePath;
        private List<SeasonalRules> _rules = new();

        public async Task<bool> LoadAsync()
        {
            try
            {
                if (!File.Exists(_filePath))
                {
                    _rules = new List<SeasonalRules>();
                    return true;
                }
                using (var stream = new FileStream(
                    _filePath,
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.Read,
                    4096,
                    useAsync: true))

                using (var reader = new StreamReader(stream))
                {
                    string json = await reader.ReadToEndAsync();
                    _rules = JsonConvert.DeserializeObject<List<SeasonalRules>>(json) ?? new List<SeasonalRules>();
                }


                
                return true;
            }
            catch
            {
                _rules = new List<SeasonalRules>();
                return false;
            }
        }

        public async Task<bool> SaveAsync()
        {
            try
            {
                var directory = Path.GetDirectoryName(_filePath);
                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);

                using (var stream = new FileStream(
                    _filePath,
                    FileMode.Create,
                    FileAccess.Write,
                    FileShare.None,
                    4096,
                    useAsync: true))
                using (var writer = new StreamWriter(stream))
                {
                    string json = JsonConvert.SerializeObject(_rules, Formatting.Indented);
                    await writer.WriteAsync(json);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public Task<IReadOnlyList<SeasonalRules>> GetAllAsync()
            => Task.FromResult((IReadOnlyList<SeasonalRules>)_rules.ToList());

        public async Task AddOrUpdateAsync(SeasonalRules rule)
        {
            var existing = _rules.SingleOrDefault(r => r.Id == rule.Id);
            if (existing == null)
            {
                // új ID generálása
                rule.Id = _rules.Any() ? _rules.Max(r => r.Id) + 1 : 1;
                _rules.Add(rule);
            }
            else
            {
                int index = _rules.IndexOf(existing);
                _rules[index] = rule;
            }

            await SaveAsync();
        }

        public async Task DeleteAsync(int ruleId)
        {
            var rule = _rules.SingleOrDefault(r => r.Id == ruleId);
            if (rule != null)
            {
                _rules.Remove(rule);
                await SaveAsync();
            }

        }
    }
}
