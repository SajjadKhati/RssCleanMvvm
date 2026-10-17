using Application.InfrastructureContracts.CacheProject.CacheBase.Cache.Interfaces;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;

namespace Infra.CacheService.CacheBaseServices.CacheServices.Class
{
    public class Cache<TKey, TItem> : ICache<TKey, TItem>
        where TKey : notnull
    {

        protected ConcurrentDictionary<TKey, TItem> itemsDictionary;


        ////////////////////////////////////////////////


        public Cache(int collectionCapacity = -1)
        {
            collectionCapacity = (collectionCapacity <= 0) ? 31 : collectionCapacity;
            this.itemsDictionary = new ConcurrentDictionary<TKey, TItem>(-1, collectionCapacity);
        }


        public Cache(IEnumerable<KeyValuePair<TKey, TItem>> collection)
        {
            this.itemsDictionary = new ConcurrentDictionary<TKey, TItem>(collection);
        }


        ////////////////////////////////////////////////


        public int Count()
        {
            return this.itemsDictionary.Count;
        }


        public bool ContainsKey(TKey key)
        {
            return this.itemsDictionary.ContainsKey(key);
        }


        public IReadOnlyCollection<TKey> GetKeys()
        {
            /// itemsDictionary.Keys در همان لحظه، یک snapshot (کپی) از کلیدها می‌گیرد.
            /// پس اگر بین این خط و iterate کردن مصرف‌کننده، نخ دیگری چیزی به دیکشنری اضافه/حذف کند،
            /// این snapshot دست‌نخورده می‌ماند و iterate امن است.
            return this.itemsDictionary.Keys.ToList().AsReadOnly();
        }


        public bool TryGetValue(TKey key, out TItem value)
        {
            return this.itemsDictionary.TryGetValue(key, out value);
        }


        public IReadOnlyList<TItem> GetAllValuesCopy()
        {
            /// itemsDictionary.Values در همان لحظه، یک snapshot (کپی) از مقدارها می‌گیرد.
            /// پس مراحل بعدی (ToList و AsReadOnly) روی همان snapshot کار می‌کنند، نه روی داده‌ی زنده.
            /// تغییرات همزمانِ نخ‌های دیگر روی itemsDictionary، به این snapshot آسیب نمی‌زند.
            /// به همین دلیل، این زنجیره‌ی چند متدی race ندارد — برخلاف ContainsKey + Get که هر دو روی داده‌ی زنده کار می‌کنند.
            return this.itemsDictionary.Values.ToList().AsReadOnly();
        }


        public IReadOnlyDictionary<TKey, TItem> GetAllKeyValues()
        {
            /// دقت کنید که سازنده ی ReadOnlyDictionary ، آیتم های دیکشنری را کپی نمیکنه .
            /// برخلاف سازنده ها ی Dictionary و ConcurrentDictionary که کپی میکنه ، 
            /// یعنی ReadOnlyDictionary ، همون شیِ دیکشنری را اشاره میکنه که itemsDictionary هم هست .
            /// یعنی با تغییر در itemsDictionary ، شی ReadOnlyDictionary هم تغییر میکنه .
            return new ReadOnlyDictionary<TKey, TItem>(this.itemsDictionary);
        }


        public IReadOnlyDictionary<TKey, TItem> GetAllKeyValuesCopy()
        {
            ///  سازنده ها ی Dictionary و ConcurrentDictionary کالکشن را کپی میکنن ، 
            ///  همچنین چون شی Dictionary زیر ، اولا نهایتا در wrapper ئه ReadOnlyDictionary پیچیده میشه و کسی نمیتونه در اون چیزی بنویسه و -
            ///  دوما چون شی Dictionary ، بخاطر قفل داخلی که نداره (نسبت به ConcurrentDictionary که قفل داخلی داره) ، موقع خوندن میتونه توسط چند نخ هم خونده بشه و سریعتر هست .
            return new Dictionary<TKey, TItem>(this.itemsDictionary).AsReadOnly();
        }


        public TItem GetOrAdd(TKey key, TItem value)
        {
            return this.itemsDictionary.GetOrAdd(key, value);
        }


        public TItem GetOrAdd(TKey key, Func<TKey, TItem> addValueFactory)
        {
            return this.itemsDictionary.GetOrAdd(key, addValueFactory);
        }


        public TItem AddOrUpdate(TKey key, TItem value)
        {
            return this.itemsDictionary.AddOrUpdate(key, value, (_, _) => value);
        }


        public TItem AddOrUpdate(TKey key, Func<TKey, TItem> addValueFactory, Func<TKey, TItem, TItem> updateValueFactory)
        {
            return this.itemsDictionary.AddOrUpdate(key, addValueFactory, updateValueFactory);
        }


        public void Set(TKey key, TItem value)
        {
            this.itemsDictionary[key] = value;
        }


        public bool TryRemove(TKey key)
        {
            return this.itemsDictionary.TryRemove(key, out TItem item);
        }


        public bool TryRemove(TKey key, out TItem value)
        {
            return this.itemsDictionary.TryRemove(key, out value);
        }


        public void Clear()
        {
            this.itemsDictionary.Clear();
        }

    }
}
