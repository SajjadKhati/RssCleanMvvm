using Application.InfrastructureContracts.DataAccessProject.AppQuery.Factory.Interface;
using Application.InfrastructureContracts.DataAccessProject.DataAccessBase.DatabaseInitialization.Interface;
using Application.InfrastructureContracts.JsonProject.Json.Interface;
using Application.UseCases.LoadRssSiteUseCaseModule.Class;
using Application.UseCases.LoadRssSiteUseCaseModule.Interface;
using CompositionRoot.WpfApp.StartupModules.AppInitializerModule.Class;
using Infra.DataAccess.AppQueryService.Class.Factory.Class;
using Infra.DataAccess.DataAccessBaseServices.DatabaseInitializationService.Class;
using Infra.DataAccess.DbContextService.Class;
using Infra.DataAccess.EfEntityMapperService.Class;
using Infra.JsonService.JsonServices.Class;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModel.Services.RssSiteViewModelMapperService.Class;
using ViewModel.ViewModelModules.MainWindowViewModelModule.Class;
using ViewModel.ViewModelModules.RssUserControlViewModelModule.Class;

namespace CompositionRoot.WpfApp.StartupModules.AppCompositionRootModule.Class
{
    public class AppCompositionRoot
    {

        private string connectionString;


        ///////////////////////////////////////////////////////////////////////


        public AppCompositionRoot(string connectionString)
        {
            this.connectionString = connectionString;
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
