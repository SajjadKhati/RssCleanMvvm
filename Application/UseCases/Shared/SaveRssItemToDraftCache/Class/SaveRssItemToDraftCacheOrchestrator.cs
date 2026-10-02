using Application.InfrastructureContracts.CacheProject.AppDraftCacheManager.RssItemDraftCacheManager.Interface;
using Application.InfrastructureContracts.DataAccessProject.AppQuery.RssItemDtoQuery.Factory.Interface;
using Application.InfrastructureContracts.DataAccessProject.AppQuery.RssItemDtoQuery.Interface;
using Application.OverallDataModels.OverallDto.EntityDto;
using Application.UseCases.Shared.RssItemAllSources.Interface;
using Application.UseCases.Shared.SaveRssItemToDraftCache.DataModels;
using Application.UseCases.Shared.SaveRssItemToDraftCache.Interface;
using Domain.ApplicationServiceContracts.RssItemAllSourcesOrchestrator.Interface;
using Domain.DomainEntities.RssEntity.Class;

namespace Application.UseCases.Shared.SaveRssItemToDraftCache.Class
{
    public class SaveRssItemToDraftCacheOrchestrator : ISaveRssItemToDraftCacheOrchestrator
    {

        private IRssItemDraftCacheManager rssItemDraftCacheManager;

        private IRssItemDtoQueryFactory rssItemDtoQueryFactory;

        private IRssItemExistenceChecker rssItemExistenceChecker;

        private IRssItemAllSourcesOrchestrator rssItemAllSourcesOrchestrator;



        /////////////////////////////////////////////////////////////////////////


        public SaveRssItemToDraftCacheOrchestrator(IRssItemDraftCacheManager rssItemDraftCacheManager, 
            IRssItemDtoQueryFactory rssItemDtoQueryFactory, IRssItemExistenceChecker rssItemExistenceChecker, 
            IRssItemAllSourcesOrchestrator rssItemAllSourcesOrchestrator)
        {
            this.rssItemDraftCacheManager = rssItemDraftCacheManager;
            this.rssItemDtoQueryFactory = rssItemDtoQueryFactory;
            this.rssItemExistenceChecker = rssItemExistenceChecker;
            this.rssItemAllSourcesOrchestrator = rssItemAllSourcesOrchestrator;
        }


        /////////////////////////////////////////////////////////////////////////


        public void SaveToDraftCache(Uri rssItemId, Func<RssItemWithDto, RssItemWithDto> updateFunc)
        {
            bool isExistInAllSources = this.rssItemExistenceChecker.IsRssItemExist(rssItemId);
            if (isExistInAllSources == false)
                throw new InvalidOperationException($"شناسه ی «{rssItemId}» نه در دیتابیس و نه در کش موقت ، وجود ندارد .");

            RssItemWithDto currentRssItemWithDto = this.rssItemAllSourcesOrchestrator.GetRssItemWithDto(rssItemId);
            RssItemWithDto updatedRssItemWithDto = updateFunc(currentRssItemWithDto);
            this.EnsureUpdateFuncReturnedNewInstances(currentRssItemWithDto, updatedRssItemWithDto);
            bool existsInDatabase = this.IsExistsRssItemInDatabase(rssItemId.AbsoluteUri);
            this.MarkInDraftCache(rssItemId, updatedRssItemWithDto, existsInDatabase);
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
            if (current.Equals(updated) == true)
                throw new InvalidOperationException($"updateFunc باید یک نمونه‌ی جدید از {nameof(RssItemWithDto)} برگرداند. تغییر مستقیم روی شیء فعلی مجاز نیست.");
            
            if (current.RssItem.Equals(updated.RssItem) == true)
                throw new InvalidOperationException($"متد updateFunc باید یک نمونه‌ی جدید از {nameof(RssItem)} برگرداند. تغییر مستقیم روی شیء فعلی مجاز نیست.");

            if (current.RssItemDto.Equals(updated.RssItemDto) == true)
                throw new InvalidOperationException($"متد updateFunc باید یک نمونه‌ی جدید از {nameof(RssItemDto)} برگرداند. تغییر مستقیم روی شیء فعلی مجاز نیست.");
        }


        private bool IsExistsRssItemInDatabase(string rssItemId)
        {
            using IRssItemDtoQuery rssItemDtoQuery = this.rssItemDtoQueryFactory.Create();
            return rssItemDtoQuery.IsRssItemExist(rssItemId);
        }


        private void MarkInDraftCache(Uri rssItemId, RssItemWithDto updatedRssItemWithDto, bool existsInDatabase)
        {
            if (existsInDatabase == true)
                this.rssItemDraftCacheManager.MarkAsModifiedInDraftCache(rssItemId, updatedRssItemWithDto.RssItem, updatedRssItemWithDto.RssItemDto);
            else
                this.rssItemDraftCacheManager.MarkAsAddedInDraftCache(rssItemId, updatedRssItemWithDto.RssItem, updatedRssItemWithDto.RssItemDto);
        }

    }
}
