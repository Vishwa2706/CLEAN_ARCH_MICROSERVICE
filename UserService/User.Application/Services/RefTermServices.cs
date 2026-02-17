using Shared.Reddis.Contract;
using User.Application.Contracts;

namespace User.Application.Services;

public class RefTermServices
{
    private readonly IRedisCacheService _cache;
    private readonly IRefTermRepository _repo;

    public RefTermServices(IRedisCacheService cache, IRefTermRepository repo)
    {
        _cache = cache;
        _repo = repo;
    }

    public async Task<List<string>> GetRefTermsAsync(string termType)
    {
        string cacheKey = $"REFTERM_{termType}";

        var cached = await _cache.GetAsync<List<string>>(cacheKey);

        if (cached != null && cached.Any())
        {
            Console.WriteLine($"🔥 CACHE HIT → {cacheKey}");
            return cached;
        }

        Console.WriteLine($"💾 DB HIT → {cacheKey}");

        var dbData = await _repo.GetTermsAsync(termType);

        if (dbData.Any())
        {
            Console.WriteLine($"📦 Saving to CACHE → {cacheKey}");

            await _cache.SetAsync(cacheKey, dbData, TimeSpan.FromMinutes(30));
        }

        return dbData;
    }
}
