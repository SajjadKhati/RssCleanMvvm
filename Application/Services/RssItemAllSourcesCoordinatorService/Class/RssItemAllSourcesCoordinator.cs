using Domain.ApplicationServiceContracts.RssItemAllSourcesCoordinator.Interface;
using Application.InfrastructureContracts.CacheProject.AppDraftCacheManager.RssItemDraftCacheManager.Interface;
using Application.InfrastructureContracts.DataAccessProject.AppQuery.RssItemDtoQuery.Interface;

namespace Application.Services.RssItemAllSourcesCoordinatorService.Class
{
    public class RssItemAllSourcesCoordinator : IRssItemExistenceChecker
    {

        private IRssItemDraftCacheManager rssItemDraftCacheManager;

        private IRssItemDtoQuery rssItemDtoQuery;


        //////////////////////////////////////////////////////////////////

        public RssItemAllSourcesCoordinator(IRssItemDraftCacheManager rssItemDraftCacheManager, IRssItemDtoQuery rssItemDtoQuery)
        {
            this.rssItemDraftCacheManager = rssItemDraftCacheManager;
            this.rssItemDtoQuery = rssItemDtoQuery;
        }


        //////////////////////////////////////////////////////////////////


        public bool IsRssItemExist(Uri rssItemId)
        {
            bool isRssItemExistInDraftCache = this.rssItemDraftCacheManager.ContainKey(rssItemId);
            if(isRssItemExistInDraftCache == true)
                return true;
            bool isRssItemExistInDatabase = this.rssItemDtoQuery.IsRssItemExist(rssItemId.AbsoluteUri);
            if(isRssItemExistInDatabase == true) 
                return true;
            return false;
        }

    }
}
