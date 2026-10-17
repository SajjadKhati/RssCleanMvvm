using Application.InfrastructureContracts.CacheProject.AppDraftCacheManager.RssItemDraftCacheManager.Interface;
using Application.InfrastructureContracts.CacheProject.CacheBase.DraftCache.DataModels;
using Application.InfrastructureContracts.DataAccessProject.AppQuery.RssItemDtoQuery.Factory.Interface;
using Application.InfrastructureContracts.DataAccessProject.AppQuery.RssItemDtoQuery.Interface;
using Application.InfrastructureContracts.DataAccessProject.AppQuery.RssItemWithDtoQuery.Factory.Interface;
using Application.InfrastructureContracts.DataAccessProject.AppQuery.RssItemWithDtoQuery.Interface;
using Application.InfrastructureContracts.DataAccessProject.AppRepository.RssItemRepository.DataModels;
using Application.InfrastructureContracts.DataAccessProject.AppRepository.RssItemRepository.Factory.Interfaces;
using Application.InfrastructureContracts.DataAccessProject.AppRepository.RssItemRepository.Interfaces;
using Application.InfrastructureContracts.DataAccessProject.DataAccessBase.UnitOfWork.Interfaces;
using Application.InfrastructureContracts.DataAccessProject.UnitOfWorkFactory.Interfaces;
using Application.InfrastructureContracts.ImageProject.ImageFileSaver.DataModels.Class;
using Application.InfrastructureContracts.ImageProject.ImageSettings.ImageSettingsProvider.Interfaces;
using Application.InfrastructureContracts.ImageProject.SharedDataModels.Class;
using Application.InfrastructureContracts.SharedProject.AppFolderPath.Interface;
using Application.OverallDataModels.OverallDto.CompositeDto;
using Application.OverallDataModels.OverallDto.EntityDto;
using Application.UseCases.Shared.RssItemAllSources.Interface;
using Domain.ApplicationServiceContracts.RssItemAllSourcesOrchestrator.Interface;
using Domain.DomainEntities.RssEntity.Class;

namespace Application.UseCases.Shared.RssItemAllSources.Class
{
    public class RssItemAllSourcesOrchestrator : IRssItemExistenceChecker, IRssItemAllSourcesOrchestrator
    {

        private IRssItemDraftCacheReader rssItemDraftCacheReader;

        private IRssItemDraftCacheWriter rssItemDraftCacheWriter;

        private IRssItemWithDtoQueryFactory rssItemWithDtoQueryFactory;

        private IRssItemDtoQueryFactory rssItemDtoQueryFactory;

        private IRssItemRepositoryFactory rssItemRepositoryFactory;

        private IUnitOfWorkFactory unitOfWorkFactory;

        private IImageSettingsReader imageSettingsReader;

        private IPersistentImageFolderPath persistentImageFolderPath;


        //////////////////////////////////////////////////////////////////


        public RssItemAllSourcesOrchestrator(IRssItemDraftCacheReader rssItemDraftCacheReader, IRssItemDraftCacheWriter rssItemDraftCacheWriter, 
            IRssItemWithDtoQueryFactory rssItemWithDtoQueryFactory, IRssItemDtoQueryFactory rssItemDtoQueryFactory,
            IRssItemRepositoryFactory rssItemRepositoryFactory, IUnitOfWorkFactory unitOfWorkFactory,
            IImageSettingsReader imageSettingsReader, IPersistentImageFolderPath persistentImageFolderPath)
        {
            this.rssItemDraftCacheReader = rssItemDraftCacheReader;
            this.rssItemDraftCacheWriter = rssItemDraftCacheWriter;
            this.rssItemWithDtoQueryFactory = rssItemWithDtoQueryFactory;
            this.rssItemDtoQueryFactory = rssItemDtoQueryFactory;
            this.rssItemRepositoryFactory = rssItemRepositoryFactory;
            this.unitOfWorkFactory = unitOfWorkFactory;
            this.imageSettingsReader = imageSettingsReader;
            this.persistentImageFolderPath = persistentImageFolderPath;
        }


        //////////////////////////////////////////////////////////////////


        /// <summary>
        /// بررسی وجود یک RssItem — در cache (اول)، سپس در DB (با double-check).
        /// 
        /// ⚠️ چرا این متد قفل ندارد؟
        /// این متد فقط می‌خواند — هیچ stateای را mutate نمی‌کند.
        /// تمام خواندن‌ها اتمی هستند (ContainKey روی ConcurrentDictionary، 
        /// و DB query خودش). حتی اگر بین دو خواندن، نخ دیگری چیزی بنویسد، 
        /// ناسازگاری داخلی رخ نمی‌دهد.
        /// 
        /// double-check، همان کاری را می‌کند که قفل می‌کرد — بدون overhead:
        /// اگر X در فاصله‌ی DB query به cache اضافه شد، آن را می‌گیرد.
        /// پنجره‌ی باقی‌مانده (بین چک دوم و return) نانوثانیه است و بی‌اهمیت 
        /// — چون خروجی فقط یک bool است، نه یک action.
        /// </summary>
        /// <param name="rssItemId"></param>
        /// <returns></returns>
        public bool IsRssItemExist(Uri rssItemId)
        {
            bool isRssItemExistInDraftCache = this.rssItemDraftCacheReader.ContainKey(rssItemId);
            if(isRssItemExistInDraftCache == true)
                return true;

            bool isRssItemExistInDatabase = this.IsRssItemExistInDatabase(rssItemId.AbsoluteUri);

            /// ⚠️ الگوی Double-Checked Cache:
            ///   ۱. چک اول در draft — اگر بود، برگردان.
            ///   ۲. DB query (کند).
            ///   ۳. بازچک در draft — اگر در این فاصله، نخ دیگری draft جدید ساخته، 
            ///      آن را برگردان.
            ///   ۴. در غیر این صورت، نتیجه‌ی DB را برگردان.
            /// 
            /// چرا بازچک؟ DB query کند است (چند ms). در این فاصله، نخ دیگری می‌تواند 
            /// نسخه‌ی جدیدتر را در draft بسازد. بدون بازچک، نسخه‌ی قدیمی DB برگردانده 
            /// می‌شود. (پنجره‌ی باقی‌مانده: چند نانوثانیه — پذیرفته‌شده.)
            isRssItemExistInDraftCache = this.rssItemDraftCacheReader.ContainKey(rssItemId);
            if (isRssItemExistInDraftCache == true)
                return true;
            return isRssItemExistInDatabase;
        }


        public RssItem GetRssItem(Uri rssItemId)
        {
            if (this.rssItemDraftCacheReader.TryGetRssItemDraftEntry(rssItemId, out DraftEntry<RssItem> rssItemDraftEntry) == true && 
                rssItemDraftEntry.Item != null)
                return rssItemDraftEntry.Item;

            RssItem rssItemInDb = this.GetRssItemFromDatabase(rssItemId.AbsoluteUri);

            /// ⚠️ الگوی Double-Checked Cache:
            ///   ۱. چک اول در draft — اگر بود، برگردان.
            ///   ۲. DB query (کند).
            ///   ۳. بازچک در draft — اگر در این فاصله، نخ دیگری draft جدید ساخته، 
            ///      آن را برگردان.
            ///   ۴. در غیر این صورت، نتیجه‌ی DB را برگردان.
            /// 
            /// چرا بازچک؟ DB query کند است (چند ms). در این فاصله، نخ دیگری می‌تواند 
            /// نسخه‌ی جدیدتر را در draft بسازد. بدون بازچک، نسخه‌ی قدیمی DB برگردانده 
            /// می‌شود. (پنجره‌ی باقی‌مانده: چند نانوثانیه — پذیرفته‌شده.)
            if (this.rssItemDraftCacheReader.TryGetRssItemDraftEntry(rssItemId, out rssItemDraftEntry) == true &&
                rssItemDraftEntry.Item != null)
                return rssItemDraftEntry.Item;
            return rssItemInDb;
        }


        public RssItemDto GetRssItemDto(Uri rssItemId)
        {
            if(this.rssItemDraftCacheReader.TryGetRssItemDto(rssItemId, out RssItemDto rssItemDto) == true)
                return rssItemDto;

            RssItemDto rssItemDtoInDb = this.GetRssItemDtoFromDatabase(rssItemId.AbsoluteUri);

            /// ⚠️ الگوی Double-Checked Cache:
            ///   ۱. چک اول در draft — اگر بود، برگردان.
            ///   ۲. DB query (کند).
            ///   ۳. بازچک در draft — اگر در این فاصله، نخ دیگری draft جدید ساخته، 
            ///      آن را برگردان.
            ///   ۴. در غیر این صورت، نتیجه‌ی DB را برگردان.
            /// 
            /// چرا بازچک؟ DB query کند است (چند ms). در این فاصله، نخ دیگری می‌تواند 
            /// نسخه‌ی جدیدتر را در draft بسازد. بدون بازچک، نسخه‌ی قدیمی DB برگردانده 
            /// می‌شود. (پنجره‌ی باقی‌مانده: چند نانوثانیه — پذیرفته‌شده.)
            if (this.rssItemDraftCacheReader.TryGetRssItemDto(rssItemId, out rssItemDto) == true)
                return rssItemDto;
            return rssItemDtoInDb;
        }


        /// <summary>
        /// به دلیل کارایی ، این متد را بصورت مجزا نوشتیم .
        /// یعنی به دلیل کارایی و افزایش سرعت ، در این متد ، متدهای GetRssItem و GetRssItemDto را فراخوانی نکردیم که البته فقط اندکی باعث نقض کدنویسی تکراری یا Dry شد .
        /// </summary>
        /// <param name="rssItemId"></param>
        /// <returns></returns>
        public RssItemWithDto GetRssItemWithDto(Uri rssItemId)
        {
            if(this.rssItemDraftCacheReader.TryGetRssItemWithDto(rssItemId, out RssItemWithDto fromDraftCache) == true && fromDraftCache != null)
                return fromDraftCache;

            RssItemWithDto rssItemWithDtoInDb = this.GetRssItemWithDtoFromQuery(rssItemId.AbsoluteUri);

            /// ⚠️ الگوی Double-Checked Cache:
            ///   ۱. چک اول در draft — اگر بود، برگردان.
            ///   ۲. DB query (کند).
            ///   ۳. بازچک در draft — اگر در این فاصله، نخ دیگری draft جدید ساخته، 
            ///      آن را برگردان.
            ///   ۴. در غیر این صورت، نتیجه‌ی DB را برگردان.
            /// 
            /// چرا بازچک؟ DB query کند است (چند ms). در این فاصله، نخ دیگری می‌تواند 
            /// نسخه‌ی جدیدتر را در draft بسازد. بدون بازچک، نسخه‌ی قدیمی DB برگردانده 
            /// می‌شود. (پنجره‌ی باقی‌مانده: چند نانوثانیه — پذیرفته‌شده.)
            if (this.rssItemDraftCacheReader.TryGetRssItemWithDto(rssItemId, out fromDraftCache) == true && fromDraftCache != null)
                return fromDraftCache;
            return rssItemWithDtoInDb;
        }


        /// <summary>
        /// خواندن RssItemWithDto به‌همراه snapshot از DraftEntry — با fallback به DB.
        /// 
        /// ⚠️ چرا DraftEntry هم برمی‌گردد؟
        /// برای optimistic concurrency: اگر آیتم از cache خوانده شد، DraftEntry همان شیء است.
        /// اگر از DB خوانده شد، null برمی‌گردد (یعنی «در cache نبود»).
        /// 
        /// ⚠️ الگوی Double-Checked Cache:
        ///   ۱. چک اول در draft — اگر بود، برگردان.
        ///   ۲. DB query (کند).
        ///   ۳. بازچک در draft — اگر در این فاصله، نخ دیگری draft جدید ساخته، آن را برگردان.
        ///   ۴. در غیر این صورت، نتیجه‌ی DB را برگردان (ممکن است null باشد).
        /// 
        /// ⚠️ نکته‌ی مهم: بازچک، حتی وقتی DB نتیجه null داده هم انجام می‌شود — 
        /// چون ممکن است در فاصله‌ی DB query، نخ دیگری draft جدید ساخته باشد. اگر 
        /// بازچک را نادیده بگیریم، آن draft جدید را از دست می‌دهیم.
        /// </summary>
        /// <param name="rssItemId"></param>
        /// <param name="rssItemWithDto"></param>
        /// <param name="originalDraftEntry"></param>
        /// <returns></returns>
        public bool TryGetRssItemWithDtoAndDraftEntry(
            Uri rssItemId, out RssItemWithDto rssItemWithDto, out DraftEntry<RssItem> originalDraftEntry)
        {
            if (this.rssItemDraftCacheReader.TryGetRssItemWithDtoAndDraftEntry(
                rssItemId, out rssItemWithDto, out originalDraftEntry) == true && rssItemWithDto != null)
                return true;

            RssItemWithDto rssItemWithDtoInDb = this.GetRssItemWithDtoFromQuery(rssItemId.AbsoluteUri);

            if (this.rssItemDraftCacheReader.TryGetRssItemWithDtoAndDraftEntry(
                    rssItemId, out rssItemWithDto, out originalDraftEntry) == true && rssItemWithDto != null)
                return true;
            originalDraftEntry = null;
            rssItemWithDto = rssItemWithDtoInDb;
            return rssItemWithDto != null;
        }


        /// <summary>
        /// commit همه‌ی آیتم‌های موجود در کش موقت به دیتابیس.
        /// 
        /// ⚠️ چرا بین GetAllDraftCacheKeys و CommitDraftCacheItemBatchToDatabase 
        /// قفل لازم نیست؟
        /// چون:
        ///  - GetAllDraftCacheKeys یک snapshot اتمی می‌گیرد.
        ///  - اگر آیتم جدیدی در این فاصله اضافه شود، در دور بعدی commit خواهد شد.
        ///  - اگر آیتمی حذف شده باشد، TryGetRssItemDraftEntry آن را پیدا نمی‌کند 
        ///    و از پردازش صرف‌نظر می‌شود.
        ///  - اگر آیتمی به‌روز شده باشد، TryRemoveIfEntryUnchanged از حذف نسخه‌ی 
        ///    جدید جلوگیری می‌کند.
        /// پس هیچ Data Loss یا Duplicate Commit رخ نمی‌دهد.
        /// 
        /// متد TryGetRssItemDraftEntry هم درون چندین پشته ی درونی و درون متد StageDraftCacheItemBatch ، فراخوانی میشود .
        /// </summary>
        /// <returns></returns>
        public IList<Uri> CommitAllDraftCacheItemsToDatabase()
        {
            IReadOnlyCollection<Uri> draftCacheKeys = this.rssItemDraftCacheReader.GetAllDraftCacheKeys();
            if (draftCacheKeys == null || draftCacheKeys.Count < 1)
                return null;
            return this.CommitDraftCacheItemBatchToDatabase(draftCacheKeys);
        }


        public bool CommitDraftCacheItemToDatabase(Uri rssItemId)
        {
            IList<Uri> committed = this.CommitDraftCacheItemBatchToDatabase(new[] { rssItemId });
            return committed.Count > 0;
        }


        /// <summary>
        /// commit کالکشنی از draftها به دیتابیس — با یک SaveChanges.
        /// 
        /// ⚠️ چرا قفل ندارد؟
        ///  ۱. متد TryGetRssItemDraftEntry اتمی است (write lock داخلی 
        ///     RssItemDraftCacheManager آن را تضمین می‌کند).
        ///  ۲. متد StageDraftCacheItemBatch روی unitOfWork و Repository محلی کار می‌کند 
        ///     (اطلاعاتی را به اشتراک نمیگذارد).
        ///  ۳. متد SaveChanges خودش تراکنش دارد (all-or-nothing).
        ///  ۴. متد TryRemoveIfEntryUnchanged اتمی و conditional است — پس نسخه‌ی 
        ///     جدیدی که در فاصله ساخته شده باشد، حذف نخواهد شد.
        ///     
        /// متد TryGetRssItemDraftEntry هم درون چندین پشته ی درونی و درون متد StageDraftCacheItemBatch ، فراخوانی میشود .
        /// </summary>
        /// <param name="rssItemIds"></param>
        /// <returns></returns>
        public IList<Uri> CommitDraftCacheItemBatchToDatabase(IEnumerable<Uri> rssItemIds)
        {
            using IUnitOfWork unitOfWork = this.unitOfWorkFactory.Create();
            IRssItemRepository repository = this.rssItemRepositoryFactory.Create(unitOfWork);

            Dictionary<Uri, DraftEntry<RssItem>> stagedDraftEntryDictionary = this.StageDraftCacheItemBatch(rssItemIds, repository);
            if (stagedDraftEntryDictionary.Count < 1)
                return null;

            unitOfWork.SaveChanges();

            foreach (var stagedDraftEntryKeyValuePair in stagedDraftEntryDictionary)
                this.rssItemDraftCacheWriter.TryRemoveIfEntryUnchanged(stagedDraftEntryKeyValuePair.Key, stagedDraftEntryKeyValuePair.Value);

            return stagedDraftEntryDictionary.Keys.ToList();
        }


        //////////////////////////////////////////////////////////////////


        private bool IsRssItemExistInDatabase(string rssItemId)
        {
            using IRssItemDtoQuery rssItemDtoQuery = this.rssItemDtoQueryFactory.Create();
            return rssItemDtoQuery.IsRssItemExist(rssItemId);
        }


        private RssItem GetRssItemFromDatabase(string rssItemId)
        {
            using IUnitOfWork unitOfWork = this.unitOfWorkFactory.Create();
            IRssItemRepository rssItemRepository = this.rssItemRepositoryFactory.Create(unitOfWork);
            return rssItemRepository.FindByKey(rssItemId);
        }


        private RssItemDto GetRssItemDtoFromDatabase(string rssItemId)
        {
            using IRssItemDtoQuery rssItemDtoQuery = this.rssItemDtoQueryFactory.Create();
            return rssItemDtoQuery.FindByKey(rssItemId);
        }


        private RssItemWithDto GetRssItemWithDtoFromQuery(string rssItemId)
        {
            using IRssItemWithDtoQuery rssItemWithDtoQuery = this.rssItemWithDtoQueryFactory.Create();
            return rssItemWithDtoQuery.FindByKey(rssItemId);
        }


        private Dictionary<Uri, DraftEntry<RssItem>> StageDraftCacheItemBatch(IEnumerable<Uri> rssItemIds, IRssItemRepository repository)
        {
            Dictionary<Uri, DraftEntry<RssItem>> stagedDraftEntryDictionary = new Dictionary<Uri, DraftEntry<RssItem>>();
            foreach (Uri id in rssItemIds)
            {
                if (this.rssItemDraftCacheReader.TryGetRssItemDraftEntry(id, out DraftEntry<RssItem> rssItemDraftEntry) == false
                    || rssItemDraftEntry.Item == null)
                    continue;

                ImageSaveOptions imageSaveOptions = this.BuildImageSaveOptions(rssItemDraftEntry.Item);
                this.StageDraftChangeInRepository(repository, rssItemDraftEntry, imageSaveOptions);
                stagedDraftEntryDictionary[id] = rssItemDraftEntry;
            }
            return stagedDraftEntryDictionary;
        }


        private ImageSaveOptions BuildImageSaveOptions(RssItem rssItem)
        {
            ImageSettings<ImageInfo> imageInfoSettings = this.imageSettingsReader.GetOrDefault(rssItem.Id, rssItem.Argb8BitImage);
            return new ImageSaveOptions
            {
                ImageInfoSettings = imageInfoSettings,
                FolderPath = this.persistentImageFolderPath.FolderPath
            };
        }


        private void StageDraftChangeInRepository(IRssItemRepository repository,
            DraftEntry<RssItem> rssItemDraftEntry, ImageSaveOptions imageSaveOptions)
        {
            switch (rssItemDraftEntry.DraftState)
            {
                case DraftState.Added:
                    repository.Add(rssItemDraftEntry.Item, imageSaveOptions);
                    break;
                case DraftState.Modified:
                    repository.Update(rssItemDraftEntry.Item, imageSaveOptions);
                    break;
                case DraftState.Deleted:
                    repository.Delete(rssItemDraftEntry.Item);
                    break;
            }
        }

    }
}
