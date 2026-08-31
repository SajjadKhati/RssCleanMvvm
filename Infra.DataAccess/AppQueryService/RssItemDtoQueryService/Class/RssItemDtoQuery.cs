using Application.InfrastructureContracts.CacheProject.CacheBase.Cache.Interfaces;
using Application.InfrastructureContracts.DataAccessProject.AppQuery.RssItemDtoQuery.Interface;
using Application.OverallDataModels.OverallDto.EntityDto;
using Infra.DataAccess.DataAccessBaseServices.RepositoryQueryBaseService.Class;
using Infra.DataAccess.DbContextService.Class;
using Infra.DataAccess.EfEntities.Class;
using Infra.DataAccess.EfEntityMapperService.Class;
using Infra.DataAccess.ExpressionFactoryService.Class;

namespace Infra.DataAccess.AppQueryService.RssItemDtoQueryService.Class
{
    public class RssItemDtoQuery : DbContextOwningQueryBase, IRssItemDtoQuery
    {

        private ICache<Uri, RssItemEfEntity> persistenceCache;

        private RssItemDtoEfEntityMapper rssItemDtoMapper;

        private RssItemEfEntityExpressionFactory expressionFactory;


        ////////////////////////////////////////////////////////////////////////////////////////////


        public RssItemDtoQuery(RssDbContext dbContext, ICache<Uri, RssItemEfEntity> persistenceCache, 
            RssItemDtoEfEntityMapper rssItemDtoMapper, RssItemEfEntityExpressionFactory expressionFactory) : base(dbContext)
        {
            this.persistenceCache = persistenceCache;
            this.rssItemDtoMapper = rssItemDtoMapper;
            this.expressionFactory = expressionFactory;
        }


        ////////////////////////////////////////////////////////////////////////////////////////////


        public bool IsRssItemExist(string rssItemId)
        {
            // لازم به جستجو در کش نیست .
            return this.FindByKeyEntity<string, RssItemEfEntity>(rssItemId) != null;
        }


        public RssItemDto FindByKey(string rssItemId)
        {
            RssItemDto rssItemDto = this.GetRssItemDtoFromCache(rssItemId);
            return (rssItemDto != null)? rssItemDto : this.GetRssItemDtoFromDatabase(rssItemId);
        }


        public IEnumerable<RssItemDto> FindByFeedId(string rssFeedId)
        {
            IList<RssItemDto> cachedRssItemDtos = this.GetRssItemDtosByFeedIdFromCache(rssFeedId) ?? new List<RssItemDto>();
            IList<RssItemDto> databaseRssItemDtos = this.GetUncachedRssItemDtosByFeedIdFromDatabase(rssFeedId) ?? new List<RssItemDto>();
            return cachedRssItemDtos.Concat(databaseRssItemDtos)?.ToList();
        }


        ////////////////////////////////////////////////////////////////////////////////////////////


        private RssItemDto GetRssItemDtoFromCache(string rssItemId)
        {
            Uri rssItemUriKey = new Uri(rssItemId);
            if (this.persistenceCache.ContainsKey(rssItemUriKey) == true)
            {
                RssItemEfEntity rssItemEfEntity = this.persistenceCache.Get(rssItemUriKey);
                return this.rssItemDtoMapper.ToRssItemDto(rssItemEfEntity);
            }
            return null;
        }


        private IList<RssItemDto> GetRssItemDtosByFeedIdFromCache(string rssFeedId)
        {
            return this.persistenceCache.GetAll()?.
                Where(this.expressionFactory.FuncByFeedId(rssFeedId))?.
                Select(rssItemEfEntity => this.rssItemDtoMapper.ToRssItemDto(rssItemEfEntity))?.
                ToList();
        }


        private RssItemDto GetRssItemDtoFromDatabase(string rssItemId)
        {
            RssItemEfEntity rssItemEfEntity = this.FindByKeyEntity<string, RssItemEfEntity>(rssItemId);
            if (rssItemEfEntity == null)
                return null;
            return this.rssItemDtoMapper.ToRssItemDto(rssItemEfEntity);
        }


        private IList<RssItemDto> GetUncachedRssItemDtosByFeedIdFromDatabase(string rssFeedId)
        {
            IList<string> cacheKeys = this.GetCacheKeysByFeedId(rssFeedId);

            IQueryable<RssItemEfEntity> rssItemEfEntityQuery = this.FindByAttributeEntity<RssItemEfEntity>(
                this.expressionFactory.ExpressionByFeedId(rssFeedId));

            if (cacheKeys != null && cacheKeys.Count > 0)
            {
                rssItemEfEntityQuery = rssItemEfEntityQuery.Where(rssItemEfEntity => cacheKeys.Contains(rssItemEfEntity.Id) == false);
            }

            return rssItemEfEntityQuery
                .ToList()
                .Select(e => this.rssItemDtoMapper.ToRssItemDto(e))
                .ToList();
        }


        private IList<string> GetCacheKeysByFeedId(string rssFeedId)
        {
            return this.persistenceCache.GetAllKeyValuesSnapshot()?.
                Where(keyValue => keyValue.Value.RssFeedEfEntityId == rssFeedId)?.
                Select(keyValue => keyValue.Value.Id)?.
                ToList();
        }

    }
}
