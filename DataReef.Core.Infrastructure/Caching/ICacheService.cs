using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.Core.Infrastructure.Caching
{
    public interface ICacheService
    {
        /// <summary>
        /// Gets or sets the value associated with the specified key.
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="key">The key of the value to get.</param>
        /// <returns>The value associated with the specified key.</returns>
        T Get<T>(string key);

        /// <summary>
        /// Gets or sets the value associated with the specified key.
        /// If the cache does not exist the callback functuion is used to get actual data and add it to Cahce
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="key">The key of the value to get.</param>
        /// <param name="getItemCallback">Method that gets called if the item is not cached</param>
        /// <returns></returns>
        T Get<T>(string key, Func<T> getItemCallback);

        /// <summary>
        /// Gets or sets the value associated with the specified key.
        /// If the cache does not exist, the callback functuion is used to get actual data and add it to Cahce with cache duration.
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="key">The key of the value to get.</param>
        /// <param name="cacheTime">Time to cache the value in minutes.</param>
        /// <param name="getItemCallback">Method that gets called if the item is not cached</param>
        /// <returns></returns>
        T Get<T>(string key, int cacheTime, Func<T> getItemCallback);

        /// <summary>
        /// Adds the specified key and object to the state manager.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">Key</param>
        /// <param name="data">Value</param>
        void Set<T>(string key, T data);

        /// <summary>
        /// Adds the specified key and object to the state manager.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">key</param>
        /// <param name="data">value</param>
        /// <param name="cacheTime">Time to cache the value in minutes.</param>
        void Set<T>(string key, T data, int cacheTime);

        /// <summary>
        /// Gets a value indicating whether the value associated with the specified key is in the state manager.
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>Result</returns>
        bool IsSet(string key);

        /// <summary>
        /// Removes the value with the specified key from the state manager.
        /// </summary>
        /// <param name="key">/key</param>
        void Remove(string key);

        /// <summary>
        /// Clear all state manager data
        /// </summary>
        void Clear();

    }
}
