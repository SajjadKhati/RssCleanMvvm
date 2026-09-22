#region فضای نام
using Application.InfrastructureContracts.CacheProject.AppDraftCacheManager.DraftCacheWriter.Interface;
using Application.InfrastructureContracts.CacheProject.AppDraftCacheManager.RssItemDraftCacheManager.Interface;
using Application.InfrastructureContracts.CacheProject.CacheBase.Cache.Interfaces;
using Application.InfrastructureContracts.DataAccessProject.AppQuery.RssItemDtoQuery.Factory.Interface;
using Application.InfrastructureContracts.DataAccessProject.AppQuery.RssSiteQuery.Factory.Interface;
using Application.InfrastructureContracts.DataAccessProject.AppRepository.RssItemRepository.Factory.Interfaces;
using Application.InfrastructureContracts.DataAccessProject.RssItemDtoFactory.Interface;
using Application.InfrastructureContracts.DataAccessProject.UnitOfWorkFactory.Interfaces;
using Application.InfrastructureContracts.HttpProject.HttpRequest.Factory.Interfaces;
using Application.InfrastructureContracts.ImageProject.ImageDataExtractor.Interfaces;
using Application.InfrastructureContracts.ImageProject.ImageFileReader.Interfaces;
using Application.InfrastructureContracts.ImageProject.ImageFileSaver.DataModels.Class;
using Application.InfrastructureContracts.ImageProject.ImageFileSaver.Interfaces;
using Application.InfrastructureContracts.ImageProject.ImageSettings.ImageInfoToImageSettingsConverterService.Interface;
using Application.InfrastructureContracts.ImageProject.ImageSettings.ImageSettingsProvider.Interfaces;
using Application.InfrastructureContracts.ImageProject.SharedDataModels.Class;
using Application.InfrastructureContracts.SharedProject.DateTimes.DateTimeParser.Interface;
using Application.InfrastructureContracts.SharedProject.DateTimes.PersianCalenderConvertor.Interface;
using Application.InfrastructureContracts.SharedProject.FilePathMaker.Interfaces;
using Application.InfrastructureContracts.SharedProject.Hasher.Interface;
using Application.InfrastructureContracts.SharedProject.ImageFileExtensionResolver.Interface;
using Application.InfrastructureContracts.SharedProject.SharedMapper.RssItemDtoMapper.Interface;
using Application.InfrastructureContracts.XmlProject.SharedDataModels.Class;
using Application.InfrastructureContracts.XmlProject.XmlParser.Interfaces;
using Application.Services.ImageNormalizerServices._0_ImageNormalizerService.Class;
using Application.Services.ImageNormalizerServices._0_ImageNormalizerService.Interface;
using Application.Services.ImageNormalizerServices._1_ImageChannelCountNormalizerService.Factory.Class;
using Application.Services.ImageNormalizerServices._1_ImageChannelCountNormalizerService.Factory.Interface;
using Application.Services.ImageNormalizerServices._2_ImageChannelSequenceNormalizerService.Factory.Class;
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
using CompositionRoot.Shared.ClassLibrary.Services.RssFeedViewModelFactoryService.Class;
using CompositionRoot.Shared.ClassLibrary.Services.RssItemViewModelFactoryService.Class;
using CompositionRoot.Shared.ClassLibrary.StartupModules.AppPathsModule.Class;
using Domain.ApplicationServiceContracts.RssItemAllSourcesCoordinator.Interface;
using Domain.DomainService.RssItemDomainService.Class;
using Domain.DomainService.RssItemDomainService.Interface;
using Infra.CacheService.AppDraftCacheManagerService.DraftCacheWriterService.Class;
using Infra.CacheService.CacheBaseServices.CacheServices.Class;
using Infra.DataAccess.AppQueryService.RssItemDtoQueryService.Factory.Class;
using Infra.DataAccess.AppQueryService.RssSiteQueryService.Class.Factory;
using Infra.DataAccess.AppRepositoryService.RssItemRepositoryService.Factory.Class;
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
using Infra.ImageService.ImageSettingsServices.ImageInfoToImageSettingsConverterService.Class;
using Infra.ImageService.ImageSettingsServices.ImageSettingsProviderService.Class;
using Infra.ImageService.InfraImageCreatorService.Class;
using Infra.ImageService.MappingService.Class;
using Infra.SharedService.DateTimesServices.DateTimeParserService.Class;
using Infra.SharedService.DateTimesServices.PersianCalenderConvertorService.Class;
using Infra.SharedService.FilePathMakerService.Class;
using Infra.SharedService.ImageFileExtensionResolverService.Class;
using Infra.SharedService.MapperService.ImageSettingsMapperService.Class;
using Infra.SharedService.StringHasherService.Class;
using Infra.XmlService.XamlParserService.Class;
using Shared.ImageServices.ImageUtilityService.Class;
using Shared.ImageServices.ImageUtilityService.Interface;
using ViewModel.Contracts.CompositionRootContracts.RssFeedViewModelFactory.Interface;
using ViewModel.Contracts.CompositionRootContracts.RssItemViewModelFactory.Interface;
using ViewModel.Contracts.ViewContracts.AppearanceSwitcher.Interface;
using ViewModel.Services.RssSiteViewModelMapperService.Class;
using ViewModel.ViewModelModules.MainWindowViewModelModule.Class;
using ViewModel.ViewModelModules.RssUserControlViewModelModule.Class;
#endregion

namespace CompositionRoot.Shared.ClassLibrary.StartupModules.ViewModelCompositionRootModule.Class
{
    public class ViewModelCompositionRoot
    {

        // ====== تزریق‌های Stateful / مشترک و شی های Singleton ======
        private readonly string connectionString;

        private readonly AppPaths appPaths;

        private readonly IRssItemDraftCacheManager rssItemDraftCacheManager;

        private readonly ICache<Uri, RssItemEfEntity> persistenceCache;

        private readonly ICache<Uri, RssFeedConfig> rssFeedConfigCache;

        private readonly IAppearanceSwitcher appearanceSwitcher;

        // ====== کش و ImageSettings (داخلی) ======
        private ICache<Uri, ImageSettings<ImageInfo>> imageSettingsCache;

        private IImageSettingsWriter imageSettingsWriter;

        private IImageSettingsReader imageSettingsReader;

        // ====== Factoryهای Stateless ======
        private IRssItemRepositoryFactory rssItemRepositoryFactory;

        private IRssItemDtoQueryFactory rssItemDtoQueryFactory;

        private IRssSiteQueryFactory rssSiteQueryFactory;

        private IUnitOfWorkFactory unitOfWorkFactory;

        private IHttpRequestFactory httpRequestFactory;

        private IImageChannelCountNormalizerFactory<byte> imageChannelCountNormalizerFactory;

        private IImageChannelSequenceNormalizerFactory<byte> imageChannelSequenceNormalizerFactory;

        private IRssItemViewModelFactory rssItemViewModelFactory;

        private IRssFeedViewModelFactory rssFeedViewModelFactory;

        // ====== وابسته‌ها ======
        private IRssItemExistenceChecker rssItemExistenceChecker;

        private IRssItemAllSourcesCoordinator rssItemAllSourcesCoordinator;

        private IRssItemDtoFactory rssItemDtoFactory;

        private IRssItemEfEntityFactory rssItemEfEntityFactory;


        ///////////////////////////////////////////////////////////////////////


        public ViewModelCompositionRoot(string connectionString, AppPaths appPaths, IRssItemDraftCacheManager rssItemDraftCacheManager,
            ICache<Uri, RssItemEfEntity> persistenceCache, ICache<Uri, RssFeedConfig> rssFeedConfigCache, IAppearanceSwitcher appearanceSwitcher)
        {
            // ====== تزریق‌ها (فقط ذخیره) ======
            this.connectionString = connectionString;
            this.appPaths = appPaths;
            this.rssItemDraftCacheManager = rssItemDraftCacheManager;
            this.persistenceCache = persistenceCache;
            this.rssFeedConfigCache = rssFeedConfigCache;
            this.appearanceSwitcher = appearanceSwitcher;

            // ====== راه‌اندازی (به ترتیب وابستگی) ======
            this.InitializeImageSettings();
            this.InitializeFactories();
            this.InitializeDependentServices();
            this.InitializeViewModelFactories();
        }


        ///////////////////////////////////////////////////////////////////////


        // ۱. کش و ImageSettings
        private void InitializeImageSettings()
        {
            this.imageSettingsCache = new Cache<Uri, ImageSettings<ImageInfo>>();
            this.imageSettingsWriter = this.CreateImageSettingsWriter();
            this.imageSettingsReader = (IImageSettingsReader)this.imageSettingsWriter;
        }

        // ۲. Factoryهای Stateless
        private void InitializeFactories()
        {
            this.rssItemRepositoryFactory = this.CreateRssItemRepositoryFactory();
            this.rssItemDtoQueryFactory = this.CreateRssItemDtoQueryFactory();
            this.rssSiteQueryFactory = this.CreateRssSiteQueryFactory();
            this.unitOfWorkFactory = this.CreateUnitOfWorkFactory();
            this.httpRequestFactory = this.CreateHttpRequestFactory();
            this.imageChannelCountNormalizerFactory = this.CreateImageChannelCountNormalizerFactory();
            this.imageChannelSequenceNormalizerFactory = this.CreateImageChannelSequenceNormalizerFactory();
        }

        // ۳. وابسته‌ها (به Factoryها نیاز دارند)
        private void InitializeDependentServices()
        {
            this.rssItemExistenceChecker = this.CreateRssItemExistenceChecker();
            this.rssItemAllSourcesCoordinator = (IRssItemAllSourcesCoordinator)this.rssItemExistenceChecker;
            this.rssItemDtoFactory = this.CreateRssItemDtoFactory();
            this.rssItemEfEntityFactory = (IRssItemEfEntityFactory)this.rssItemDtoFactory;
        }

        // ۴. Factoryهای ViewModel (به UseCaseها نیاز دارند)
        private void InitializeViewModelFactories()
        {
            this.rssItemViewModelFactory = this.CreateRssItemViewModelFactory();
            this.rssFeedViewModelFactory = this.CreateRssFeedViewModelFactory();
        }


        ///////////////////////////////////////////////////////////////////////


        public MainWindowViewModel CreateMainWindowViewModel()
        {
            RssUserControlViewModel rssUserControlViewModel = this.CreateRssUserControlViewModel();
            MainWindowViewModel mainWindowViewModel = new MainWindowViewModel(rssUserControlViewModel, this.appearanceSwitcher);
            return mainWindowViewModel;
        }


        ///////////////////////////////////////////////////////////////////////


        private RssUserControlViewModel CreateRssUserControlViewModel()
        {
            ILoadRssSiteUseCase loadRssSiteUseCase = this.CreateLoadRssSiteUseCase();
            RssSiteInfoMapper rssSiteInfoMapper = this.CreateRssSiteInfoMapper();
            return new RssUserControlViewModel(loadRssSiteUseCase, rssSiteInfoMapper);
        }

        private ILoadRssSiteUseCase CreateLoadRssSiteUseCase()
        {
            return new LoadRssSiteUseCase(this.rssSiteQueryFactory);
        }

        private RssSiteInfoMapper CreateRssSiteInfoMapper()
        {
            return new RssSiteInfoMapper(this.rssFeedViewModelFactory);
        }


        ///////////////////////////////////////////////////////////////////////


        private IRssFeedViewModelFactory CreateRssFeedViewModelFactory()
        {
            IDownLoadRssItemsUseCase downLoadRssItemsUseCase = this.CreateDownLoadRssItemsUseCase();
            return new RssFeedViewModelFactory(downLoadRssItemsUseCase, this.rssItemViewModelFactory);
        }

        private IDownLoadRssItemsUseCase CreateDownLoadRssItemsUseCase()
        {
            IRssItemDownloaderIntoDraftCache rssItemDownloaderIntoDraftCache = this.CreateRssItemDownloaderIntoDraftCache();
            return new DownLoadRssItemsUseCase(rssItemDownloaderIntoDraftCache, this.rssFeedConfigCache);
        }

        private IRssItemDownloaderIntoDraftCache CreateRssItemDownloaderIntoDraftCache()
        {
            IRssItemsDownLoader rssItemsDownLoader = this.CreateRssItemsDownLoader();
            return new RssItemDownloaderIntoDraftCache(rssItemsDownLoader, this.rssItemDtoFactory,
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


        ///////////////////////////////////////////////////////////////////////


        private IImageSettingsWriter CreateImageSettingsWriter()
        {
            IImageInfoToImageSettingsConverter imageSettingsConverter = this.CreateImageInfoToImageSettingsConverter();
            return new ImageSettingsProvider(this.imageSettingsCache, imageSettingsConverter);
        }

        private IImageInfoToImageSettingsConverter CreateImageInfoToImageSettingsConverter()
        {
            return new ImageInfoToImageSettingsConverter();
        }


        ///////////////////////////////////////////////////////////////////////


        private IRssItemRepositoryFactory CreateRssItemRepositoryFactory()
        {
            IDraftCacheWriter draftCacheWriter = new DraftCacheWriter();
            IImageDataExtractor imageDataExtractor = new ImageDataExtractor(new InfraImageCreator(), new ImageInfoMemberMapper());
            IImageFileReader imageFileReader = new ImageFileReader(imageDataExtractor);
            IImageFileSaver imageFileSaver = new ImageFileSaver(new ImageUtility(), new ImageFileSettingsMemberMapper(new ImageInfoMemberMapper()));

            IImageFileSaverCoordinator imageFileSaverCoordinator = new ImageFileSaverCoordinator(
                imageFileSaver, new ImageFileExtensionResolver(), new FilePathMaker(new StringHasher()), new ImageSettingsMapper());
            RssItemEfEntityMapper rssItemEfEntityMapper = new RssItemEfEntityMapper();
            IRssItemEfEntityFactory rssItemEfEntityFactory = new RssItemDtoEfEntityFactory(
                imageFileSaverCoordinator, new RssItemDtoMapper(new PersianCalenderConvertor()), new RssItemEfEntityMapper());
            RssItemEfEntityExpressionFactory expressionFactory = new RssItemEfEntityExpressionFactory();

            return new RssItemRepositoryFactory(rssItemEfEntityMapper, this.persistenceCache, draftCacheWriter,
                imageFileReader, rssItemEfEntityFactory, expressionFactory);
        }

        private IRssItemDtoQueryFactory CreateRssItemDtoQueryFactory()
        {
            RssItemDtoEfEntityMapper rssItemDtoEfEntityMapper = new RssItemDtoEfEntityMapper(new PersianCalenderConvertor());
            RssItemEfEntityExpressionFactory expressionFactory = new RssItemEfEntityExpressionFactory();
            return new RssItemDtoQueryFactory(this.connectionString, this.persistenceCache, rssItemDtoEfEntityMapper, expressionFactory);
        }

        private IRssSiteQueryFactory CreateRssSiteQueryFactory()
        {
            RssSiteEfEntityMapper rssSiteEfEntityMapper = new RssSiteEfEntityMapper(new RssFeedEfEntityMapper());
            return new RssSiteQueryFactory(this.connectionString, rssSiteEfEntityMapper);
        }

        private IUnitOfWorkFactory CreateUnitOfWorkFactory()
        {
            return new UnitOfWorkFactory(this.connectionString);
        }

        private IHttpRequestFactory CreateHttpRequestFactory()
        {
            return new HttpRequestFactory();
        }

        private IRssItemViewModelFactory CreateRssItemViewModelFactory()
        {
            return new RssItemViewModelFactory();
        }

        private IImageChannelCountNormalizerFactory<byte> CreateImageChannelCountNormalizerFactory()
        {
            return new ImageChannelCountNormalizerFactory();
        }

        private IImageChannelSequenceNormalizerFactory<byte> CreateImageChannelSequenceNormalizerFactory()
        {
            return new ImageChannelSequenceNormalizerFactory();
        }


        ///////////////////////////////////////////////////////////////////////


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
            IImageFileExtensionResolver imageFileExtensionResolver = this.CreateImageFileExtensionResolver();
            return new ImageFileSaverCoordinator(imageFileSaver, imageFileExtensionResolver, filePathMaker, imageSettingsMapper);
        }

        private IImageFileExtensionResolver CreateImageFileExtensionResolver()
        {
            return new ImageFileExtensionResolver();
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
            IStringHasher stringHasher = this.CreateStringHasher();
            return new FilePathMaker(stringHasher);
        }

        private IStringHasher CreateStringHasher()
        {
            return new StringHasher();
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
