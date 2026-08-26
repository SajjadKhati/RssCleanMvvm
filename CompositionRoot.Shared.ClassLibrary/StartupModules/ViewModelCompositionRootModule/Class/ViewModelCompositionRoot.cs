using Application.InfrastructureContracts.DataAccessProject.AppQuery.RssSiteQuery.Factory.Interface;
using Application.InfrastructureContracts.XmlProject.SharedDataModels.Class;
using Application.UseCases.LoadRssSiteUseCaseModule.Class;
using Application.UseCases.LoadRssSiteUseCaseModule.Interface;
using Infra.DataAccess.AppQueryService.RssSiteQueryService.Class.Factory;
using Infra.DataAccess.EfEntityMapperService.Class;
using ViewModel.Services.RssSiteViewModelMapperService.Class;
using ViewModel.ViewModelModules.MainWindowViewModelModule.Class;
using ViewModel.ViewModelModules.RssUserControlViewModelModule.Class;

namespace CompositionRoot.Shared.ClassLibrary.StartupModules.ViewModelCompositionRootModule.Class
{
    public class ViewModelCompositionRoot
    {

        private string connectionString;

        private IReadOnlyDictionary<Uri, RssFeedXmlParsingConfig> rssFeedXmlParsingConfigDictionary;


        ///////////////////////////////////////////////////////////////////////


        public ViewModelCompositionRoot(string connectionString, IReadOnlyDictionary<Uri, RssFeedXmlParsingConfig> rssFeedXmlParsingConfigDictionary)
        {
            this.connectionString = connectionString;
            this.rssFeedXmlParsingConfigDictionary = rssFeedXmlParsingConfigDictionary;
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
