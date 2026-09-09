using Application.InfrastructureContracts.CacheProject.AppDraftCacheManager.DraftCacheWriter.Interface;
using Application.InfrastructureContracts.CacheProject.AppDraftCacheManager.RssItemDraftCacheManager.Interface;
using Application.InfrastructureContracts.CacheProject.CacheBase.Cache.Interfaces;
using Application.InfrastructureContracts.CacheProject.CacheBase.DraftCache.Interfaces;
using Application.InfrastructureContracts.DataAccessProject.AppQuery.RssFeedXmlParsingConfigQuery.Interface;
using Application.InfrastructureContracts.DataAccessProject.AppQuery.RssItemDtoQuery.Factory.Interface;
using Application.InfrastructureContracts.DataAccessProject.AppQuery.RssSiteQuery.Factory.Interface;
using Application.InfrastructureContracts.DataAccessProject.AppRepository.RssItemRepository.Factory.Interfaces;
using Application.InfrastructureContracts.DataAccessProject.UnitOfWorkFactory.Interfaces;
using Application.InfrastructureContracts.HttpProject.HttpRequest.Factory.Interfaces;
using Application.InfrastructureContracts.ImageProject.ImageDataExtractor.Interfaces;
using Application.InfrastructureContracts.ImageProject.ImageFileReader.Interfaces;
using Application.InfrastructureContracts.ImageProject.ImageFileSaver.Interfaces;
using Application.InfrastructureContracts.SharedProject.FileManager.Interface;
using Application.InfrastructureContracts.XmlProject.SharedDataModels.Class;
using Application.OverallDataModels.OverallDto.EntityDto;
using Application.Services.ImageNormalizerServices._1_ImageChannelCountNormalizerService.Factory.Class;
using Application.Services.ImageNormalizerServices._1_ImageChannelCountNormalizerService.Factory.Interface;
using Application.Services.ImageNormalizerServices._2_ImageChannelSequenceNormalizerService.Factory.Class;
using Application.Services.ImageNormalizerServices._2_ImageChannelSequenceNormalizerService.Factory.Interface;
using Application.Services.Mapper;
using Domain.DomainEntities.RssEntity.Class;
using Infra.CacheService.AppDraftCacheManagerService.DraftCacheWriterService.Class;
using Infra.CacheService.AppDraftCacheManagerService.RssItemDraftCacheManagerService.Class;
using Infra.CacheService.CacheBaseServices.CacheServices.Class;
using Infra.CacheService.CacheBaseServices.DraftCacheServices.Class;
using Infra.DataAccess.AppQueryService.RssFeedXmlParsingConfigQueryService.Class;
using Infra.DataAccess.AppQueryService.RssItemDtoQueryService.Factory.Class;
using Infra.DataAccess.AppQueryService.RssSiteQueryService.Class.Factory;
using Infra.DataAccess.AppRepositoryService.RssItemRepositoryService.Factory.Class;
using Infra.DataAccess.DbContextService.Class;
using Infra.DataAccess.EfEntities.Class;
using Infra.DataAccess.EfEntityMapperService.Class;
using Infra.DataAccess.ExpressionFactoryService.Class;
using Infra.DataAccess.RssItemDtoEfEntityFactoryService.Class;
using Infra.DataAccess.RssItemDtoEfEntityFactoryService.Interface;
using Infra.DataAccess.UnitOfWorkFactoryService.Class;
using Infra.HttpService.HttpRequestService.Factory.Class;
using Infra.ImageService.ImageDataExtractorService.Class;
using Infra.ImageService.ImageFileReaderService.Class;
using Infra.ImageService.ImageFileSaverService.Class;
using Infra.ImageService.InfraImageCreatorService.Class;
using Infra.ImageService.MappingService.Class;
using Infra.SharedService.FileManagerService.Class;
using Infra.SharedService.FilePathMakerService.Class;
using Infra.SharedService.MapperService.ImageSettingsMapperService.Class;
using Shared.ImageServices.ImageUtilityService.Class;

namespace CompositionRoot.Shared.ClassLibrary.StartupModules.SingletonProviderModule.Class
{
    public class SingletonProvider
    {

        private IRssItemDraftCacheManager _rssItemDraftCacheManager;

        private ICache<Uri, RssItemEfEntity> _persistenceCache;

        private ICache<Uri, RssFeedConfig> _rssFeedConfigCache;

        private IRssItemRepositoryFactory _rssItemRepositoryFactory;

        private IRssItemDtoQueryFactory _rssItemDtoQueryFactory;

        private IRssSiteQueryFactory _rssSiteQueryFactory;

        private IUnitOfWorkFactory _unitOfWorkFactory;

        private IHttpRequestFactory _httpRequestFactory;

        private IImageChannelCountNormalizerFactory<byte> _imageChannelCountNormalizerFactory;

        private IImageChannelSequenceNormalizerFactory<byte> _imageChannelSequenceNormalizerFactory;


        ////////////////////////////////////////////////////////////////////////


        public IRssItemDraftCacheManager RssItemDraftCacheManager
        {
            get => this._rssItemDraftCacheManager;
            private set
            {
                if(this._rssItemDraftCacheManager == null)
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

        public ICache<Uri, RssFeedConfig> RssFeedConfigCache
        {
            get => this._rssFeedConfigCache;
            private set
            {
                if (this._rssFeedConfigCache == null)
                    this._rssFeedConfigCache = value;
            }
        }

        /// Factory ها
        public IRssItemRepositoryFactory RssItemRepositoryFactory
        {
            get => this._rssItemRepositoryFactory;
            private set
            {
                if (this._rssItemRepositoryFactory == null)
                    this._rssItemRepositoryFactory = value;
            }
        }

        public IRssItemDtoQueryFactory RssItemDtoQueryFactory
        {
            get => this._rssItemDtoQueryFactory;
            private set
            {
                if (this._rssItemDtoQueryFactory == null)
                    this._rssItemDtoQueryFactory = value;
            }
        }

        public IRssSiteQueryFactory RssSiteQueryFactory
        {
            get => this._rssSiteQueryFactory;
            private set
            {
                if (this._rssSiteQueryFactory == null)
                    this._rssSiteQueryFactory = value;
            }
        }

        public IUnitOfWorkFactory UnitOfWorkFactory
        {
            get => this._unitOfWorkFactory;
            private set
            {
                if (this._unitOfWorkFactory == null)
                    this._unitOfWorkFactory = value;
            }
        }

        public IHttpRequestFactory HttpRequestFactory
        {
            get => this._httpRequestFactory;
            private set
            {
                if (this._httpRequestFactory == null)
                    this._httpRequestFactory = value;
            }
        }

        public IImageChannelCountNormalizerFactory<byte> ImageChannelCountNormalizerFactory
        {
            get => this._imageChannelCountNormalizerFactory;
            private set
            {
                if (_imageChannelCountNormalizerFactory == null)
                    this._imageChannelCountNormalizerFactory = value;
            }
        }

        public IImageChannelSequenceNormalizerFactory<byte> ImageChannelSequenceNormalizerFactory
        {
            get => this._imageChannelSequenceNormalizerFactory;
            private set
            {
                if (this._imageChannelSequenceNormalizerFactory == null)
                    this._imageChannelSequenceNormalizerFactory = value;
            }
        }


        ////////////////////////////////////////////////////////////////////////


        private readonly string connectionString;

        private bool isInitialized;


        ////////////////////////////////////////////////////////////////////////


        public SingletonProvider(string connectionString)
        {
            this.connectionString = connectionString;
            this.isInitialized = false;
        }


        ////////////////////////////////////////////////////////////////////////


        public async Task InitializeComponentsAsync()
        {
            if (this.isInitialized == true)
                return;
            this.isInitialized = true;
            ///
            this.RssItemDraftCacheManager = this.GetRssItemDraftCacheManager();
            this.PersistenceCache = this.GetPersistenceCache();
            this.RssFeedConfigCache = await Task.Run(() => this.GetRssFeedConfigCache(connectionString));
            ///
            this.RssItemRepositoryFactory = this.GetRssItemRepositoryFactory();
            this.RssItemDtoQueryFactory = this.GetRssItemDtoQueryFactory();
            this.RssSiteQueryFactory = this.GetRssSiteQueryFactory();
            this.UnitOfWorkFactory = this.GetUnitOfWorkFactory();
            this.HttpRequestFactory = this.GetHttpRequestFactory();
            this.ImageChannelCountNormalizerFactory = this.GetImageChannelCountNormalizerFactory();
            this.ImageChannelSequenceNormalizerFactory = this.GetImageChannelSequenceNormalizerFactory();
        }


        ////////////////////////////////////////////////////////////////////////


        private IRssItemDraftCacheManager GetRssItemDraftCacheManager()
        {
            IDraftCache<Uri, RssItem> rssItemDraftCache = new DraftCache<Uri, RssItem>();
            IDraftCache<Uri, RssItemDto> rssItemDtoDraftCache = new DraftCache<Uri, RssItemDto>();
            IDraftCacheWriter draftCacheWriter = new DraftCacheWriter();
            IFileManager fileManager = new FileManager();
            return new RssItemDraftCacheManager(rssItemDraftCache, rssItemDtoDraftCache, draftCacheWriter, fileManager);
        }


        private ICache<Uri, RssItemEfEntity> GetPersistenceCache()
        {
            return new Cache<Uri, RssItemEfEntity>();
        }


        private ICache<Uri, RssFeedConfig> GetRssFeedConfigCache(string connectionString)
        {
            RssDbContext rssDbContext = new RssDbContext(connectionString);
            RssFeedConfigEfEntityMapper configEfEntityMapper = new RssFeedConfigEfEntityMapper();
            using IRssFeedConfigQuery rssFeedXmlParsingConfigQuery = new RssFeedConfigQuery(rssDbContext, configEfEntityMapper);
            IReadOnlyDictionary<Uri, RssFeedConfig> rssFeedConfigDictionary = rssFeedXmlParsingConfigQuery.FindAll();
            return new Cache<Uri, RssFeedConfig>(rssFeedConfigDictionary);
        }


        private IRssItemRepositoryFactory GetRssItemRepositoryFactory()
        {
            IDraftCacheWriter draftCacheWriter = new DraftCacheWriter();
            IImageDataExtractor imageDataExtractor = new ImageDataExtractor(new InfraImageCreator(), new ImageInfoMemberMapper());
            IImageFileReader imageFileReader = new ImageFileReader(imageDataExtractor);
            IImageFileSaver imageFileSaver = new ImageFileSaver(new ImageUtility(), new ImageFileSettingsMemberMapper(new ImageInfoMemberMapper()));
            IImageFileSaverCoordinator imageFileSaverCoordinator = new ImageFileSaverCoordinator(
                imageFileSaver, new FilePathMaker(), new ImageSettingsMapper());
            RssItemEfEntityMapper rssItemEfEntityMapper = new RssItemEfEntityMapper();
            IRssItemEfEntityFactory rssItemEfEntityFactory = new RssItemDtoEfEntityFactory(
                imageFileSaverCoordinator, new RssItemDtoMapper(), new RssItemEfEntityMapper());
            RssItemEfEntityExpressionFactory expressionFactory = new RssItemEfEntityExpressionFactory();

            return new RssItemRepositoryFactory(rssItemEfEntityMapper, this.PersistenceCache, draftCacheWriter,
                imageFileReader, rssItemEfEntityFactory, expressionFactory);
        }


        private IRssItemDtoQueryFactory GetRssItemDtoQueryFactory()
        {
            RssItemDtoEfEntityMapper rssItemDtoEfEntityMapper = new RssItemDtoEfEntityMapper();
            RssItemEfEntityExpressionFactory expressionFactory = new RssItemEfEntityExpressionFactory();
            return new RssItemDtoQueryFactory(this.connectionString, this.PersistenceCache, rssItemDtoEfEntityMapper, expressionFactory);
        }


        private IRssSiteQueryFactory GetRssSiteQueryFactory()
        {
            RssSiteEfEntityMapper rssSiteEfEntityMapper = new RssSiteEfEntityMapper(new RssFeedEfEntityMapper());
            return new RssSiteQueryFactory(this.connectionString, rssSiteEfEntityMapper);
        }


        private IUnitOfWorkFactory GetUnitOfWorkFactory()
        {
            return new UnitOfWorkFactory(this.connectionString);
        }


        private IHttpRequestFactory GetHttpRequestFactory()
        {
            return new HttpRequestFactory();
        }


        private IImageChannelCountNormalizerFactory<byte> GetImageChannelCountNormalizerFactory()
        {
            return new ImageChannelCountNormalizerFactory();
        }


        private IImageChannelSequenceNormalizerFactory<byte> GetImageChannelSequenceNormalizerFactory()
        {
            return new ImageChannelSequenceNormalizerFactory();
        }

    }
}
