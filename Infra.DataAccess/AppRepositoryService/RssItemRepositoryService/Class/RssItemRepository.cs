using Application.InfrastructureContracts.CacheProject.AppDraftCacheManager.DraftCacheWriter.Interface;
using Application.InfrastructureContracts.CacheProject.CacheBase.Cache.Interfaces;
using Application.InfrastructureContracts.DataAccessProject.AppRepository.RssItemRepository.DataModels;
using Application.InfrastructureContracts.DataAccessProject.AppRepository.RssItemRepository.Interfaces;
using Application.InfrastructureContracts.ImageProject.ImageFileReader.Interfaces;
using Application.InfrastructureContracts.SharedProject.Disposed.Interface;
using Domain.DomainEntities.RssEntity.Class;
using Infra.DataAccess.AppCacheManagerService.RssItemRepositoryCacheCoordinatorService.InternalClass;
using Infra.DataAccess.AppCacheManagerService.RssItemRepositoryCacheCoordinatorService.InternalInterface;
using Infra.DataAccess.DataAccessBaseServices.RepositoryQueryBaseService.Class;
using Infra.DataAccess.DbContextService.Class;
using Infra.DataAccess.EfEntities.Class;
using Infra.DataAccess.EfEntityMapperService.Class;
using Infra.DataAccess.ExpressionFactoryService.Class;
using Infra.DataAccess.RssItemDtoEfEntityFactoryService.Interface;
using Microsoft.EntityFrameworkCore;

namespace Infra.DataAccess.AppRepositoryService.RssItemRepositoryService.Class
{
    public class RssItemRepository : RepositoryBase, IRssItemRepository
    {

        private RssItemEfEntityMapper efEntityMapper;
        
        private IImageFileReader imageFileReader;

        private IRssItemRepositoryCacheCoordinator rssItemCacheCoordinator;

        private IRssItemEfEntityFactory rssItemEfEntityFactory;

        private RssItemEfEntityExpressionFactory expressionFactory;


        ////////////////////////////////////////////////////////////////////////////////////////////


        public RssItemRepository(RssDbContext rssDbContext, ICache<Uri, RssItemEfEntity> persistenceCache,
            IDraftCacheWriter draftCacheWriter, RssItemEfEntityMapper efEntityMapper, IImageFileReader imageFileReader, 
            IRssItemEfEntityFactory rssItemEfEntityFactory, RssItemEfEntityExpressionFactory expressionFactory) : base(rssDbContext)
        {
            /// شی RssItemRepositoryCacheCoordinator ، نیاز به تزریق نداره . چون هم سنگین نیست که خیلی لازم به تست باشه و -
            /// هم برای مدیریت طول عمر draftCache ، که به ازای هر شی RssItemRepository ، باید یک شی draftCache داشته باشه ، نباید تزریق بشه .
            this.rssItemCacheCoordinator = new RssItemRepositoryCacheCoordinator(persistenceCache, draftCacheWriter);
            this.efEntityMapper = efEntityMapper;
            this.imageFileReader = imageFileReader;
            this.rssItemEfEntityFactory = rssItemEfEntityFactory;
            this.expressionFactory = expressionFactory;

            rssDbContext.SavedChanges += this.DbContext_SavedChanges;
            rssDbContext.Disposed += this.DbContext_Disposed;
        }


        ////////////////////////////////////////////////////////////////////////////////////////////


        public void Add(RssItem rssItem, ImageSaveOptions imageSaveOptions)
        {
            RssItemEfEntity rssItemEfEntity = this.rssItemEfEntityFactory.CreateRssItemEfEntity(rssItem, imageSaveOptions);
            this.AddEntity(rssItemEfEntity);
            // اضافه یا به روزرسانی در کش موقت
            this.rssItemCacheCoordinator.MarkAsAddedInDraftCache(rssItem.Id, rssItemEfEntity);
        }


        public void Update(RssItem rssItem, ImageSaveOptions imageSaveOptions)
        {
            RssItemEfEntity updatedRssItemEfEntity = this.rssItemEfEntityFactory.CreateRssItemEfEntity(rssItem, imageSaveOptions);
            RssItemEfEntity oldRssItemEfEntity = this.FindRssItemEfEntityByKey(rssItem.Id.AbsoluteUri);
            this.AttachEntityToDbContext(oldRssItemEfEntity);
            /// به روزرسانی
            this.efEntityMapper.UpdateRssItemEfEntity(updatedRssItemEfEntity, oldRssItemEfEntity);
            // اضافه یا به روزرسانی در کش موقت
            this.rssItemCacheCoordinator.MarkAsModifiedInDraftCache(rssItem.Id, updatedRssItemEfEntity);
        }


        public void Delete(RssItem rssItem)
        {
            RssItemEfEntity rssItemEfEntity = this.FindRssItemEfEntityByKey(rssItem.Id.AbsoluteUri);
            this.DeleteEntity(rssItemEfEntity);
            // اضافه یا به روزرسانی در کش موقت
            this.rssItemCacheCoordinator.MarkAsDeletedInDraftCache(rssItem.Id, rssItemEfEntity);
        }


        public RssItem FindByKey(string rssItemKey)
        {
            RssItemEfEntity rssItemEfEntity = this.FindRssItemEfEntityByKey(rssItemKey);
            return this.ReconstructionRssItem(rssItemEfEntity);
        }


        public IEnumerable<RssItem> FindByFeedId(string rssFeedId)
        {
            List<RssItemEfEntity> rssItemEfEntities = this.FindByAttributeEntity<RssItemEfEntity>(
                this.expressionFactory.ExpressionByFeedId(rssFeedId))?.
                ToList();
            if (rssItemEfEntities == null || rssItemEfEntities.Count < 1)
                return null;
            return this.ReconstructionRssItems(rssItemEfEntities);
        }


        ////////////////////////////////////////////////////////////////////////////////////////////


        private void DbContext_SavedChanges(object sender, SavedChangesEventArgs e)
        {
            this.rssItemCacheCoordinator.CommitDraftCacheToPersistenceCache();
        }


        private void DbContext_Disposed(object? sender, EventArgs e)
        {
            DbContext dbContext = sender as DbContext;
            if (dbContext != null)
            {
                dbContext.SavedChanges -= this.DbContext_SavedChanges;
            }

            IDisposed disposed = sender as IDisposed;
            if(disposed != null)
            {
                disposed.Disposed -= this.DbContext_Disposed;
            }
        }


        private RssItemEfEntity FindRssItemEfEntityByKey(string rssItemKey)
        {
            return this.FindByKeyEntity<string, RssItemEfEntity>(rssItemKey);
        }


        private IEnumerable<RssItem> ReconstructionRssItems(IEnumerable<RssItemEfEntity> rssItemEfEntities)
        {
            List<RssItem> rssItems = new List<RssItem>();
            foreach (RssItemEfEntity rssItemEfEntity in rssItemEfEntities)
            {
                RssItem rssItem = this.ReconstructionRssItem(rssItemEfEntity);
                if (rssItem != null)
                    rssItems.Add(rssItem);
            }
            return (rssItems.Count > 0) ? rssItems : null;
        }


        private RssItem ReconstructionRssItem(RssItemEfEntity rssItemEfEntity)
        {
            byte[,,] imageData = this.imageFileReader.Read8BitImageFromFile(rssItemEfEntity.ImageFullFilePath);
            return this.efEntityMapper.ToRssItem(rssItemEfEntity, imageData);
        }

    }
}
