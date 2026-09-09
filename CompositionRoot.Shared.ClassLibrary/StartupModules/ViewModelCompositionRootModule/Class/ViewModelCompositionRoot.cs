using Application.InfrastructureContracts.CacheProject.AppDraftCacheManager.RssItemDraftCacheManager.Interface;
using Application.InfrastructureContracts.CacheProject.CacheBase.Cache.Interfaces;
using Application.InfrastructureContracts.DataAccessProject.AppQuery.RssItemDtoQuery.Factory.Interface;
using Application.InfrastructureContracts.DataAccessProject.AppQuery.RssSiteQuery.Factory.Interface;
using Application.InfrastructureContracts.DataAccessProject.AppRepository.RssItemRepository.Factory.Interfaces;
using Application.InfrastructureContracts.DataAccessProject.UnitOfWorkFactory.Interfaces;
using Application.InfrastructureContracts.HttpProject.HttpRequest.Factory.Interfaces;
using Application.InfrastructureContracts.XmlProject.SharedDataModels.Class;
using Application.Services.ImageNormalizerServices._1_ImageChannelCountNormalizerService.Factory.Interface;
using Application.Services.ImageNormalizerServices._2_ImageChannelSequenceNormalizerService.Factory.Interface;
using Application.UseCases.LoadRssSiteUseCaseModule.Class;
using Application.UseCases.LoadRssSiteUseCaseModule.Interface;
using Infra.DataAccess.AppQueryService.RssSiteQueryService.Class.Factory;
using Infra.DataAccess.EfEntities.Class;
using Infra.DataAccess.EfEntityMapperService.Class;
using ViewModel.Services.RssSiteViewModelMapperService.Class;
using ViewModel.ViewModelModules.MainWindowViewModelModule.Class;
using ViewModel.ViewModelModules.RssUserControlViewModelModule.Class;

namespace CompositionRoot.Shared.ClassLibrary.StartupModules.ViewModelCompositionRootModule.Class
{
    public class ViewModelCompositionRoot
    {

        private readonly string connectionString;

        private readonly IRssItemDraftCacheManager rssItemDraftCacheManager;

        private readonly ICache<Uri, RssItemEfEntity> persistenceCache;

        private readonly ICache<Uri, RssFeedConfig> rssFeedConfigCache;

        private readonly IRssItemRepositoryFactory rssItemRepositoryFactory;

        private readonly IRssItemDtoQueryFactory rssItemDtoQueryFactory;

        private readonly IRssSiteQueryFactory rssSiteQueryFactory;

        private readonly IUnitOfWorkFactory unitOfWorkFactory;

        private readonly IHttpRequestFactory httpRequestFactory;

        private readonly IImageChannelCountNormalizerFactory<byte> imageChannelCountNormalizerFactory;

        private readonly IImageChannelSequenceNormalizerFactory<byte> imageChannelSequenceNormalizerFactory;


        ///////////////////////////////////////////////////////////////////////


        public ViewModelCompositionRoot(
            string connectionString, 
            IRssItemDraftCacheManager rssItemDraftCacheManager,
            ICache<Uri, RssItemEfEntity> persistenceCache, 
            ICache<Uri, RssFeedConfig> rssFeedConfigCache, 
            IRssItemRepositoryFactory rssItemRepositoryFactory, 
            IRssItemDtoQueryFactory rssItemDtoQueryFactory,
            IRssSiteQueryFactory rssSiteQueryFactory, 
            IUnitOfWorkFactory unitOfWorkFactory, 
            IHttpRequestFactory httpRequestFactory,
            IImageChannelCountNormalizerFactory<byte> imageChannelCountNormalizerFactory,
            IImageChannelSequenceNormalizerFactory<byte> imageChannelSequenceNormalizerFactory)
        {
            this.connectionString = connectionString;
            this.rssItemDraftCacheManager = rssItemDraftCacheManager;
            this.persistenceCache = persistenceCache;
            this.rssFeedConfigCache = rssFeedConfigCache;
            this.rssItemRepositoryFactory = rssItemRepositoryFactory;
            this.rssItemDtoQueryFactory = rssItemDtoQueryFactory;
            this.rssSiteQueryFactory = rssSiteQueryFactory;
            this.unitOfWorkFactory = unitOfWorkFactory;
            this.httpRequestFactory = httpRequestFactory;
            this.imageChannelCountNormalizerFactory = imageChannelCountNormalizerFactory;
            this.imageChannelSequenceNormalizerFactory = imageChannelSequenceNormalizerFactory;
        }


        ///////////////////////////////////////////////////////////////////////


        /// <summary>
        /// فعلا این نسخه از متد تستی هست . بعدا باید کاملش کنی .
        /// مخصوصا ساخت شی RssUserControlViewModel که پارامتر اولش ، null ارسال شد .
        /// </summary>
        /// <returns></returns>
        public MainWindowViewModel CreateMainWindowViewModel()
        {
            RssFeedEfEntityMapper rssFeedEfEntityMapper = new RssFeedEfEntityMapper();
            RssSiteEfEntityMapper rssSiteEfEntityMapper = new RssSiteEfEntityMapper(rssFeedEfEntityMapper);
            IRssSiteQueryFactory rssSiteQueryFactory = new RssSiteQueryFactory(this.connectionString, rssSiteEfEntityMapper);
            ILoadRssSiteUseCase initializeAppUseCase = new LoadRssSiteUseCase(rssSiteQueryFactory);
            RssSiteInfoMapper rssSiteInfoMapper = new RssSiteInfoMapper();

            RssUserControlViewModel rssUserControlViewModel = new RssUserControlViewModel(null, initializeAppUseCase, rssSiteInfoMapper);
            MainWindowViewModel mainWindowViewModel = new MainWindowViewModel(rssUserControlViewModel);
            return mainWindowViewModel;
        }

    }
}
