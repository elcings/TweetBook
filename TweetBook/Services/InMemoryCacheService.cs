
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Threading.Tasks;

namespace TweetBook.Services
{
    public class InMemoryCacheService : ICacheService
    {
        private DateTimeOffset Expiration => DateTime.Now.AddMonths(1);
        private static readonly object Sync = new object();

        private static MemoryCache Cache => MemoryCache.Default;

      
        public bool TryGet<T>(string cacheKey, out T value)
        {
            lock (Sync)
            {
                value = default;
                if (!Cache.Contains(cacheKey))
                {
                    return false;
                }
                if (!(Cache.Get(cacheKey) is T item)) return false;
                value = item;
                return true;
            }
        }

        public bool Set<T>(string cacheKey, T value)
        {
            Set(cacheKey, value, Expiration);
            return true;
        }

        public bool Set<T>(string cacheKey, T value, DateTimeOffset expire)
        {
            lock (Sync)
            {
                if (Cache.Contains(cacheKey))
                {
                    Cache.Remove(cacheKey);
                }
                Cache.Add(cacheKey, value, expire);
                return true;
            }
        }

        public void Clear()
        {
            lock (Sync)
            {
                var cacheItems = Cache.ToList();

                foreach (var a in cacheItems)
                {
                    Cache.Remove(a.Key);
                }
            }
        }

        //public void RemoveByPrefix(string prefix)
        //{
        //    lock (Sync)
        //    {
        //        var keysToRemove =
        //            Cache
        //                .Where(c => c.Key.StartsWith(prefix, StringComparison.InvariantCultureIgnoreCase))
        //                .Select(c => c.Key)
        //                .ToList();

        //        foreach (var key in keysToRemove)
        //        {
        //            Cache.Remove(key);
        //        }
        //    }
        //}
    }
}
