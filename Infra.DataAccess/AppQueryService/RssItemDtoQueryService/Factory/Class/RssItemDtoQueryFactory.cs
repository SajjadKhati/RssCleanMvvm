using Application.InfrastructureContracts.CacheProject.CacheBase.Cache.Interfaces;
using Application.InfrastructureContracts.DataAccessProject.AppQuery.RssItemDtoQuery.Factory.Interface;
using Application.InfrastructureContracts.DataAccessProject.AppQuery.RssItemDtoQuery.Interface;
using Infra.DataAccess.AppQueryService.RssItemDtoQueryService.Class;
using Infra.DataAccess.DbContextService.Class;
using Infra.DataAccess.EfEntities.Class;
using Infra.DataAccess.EfEntityMapperService.Class;

namespace Infra.DataAccess.AppQueryService.RssItemDtoQueryService.Factory.Class
{
    public class RssItemDtoQueryFactory : IRssItemDtoQueryFactory
    {

        private string connectionString;

        private ICache<Uri, RssItemEfEntity> persistenceCache;

        private RssItemDtoMapper rssItemDtoMapper;


        /////////////////////////////////////////////////////////////////


        public RssItemDtoQueryFactory(string connectionString, ICache<Uri, RssItemEfEntity> persistenceCache, RssItemDtoMapper rssItemDtoMapper)
        {
            this.connectionString = connectionString;
            this.persistenceCache = persistenceCache;
            this.rssItemDtoMapper = rssItemDtoMapper;
        }


        /////////////////////////////////////////////////////////////////


        /// دقت کنید که این کلاس چون شی جدیدی از RssDbContext میسازه ، حتما لازم نیست که مسئول Dispose اش ، خودش باشه .
        /// در شرایط خاص مثل UnitOfWork و Query ، ما میتونیم مالکیتِ Disposable کردن را به این سرویس ها تغییر بدیم .
        /// یعنی سرویس Factory شون ، فقط شی DbContext را بسازه اما این سرویس ها ، مسئول Dispose کردنش باشند .
        public IRssItemDtoQuery Create()
        {
            return new RssItemDtoQuery(new RssDbContext(this.connectionString), this.persistenceCache, this.rssItemDtoMapper);
        }

    }
}
