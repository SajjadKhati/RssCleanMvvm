using Application.OverallDataModels.OverallDto.EntityDto;
using Application.UseCases.DownLoadRssItemsUseCaseModule.DataModels;
using Application.UseCases.DownLoadRssItemsUseCaseModule.Interface;
using Application.UseCases.LoadRssSiteUseCaseModule.Interface;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Shared.CollectionService.Class;
using System.Collections.Generic;
using ViewModel.CompositionRootContracts.RssItemViewModelFactory.Interface;
using ViewModel.DataModels;
using ViewModel.Services.RangeObservableCollectionService.Class;
using ViewModel.Services.RssSiteViewModelMapperService.Class;
using ViewModel.Services.SharedMessengerKeyService.Class;
using ViewModel.ViewModelModules.RssItemViewModelModule.Class;

namespace ViewModel.ViewModelModules.RssUserControlViewModelModule.Class
{
    public class RssUserControlViewModel : ObservableObject
    {

        private IRssItemViewModelFactory rssItemViewModelFactory;

        private ILoadRssSiteUseCase loadRssSiteUseCase;

        private IDownLoadRssItemsUseCase downLoadRssItemsUseCase;

        private RssSiteInfoMapper rssSiteInfoMapper;


        /////////////////////////////////////////////////////////////////////////////


        private RssFeedInfo _selectedRssFeedInfo;


        /////////////////////////////////////////////////////////////////////////////


        public RangeObservableCollection<RssSiteInfo> RssSiteInfos { get; }

        public RangeObservableCollection<RssItemViewModel> RssItemViewModels { get; }

        public RssFeedInfo SelectedRssFeedInfo
        {
            get => this._selectedRssFeedInfo;
            set
            {
                if(this.SetProperty(ref  this._selectedRssFeedInfo, value) == true)
                    /// اعلام میشه که وضعیت CanExecute در کامندِ DownLoadRssItemsCommand تغییر کرد تا با اجرای متد CanExecute در این کامند از سوی کنترلِ ButtonBase ئه بایند شده ، 
                    /// در صورتی که مقدار true برگردانده بشه ، وضعیت فعال شدنِ کنترلِ ButtonBase ، تغییر کنه .
                    this.DownLoadRssItemsCommand.NotifyCanExecuteChanged();
            }
        }


        public IAsyncRelayCommand LoadRssSiteCommand {  get; }

        public IAsyncRelayCommand LoadRssItemsCommand { get; }

        public IAsyncRelayCommand DownLoadRssItemsCommand { get; }


        /////////////////////////////////////////////////////////////////////////////


        public RssUserControlViewModel(IRssItemViewModelFactory rssItemViewModelFactory, ILoadRssSiteUseCase loadRssSiteUseCase,
            IDownLoadRssItemsUseCase downLoadRssItemsUseCase, RssSiteInfoMapper rssSiteInfoMapper)
        {
            this.rssItemViewModelFactory = rssItemViewModelFactory;
            this.loadRssSiteUseCase = loadRssSiteUseCase;
            this.downLoadRssItemsUseCase = downLoadRssItemsUseCase;
            this.rssSiteInfoMapper = rssSiteInfoMapper;

            this.RssSiteInfos = new RangeObservableCollection<RssSiteInfo>();
            this.RssItemViewModels = new RangeObservableCollection<RssItemViewModel>();
            this.LoadRssSiteCommand = new AsyncRelayCommand(this.LoadRssSite);
            this.LoadRssItemsCommand = new AsyncRelayCommand(this.LoadRssItems, this.CanExecuteLoadRssItems);
            this.DownLoadRssItemsCommand = new AsyncRelayCommand(this.DownLoadRssItems, this.CanExecuteDownLoadRssItems);
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


        private bool CanExecuteLoadRssItems()
        {
            return this.SelectedRssFeedInfo != null;
        }


        private async Task LoadRssItems()
        {
            // LoadRssItemsUseCase
        }


        private bool CanExecuteDownLoadRssItems()
        {
            return this.SelectedRssFeedInfo != null;
        }


        private async Task DownLoadRssItems()
        {
            try
            {
                RssItemDtosResult rssItemDtosResult = await this.downLoadRssItemsUseCase.ExecuteAsync(new Uri(this.SelectedRssFeedInfo.Id));
                if (rssItemDtosResult == null)
                    return;

                ICollection<RssItemViewModel> rssItemViewModels = this.ConvertToRssItemViewModels(rssItemDtosResult.CreatedRssItemDtos);
                this.InsertToBeginningIndexOfRssItemViewModels(rssItemViewModels);

                if(rssItemDtosResult.CreationErrors != null && rssItemDtosResult.CreationErrors.Count > 0)
                {
                    Exception rssItemException = new Exception($"به دلایل مختلف از جمله تکراری بودن خبر که از قبل در حافظه یا دیتابیس وجود داشته ، یا سازگار نبودن تصویرِ خبر ، یا هر دلایل دیگری ، دریافت {rssItemDtosResult.CreationErrors.Count} خبر ، انجام نشد .");
                    WeakReferenceMessenger.Default.Send(rssItemException, SharedMessengerKey.Exception_ErrorKey);
                }
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


        private ICollection<RssItemViewModel> ConvertToRssItemViewModels(ICollection<RssItemDto> rssItemDtos)
        {
            if (rssItemDtos == null || rssItemDtos.Count == 0)
                return null;

            ICollection<RssItemViewModel> rssItemViewModels = new List<RssItemViewModel>();
            foreach (RssItemDto rssItemDto in rssItemDtos)
            {
                RssItemViewModel rssItemViewModel = this.rssItemViewModelFactory.Create(rssItemDto);
                rssItemViewModels.Add(rssItemViewModel);
            }
            return rssItemViewModels;
        }


        private void InsertToBeginningIndexOfRssItemViewModels(ICollection<RssItemViewModel> rssItemViewModels)
        {
            if (rssItemViewModels == null || rssItemViewModels.Count == 0)
                return;

            int startingIndex = 0;
            this.RssItemViewModels.InsertRangeAt(rssItemViewModels, startingIndex);
        }

    }
}
