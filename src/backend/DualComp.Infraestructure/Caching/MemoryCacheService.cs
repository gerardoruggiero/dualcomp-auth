using Microsoft.Extensions.Caching.Memory;

namespace DualComp.Infraestructure.Caching
{
	public sealed class MemoryCacheService : ICacheService
	{
		private readonly IMemoryCache _cache;
		public MemoryCacheService(IMemoryCache cache) => _cache = cache;

		public Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
		{
			_cache.TryGetValue(key, out T? value);
			return Task.FromResult(value);
		}

		public Task SetAsync<T>(string key, T value, TimeSpan? expiry = null, CancellationToken cancellationToken = default)
		{
			var options = new MemoryCacheEntryOptions();
			if (expiry.HasValue) options.SetAbsoluteExpiration(expiry.Value);
			_cache.Set(key, value, options);
			return Task.CompletedTask;
		}

		public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
		{
			_cache.Remove(key);
			return Task.CompletedTask;
		}
	}
}


