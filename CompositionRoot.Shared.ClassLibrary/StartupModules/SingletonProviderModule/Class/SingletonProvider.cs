using Application.InfrastructureContracts.CacheProject.AppDraftCacheManager.DraftCacheWriter.Interface;
using Application.InfrastructureContracts.CacheProject.AppDraftCacheManager.RssItemDraftCacheManager.Interface;
using Application.InfrastructureContracts.CacheProject.CacheBase.Cache.Interfaces;
using Application.InfrastructureContracts.CacheProject.CacheBase.DraftCache.Interfaces;
using Application.InfrastructureContracts.DataAccessProject.AppQuery.RssFeedXmlParsingConfigQuery.Interface;
using Application.InfrastructureContracts.SharedProject.FileManager.Interface;
using Application.InfrastructureContracts.SharedProject.FolderManager.Interface;
using Application.InfrastructureContracts.XmlProject.SharedDataModels.Class;
using Application.OverallDataModels.OverallDto.EntityDto;
using CompositionRoot.Shared.ClassLibrary.StartupModules.AppPathsModule.Class;
using Domain.DomainEntities.RssEntity.Class;
using Infra.CacheService.AppDraftCacheManagerService.DraftCacheWriterService.Class;
using Infra.CacheService.AppDraftCacheManagerService.RssItemDraftCacheManagerService.Class;
using Infra.CacheService.CacheBaseServices.CacheServices.Class;
using Infra.CacheService.CacheBaseServices.DraftCacheServices.Class;
using Infra.DataAccess.AppQueryService.RssFeedXmlParsingConfigQueryService.Class;
using Infra.DataAccess.DbContextService.Class;
using Infra.DataAccess.EfEntities.Class;
using Infra.DataAccess.EfEntityMapperService.Class;
using Infra.SharedService.FileManagerService.Class;
using Infra.SharedService.FolderManagerService.Class;

namespace CompositionRoot.Shared.ClassLibrary.StartupModules.SingletonProviderModule.Class
{
    public class SingletonProvider
    {

        private readonly string connectionString;

        private readonly AppPaths appPaths;

        private bool isInitialized;


        ///////////////////////////////////////////////////////////////////////


        private RssItemDraftCacheManager _rssItemDraftCacheManager;

        private ICache<Uri, RssItemEfEntity> _persistenceCache;

        private ReaderWriterLockSlim _sharedPersistenceCacheLock;

        private ICache<Uri, RssFeedConfig> _rssFeedConfigCache;


        ///////////////////////////////////////////////////////////////////////


        public RssItemDraftCacheManager RssItemDraftCacheManager
        {
            get => this._rssItemDraftCacheManager;
            private set
            {
                if (this._rssItemDraftCacheManager == null)
                    this._rssItemDraftCacheManager = value;
            }
        }

        public ICache<Uri, RssItemEfEntity> PersistenceCache
        {
            get => this._persistenceCache;
            private set
            {
                if (this._persistenceCache == null)
                    this._persistenceCache = value;
            }
        }

        public ReaderWriterLockSlim SharedPersistenceCacheLock
        {
            get => this._sharedPersistenceCacheLock;
            private set
            {
                if(this._sharedPersistenceCacheLock == null)
                    this._sharedPersistenceCacheLock = value;
            }
        }

        public ICache<Uri, RssFeedConfig> RssFeedConfigCache
        {
            get => this._rssFeedConfigCache;
            private set
            {
                if (this._rssFeedConfigCache == null)
                    this._rssFeedConfigCache = value;
            }
        }


        ///////////////////////////////////////////////////////////////////////


        public SingletonProvider(string connectionString, AppPaths appPaths)
        {
            this.connectionString = connectionString;
            this.appPaths = appPaths;
            this.isInitialized = false;
        }


        ///////////////////////////////////////////////////////////////////////


        public async Task InitializeSingletonPropertiesAsync()
        {
            if (this.isInitialized == true)
                return;
            this.isInitialized = true;

            this.RssItemDraftCacheManager = this.CreateRssItemDraftCacheManager();
            this.PersistenceCache = this.CreatePersistenceCache();
            this.SharedPersistenceCacheLock = CreateSharedPersistenceCacheLock();
            this.RssFeedConfigCache = await Task.Run(() => this.CreateRssFeedConfigCache(this.connectionString));
        }


        private RssItemDraftCacheManager CreateRssItemDraftCacheManager()
        {
            IDraftCacheWriter draftCacheWriter = new DraftCacheWriter();
            IFileManager fileManager = new FileManager();
            IFolderManager folderManager = new FolderManager();
            return new RssItemDraftCacheManager(draftCacheWriter, fileManager,
                folderManager, this.appPaths);
        }


        private ICache<Uri, RssItemEfEntity> CreatePersistenceCache()
        {
            return new Cache<Uri, RssItemEfEntity>();
        }


        private ReaderWriterLockSlim CreateSharedPersistenceCacheLock()
        {
            return new ReaderWriterLockSlim();
        }


        private ICache<Uri, RssFeedConfig> CreateRssFeedConfigCache(string connectionString)
        {
            RssDbContext rssDbContext = new RssDbContext(connectionString);
            RssFeedConfigEfEntityMapper configEfEntityMapper = new RssFeedConfigEfEntityMapper();
            using IRssFeedConfigQuery rssFeedXmlParsingConfigQuery = new RssFeedConfigQuery(rssDbContext, configEfEntityMapper);
            IReadOnlyDictionary<Uri, RssFeedConfig> rssFeedConfigDictionary = rssFeedXmlParsingConfigQuery.FindAll();
            return new Cache<Uri, RssFeedConfig>(rssFeedConfigDictionary);
        }

    }
}