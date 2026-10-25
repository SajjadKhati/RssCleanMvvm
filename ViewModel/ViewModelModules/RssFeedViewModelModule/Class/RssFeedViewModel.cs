using Application.OverallDataModels.OverallDto.EntityDto;
using Application.UseCases.Entry.DownLoadRssItems.DataModels;
using Application.UseCases.Entry.DownLoadRssItems.Interface;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.ComponentModel;
using ViewModel.Contracts.CompositionRootContracts.RssItemViewModelFactory.Interface;
using ViewModel.Services.RangeObservableCollectionService.Class;
using ViewModel.Services.SharedMessengerKeyService.Class;
using ViewModel.ViewModelModules.RssItemViewModelModule.Class;
using ViewModel.Services.CommandNotifyService.Class;

namespace ViewModel.ViewModelModules.RssFeedViewModelModule.Class
{
    public class RssFeedViewModel
    {

        private IDownLoadRssItemsUseCase downLoadRssItemsUseCase;

        private IRssItemViewModelFactory rssItemViewModelFactory;

        private readonly string[] ObservedProperties =
        {
            nameof(IAsyncRelayCommand.IsRunning),
            nameof(IAsyncRelayCommand.IsCancellationRequested),
            nameof(IAsyncRelayCommand.CanBeCanceled)
        };

        /////////////////////////////////////////////////////////////////////////////


        public string Id { get; }

        public string Title { get; }

        public RangeObservableCollection<RssItemViewModel> RssItemViewModels { get; }


        /////////////////////////////////////////////////////////////////////////////


        public IAsyncRelayCommand LoadRssItemsCommand { get; }

        public IAsyncRelayCommand DownLoadRssItemsCommand { get; }

        public IRelayCommand CancelCurrentOperationCommand { get; }


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

            this.LoadRssItemsCommand = new AsyncRelayCommand(this.LoadRssItems, this.CanLoadRssItems);
            this.DownLoadRssItemsCommand = new AsyncRelayCommand(this.DownLoadRssItems, this.CanDownLoadRssItems);
            this.CancelCurrentOperationCommand = new RelayCommand(this.CancelCurrentOperation, this.CanCancelCurrentOperation);
            this.SetupCommandNotifyCanExecuteChanged();
        }


        /////////////////////////////////////////////////////////////////////////////

        private bool CanLoadRssItems()
        {
            throw new NotImplementedException();
        }


        private async Task LoadRssItems(CancellationToken cancellationToken)
        {
            /// ابتدا و انتهای این متد ، متد this.CancelCurrentOperationCommand.NotifyCanExecuteChanged(); را فراخوانی کن .
            throw new NotImplementedException();
        }


        private bool CanDownLoadRssItems()
        {
            return this.LoadRssItemsCommand.IsRunning == false;
        }


        private async Task DownLoadRssItems(CancellationToken cancellationToken)
        {
            try
            {
                RssItemDtosResult rssItemDtosResult = await this.downLoadRssItemsUseCase.ExecuteAsync(new Uri(this.Id), cancellationToken);
                if (rssItemDtosResult.CreatedRssItemDtos != null)
                {
                    ICollection<RssItemViewModel> rssItemViewModels = this.ConvertToRssItemViewModels(rssItemDtosResult.CreatedRssItemDtos);
                    this.InsertToBeginningIndexOfRssItemViewModels(rssItemViewModels);
                }
                this.SendMessages(rssItemDtosResult);
            }
            catch (Exception exception)
            {
                /// خطاهای XML/شبکه (که در Orchestrator گرفته نشدن) اینجا گرفته می‌شن
                WeakReferenceMessenger.Default.Send(exception, SharedMessengerKey.Exception_ErrorKey);
            }
        }


        private bool CanCancelCurrentOperation()
        {
            ///CanBeCanceled در IAsyncRelayCommand خودش دو چیز رو چک می‌کنه:
            ///آیا کامند در حال اجراست؟ (IsRunning)
            ///    آیا delegate ـش CancellationToken می‌گیره؟ (پس Cancel() معنی داره)
            return (this.LoadRssItemsCommand.CanBeCanceled == true && this.LoadRssItemsCommand.IsCancellationRequested == false) || 
                (this.DownLoadRssItemsCommand.CanBeCanceled == true && this.DownLoadRssItemsCommand.IsCancellationRequested == false);
        }


        private void CancelCurrentOperation()
        {
            if (this.LoadRssItemsCommand.CanBeCanceled == true)      // ✅ true → Cancel
                this.LoadRssItemsCommand.Cancel();

            if (this.DownLoadRssItemsCommand.CanBeCanceled == true)  // ✅ false → skip
                this.DownLoadRssItemsCommand.Cancel();
        }


        /////////////////////////////////////////////////////////////////////////////


        /// <summary>
        /// وابستگی‌های CanExecute بین کامندهای این ViewModel را برقرار می‌کند.
        ///
        /// چرا لازم است؟
        /// کامندهای async، propertyهای وضعیتی (IsRunning, IsCancellationRequested,
        /// CanBeCanceled) را fire می‌کنند. اما WPF به تغییر این propertyها
        /// به‌طور خودکار واکنش نشان نمی‌دهد — کامندهای دیگر، CanExecute خود را
        /// دوباره ارزیابی نمی‌کنند.
        ///
        /// این متد، دو جهت وابستگی را سیم‌کشی می‌کند:
        ///
        ///   ۱. Load → DownLoad و Cancel:
        ///      - DownLoad.CanExecute = !Load.IsRunning  (چون Load و DownLoad نباید همزمان اجرا شوند)
        ///      - Cancel.CanExecute به Load.CanBeCanceled و Load.IsCancellationRequested وابسته است
        ///
        ///   ۲. DownLoad → Load و Cancel:
        ///      - Load.CanExecute = !DownLoad.IsRunning  (همان دلیل بالا)
        ///      - Cancel.CanExecute به DownLoad.CanBeCanceled و DownLoad.IsCancellationRequested وابسته است
        ///
        /// نتیجه: هر تغییری در وضعیت Load یا DownLoad، بلافاصله CanExecute
        /// کامندهای وابسته را دوباره ارزیابی می‌کند و دکمه‌ها (لغو دانلود، دریافت
        /// اخبار جدید، ...) با state واقعی هم‌گام می‌مانند.
        ///
        /// نکته: CancelCurrentOperationCommand فقط مقصد notify است، نه منبع
        /// (چون IRelayCommand است، نه IAsyncRelayCommand — propertyهای وضعیتی ندارد).
        /// </summary>
        private void SetupCommandNotifyCanExecuteChanged()
        {
            /// ۱. Load منبع تغییرات → DownLoad و Cancel را notify می‌کند.
            /// هر بار Load.IsRunning یا Load.IsCancellationRequested یا Load.CanBeCanceled
            /// عوض شود، DownLoad.CanExecute و Cancel.CanExecute دوباره ارزیابی می‌شوند.
            this.LoadRssItemsCommand.PropagateNotifyCanExecuteChangedToCommands(
                this.ObservedProperties,
                this.DownLoadRssItemsCommand,
                this.CancelCurrentOperationCommand);

            /// ۲. DownLoad منبع تغییرات → Load و Cancel را notify می‌کند.
            /// (متقارن با مورد ۱. برای جلوگیری از اجرای همزمان Load و DownLoad،
            /// و هم‌گام‌سازی Cancel با وضعیت هر دو کامند.)
            this.DownLoadRssItemsCommand.PropagateNotifyCanExecuteChangedToCommands(
                this.ObservedProperties,
                this.LoadRssItemsCommand,
                this.CancelCurrentOperationCommand);
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
            this.RssItemViewModels.InsertRangeWithSingleNotification(rssItemViewModels, startingIndex);
        }


        private void SendMessages(RssItemDtosResult rssItemDtosResult)
        {
            /// ۱. لغو کاربر — بالاترین اولویت
            /// چون اگه کاربر لغو کرده، پیام «هیچ خبری نیست» یا «خطا» گمراه‌کننده‌ست.
            if (rssItemDtosResult.WasUserCancelled == true)
            {
                string cancelMessage = (rssItemDtosResult.CreatedRssItemDtos.Count > 0)
                    ? $"عملیات لغو شد. {rssItemDtosResult.CreatedRssItemDtos.Count} خبر در کش موقت ذخیره شد ."
                    : "عملیات لغو شد.";
                WeakReferenceMessenger.Default.Send(cancelMessage, SharedMessengerKey.String_GeneralMessageKey);
                return;
            }
            /// ۲. پیام موفقیت یا خالی
            if (rssItemDtosResult.CreatedRssItemDtos.Count > 0)
            {
                string message = $"تعداد {rssItemDtosResult.CreatedRssItemDtos.Count} خبر جدید دریافت شد .";
                WeakReferenceMessenger.Default.Send(message, SharedMessengerKey.String_GeneralMessageKey);
            }
            else if (rssItemDtosResult.CreationErrors.Count == 0)
            {
                // نه آیتم جدید، نه خطا → واقعاً هیچ خبر جدیدی نیست
                WeakReferenceMessenger.Default.Send("هیچ خبر جدیدی وجود ندارد .", SharedMessengerKey.String_GeneralMessageKey);
            }

            /// ۳. خطاها (فقط اولی برای نمایش)
            /// InnerException پاس داده می‌شه تا stack trace اصلی حفظ بشه.
            if (rssItemDtosResult.CreationErrors.Count > 0)
            {
                string errorMessage = $"دریافت {rssItemDtosResult.CreationErrors.Count} خبر انجام نشد . " +
                    $"دلیل خطای اول :\n\n{rssItemDtosResult.CreationErrors[0].Message}";
                Exception firstError = new Exception(errorMessage, rssItemDtosResult.CreationErrors[0]);
                WeakReferenceMessenger.Default.Send(firstError, SharedMessengerKey.Exception_ErrorKey);
            }
        }

    }
}
