using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TweetBook.Services
{
    public interface ICacheService
    {
        bool TryGet<T>(string cacheKey, out T value);
        bool Set<T>(string cacheKey, T value);
        bool Set<T>(string cacheKey, T value, DateTimeOffset expire);
        void Clear();
        void RemoveByPrefix(string prefix);
    }
}
