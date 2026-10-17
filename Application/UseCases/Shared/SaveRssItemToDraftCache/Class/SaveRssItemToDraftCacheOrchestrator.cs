using Application.InfrastructureContracts.CacheProject.AppDraftCacheManager.RssItemDraftCacheManager.Interface;
using Application.InfrastructureContracts.CacheProject.CacheBase.DraftCache.DataModels;
using Application.InfrastructureContracts.DataAccessProject.AppQuery.RssItemDtoQuery.Factory.Interface;
using Application.InfrastructureContracts.DataAccessProject.AppQuery.RssItemDtoQuery.Interface;
using Application.OverallDataModels.OverallDto.CompositeDto;
using Application.OverallDataModels.OverallDto.EntityDto;
using Application.UseCases.Shared.RssItemAllSources.Interface;
using Application.UseCases.Shared.SaveRssItemToDraftCache.Interface;
using Domain.DomainEntities.RssEntity.Class;

namespace Application.UseCases.Shared.SaveRssItemToDraftCache.Class
{
    public class SaveRssItemToDraftCacheOrchestrator : ISaveRssItemToDraftCacheOrchestrator
    {

        private IRssItemDraftCacheWriter rssItemDraftCacheWriter;

        private IRssItemDtoQueryFactory rssItemDtoQueryFactory;

        private IRssItemAllSourcesOrchestrator rssItemAllSourcesOrchestrator;



        /////////////////////////////////////////////////////////////////////////


        public SaveRssItemToDraftCacheOrchestrator(IRssItemDraftCacheWriter rssItemDraftCacheWriter, 
            IRssItemDtoQueryFactory rssItemDtoQueryFactory, IRssItemAllSourcesOrchestrator rssItemAllSourcesOrchestrator)
        {
            this.rssItemDraftCacheWriter = rssItemDraftCacheWriter;
            this.rssItemDtoQueryFactory = rssItemDtoQueryFactory;
            this.rssItemAllSourcesOrchestrator = rssItemAllSourcesOrchestrator;
        }


        /////////////////////////////////////////////////////////////////////////


        public bool SaveToDraftCache(Uri rssItemId, Func<RssItemWithDto, RssItemWithDto> updateFunc)
        {
            /// snapshot اتمی از cache: هم DTO فعلی را می‌گیریم، هم DraftEntry را.
            /// این دو باید از یک لحظه‌ی سازگار باشند — وگرنه optimistic check بعدی 
            /// بی‌معنی می‌شود. اگر آیتم در cache نباشد، originalDraftEntry برابر null 
            /// می‌شود (که یعنی «انتظار داریم هنوز در cache نباشد»).
            if (this.rssItemAllSourcesOrchestrator.TryGetRssItemWithDtoAndDraftEntry(
                rssItemId, out RssItemWithDto currentRssItemWithDto, out DraftEntry<RssItem> originalDraftEntry) == false)
                throw new InvalidOperationException($"شناسه ی «{rssItemId}» نه در دیتابیس و نه در کش موقت ، وجود ندارد .");

            RssItemWithDto updatedRssItemWithDto = updateFunc(currentRssItemWithDto);

            this.EnsureUpdateFuncReturnedNewInstances(currentRssItemWithDto, updatedRssItemWithDto);
            /// وجود یا نبود آیتم در دیتابیس را چک می‌کنیم تا پرچم درست (Added یا Modified) را انتخاب کنیم.
            bool existsInDatabase = this.IsExistsRssItemInDatabase(rssItemId.AbsoluteUri);
            /// ⚠️ پنجره‌ی existsInDatabase: از پایان IsExistsRssItemInDatabase تا این خط، 
            /// مقدار existsInDatabase ممکن است کهنه شود — اگر کسی در این فاصله آیتم را 
            /// در DB تغییر دهد. ولی این پنجره دو دلیل بی‌اهمیت است:
            ///   ۱. کوتاه است (چند نانوثانیه) و فقط به همان آیتم در DB مربوط است.
            ///   ۲. اگر cache در این فاصله تغییر کرده باشد، TryMarkInDraftCache 
            ///      با ReferenceEquals آن را می‌گیرد و false برمی‌گرداند — پس 
            ///      با داده‌ی کهنه نوشته نمی‌شود.
            /// راه‌حل کامل (تراکنش توزیع‌شده بین DB و cache) برای این پروژه overkill است.
            ///
            /// نوشتن conditional: اگر cache از لحظه‌ی snapshot تغییر کرده باشد، 
            /// TryMarkInDraftCache مقدار false برمی‌گرداند — تا نسخه‌ی جدیدترِ 
            /// نخ دیگری overwrite نشود. در آن صورت، کاربر باید دوباره تلاش کند.
            bool isMarkedInDraftCache = this.TryMarkInDraftCache(rssItemId, updatedRssItemWithDto, existsInDatabase, originalDraftEntry);
            return isMarkedInDraftCache;
        }


        //////////////////////////////////////////////////////////////////////////


        /// <summary>
        /// اطمینان حاصل می‌کند که updateFunc نمونه‌های جدیدی برگردانده باشد،
        /// نه اینکه همان اشیاء موجود را تغییر داده باشد.
        ///
        /// چرا مهم است؟
        /// اگر updateFunc همان اشیاء فعلی را برگرداند (تغییر درجا)، آن اشیاء
        /// ممکن است پیش از آماده شدن برای ذخیره، به‌صورت ناخواسته تغییر کرده باشند.
        /// این نقض جداسازی بین داده‌ی ذخیره‌شده و داده‌ی در حال ویرایش است.
        /// </summary>
        private void EnsureUpdateFuncReturnedNewInstances(RssItemWithDto current, RssItemWithDto updated)
        {
            if (object.ReferenceEquals(current, updated) == true)
                throw new InvalidOperationException($"updateFunc باید یک نمونه‌ی جدید از {nameof(RssItemWithDto)} برگرداند. تغییر مستقیم روی شیء فعلی مجاز نیست.");
            
            if (object.ReferenceEquals(current.RssItem, updated.RssItem) == true)
                throw new InvalidOperationException($"متد updateFunc باید یک نمونه‌ی جدید از {nameof(RssItem)} برگرداند. تغییر مستقیم روی شیء فعلی مجاز نیست.");

            if (object.ReferenceEquals(current.RssItemDto, updated.RssItemDto) == true)
                throw new InvalidOperationException($"متد updateFunc باید یک نمونه‌ی جدید از {nameof(RssItemDto)} برگرداند. تغییر مستقیم روی شیء فعلی مجاز نیست.");
        }


        private bool IsExistsRssItemInDatabase(string rssItemId)
        {
            using IRssItemDtoQuery rssItemDtoQuery = this.rssItemDtoQueryFactory.Create();
            return rssItemDtoQuery.IsRssItemExist(rssItemId);
        }


        private bool TryMarkInDraftCache(Uri rssItemId, RssItemWithDto updatedRssItemWithDto, 
            bool existsInDatabase, DraftEntry<RssItem> originalDraftEntry)
        {
            if (existsInDatabase == true)
                return this.rssItemDraftCacheWriter.TryMarkAsModifiedIfDraftEntryUnchanged(rssItemId, updatedRssItemWithDto, originalDraftEntry);
            else
                return this.rssItemDraftCacheWriter.TryMarkAsAddedIfDraftEntryUnchanged(rssItemId, updatedRssItemWithDto, originalDraftEntry);
        }

    }
}
