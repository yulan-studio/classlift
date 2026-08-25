using Core.Models;
using Core.R2;
using System.Collections.Concurrent;
using System.Text.Json;

namespace Core.Services
{
    public sealed class OrganizationTerminologyService
    {
        private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);
        private readonly R2StorageService _storageService;
        private readonly ConcurrentDictionary<string, CacheEntry> _cache = new();

        public OrganizationTerminologyService(R2StorageService storageService)
        {
            _storageService = storageService;
        }

        public async Task<OrganizationTerminology> GetAsync(string databaseName)
        {
            if (_cache.TryGetValue(databaseName, out var cached)
                && cached.ExpiresAt > DateTimeOffset.UtcNow)
            {
                return cached.Value;
            }

            var terminology = new OrganizationTerminology();
            string? json = null;
            try
            {
                json = await _storageService.GetTextAsync(GetObjectKey(databaseName));
            }
            catch
            {
                // Branding storage should never prevent the application from loading.
            }

            if (!string.IsNullOrWhiteSpace(json))
            {
                try
                {
                    terminology = JsonSerializer.Deserialize<OrganizationTerminology>(json)
                        ?? terminology;
                }
                catch (JsonException)
                {
                    // Keep the safe defaults if a stored setting is malformed.
                }
            }

            _cache[databaseName] = new CacheEntry(
                terminology,
                DateTimeOffset.UtcNow.Add(CacheDuration));
            return terminology;
        }

        public async Task SaveAsync(
            string databaseName,
            OrganizationTerminology terminology)
        {
            var json = JsonSerializer.Serialize(terminology);
            await _storageService.UploadTextAsync(GetObjectKey(databaseName), json);
            _cache[databaseName] = new CacheEntry(
                terminology,
                DateTimeOffset.UtcNow.Add(CacheDuration));
        }

        private static string GetObjectKey(string databaseName) =>
            $"branding/{databaseName}/terminology.json";

        private sealed record CacheEntry(
            OrganizationTerminology Value,
            DateTimeOffset ExpiresAt);
    }
}
