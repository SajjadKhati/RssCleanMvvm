using Application.OverallDataModels.OverallDto.EntityDto;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections.ObjectModel;
using Application.UseCases.LoadRssSiteUseCaseModule.Interface;
using ViewModel.DataModels;
using ViewModel.Services.RssItemViewModelFactoryService.Interface;
using ViewModel.Services.RssSiteViewModelMapperService.Class;
using ViewModel.Services.SharedMessengerKeyService.Class;
using ViewModel.ViewModelModules.RssItemViewModelModule.Class;

namespace ViewModel.ViewModelModules.RssUserControlViewModelModule.Class
{
    public class RssUserControlViewModel
    {

        private IRssItemViewModelFactory rssItemViewModelFactory;

        private ILoadRssSiteUseCase loadRssSiteUseCase;

        private RssSiteInfoMapper rssSiteInfoMapper;


        /////////////////////////////////////////////////////////////////////////////


        public ObservableCollection<RssSiteInfo> RssSiteInfos { get; }

        public ObservableCollection<RssItemViewModel> RssItemViewModels { get; }


        public IAsyncRelayCommand LoadRssSiteCommand {  get; }


        /////////////////////////////////////////////////////////////////////////////


        public RssUserControlViewModel(IRssItemViewModelFactory rssItemViewModelFactory, ILoadRssSiteUseCase loadRssSiteUseCase,
            RssSiteInfoMapper rssSiteInfoMapper)
        {
            this.rssItemViewModelFactory = rssItemViewModelFactory;
            this.loadRssSiteUseCase = loadRssSiteUseCase;
            this.rssSiteInfoMapper = rssSiteInfoMapper;

            this.RssSiteInfos = new ObservableCollection<RssSiteInfo>();
            this.RssItemViewModels = new ObservableCollection<RssItemViewModel>();
            this.LoadRssSiteCommand = new AsyncRelayCommand(this.LoadRssSite);
        }


        /////////////////////////////////////////////////////////////////////////////


        private async Task LoadRssSite()
        {
            try
            {
                IList<RssSiteDto> rssSiteDtos = await this.loadRssSiteUseCase.ExecuteAsync();
                this.AddToRssSiteInfos(rssSiteDtos);
            }
            catch (Exception exception)
            {
                WeakReferenceMessenger.Default.Send(exception, SharedMessengerKey.Exception_ErrorKey);
            }
        }


        /////////////////////////////////////////////////////////////////////////////


        private void AddToRssSiteInfos(IList<RssSiteDto> rssSiteDtos)
        {
            this.RssSiteInfos.Clear();

            foreach (RssSiteDto rssSiteDto in rssSiteDtos)
            {
                RssSiteInfo rssSiteInfos = this.rssSiteInfoMapper.ToRssSiteInfo(rssSiteDto);
                this.RssSiteInfos.Add(rssSiteInfos);
            }
        }

    }
}
