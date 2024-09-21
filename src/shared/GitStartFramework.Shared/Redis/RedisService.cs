using Newtonsoft.Json;
using StackExchange.Redis;

namespace GitStartFramework.Shared.Redis
{
    public interface IRedisService
    {
        Task<string> GetValueAsync(string key);

        Task SetValueAsync(string key, string value, TimeSpan? expiry = null);

        Task DeleteValueAsync(string key);

        Task<T> GetAsync<T>(string key, Func<Task<T>> func);
    }

    public class RedisService : IRedisService
    {
        private readonly IConnectionMultiplexer _connectionMultiplexer;

        public RedisService(IConnectionMultiplexer connectionMultiplexer)
        {
            _connectionMultiplexer = connectionMultiplexer;
        }

        public async Task<string> GetValueAsync(string key)
        {
            var database = _connectionMultiplexer.GetDatabase();
            return await database.StringGetAsync(key);
        }

        public async Task SetValueAsync(string key, string value, TimeSpan? expiry = null)
        {
            var database = _connectionMultiplexer.GetDatabase();
            await database.StringSetAsync(key, value, expiry);
        }

        public async Task DeleteValueAsync(string key)
        {
            var database = _connectionMultiplexer.GetDatabase();
            await database.KeyDeleteAsync(key);
        }

        public async Task<T> GetAsync<T>(string key, Func<Task<T>> func)
        {
            var database = _connectionMultiplexer.GetDatabase();

            var data = await database.StringGetAsync(key);

            if (string.IsNullOrWhiteSpace(data))
            {
                var result = await func();
                await SetValueAsync(key, JsonConvert.SerializeObject(result));
                return result;
            }
            return JsonConvert.DeserializeObject<T>(data!)!;
        }
    }
}