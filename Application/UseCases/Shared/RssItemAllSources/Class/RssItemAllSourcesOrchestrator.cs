using Application.InfrastructureContracts.CacheProject.AppDraftCacheManager.RssItemDraftCacheManager.Interface;
using Application.InfrastructureContracts.CacheProject.CacheBase.DraftCache.DataModels;
using Application.InfrastructureContracts.DataAccessProject.AppQuery.RssItemDtoQuery.Factory.Interface;
using Application.InfrastructureContracts.DataAccessProject.AppQuery.RssItemDtoQuery.Interface;
using Application.InfrastructureContracts.DataAccessProject.AppRepository.RssItemRepository.DataModels;
using Application.InfrastructureContracts.DataAccessProject.AppRepository.RssItemRepository.Factory.Interfaces;
using Application.InfrastructureContracts.DataAccessProject.AppRepository.RssItemRepository.Interfaces;
using Application.InfrastructureContracts.DataAccessProject.DataAccessBase.UnitOfWork.Interfaces;
using Application.InfrastructureContracts.DataAccessProject.UnitOfWorkFactory.Interfaces;
using Application.OverallDataModels.OverallDto.EntityDto;
using Application.UseCases.Shared.RssItemAllSources.Interface;
using Application.UseCases.Shared.SaveRssItemToDraftCache.DataModels;
using Domain.ApplicationServiceContracts.RssItemAllSourcesOrchestrator.Interface;
using Domain.DomainEntities.RssEntity.Class;

namespace Application.UseCases.Shared.RssItemAllSources.Class
{
    public class RssItemAllSourcesOrchestrator : IRssItemExistenceChecker, IRssItemAllSourcesOrchestrator
    {

        private IRssItemDraftCacheManager rssItemDraftCacheManager;

        private IRssItemDtoQueryFactory rssItemDtoQueryFactory;

        private IRssItemRepositoryFactory rssItemRepositoryFactory;

        private IUnitOfWorkFactory unitOfWorkFactory;


        //////////////////////////////////////////////////////////////////


        public RssItemAllSourcesOrchestrator(IRssItemDraftCacheManager rssItemDraftCacheManager, IRssItemDtoQueryFactory rssItemDtoQueryFactory,
            IRssItemRepositoryFactory rssItemRepositoryFactory, IUnitOfWorkFactory unitOfWorkFactory)
        {
            this.rssItemDraftCacheManager = rssItemDraftCacheManager;
            this.rssItemDtoQueryFactory = rssItemDtoQueryFactory;
            this.rssItemRepositoryFactory = rssItemRepositoryFactory;
            this.unitOfWorkFactory = unitOfWorkFactory;
        }


        //////////////////////////////////////////////////////////////////


        public bool IsRssItemExist(Uri rssItemId)
        {
            bool isRssItemExistInDraftCache = this.rssItemDraftCacheManager.ContainKey(rssItemId);
            if(isRssItemExistInDraftCache == true)
                return true;

            using IRssItemDtoQuery rssItemDtoQuery = this.rssItemDtoQueryFactory.Create();
            bool isRssItemExistInDatabase = rssItemDtoQuery.IsRssItemExist(rssItemId.AbsoluteUri);
            if(isRssItemExistInDatabase == true) 
                return true;
            return false;
        }


        public RssItem GetRssItem(Uri rssItemId)
        {
            bool isRssItemExistInDraftCache = this.rssItemDraftCacheManager.ContainKey(rssItemId);
            if (isRssItemExistInDraftCache == true)
                return this.rssItemDraftCacheManager.GetRssItemDraftEntry(rssItemId).Item;

            using IUnitOfWork unitOfWork = this.unitOfWorkFactory.Create();
            IRssItemRepository rssItemRepository = this.rssItemRepositoryFactory.Create(unitOfWork);
            return rssItemRepository.FindByKey(rssItemId.AbsoluteUri);
        }


        public RssItemDto GetRssItemDto(Uri rssItemId)
        {
            bool isRssItemExistInDraftCache = this.rssItemDraftCacheManager.ContainKey(rssItemId);
            if (isRssItemExistInDraftCache == true)
                return this.rssItemDraftCacheManager.GetRssItemDto(rssItemId);

            using IRssItemDtoQuery rssItemDtoQuery = this.rssItemDtoQueryFactory.Create();
            return rssItemDtoQuery.FindByKey(rssItemId.AbsoluteUri);
        }


        /// <summary>
        /// به دلیل کارایی ، این متد را بصورت مجزا نوشتیم .
        /// یعنی به دلیل کارایی و افزایش سرعت ، در این متد ، متدهای GetRssItem و GetRssItemDto را فراخوانی نکردیم که البته فقط اندکی باعث نقض کدنویسی تکراری یا Dry شد .
        /// </summary>
        /// <param name="rssItemId"></param>
        /// <returns></returns>
        public RssItemWithDto GetRssItemWithDto(Uri rssItemId)
        {
            RssItemWithDto fromDraftCache = this.GetRssItemWithDtoFromDraftCache(rssItemId);
            if (fromDraftCache is not null)
                return fromDraftCache;

            string rssItemIdString = rssItemId.AbsoluteUri;
            RssItem rssItem = this.GetRssItemFromRepository(rssItemIdString);
            RssItemDto rssItemDto = this.GetRssItemDtoFromQuery(rssItemIdString);
            if (rssItem is null || rssItemDto is null)
                return null;
            return new RssItemWithDto(rssItem, rssItemDto);
        }


        public bool CommitDraftCacheItemToDatabase(Uri rssItemId, ImageSaveOptions imageSaveOptions)
        {
            if (this.rssItemDraftCacheManager.ContainKey(rssItemId) == false)
                throw new Exception("چنین شناسه ی rssItem ای درون سرویس کش موقت ، وجود ندارد .");
            DraftEntry<RssItem> rssItemDraftEntry = this.rssItemDraftCacheManager.GetRssItemDraftEntry(rssItemId);
            if(rssItemDraftEntry == null)
                return false;
            this.SaveToDatabase(rssItemDraftEntry, imageSaveOptions);
            this.rssItemDraftCacheManager.Remove(rssItemId);
            return true;
        }


        //////////////////////////////////////////////////////////////////


        private void SaveToDatabase(DraftEntry<RssItem> rssItemDraftEntry, ImageSaveOptions imageSaveOptions)
        {
            if (rssItemDraftEntry == null)
                throw new Exception($"{nameof(rssItemDraftEntry)} نباید null باشد .");

            using IUnitOfWork unitOfWork = this.unitOfWorkFactory.Create();
            IRssItemRepository rssItemRepository = this.rssItemRepositoryFactory.Create(unitOfWork);

            switch (rssItemDraftEntry.DraftState)
            {
                case DraftState.Added:
                        rssItemRepository.Add(rssItemDraftEntry.Item, imageSaveOptions);
                        break;
                case DraftState.Modified:
                    rssItemRepository.Update(rssItemDraftEntry.Item, imageSaveOptions);
                    break;
                case DraftState.Deleted:
                    rssItemRepository.Delete(rssItemDraftEntry.Item);
                    break;
            }
            unitOfWork.SaveChanges();
        }


        private RssItemWithDto GetRssItemWithDtoFromDraftCache(Uri rssItemId)
        {
            if (this.rssItemDraftCacheManager.ContainKey(rssItemId) == false)
                return null;

            RssItem rssItem = this.rssItemDraftCacheManager.GetRssItemDraftEntry(rssItemId).Item;
            RssItemDto rssItemDto = this.rssItemDraftCacheManager.GetRssItemDto(rssItemId);
            if (rssItem is null || rssItemDto is null)
                throw new Exception($"در کش موقت ، هر دوی {nameof(rssItem)} و {nameof(rssItemDto)} ، وجود نداره . که نباید این طور باشه . " +
                                    $"\nیعنی فقط یکی از این اشیاء در کش موقت هست ، و دیگری ، نیست .");
            return new RssItemWithDto(rssItem, rssItemDto);
        }


        private RssItem GetRssItemFromRepository(string rssItemId)
        {
            using IUnitOfWork unitOfWork = this.unitOfWorkFactory.Create();
            IRssItemRepository rssItemRepository = this.rssItemRepositoryFactory.Create(unitOfWork);
            return rssItemRepository.FindByKey(rssItemId);
        }


        private RssItemDto GetRssItemDtoFromQuery(string rssItemId)
        {
            using IRssItemDtoQuery rssItemDtoQuery = this.rssItemDtoQueryFactory.Create();
            return rssItemDtoQuery.FindByKey(rssItemId);
        }

    }
}
