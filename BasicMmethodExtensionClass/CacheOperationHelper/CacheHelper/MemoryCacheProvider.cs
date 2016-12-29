﻿using System;
using System.Threading.Tasks;
using Helper.CacheHelper;

namespace BasicMmethodExtensionClass.CacheOperationHelper.CacheHelper
{
   public class MemoryCacheProvider : ICacheProvider
    {
        private ObjectCache Cache => MemoryCache.Default;

        private readonly object _lock = new object();

        /// <summary>
        /// Gets the item from the cache, if the item is not present 
        /// then we will get that item and store it in the cache.
        /// </summary>
        /// <typeparam name="T">Type to store in the cache.</typeparam>
        /// <param name="key">The key.</param>
        /// <param name="itemCallback">The item callback. This will be called if the item is not present in the cache.
        /// </param>
        /// <param name="cacheTime">The amount of time we want to cache the object.</param>
        /// <returns>A copy of the cached object.</returns>
        /// <remarks>If the <c><![CDATA[Func<T>]]> itemCallback</c> is null and the item is not in the cache it will throw a <see cref="NullReferenceException"/>.
        /// <para>If you do not want to change the object in the cache (since it's a copy returned and not a reference) you will need to <see cref="Remove"/>
        /// the cached item and then <see cref="Set"/> it, or just call this method.</para></remarks>
        public T GetOrSet<T>(string key, Func<T> itemCallback, int cacheTime = 20) where T : class
        {
            var item = Get<T>(key);
            if (item == null)
            {
                item = itemCallback();
                if (item != null)
                {
                    Set(key, item, cacheTime);
                }
            }

            // Return a copy, not the stored cache reference
            // The cached object will not change
            // If we 
            return item.CloneJson();
        }

        public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> itemCallback, int cacheTime = 20) where T : class
        {
            var item = Get<T>(key);
            if (item == null)
            {
                item = await itemCallback();
                if (item != null)
                {
                    Set(key, item, cacheTime);
                }
            }

            // Return a copy, not the stored cache reference
            // The cached object will not change
            return item.CloneJson();
        }

        /// <summary>
        /// Gets the specified item from the cache.
        /// </summary>
        /// <typeparam name="T">Type to get from the cache</typeparam>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public T Get<T>(string key) where T : class
        {
            lock (_lock)
            {
                return Cache.Get(key) as T;
            }
        }

        /// <summary>
        /// Set/Store the specified object in the cache
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="data">The object we want to store.</param>
        /// <param name="cacheTime">The amount of time we want to cache the object.</param>
        public void Set(string key, object data, int cacheTime = 20)
        {
            var policy = new CacheItemPolicy { AbsoluteExpiration = DateTime.Now + TimeSpan.FromMinutes(cacheTime) };
            lock (_lock)
            {
                Cache.Remove(key);
                Cache.Add(new CacheItem(key, data), policy);
            }
        }

        /// <summary>
        /// Removes the specified object from the cache.
        /// </summary>
        /// <param name="key">The key.</param>
        public void Remove(string key)
        {
            var keys = Cache.Where(x => x.Key.Contains(key));
            foreach (var k in keys)
            {
                lock (_lock)
                {
                    Cache.Remove(k.Key);
                }
            }
        }
    }
}
