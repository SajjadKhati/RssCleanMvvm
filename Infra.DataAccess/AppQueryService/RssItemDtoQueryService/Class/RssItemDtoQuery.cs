using Application.InfrastructureContracts.CacheProject.CacheBase.Cache.Interfaces;
using Application.InfrastructureContracts.DataAccessProject.AppQuery.RssItemDtoQuery.Interface;
using Application.OverallDataModels.OverallDto.EntityDto;
using Infra.DataAccess.DataAccessBaseServices.RepositoryQueryBaseService.Class;
using Infra.DataAccess.DbContextService.Class;
using Infra.DataAccess.EfEntities.Class;
using Infra.DataAccess.EfEntityMapperService.Class;

namespace Infra.DataAccess.AppQueryService.RssItemDtoQueryService.Class
{
    public class RssItemDtoQuery : DbContextOwningQueryBase, IRssItemDtoQuery
    {

        private ICache<Uri, RssItemEfEntity> persistenceCache;

        private RssItemDtoMapper rssItemDtoMapper;


        ////////////////////////////////////////////////////////////////////////////////////////////


        public RssItemDtoQuery(RssDbContext dbContext, ICache<Uri, RssItemEfEntity> persistenceCache, RssItemDtoMapper rssItemDtoMapper) 
            : base(dbContext)
        {
            this.persistenceCache = persistenceCache;
            this.rssItemDtoMapper = rssItemDtoMapper;
        }


        ////////////////////////////////////////////////////////////////////////////////////////////


        public RssItemDto FindByKey(string key)
        {
            RssItemDto rssItemDto = this.GetRssItemDtoFromCache(key);
            return (rssItemDto != null)? rssItemDto : this.GetRssItemDtoFromDatabase(key);
        }


        public IEnumerable<RssItemDto> FindAll()
        {
            IList<RssItemDto> cachedRssItemDtos = this.GetAllRssItemDtoFromCache() ?? new List<RssItemDto>();
            var databaseRssItemDtos = this.GetUncachedRssItemDtosFromDatabase() ?? new List<RssItemDto>();
            return cachedRssItemDtos.Concat(databaseRssItemDtos)?.ToList();
        }


        ////////////////////////////////////////////////////////////////////////////////////////////


        private RssItemDto GetRssItemDtoFromCache(string key)
        {
            Uri uriKey = new Uri(key);
            if (this.persistenceCache.ContainsKey(uriKey) == true)
            {
                RssItemEfEntity rssItemEfEntity = this.persistenceCache.Get(uriKey);
                return this.rssItemDtoMapper.ToRssItemDto(rssItemEfEntity);
            }
            return null;
        }


        private IList<RssItemDto> GetAllRssItemDtoFromCache()
        {
            return this.persistenceCache.GetAll()?.Select(rssItemEfEntity => this.rssItemDtoMapper.ToRssItemDto(rssItemEfEntity))?.ToList();
        }


        private RssItemDto GetRssItemDtoFromDatabase(string key)
        {
            RssItemEfEntity rssItemEfEntity = this.FindByKeyEntity<string, RssItemEfEntity>(key);
            if (rssItemEfEntity == null)
                return null;
            return this.rssItemDtoMapper.ToRssItemDto(rssItemEfEntity);
        }


        private IList<RssItemDto> GetUncachedRssItemDtosFromDatabase()
        {
            IList<string> cacheKeys = this.persistenceCache.GetKeys().Select(uri=>uri.AbsoluteUri).ToList();

            return this.FindAllEntity<RssItemEfEntity>()?.
                Where(rssItemEfEntity => cacheKeys.Contains(rssItemEfEntity.Id) == false)?.
                ToList()?.
                Select(rssItemEfEntity => this.rssItemDtoMapper.ToRssItemDto(rssItemEfEntity))?.
                ToList();
        }

    }
}
