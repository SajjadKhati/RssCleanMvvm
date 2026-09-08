using Application.InfrastructureContracts.CacheProject.AppDraftCacheManager.RssItemDraftCacheManager.Interface;
using Application.InfrastructureContracts.CacheProject.CacheBase.DraftCache.DataModels;
using Application.InfrastructureContracts.DataAccessProject.AppQuery.RssItemDtoQuery.Factory.Interface;
using Application.InfrastructureContracts.DataAccessProject.AppQuery.RssItemDtoQuery.Interface;
using Application.InfrastructureContracts.DataAccessProject.AppRepository.RssItemRepository.DataModels;
using Application.InfrastructureContracts.DataAccessProject.AppRepository.RssItemRepository.Factory.Interfaces;
using Application.InfrastructureContracts.DataAccessProject.AppRepository.RssItemRepository.Interfaces;
using Application.InfrastructureContracts.DataAccessProject.DataAccessBase.UnitOfWork.Interfaces;
using Application.InfrastructureContracts.DataAccessProject.UnitOfWorkFactory.Interfaces;
using Application.Services.RssItemAllSourcesCoordinatorService.Interface;
using Domain.ApplicationServiceContracts.RssItemAllSourcesCoordinator.Interface;
using Domain.DomainEntities.RssEntity.Class;

namespace Application.Services.RssItemAllSourcesCoordinatorService.Class
{
    public class RssItemAllSourcesCoordinator : IRssItemExistenceChecker, IRssItemAllSourcesCoordinator
    {

        private IRssItemDraftCacheManager rssItemDraftCacheManager;

        private IRssItemDtoQueryFactory rssItemDtoQueryFactory;

        private IRssItemRepositoryFactory rssItemRepositoryFactory;

        private IUnitOfWorkFactory unitOfWorkFactory;


        //////////////////////////////////////////////////////////////////


        public RssItemAllSourcesCoordinator(IRssItemDraftCacheManager rssItemDraftCacheManager, IRssItemDtoQueryFactory rssItemDtoQueryFactory,
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

    }
}
