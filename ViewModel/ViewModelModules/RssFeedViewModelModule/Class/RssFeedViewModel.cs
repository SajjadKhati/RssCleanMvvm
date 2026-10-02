using Application.OverallDataModels.OverallDto.EntityDto;
using Application.UseCases.Entry.DownLoadRssItems.DataModels;
using Application.UseCases.Entry.DownLoadRssItems.Interface;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ViewModel.Contracts.CompositionRootContracts.RssItemViewModelFactory.Interface;
using ViewModel.Services.RangeObservableCollectionService.Class;
using ViewModel.Services.SharedMessengerKeyService.Class;
using ViewModel.ViewModelModules.RssItemViewModelModule.Class;

namespace ViewModel.ViewModelModules.RssFeedViewModelModule.Class
{
    public class RssFeedViewModel
    {

        private IDownLoadRssItemsUseCase downLoadRssItemsUseCase;

        private IRssItemViewModelFactory rssItemViewModelFactory;


        /////////////////////////////////////////////////////////////////////////////


        public string Id { get; }

        public string Title { get; }

        public RangeObservableCollection<RssItemViewModel> RssItemViewModels { get; }


        /////////////////////////////////////////////////////////////////////////////


        public IAsyncRelayCommand DownLoadRssItemsCommand { get; }


        /////////////////////////////////////////////////////////////////////////////


        public RssFeedViewModel(string id, string title, IDownLoadRssItemsUseCase downLoadRssItemsUseCase, 
            IRssItemViewModelFactory rssItemViewModelFactory)
        {
            this.Id = id;
            this.Title = title;
            /// اطلاعات آیتم های RssItemViewModels چون بعدا باید یا دانلود شوند و یا از دیتابیس گرفته شوند ، پس در سازنده نیاز به این اطلاعات نیست . 
            /// در اینجا فقط کالکشن خالی میسازیم .
            this.RssItemViewModels = new RangeObservableCollection<RssItemViewModel>();

            this.downLoadRssItemsUseCase = downLoadRssItemsUseCase;
            this.rssItemViewModelFactory = rssItemViewModelFactory;
            this.DownLoadRssItemsCommand = new AsyncRelayCommand(this.DownLoadRssItems);
        }


        /////////////////////////////////////////////////////////////////////////////


        private async Task DownLoadRssItems()
        {
            try
            {
                RssItemDtosResult rssItemDtosResult = await this.downLoadRssItemsUseCase.ExecuteAsync(new Uri(this.Id));
                if (rssItemDtosResult == null)
                    return;

                ICollection<RssItemViewModel> rssItemViewModels = this.ConvertToRssItemViewModels(rssItemDtosResult.CreatedRssItemDtos);
                this.InsertToBeginningIndexOfRssItemViewModels(rssItemViewModels);
                this.SendMessages(rssItemDtosResult);
            }
            catch (Exception exception)
            {
                WeakReferenceMessenger.Default.Send(exception, SharedMessengerKey.Exception_ErrorKey);
            }
        }


        /////////////////////////////////////////////////////////////////////////////


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
            this.RssItemViewModels.InsertRangeWithSingleNotification(rssItemViewModels, startingIndex);
        }


        private void SendMessages(RssItemDtosResult rssItemDtosResult)
        {
            if (rssItemDtosResult == null)
                return;
            string message = "";
            if (rssItemDtosResult.CreatedRssItemDtos == null || rssItemDtosResult.CreatedRssItemDtos.Count < 1)
            {
                message = "هیچ خبر جدیدی وجود ندارد .";
            }
            else
            {
                message = $"تعداد  {rssItemDtosResult.CreatedRssItemDtos.Count}  خبر جدید دریافت شد .";
            }
            WeakReferenceMessenger.Default.Send(message, SharedMessengerKey.String_GeneralMessageKey);

            if (rssItemDtosResult.CreationErrors != null && rssItemDtosResult.CreationErrors.Count > 0)
            {
                Exception rssItemException = new Exception(
                    $"به دلایل مختلف از جمله تکراری بودن خبر که از قبل در حافظه یا دیتابیس وجود داشته ، یا سازگار نبودن تصویرِ خبر ، یا هر دلایل دیگری ، دریافت " +
                    $"{rssItemDtosResult.CreationErrors.Count} خبر ، انجام نشد . دلیل خطای اول :\n\n{rssItemDtosResult.CreationErrors[0].Message}");
                WeakReferenceMessenger.Default.Send(rssItemException, SharedMessengerKey.Exception_ErrorKey);
            }
        }

    }
}
