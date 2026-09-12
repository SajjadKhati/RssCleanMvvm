using Application.InfrastructureContracts.CacheProject.AppDraftCacheManager.RssItemDraftCacheManager.Interface;
using Application.InfrastructureContracts.CacheProject.CacheBase.Cache.Interfaces;
using Application.InfrastructureContracts.DataAccessProject.AppQuery.RssItemDtoQuery.Factory.Interface;
using Application.InfrastructureContracts.DataAccessProject.AppQuery.RssSiteQuery.Factory.Interface;
using Application.InfrastructureContracts.DataAccessProject.AppRepository.RssItemRepository.DataModels;
using Application.InfrastructureContracts.DataAccessProject.AppRepository.RssItemRepository.Factory.Interfaces;
using Application.InfrastructureContracts.DataAccessProject.RssItemDtoFactory.Interface;
using Application.InfrastructureContracts.DataAccessProject.UnitOfWorkFactory.Interfaces;
using Application.InfrastructureContracts.HttpProject.HttpRequest.Factory.Interfaces;
using Application.InfrastructureContracts.ImageProject.ImageDataExtractor.Interfaces;
using Application.InfrastructureContracts.ImageProject.ImageFileSaver.Interfaces;
using Application.InfrastructureContracts.ImageProject.ImageSettings.ImageSettingsProvider.Interfaces;
using Application.InfrastructureContracts.SharedProject.DateTimes.DateTimeParser.Interface;
using Application.InfrastructureContracts.SharedProject.DateTimes.PersianCalenderConvertor.Interface;
using Application.InfrastructureContracts.SharedProject.FilePathMaker.Interfaces;
using Application.InfrastructureContracts.SharedProject.SharedMapper.RssItemDtoMapper.Interface;
using Application.InfrastructureContracts.XmlProject.SharedDataModels.Class;
using Application.InfrastructureContracts.XmlProject.XmlParser.Interfaces;
using Application.Services.ImageNormalizerServices._0_ImageNormalizerService.Class;
using Application.Services.ImageNormalizerServices._0_ImageNormalizerService.Interface;
using Application.Services.ImageNormalizerServices._1_ImageChannelCountNormalizerService.Factory.Interface;
using Application.Services.ImageNormalizerServices._2_ImageChannelSequenceNormalizerService.Factory.Interface;
using Application.Services.ImageNormalizerServices.ImageInfoValidatorService.Class;
using Application.Services.Mapper;
using Application.Services.RssItemAllSourcesCoordinatorService.Class;
using Application.Services.RssItemAllSourcesCoordinatorService.Interface;
using Application.Services.RssItemCreationServices.RssItemBuilderService.Class;
using Application.Services.RssItemCreationServices.RssItemBuilderService.Interface;
using Application.Services.RssItemCreationServices.RssItemDataExtractorService.Class;
using Application.Services.RssItemCreationServices.RssItemDataExtractorService.Interface;
using Application.Services.RssItemDownloaderIntoDraftCacheService.Class;
using Application.Services.RssItemDownloaderIntoDraftCacheService.Interface;
using Application.Services.RssItemsDownLoaderService.Class;
using Application.Services.RssItemsDownLoaderService.Interface;
using Application.UseCases.DownLoadRssItemsUseCaseModule.Class;
using Application.UseCases.DownLoadRssItemsUseCaseModule.Interface;
using Application.UseCases.LoadRssSiteUseCaseModule.Class;
using Application.UseCases.LoadRssSiteUseCaseModule.Interface;
using CompositionRoot.Shared.ClassLibrary.Services.RssItemViewModelFactoryService.Class;
using CompositionRoot.Shared.ClassLibrary.StartupModules.AppPathsModule.Class;
using Domain.ApplicationServiceContracts.RssItemAllSourcesCoordinator.Interface;
using Domain.DomainService.RssItemDomainService.Class;
using Domain.DomainService.RssItemDomainService.Interface;
using Infra.DataAccess.AppQueryService.RssSiteQueryService.Class.Factory;
using Infra.DataAccess.EfEntities.Class;
using Infra.DataAccess.EfEntityMapperService.Class;
using Infra.DataAccess.RssItemDtoEfEntityFactoryService.Class;
using Infra.DataAccess.RssItemDtoEfEntityFactoryService.Interface;
using Infra.ImageService.ImageDataExtractorService.Class;
using Infra.ImageService.ImageFileSaverService.Class;
using Infra.ImageService.InfraImageCreatorService.Class;
using Infra.ImageService.MappingService.Class;
using Infra.SharedService.DateTimesServices.DateTimeParserService.Class;
using Infra.SharedService.DateTimesServices.PersianCalenderConvertorService.Class;
using Infra.SharedService.FilePathMakerService.Class;
using Infra.SharedService.MapperService.ImageSettingsMapperService.Class;
using Infra.XmlService.XamlParserService.Class;
using Shared.ImageServices.ImageUtilityService.Class;
using Shared.ImageServices.ImageUtilityService.Interface;
using ViewModel.CompositionRootContracts.RssItemViewModelFactory.Interface;
using ViewModel.Services.RssSiteViewModelMapperService.Class;
using ViewModel.ViewModelModules.MainWindowViewModelModule.Class;
using ViewModel.ViewModelModules.RssUserControlViewModelModule.Class;

namespace CompositionRoot.Shared.ClassLibrary.StartupModules.ViewModelCompositionRootModule.Class
{
    public class ViewModelCompositionRoot
    {

        private readonly string connectionString;

        private readonly AppPaths appPaths;

        private readonly IRssItemDraftCacheManager rssItemDraftCacheManager;

        private readonly ICache<Uri, RssItemEfEntity> persistenceCache;

        private readonly ICache<Uri, RssFeedConfig> rssFeedConfigCache;

        private readonly IImageSettingsWriter imageSettingsWriter;

        private readonly IImageSettingsReader imageSettingsReader;

        private readonly IRssItemRepositoryFactory rssItemRepositoryFactory;

        private readonly IRssItemDtoQueryFactory rssItemDtoQueryFactory;

        private readonly IRssSiteQueryFactory rssSiteQueryFactory;

        private readonly IUnitOfWorkFactory unitOfWorkFactory;

        private readonly IHttpRequestFactory httpRequestFactory;

        private readonly IRssItemViewModelFactory rssItemViewModelFactory;

        private readonly IImageChannelCountNormalizerFactory<byte> imageChannelCountNormalizerFactory;

        private readonly IImageChannelSequenceNormalizerFactory<byte> imageChannelSequenceNormalizerFactory;
        /// 
        private readonly IRssItemExistenceChecker rssItemExistenceChecker;

        private readonly IRssItemAllSourcesCoordinator rssItemAllSourcesCoordinator;

        private readonly IRssItemDtoFactory rssItemDtoFactory;

        private readonly IRssItemEfEntityFactory rssItemEfEntityFactory;


        ///////////////////////////////////////////////////////////////////////


        public ViewModelCompositionRoot(
            string connectionString, 
            AppPaths appPaths,
            IRssItemDraftCacheManager rssItemDraftCacheManager,
            ICache<Uri, RssItemEfEntity> persistenceCache, 
            ICache<Uri, RssFeedConfig> rssFeedConfigCache,
            IImageSettingsWriter imageSettingsWriter,
            IImageSettingsReader imageSettingsReader,
            IRssItemRepositoryFactory rssItemRepositoryFactory, 
            IRssItemDtoQueryFactory rssItemDtoQueryFactory,
            IRssSiteQueryFactory rssSiteQueryFactory, 
            IUnitOfWorkFactory unitOfWorkFactory, 
            IHttpRequestFactory httpRequestFactory,
            IRssItemViewModelFactory rssItemViewModelFactory,
            IImageChannelCountNormalizerFactory<byte> imageChannelCountNormalizerFactory,
            IImageChannelSequenceNormalizerFactory<byte> imageChannelSequenceNormalizerFactory)
        {
            this.connectionString = connectionString;
            this.appPaths = appPaths;
            this.rssItemDraftCacheManager = rssItemDraftCacheManager;
            this.persistenceCache = persistenceCache;
            this.rssFeedConfigCache = rssFeedConfigCache;
            this.imageSettingsWriter = imageSettingsWriter;
            this.imageSettingsReader = imageSettingsReader;
            this.rssItemRepositoryFactory = rssItemRepositoryFactory;
            this.rssItemDtoQueryFactory = rssItemDtoQueryFactory;
            this.rssSiteQueryFactory = rssSiteQueryFactory;
            this.unitOfWorkFactory = unitOfWorkFactory;
            this.httpRequestFactory = httpRequestFactory;
            this.rssItemViewModelFactory = rssItemViewModelFactory;
            this.imageChannelCountNormalizerFactory = imageChannelCountNormalizerFactory;
            this.imageChannelSequenceNormalizerFactory = imageChannelSequenceNormalizerFactory;
            this.rssItemExistenceChecker = this.CreateRssItemExistenceChecker();
            this.rssItemAllSourcesCoordinator = (IRssItemAllSourcesCoordinator)this.rssItemExistenceChecker;
            this.rssItemDtoFactory = this.CreateRssItemDtoFactory();
            this.rssItemEfEntityFactory = (IRssItemEfEntityFactory)this.rssItemDtoFactory;
        }


        ///////////////////////////////////////////////////////////////////////


        public MainWindowViewModel CreateMainWindowViewModel()
        {
            RssUserControlViewModel rssUserControlViewModel = this.CreateRssUserControlViewModel();
            MainWindowViewModel mainWindowViewModel = new MainWindowViewModel(rssUserControlViewModel);
            return mainWindowViewModel;
        }


        ///////////////////////////////////////////////////////////////////////


        private RssUserControlViewModel CreateRssUserControlViewModel()
        {
            ILoadRssSiteUseCase loadRssSiteUseCase = this.CreateLoadRssSiteUseCase();
            IDownLoadRssItemsUseCase downLoadRssItemsUseCase = this.CreateDownLoadRssItemsUseCase();
            RssSiteInfoMapper rssSiteInfoMapper = this.CreateRssSiteInfoMapper();
            return new RssUserControlViewModel(this.rssItemViewModelFactory, loadRssSiteUseCase, downLoadRssItemsUseCase, rssSiteInfoMapper);
        }

        private ILoadRssSiteUseCase CreateLoadRssSiteUseCase()
        {
            return new LoadRssSiteUseCase(this.rssSiteQueryFactory);
        }

        private IDownLoadRssItemsUseCase CreateDownLoadRssItemsUseCase()
        {
            IRssItemDownloaderIntoDraftCache rssItemDownloaderIntoDraftCache = this.CreateRssItemDownloaderIntoDraftCache();
            return new DownLoadRssItemsUseCase(rssItemDownloaderIntoDraftCache, this.rssFeedConfigCache);
        }

        private RssSiteInfoMapper CreateRssSiteInfoMapper()
        {
            return new RssSiteInfoMapper();
        }

        /////////////////////////////////////////
        
        private IRssItemDownloaderIntoDraftCache CreateRssItemDownloaderIntoDraftCache()
        {
            //RssFeedEfEntityMapper rssFeedEfEntityMapper = new RssFeedEfEntityMapper();
            //RssSiteEfEntityMapper rssSiteEfEntityMapper = new RssSiteEfEntityMapper(rssFeedEfEntityMapper);
            //IRssSiteQueryFactory rssSiteQueryFactory = new RssSiteQueryFactory(this.connectionString, rssSiteEfEntityMapper);
            //ILoadRssSiteUseCase initializeAppUseCase = new LoadRssSiteUseCase(rssSiteQueryFactory);

            IRssItemsDownLoader rssItemsDownLoader = this.CreateRssItemsDownLoader();
            //IRssItemDtoFactory rssItemDtoFactory = this.CreateRssItemDtoFactory();
            return new RssItemDownloaderIntoDraftCache(rssItemsDownLoader, rssItemDtoFactory, 
                this.rssItemDraftCacheManager, this.imageSettingsReader, this.appPaths.TempImagesFolderPath);
        }

        private IRssItemsDownLoader CreateRssItemsDownLoader()
        {
            IXmlParser xmlParser = this.CreateXmlParser();
            IRssItemBuilder rssItemBuilder = this.CreateRssItemBuilder();
            return new RssItemsDownLoader(this.httpRequestFactory, xmlParser, rssItemBuilder);
        }

        private IXmlParser CreateXmlParser()
        {
            return new XmlParser();
        }

        private IRssItemBuilder CreateRssItemBuilder()
        {
            IRssItemDomainService rssItemDomainService = this.CreateRssItemDomainService();
            IRssItemDataExtractor rssItemDataExtractor = this.CreateRssItemDataExtractor();
            return new RssItemBuilder(rssItemDomainService, rssItemDataExtractor, this.imageSettingsWriter);
        }

        private IRssItemDomainService CreateRssItemDomainService()
        {
            return new RssItemDomainService(this.rssItemExistenceChecker);
        }

        private IRssItemExistenceChecker CreateRssItemExistenceChecker()
        {
            return new RssItemAllSourcesCoordinator(this.rssItemDraftCacheManager, this.rssItemDtoQueryFactory, 
                this.rssItemRepositoryFactory, this.unitOfWorkFactory);
        }

        private IRssItemDataExtractor CreateRssItemDataExtractor()
        {
            IDateTimeParser dateTimeParser = this.CreateDateTimeParser();
            IImageDataExtractor imageDataExtractor = this.CreateImageDataExtractor();
            IImageNormalizer<byte> imageNormalizer = this.CreateImageNormalizer();
            return new RssItemDataExtractor(dateTimeParser, this.httpRequestFactory, imageDataExtractor, imageNormalizer);
        }

        private IDateTimeParser CreateDateTimeParser()
        {
            return new DateTimeParser();
        }

        private IImageDataExtractor CreateImageDataExtractor()
        {
            InfraImageCreator infraImageCreator = this.CreateInfraImageCreator();
            ImageInfoMemberMapper imageInfoMemberMapper = this.CreateImageInfoMemberMapper();
            return new ImageDataExtractor(infraImageCreator, imageInfoMemberMapper);
        }

        private InfraImageCreator CreateInfraImageCreator()
        {
            return new InfraImageCreator();
        }

        private ImageInfoMemberMapper CreateImageInfoMemberMapper()
        {
            return new ImageInfoMemberMapper();
        }

        private IImageNormalizer<byte> CreateImageNormalizer()
        {
            ImageInfoValidator imageInfoValidator = this.CreateImageInfoValidator();
            return new Argb8BitImageNormalizer(imageInfoValidator, this.imageChannelCountNormalizerFactory,
                this.imageChannelSequenceNormalizerFactory);
        }

        private ImageInfoValidator CreateImageInfoValidator()
        {
            return new ImageInfoValidator();
        }

        private IRssItemDtoFactory CreateRssItemDtoFactory()
        {
            IImageFileSaverCoordinator imageFileSaverCoordinator = this.CreateImageFileSaverCoordinator();
            IRssItemDtoMapper rssItemDtoMapper = this.CreateRssItemDtoMapper();
            RssItemEfEntityMapper rssItemEfEntityMapper = this.CreateRssItemEfEntityMapper();
            return new RssItemDtoEfEntityFactory(imageFileSaverCoordinator, rssItemDtoMapper, rssItemEfEntityMapper);
        }

        private IImageFileSaverCoordinator CreateImageFileSaverCoordinator()
        {
            IImageFileSaver imageFileSaver = this.CreateImageFileSaver();
            IFilePathMaker filePathMaker = this.CreateFilePathMaker();
            ImageSettingsMapper imageSettingsMapper = this.CreateImageSettingsMapper();
            return new ImageFileSaverCoordinator(imageFileSaver, filePathMaker, imageSettingsMapper);
        }

        private IImageFileSaver CreateImageFileSaver()
        {
            IImageUtility imageUtility = this.CreateImageUtility();
            ImageFileSettingsMemberMapper imageFileSettingsMemberMapper = this.CreateImageFileSettingsMemberMapper();
            return new ImageFileSaver(imageUtility, imageFileSettingsMemberMapper);
        }

        private IImageUtility CreateImageUtility()
        {
            return new ImageUtility();
        }

        private ImageFileSettingsMemberMapper CreateImageFileSettingsMemberMapper()
        {
            ImageInfoMemberMapper imageInfoMemberMapper = this.CreateImageInfoMemberMapper();
            return new ImageFileSettingsMemberMapper(imageInfoMemberMapper);
        }

        private IFilePathMaker CreateFilePathMaker()
        {
            return new FilePathMaker();
        }

        private ImageSettingsMapper CreateImageSettingsMapper()
        {
            return new ImageSettingsMapper();
        }

        private IRssItemDtoMapper CreateRssItemDtoMapper()
        {
            IPersianCalenderConvertor persianCalenderConvertor = this.CreatePersianCalenderConvertor();
            return new RssItemDtoMapper(persianCalenderConvertor);
        }

        private IPersianCalenderConvertor CreatePersianCalenderConvertor()
        {
            return new PersianCalenderConvertor();
        }

        private RssItemEfEntityMapper CreateRssItemEfEntityMapper()
        {
            return new RssItemEfEntityMapper();
        }













    }
}
