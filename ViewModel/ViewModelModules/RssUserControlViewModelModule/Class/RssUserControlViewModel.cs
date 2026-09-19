using Application.OverallDataModels.OverallDto.EntityDto;
using Application.UseCases.DownLoadRssItemsUseCaseModule.DataModels;
using Application.UseCases.DownLoadRssItemsUseCaseModule.Interface;
using Application.UseCases.LoadRssSiteUseCaseModule.Interface;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Shared.CollectionService.Class;
using System.Collections.Generic;
using ViewModel.Contracts.CompositionRootContracts.RssItemViewModelFactory.Interface;
using ViewModel.Services.RangeObservableCollectionService.Class;
using ViewModel.Services.RssSiteViewModelMapperService.Class;
using ViewModel.Services.SharedMessengerKeyService.Class;
using ViewModel.ViewModelModules.RssFeedViewModelModule.Class;
using ViewModel.ViewModelModules.RssItemViewModelModule.Class;
using ViewModel.ViewModelModules.RssSiteViewModelModule.Class;

namespace ViewModel.ViewModelModules.RssUserControlViewModelModule.Class
{
    public class RssUserControlViewModel : ObservableObject
    {

        private RssSiteViewModel _selectedRssSiteViewModel;


        /////////////////////////////////////////////////////////////////////////////


        private ILoadRssSiteUseCase loadRssSiteUseCase;

        private RssSiteInfoMapper rssSiteInfoMapper;


        /////////////////////////////////////////////////////////////////////////////


        public RangeObservableCollection<RssSiteViewModel> RssSiteViewModels { get; }


        public RssSiteViewModel SelectedRssSiteViewModel
        {
            get => this._selectedRssSiteViewModel;
            set
            {
                this.SetProperty(ref this._selectedRssSiteViewModel, value);
            }
        }


        /////////////////////////////////////////////////////////////////////////////


        public IAsyncRelayCommand LoadRssSiteCommand {  get; }

        public IAsyncRelayCommand LoadRssItemsCommand { get; }


        /////////////////////////////////////////////////////////////////////////////


        public RssUserControlViewModel(ILoadRssSiteUseCase loadRssSiteUseCase, RssSiteInfoMapper rssSiteInfoMapper)
        {
            this.loadRssSiteUseCase = loadRssSiteUseCase;
            this.rssSiteInfoMapper = rssSiteInfoMapper;

            this.RssSiteViewModels = new RangeObservableCollection<RssSiteViewModel>();
            this.LoadRssSiteCommand = new AsyncRelayCommand(this.LoadRssSite);
            this.LoadRssItemsCommand = new AsyncRelayCommand(this.LoadRssItems, this.CanExecuteLoadRssItems);
        }


        /////////////////////////////////////////////////////////////////////////////


        private async Task LoadRssSite()
        {
            try
            {
                IList<RssSiteDto> rssSiteDtos = await this.loadRssSiteUseCase.ExecuteAsync();
                this.AddToRssSiteViewModels(rssSiteDtos);
            }
            catch (Exception exception)
            {
                WeakReferenceMessenger.Default.Send(exception, SharedMessengerKey.Exception_ErrorKey);
            }
        }


        private bool CanExecuteLoadRssItems()
        {
            throw new NotImplementedException();
        }


        private async Task LoadRssItems()
        {
            // LoadRssItemsUseCase
        }


        /////////////////////////////////////////////////////////////////////////////


        private void AddToRssSiteViewModels(IList<RssSiteDto> rssSiteDtos)
        {
            this.RssSiteViewModels.Clear();

            foreach (RssSiteDto rssSiteDto in rssSiteDtos)
            {
                RssSiteViewModel rssSiteViewModel = this.rssSiteInfoMapper.ToRssSiteViewModel(rssSiteDto);
                this.RssSiteViewModels.Add(rssSiteViewModel);
            }
        }

    }
}
