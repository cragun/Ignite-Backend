using DataReef.Core.Attributes;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.Core.Infrastructure.Caching
{
    [Service(typeof(ICacheService))]
    public class RedisCacheService : ICacheService
    {
        private static bool _useCache = Convert.ToBoolean(ConfigurationManager.AppSettings["Cache_Active"]);

        private static string _cacheConnection = ConfigurationManager.AppSettings["Cache_Connection"];

        private static int defaultCacheTime = Convert.ToInt32(ConfigurationManager.AppSettings["Cache_DefaultCacheTime"]);

        private RedisManagerPool _pool;
        private IRedisClient Cache
        {
            get
            {
                if (_pool == null)
                {
                    _pool = new RedisManagerPool(_cacheConnection);
                }
                return _pool.GetClient();
            }
        }

        public T Get<T>(string key)
        {
            return _useCache ? Cache.Get<T>(GetCacheKey(key)) : default(T);
        }

        public T Get<T>(string key, Func<T> getItemCallback)
        {
            return Get<T>(GetCacheKey(key), defaultCacheTime, getItemCallback);
        }

        public T Get<T>(string key, int cacheTime, Func<T> getItemCallback)
        {
            if (!_useCache)
            {
                return getItemCallback();
            }

            var cacheKey = GetCacheKey(key);

            if (Cache.ContainsKey(cacheKey))
            {
                return Cache.Get<T>(cacheKey);
            }

            T item = getItemCallback();
            if (item != null)
                Set(cacheKey, item, cacheTime);

            return item;
        }

        public void Set<T>(string key, T data)
        {
            Set(GetCacheKey(key), data, defaultCacheTime);
        }

        public void Set<T>(string key, T data, int cacheTime)
        {
            if (!_useCache)
                return;

            Cache.Add(GetCacheKey(key), data, DateTime.Now + TimeSpan.FromMinutes(cacheTime));
        }

        public bool IsSet(string key)
        {
            return _useCache ? Cache.ContainsKey(GetCacheKey(key)) : false;
        }

        public void Remove(string key)
        {
            if (!_useCache)
                return;
            Cache.Remove(GetCacheKey(key));
        }

        public void Clear()
        {
            if (!_useCache)
                return;
            Cache.FlushAll();
        }

        /// <summary>
        /// Created this method in case we want to use the same cache server for multiple apps / environments. 
        /// We could prefix every key w/ the app + environment name.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private string GetCacheKey(string key)
        {
            return key;
        }
    }
}
