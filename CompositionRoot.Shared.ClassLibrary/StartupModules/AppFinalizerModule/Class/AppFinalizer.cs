using Application.InfrastructureContracts.CacheProject.AppDraftCacheManager.RssItemDraftCacheManager.Interface;

namespace CompositionRoot.Shared.ClassLibrary.StartupModules.AppFinalizerModule.Class
{
    public class AppFinalizer : IDisposable
    {

        private IRssItemDraftCacheManager rssItemDraftCacheManager;


        ////////////////////////////////////////////////////////////////////////


        public AppFinalizer(IRssItemDraftCacheManager rssItemDraftCacheManager)
        {
            this.rssItemDraftCacheManager = rssItemDraftCacheManager;
        }


        ////////////////////////////////////////////////////////////////////////


        public void Dispose()
        {
            this.rssItemDraftCacheManager.Dispose();
        }

    }
}
