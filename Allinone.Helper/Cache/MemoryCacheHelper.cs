using Allinone.Domain.Enums;
using Microsoft.Extensions.Caching.Memory;
using System.Collections;
using System.Collections.Generic;

namespace Allinone.Helper.Cache
{
    public class MemoryCacheHelper
    {
        public static Dictionary<int, string> CacheTodolistType { get; set; }
        public static Dictionary<int, string> CacheDSTranType { get; set; }

        public static List<EnumModel> CacheTodolistTypeList { get; set; }
        public static List<EnumModel> CacheDSTranTypeList { get; set; }
        public static List<EnumModel> CacheKanbanTypeList { get; set; }
        public static List<EnumModel> CacheKanbanStatusList { get; set; }


        private readonly IMemoryCache _cache;

        public MemoryCacheHelper(IMemoryCache cache)
        {
            _cache = cache;
            PresetValue();
        }

        // Async: Get or create cache value
        public async Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> getItemCallback, TimeSpan? expiration = null)
        {
            if (_cache.TryGetValue(key, out T value))
            {
                return value;
            }

            value = await getItemCallback();

            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(5)
            };

            _cache.Set(key, value, options);
            return value;
        }

        //self create
        public async Task SetAsync<T>(string key, Func<Task<T>> getItemCallback, TimeSpan? expiration = null)
        {
            var value = await getItemCallback();

            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(5)
            };

            _cache.Set(key, value, options);
        }

        // Sync Set
        public void Set<T>(string key, T value, TimeSpan? expiration = null)
        {
            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(5)
            };
            _cache.Set(key, value, options);
        }

        // Sync TryGet
        public bool TryGetValue<T>(string key, out T value)
        {
            return _cache.TryGetValue(key, out value);
        }

        // Remove key
        public void Remove(string key)
        {
            _cache.Remove(key);
        }

        private void PresetValue2()
        {
            var cacheTodolistType = new List<EnumModel>();
            var cacheDSTranType = new List<EnumModel>();

            foreach (var item in Enum.GetValues<EnumTodolistType>())
            {
                cacheTodolistType.Add(new EnumModel
                {
                    ID = (int)item,
                    Name = item.ToString()
                });
            }


            foreach (var item in Enum.GetValues<EnumDSTranType>())
            {
                cacheDSTranType.Add(new EnumModel
                {
                    ID = (int)item,
                    Name = item.ToString()
                });
            }

            _cache.Set(CacheKeys.TodolistTypeKey, cacheTodolistType);
            _cache.Set(CacheKeys.DSTranTypeKey, cacheDSTranType);
        }

        private void PresetValue()
        {
            CacheTodolistType = Enum.GetValues<EnumTodolistType>().ToDictionary(x => (int)x, x => x.ToString());
            CacheDSTranType = Enum.GetValues<EnumDSTranType>().ToDictionary(x => (int)x, x => x.ToString());

            CacheKanbanTypeList =
                [.. Enum.GetValues<EnumKanbanType>().Select(kvp => new EnumModel { ID = (int)kvp, Name = kvp.ToString() })];

            CacheKanbanStatusList =
                [.. Enum.GetValues<EnumKanbanStatus>().Select(kvp => new EnumModel { ID = (int)kvp, Name = kvp.ToString() })];

            CacheTodolistTypeList =
                [.. CacheTodolistType.Select(kvp => new EnumModel { ID = kvp.Key, Name = kvp.Value })];

            CacheDSTranTypeList =
                [.. CacheDSTranType.Select(kvp => new EnumModel { ID = kvp.Key, Name = kvp.Value })];

            //CacheDSTranTypeList =
            //    [.. CacheDSTranType.Select(kvp => (object)new DictionaryEntry { Key = kvp.Key, Value = kvp.Value })];

            _cache.Set(CacheKeys.TodolistTypeKey, CacheTodolistType);
            _cache.Set(CacheKeys.DSTranTypeKey, CacheDSTranType);
        }
    }

    public static class CacheKeys
    {
        public const string TodolistTypeKey = "TodolistType";
        public const string DSTranTypeKey = "DSTranType";
    }
}
