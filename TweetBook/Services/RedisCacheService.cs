using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Net;

namespace TweetBook.Services
{
    public class RedisCacheService : ICacheService
    {
        private IDatabase _db;
        private IServer _server;
        public RedisCacheService(IConfiguration configuration)
        {
            var ip= configuration.GetValue<string>("Redis:Server");
            var port=Int32.Parse(configuration.GetValue<string>("Redis:Port"));
            var connectionString = $"{ip}";
            var redis = ConnectionMultiplexer.Connect(connectionString);
            _db = redis.GetDatabase();
            _server = redis.GetServer(IPAddress.Parse(ip),port);
        }   
        

        public void RemoveByPrefix(string prefix)
        {
            throw new NotImplementedException();
        }

        public bool Set<T>(string cacheKey, T value)
        {
            try
            {
               var jsonValue = JsonConvert.SerializeObject(value);
               return _db.StringSet(cacheKey, jsonValue);
            }
            catch
            {
                return false;
            }
        }

        public bool Set<T>(string cacheKey, T value, DateTimeOffset expire)
        {
            try
            {
                var jsonValue = JsonConvert.SerializeObject(value);
                return _db.StringSet(cacheKey, jsonValue,TimeSpan.FromTicks(expire.Ticks));
            }
            catch 
            {
                return false;
            }
        }

        public bool TryGet<T>(string cacheKey, out T value)
        {
            try
            {
                var valueJson = _db.StringGet(cacheKey);
                if (valueJson.HasValue)
                {
                    value = JsonConvert.DeserializeObject<T>(valueJson);
                    return true;
                }
                value = default(T);
                return false;
            }
            catch 
            {
                value = default(T);
                return false;
            }
        }

        public void Clear()
        {
            try
            {
                _server.FlushAllDatabases();
            }
            catch
            {

            }
        }
    }
}
