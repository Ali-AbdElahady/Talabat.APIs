using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Talabat.Core.Services;

namespace Talabat.Service
{
	public class ResponseCacheService : IResponseCacheService
	{
		private readonly IDatabase database;

		public ResponseCacheService(IConnectionMultiplexer Redis)
        {
			database = Redis.GetDatabase();
		}
        public async Task CacheResponseAsync(string CacheKey, object Response, TimeSpan ExpireTime)
		{
			if (Response == null) return;
			var Options = new JsonSerializerOptions()
			{
				PropertyNamingPolicy = JsonNamingPolicy.CamelCase
			};
			var SerializeResponse = JsonSerializer.Serialize(Response,Options);
			await database.StringSetAsync(CacheKey, SerializeResponse, ExpireTime);
		}

		public async Task<string?> GetCachedResponse(string CacheKey)
		{
			var CachedResponse =  await database.StringGetAsync(CacheKey);
			if (CachedResponse.IsNullOrEmpty) return null;
			return CachedResponse;
		}
	}
}
