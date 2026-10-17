using Application.InfrastructureContracts.CacheProject.AppDraftCacheManager.DraftCacheWriter.Interface;
using Application.InfrastructureContracts.CacheProject.AppDraftCacheManager.RssItemDraftCacheManager.Interface;
using Application.InfrastructureContracts.CacheProject.CacheBase.DraftCache.DataModels;
using Application.InfrastructureContracts.CacheProject.CacheBase.DraftCache.Interfaces;
using Application.InfrastructureContracts.SharedProject.AppFolderPath.Interface;
using Application.InfrastructureContracts.SharedProject.FileManager.Interface;
using Application.InfrastructureContracts.SharedProject.FolderManager.Interface;
using Application.OverallDataModels.OverallDto.CompositeDto;
using Application.OverallDataModels.OverallDto.EntityDto;
using Domain.DomainEntities.RssEntity.Class;
using Infra.CacheService.CacheBaseServices.DraftCacheServices.Class;

namespace Infra.CacheService.AppDraftCacheManagerService.RssItemDraftCacheManagerService.Class
{
    /// <summary>
    /// مدیریت‌کننده‌ی کش‌های موقت.
    /// 
    /// <para>
    /// <b>سیاست Thread-Safety:</b>
    /// تمام متدهای public این رابط، thread-safe و اتمیک هستند.
    /// متدهایی که چند کش را با هم تغییر می‌دهند یا می‌خوانند، با
    /// ReaderWriterLockSlim داخلی محافظت می‌شوند — پس مصرف‌کننده نیازی
    /// به قفل بیرونی ندارد.
    /// </para>
    /// </summary>
    public class RssItemDraftCacheManager : IRssItemDraftCacheReader, IRssItemDraftCacheWriter, IDisposable
    {

        #region فیلدها

        private IDraftCache<Uri, RssItem> rssItemDraftCache;

        private IDraftCache<Uri, RssItemDto> rssItemDtoDraftCache;

        private IDraftCacheWriter draftCacheWriter;

        private IFileManager fileManager;

        private IFolderManager folderManager;

        private ITempImageFolderPath tempImagesFolderPath;

        private bool isDisposed;

        /// <summary>
        /// قفل محافظِ <b>عملیات مرکب</b> روی دو کشِ موقت (rssItemDraftCache و rssItemDtoDraftCache).
        ///
        /// <para>
        /// <b>عملیات مرکب یعنی چه؟</b>
        /// عملیاتی که از چند مرحله‌ی جدا تشکیل شده و باید «همه یا هیچ» اجرا شود.
        /// مثال: «چک کن آیتم هست، بخونش، حذفش کن» — اگر بین این مراحل، نخِ دیگری
        /// دخالت کند، نتیجه خراب می‌شود. چون هر مرحله به‌تنهایی اتمیک است (توسط
        /// ConcurrentDictionary)، اما ترکیب آن‌ها اتمیک نیست، به این قفل نیاز داریم.
        ///
        /// <b>عملیات غیرمرکب (تک‌مرحله‌ای):</b>
        /// مثل ContainKey، GetRssItemDraftEntry، GetAllRssItemDraftEntriesCopy — این‌ها
        /// فقط یک عملیات روی یک کش انجام می‌دهند و ConcurrentDictionary خودش
        /// اتمیک بودنشان را تضمین می‌کند. پس قفل لازم ندارند.
        /// </para>
        ///
        /// <para>
        /// <b>چرا ReaderWriterLockSlim؟</b>
        /// چون بعضی متدها فقط می‌خوانند (GetRssItemDtosByFeedId) و بعضی می‌نویسند
        /// (MarkAs*، Remove، Clear). با این قفل، چند نخ می‌توانند همزمان بخوانند،
        /// در حالی که نوشتن انحصاری می‌ماند. (نکته: صرفاً برای رعایت انصاف طراحی؛
        /// برای بار فعلی، lock ساده هم کافی بود.)
        /// </para>
        ///
        /// <para>
        /// <b>چرا قفل سراسری، نه قفل کلیددار (Keyed Lock)؟</b>
        /// از نظر تئوری، قفل کلیددار (به ازای هر Uri یک قفل) بهتر بود — چون به چند
        /// عملیات همزمان روی Uriهای متفاوت اجازه‌ی موازی می‌دهد. ولی در این پروژه
        /// انتخاب نشد، به این دلایل:
        /// ۱. پیچیدگی حافظه و مدیریت چرخه‌ی حیات قفل‌ها (Dispose، پاک‌سازی قفل‌های
        ///    بی‌استفاده، هماهنگی Release و Remove).
        /// ۲. ترافیک پایین: تعداد عملیات همزمان روی Uriهای متفاوت کم است، پس
        ///    گلوگاه قفل سراسری محسوس نیست.
        /// ۳. سادگی و قابلیت نگهداری: ReaderWriterLockSlim یک قفل استاندارد و 
        ///    اثبات‌شده است. اگر روزی پروفایلینگ نشان داد که این قفل گلوگاه است،
        ///    می‌توان به قفل کلیددار مهاجرت کرد.
        /// </para>
        ///
        /// <para>
        /// <b>طراحی اصولی برای قفل:</b> قفل را فقط در متدهای public قرار بدهید.
        /// در متدهای private، قفل نگذارید — مخصوصاً در حالت 
        /// LockRecursionPolicy.NoRecursion یا پیش‌فرض.
        /// </para>
        ///
        /// <para>
        /// <b>هشدار:</b> try/finally در تمام متدهای قفل‌دار الزامی است تا در صورت
        /// وقوع استثنا، قفل آزاد شود و برنامه دچار بن‌بست (deadlock) نشود.
        /// </para>
        /// </summary>
        private ReaderWriterLockSlim readerWriterLockSlim;

        #endregion


        ////////////////////////////////////////////////////////////////


        #region متد سازنده

        public RssItemDraftCacheManager(IDraftCacheWriter draftCacheWriter, IFileManager fileManager, 
            IFolderManager folderManager, ITempImageFolderPath tempImagesFolderPath)
        {
            this.rssItemDraftCache = new DraftCache<Uri, RssItem>();
            this.rssItemDtoDraftCache = new DraftCache<Uri, RssItemDto>();
            this.draftCacheWriter = draftCacheWriter;
            this.fileManager = fileManager;
            this.folderManager = folderManager;
            this.tempImagesFolderPath = tempImagesFolderPath;
            this.isDisposed = false;
            this.readerWriterLockSlim = new ReaderWriterLockSlim();
        }

        #endregion


        ////////////////////////////////////////////////////////////////


        public void MarkAsAddedInDraftCache(Uri key, RssItemWithDto rssItemWithDto)
        {
            this.readerWriterLockSlim.EnterWriteLock();
            try { this.AddOrUpdateToDraftCaches(key, rssItemWithDto, DraftState.Added); }
            finally { this.readerWriterLockSlim.ExitWriteLock(); }
        }


        public void MarkAsModifiedInDraftCache(Uri key, RssItemWithDto rssItemWithDto)
        {
            this.readerWriterLockSlim.EnterWriteLock();
            try { this.AddOrUpdateToDraftCaches(key, rssItemWithDto, DraftState.Modified); }
            finally { this.readerWriterLockSlim.ExitWriteLock(); }
        }


        public void MarkAsDeletedInDraftCache(Uri key, RssItemWithDto rssItemWithDto)
        {
            this.readerWriterLockSlim.EnterWriteLock();
            try { this.AddOrUpdateToDraftCaches(key, rssItemWithDto, DraftState.Deleted); }
            finally { this.readerWriterLockSlim.ExitWriteLock(); }
        }


        public bool TryMarkAsAddedIfDraftEntryUnchanged(Uri key, RssItemWithDto rssItemWithDto, DraftEntry<RssItem> originalDraftEntry)
        {
            this.readerWriterLockSlim.EnterWriteLock();
            try { return this.TryMarkDraftEntryIfUnchangedCore(key, rssItemWithDto, DraftState.Added, originalDraftEntry); }
            finally { this.readerWriterLockSlim.ExitWriteLock(); }
        }


        public bool TryMarkAsModifiedIfDraftEntryUnchanged(Uri key, RssItemWithDto rssItemWithDto, DraftEntry<RssItem> originalDraftEntry)
        {
            this.readerWriterLockSlim.EnterWriteLock();
            try { return this.TryMarkDraftEntryIfUnchangedCore(key, rssItemWithDto, DraftState.Modified, originalDraftEntry); }
            finally { this.readerWriterLockSlim.ExitWriteLock(); }
        }


        public bool TryMarkAsDeletedIfDraftEntryUnchanged(Uri key, RssItemWithDto rssItemWithDto, DraftEntry<RssItem> originalDraftEntry)
        {
            this.readerWriterLockSlim.EnterWriteLock();
            try { return this.TryMarkDraftEntryIfUnchangedCore(key, rssItemWithDto, DraftState.Deleted, originalDraftEntry); }
            finally { this.readerWriterLockSlim.ExitWriteLock(); }
        }


        public bool TryRemove(Uri key)
        {
            /// ⚠️ چرا قفل نوشتن لازم است؟
            /// این متد چند مرحله دارد: (۱) چک وجود آیتم، (۲) حذف فایل تصویر، (۳) حذف از دو کش.
            /// اگر بین مراحل 1 و 3 که به کش مراجعه میشود، نخ دیگری در مرحله ی دوم وارد شود (مثلاً در مرحله ی دوم ، آیتم جدیدی اضافه کند)،
            /// نتیجه ناسازگار می‌شود — چون آیتم در یک کش حذف شده ولی در کش دیگر نه، یا فایل تصویر حذف شده ولی آیتم در کش مانده.
            /// 
            /// قفل نوشتن (نه خواندن) لازم است چون این متد حالت را تغییر می‌دهد و باید در طول کل عملیات، انحصاری باشد.
            this.readerWriterLockSlim.EnterWriteLock();
            try { return this.TryRemoveCore(key); }
            finally { this.readerWriterLockSlim.ExitWriteLock(); }
        }


        public bool TryRemoveIfEntryUnchanged(Uri key, DraftEntry<RssItem> originalDraftEntry)
        {
            this.readerWriterLockSlim.EnterWriteLock();
            try { return this.TryRemoveIfEntryUnchangedCore(key, originalDraftEntry); }
            finally { this.readerWriterLockSlim.ExitWriteLock(); }
        }


        public void Clear()
        {
            /// ⚠️ قفل نوشتن لازم است و کل متد را می‌پوشاند، چون:
            /// دو مرحله دارد (حذف فایل‌ها، خالی کردن کش‌ها) که باید با هم اتمی باشند.
            /// اگر بین این دو مرحله، نخ دیگری آیتم جدیدی اضافه کند، نتیجه ناسازگار می‌شود  (آیتم جدید در کش حذف شده، ولی فایل تصویرش روی دیسک می‌ماند).
            this.readerWriterLockSlim.EnterWriteLock();
            try { this.ClearCore(); }
            finally { this.readerWriterLockSlim.ExitWriteLock(); }
        }


        public bool ContainKey(Uri key)
        {
            return this.rssItemDraftCache.ContainsKey(key);
        }


        public IReadOnlyCollection<Uri> GetAllDraftCacheKeys()
        {
            return this.rssItemDraftCache.GetKeys();
        }


        public bool TryGetRssItemDraftEntry(Uri key, out DraftEntry<RssItem> rssItemDraftEntry)
        {
            /// این متدهای خواندن ، خودشان اتمی هستند . یعنی کلاس کش که از ConcurrentDictionary  استفاده میکنه ، متدهای خواندن اش اتمی هستند ، یعنی قفل دارند -
            /// پس اینجا در متدهای خواندنی که یکبار فقط میخوانیم (چندین بار در یک متد نمیخوانیم) ، نیاز به قفل کردن ، ندارند .
            return this.rssItemDraftCache.TryGetValue(key, out rssItemDraftEntry);
        }


        public IReadOnlyList<DraftEntry<RssItem>> GetAllRssItemDraftEntriesCopy()
        {
            return this.rssItemDraftCache.GetAllValuesCopy();
        }


        public bool TryGetRssItemDto(Uri key, out RssItemDto rssItemDto)
        {
            return this.rssItemDtoDraftCache.TryGetItem(key, out rssItemDto);
        }


        public IReadOnlyList<RssItemDto> GetRssItemDtosByFeedIdCopy(Uri rssFeedId)
        {
            /// ⚠️ چرا قفل خواندن لازم است؟
            /// این متد دو خواندن از دو کش جداگانه انجام می‌دهد:
            ///   ۱. ابتدا کلیدهای مربوط به این feedId را از rssItemDraftCache می‌خواند
            ///   ۲. سپس DTOهای متناظر را از rssItemDtoDraftCache می‌خواند
            /// 
            /// اگر بین این دو خواندن، نخ نویسنده‌ای وارد شود (مثلاً یک آیتم را حذف کند)،
            /// نتیجه ناسازگار می‌شود — کلید از کش اول داریم، ولی DTO‌اش در کش دوم نیست.
            /// 
            /// قفل خواندن (نه نوشتن): چون متد فقط می‌خواند و می‌تواند چند خواننده‌ی 
            /// همزمان داشته باشد. ولی نویسنده‌ها بلاک می‌شوند تا بین دو خواندن، کسی ننویسد.
            this.readerWriterLockSlim.EnterReadLock();
            try { return this.GetRssItemDtosByFeedIdCopyCore(rssFeedId); }
            finally { this.readerWriterLockSlim.ExitReadLock(); }
        }


        /// <summary>
        /// خواندن اتمی هر دو ورودی (RssItem + RssItemDto) برای یک کلید.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="rssItemWithDto"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public bool TryGetRssItemWithDto(Uri key, out RssItemWithDto rssItemWithDto)
        {
            /// ⚠️ چرا قفل خواندن لازم است؟
            /// این متد دو خواندن از دو کش جداگانه انجام می‌دهد و باید یک جفت سازگار 
            /// برگرداند. اگر بین این دو خواندن، نخ نویسنده‌ای وارد شود (مثلاً 
            /// MarkAsModified)، ممکن است نسخه‌ی قدیمی یکی و نسخه‌ی جدید دیگری را 
            /// بخوانیم — که یک جفت ناسازگار است.
            /// 
            /// قفل خواندن (نه نوشتن): چون متد فقط می‌خواند و می‌تواند چند خواننده‌ی 
            /// همزمان داشته باشد. ولی نویسنده‌ها بلاک می‌شوند تا بین دو خواندن، 
            /// کسی ننویسد.
            this.readerWriterLockSlim.EnterReadLock();
            try { return this.TryGetRssItemWithDtoAndDraftEntryCore(key, out rssItemWithDto, out _); }
            finally { this.readerWriterLockSlim.ExitReadLock(); }
        }


        /// <summary>
        /// خواندن RssItemWithDto + DraftEntry هم‌زمان از cache — به‌صورت اتمی.
        /// 
        /// ⚠️ چرا DraftEntry هم برمی‌گردد؟
        /// برای optimistic concurrency: مصرف‌کننده می‌تواند بعداً با متدهای 
        /// TryMarkAs*IfDraftEntryUnchanged چک کند که آیا draft تغییر کرده یا نه.
        /// 
        /// ⚠️ چرا این متد جدا از TryGetRssItemWithDto است؟
        /// چون دو خروجی گرفتن (DTO + Entry) از یک متد، امضای متفاوتی دارد و 
        /// مصرف‌کننده‌هایی که فقط DTO می‌خواهند، نباید درگیر DraftEntry شوند.
        /// 
        /// ⚠️ قفل خواندن: هر دو خروجی باید از یک لحظه‌ی سازگار باشند — بدون قفل، 
        /// ممکن است DTO از نسخه‌ی قدیم و Entry از نسخه‌ی جدید باشد.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="rssItemWithDto"></param>
        /// <param name="rssItemDraftEntry"></param>
        /// <returns></returns>
        public bool TryGetRssItemWithDtoAndDraftEntry(
            Uri key, out RssItemWithDto rssItemWithDto, out DraftEntry<RssItem> rssItemDraftEntry)
        {
            this.readerWriterLockSlim.EnterReadLock();
            try { return this.TryGetRssItemWithDtoAndDraftEntryCore(key, out rssItemWithDto, out rssItemDraftEntry); }
            finally { this.readerWriterLockSlim.ExitReadLock(); }
        }


        public void Dispose()
        {
            /// ⚠️ قفل لازم نیست — چون Dispose در زمان بسته شدن برنامه یا پایان scope 
            /// صدا زده می‌شود و در آن لحظه، رقابتی بر سر کش‌ها وجود ندارد.
            this.Dispose(true);
        }


        ////////////////////////////////////////////////////////////////


        protected virtual void Dispose(bool disposing)
        {
            if (disposing == false || this.isDisposed == true)
                return;

            /// ⚠️ قفل لازم نیست — چون Dispose در زمان بسته شدن برنامه یا پایان scope 
            /// صدا زده می‌شود و در آن لحظه، رقابتی بر سر کش‌ها وجود ندارد.
            this.isDisposed = true;
            this.folderManager.DeleteFolderWithContents(this.tempImagesFolderPath.FolderPath);
            this.ClearCaches();
        }


        private bool TryMarkDraftEntryIfUnchangedCore(Uri key, RssItemWithDto rssItemWithDto,
            DraftState newState, DraftEntry<RssItem> originalDraftEntry)
        {
            /// وضعیت فعلی cache را می‌خوانیم (ممکن است null باشد اگر کلید وجود نداشته باشد).
            this.rssItemDraftCache.TryGetValue(key, out DraftEntry<RssItem> currentDraftEntry);

            /// originalDraftEntry = null  → انتظار داریم که کلید در cache نباشد
            /// originalDraftEntry != null → انتظار داریم که همان شیء باشد (نه فقط محتوای یکسان)
            ///
            /// ما «انتظار» را با «واقعیت» مقایسه می‌کنیم:
            ///   - اگر انتظار داشتیم کلید در cache نباشد ، یعنی مقدار originalDraftEntry برابر null بود ، پس :
            ///     مقدار شی جاری ، یعنی currentDraftEntry هم باید null باشد .
            ///   - اگر انتظار داشتیم شیء خاصی در cache باشد یعنی مقدار originalDraftEntry برابر null نبود ، پس :
            ///     حالا هم باید مقدار و اشاره گرِ شیِ currentDraftEntry هم با مقدار  و اشاره گرِ شیِ originalDraftEntry ، یکی باشد . یعنی اشاره گرشون فرقی نکنه .
            /// اگر انتظار ما با واقعیت نخواند، یعنی کسی در این فاصله cache را تغییر داده است.
            bool unchanged = (originalDraftEntry == null) ? (currentDraftEntry == null) : object.ReferenceEquals(currentDraftEntry, originalDraftEntry);
            if (unchanged == false)
                return false;

            this.AddOrUpdateToDraftCaches(key, rssItemWithDto, newState);
            return true;
        }


        private void AddOrUpdateToDraftCaches(Uri key, RssItemWithDto rssItemWithDto, DraftState draftState)
        {
            if (rssItemWithDto == null || rssItemWithDto.RssItem == null || rssItemWithDto.RssItemDto == null)
                throw new ArgumentNullException($"پارامتر {rssItemWithDto} و اعضایش نباید خالی یا null باشند .");

            this.draftCacheWriter.AddOrUpdateToDraftCache(this.rssItemDraftCache, key, rssItemWithDto.RssItem, draftState);
            this.draftCacheWriter.AddOrUpdateToDraftCache(this.rssItemDtoDraftCache, key, rssItemWithDto.RssItemDto, draftState);
        }


        private bool TryRemoveCore(Uri key)
        {
            if (this.rssItemDtoDraftCache.TryGetValue(key, out DraftEntry<RssItemDto> draftEntry) == false)
                return false;

            return this.TryRemoveWithFile(key, draftEntry);
        }


        private bool TryRemoveIfEntryUnchangedCore(Uri key, DraftEntry<RssItem> originalDraftEntry)
        {
            if (this.rssItemDraftCache.TryGetValue(key, out DraftEntry<RssItem> currentDraftEntry) == false ||
                object.ReferenceEquals(currentDraftEntry, originalDraftEntry) == false)
                return false;

            this.rssItemDtoDraftCache.TryGetValue(key, out DraftEntry<RssItemDto> rssItemDtoDraftEntry);
            return this.TryRemoveWithFile(key, rssItemDtoDraftEntry);
        }


        private bool TryRemoveWithFile(Uri key, DraftEntry<RssItemDto> rssItemDtoDraftEntry)
        {
            string draftImageFullFilePath = rssItemDtoDraftEntry?.Item?.ImageFullFilePath;
            if (string.IsNullOrWhiteSpace(draftImageFullFilePath) == false)
                this.TryDeleteDraftImageFile(draftImageFullFilePath);

            return this.TryRemoveCaches(key);
        }


        private void ClearCore()
        {
            this.DeleteDraftImageFiles();
            this.ClearCaches();
        }


        private IReadOnlyList<RssItemDto> GetRssItemDtosByFeedIdCopyCore(Uri rssFeedId)
        {
            IList<Uri> draftCacheKeysByFeedId = this.GetDraftCacheKeysByFeedIdCopy(rssFeedId);
            if (draftCacheKeysByFeedId == null || draftCacheKeysByFeedId.Count < 1)
                return null;
            return this.GetDraftRssItemDtosByKeysCopy(draftCacheKeysByFeedId);
        }


        private bool TryGetRssItemWithDtoAndDraftEntryCore(Uri key, out RssItemWithDto rssItemWithDto, out DraftEntry<RssItem>? rssItemDraftEntry)
        {
            rssItemWithDto = null;
            // حالت ۱: هیچ‌کدام نیستند — عادی
            if (this.rssItemDraftCache.TryGetValue(key, out rssItemDraftEntry) == false || rssItemDraftEntry.Item == null)
            {
                /// ✅ ایمن‌سازی: اگر Item null بود، entry را هم null می‌کنیم
                /// تا caller با حالت ناسازگار «false + entry غیرnull» مواجه نشود.
                rssItemDraftEntry = null;
                return false;
            }
            // حالت ۲: یکی هست، دیگری نیست — باگ
            if (this.rssItemDtoDraftCache.TryGetItem(key, out RssItemDto rssItemDto) == false)
                throw new InvalidOperationException("کلید در هر دو کش موقت وجود ندارد . باید در هر دو وجود داشته باشد .");

            rssItemWithDto = new RssItemWithDto(rssItemDraftEntry.Item, rssItemDto);
            return true;
        }


        private void DeleteDraftImageFiles()
        {
            foreach (RssItemDto rssItemDto in this.rssItemDtoDraftCache.GetAllItemsCopy())
            {
                this.TryDeleteDraftImageFile(rssItemDto.ImageFullFilePath);
            }
        }


        private bool TryDeleteDraftImageFile(string draftImageFullFilePath)
        {
            if (string.IsNullOrEmpty(draftImageFullFilePath) == true)
                return false;
            try
            {
                this.fileManager.DeleteFile(draftImageFullFilePath);
                return true;
            }
            catch (Exception exception)
            {
                return false;
            }
        }


        private void ClearCaches()
        {
            this.rssItemDraftCache.Clear();
            this.rssItemDtoDraftCache.Clear();
        }


        private bool TryRemoveCaches(Uri key)
        {
            bool isRemoved = this.rssItemDraftCache.TryRemove(key);
            this.rssItemDtoDraftCache.TryRemove(key);
            return isRemoved;
        }


        private IList<Uri> GetDraftCacheKeysByFeedIdCopy(Uri rssFeedId)
        {
            /// ⚠️ چرا از snapshot (کپی) استفاده نکردیم، بلکه روی داده‌ی زنده کار می‌کنیم؟
            /// چون متدِ فراخوان اش (یعنی متد GetRssItemDtosByFeedId) قفل خواندن داریم، و قفل از سازگاری بین دو خواندن محافظت می‌کند
            return this.rssItemDraftCache.GetAllKeyValues()?.
                Where(keyValuePairs => keyValuePairs.Value.Item.RssFeedId == rssFeedId)?.
                Select(keyValuePairs => keyValuePairs.Key)?.
                ToList();
        }


        private IReadOnlyList<RssItemDto> GetDraftRssItemDtosByKeysCopy(IList<Uri> draftCacheKeys)
        {
            /// ⚠️ چرا از snapshot (کپی) استفاده نکردیم، بلکه روی داده‌ی زنده کار می‌کنیم؟
            /// چون متدِ فراخوان اش (یعنی متد GetRssItemDtosByFeedId) قفل خواندن داریم، و قفل از سازگاری بین دو خواندن محافظت می‌کند
            return this.rssItemDtoDraftCache.GetAllKeyValues()?.
                Where(keyValuePairs => draftCacheKeys.Contains(keyValuePairs.Key))?.
                Select(keyValuePairs => keyValuePairs.Value.Item)?.
                ToList();
        }

    }
}
