using Application.InfrastructureContracts.SharedProject.DraftCacheStateMarker.Interface;
using Infra.DataAccess.EfEntities.Class;

namespace Infra.DataAccess.AppCacheManagerService.RssItemRepositoryCacheCoordinatorService.InternalInterface
{
    internal interface IRssItemRepositoryCacheCoordinator : IDraftCacheStateMarker<Uri, RssItemEfEntity>
    {
        void CommitDraftCacheToPersistenceCache();
    }
}
