using Application.OverallDataModels.OverallDto.EntityDto;
using Application.UseCases.Entry.DownLoadRssItems.DataModels;
using Application.UseCases.Entry.DownLoadRssItems.Interface;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ViewModel.Contracts.CompositionRootContracts.RssItemViewModelFactory.Interface;
using ViewModel.DataModels.MessengerDataModels.FeedOperationStateMessage.Class;
using ViewModel.DataModels.MessengerDataModels.FeedOperationStateMessage.Enum;
using ViewModel.DataModels.MessengerKey.Class;
using ViewModel.Services.CommandNotifyService.Class;
using ViewModel.Services.RangeObservableCollectionService.Class;
using ViewModel.ViewModelModules.RssItemViewModelModule.Class;

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
            throw new NotImplementedException();

            this.SendOperationStartedMessage();
            try
            {

            }
            catch (Exception exception)
            {
                
            }
            finally
            {
                this.SendOperationEndedMessage();
            }
        }


        private bool CanDownLoadRssItems()
        {
            return this.LoadRssItemsCommand.IsRunning == false;
        }


        private async Task DownLoadRssItems(CancellationToken cancellationToken)
        {
            this.SendOperationStartedMessage();
            try
            {
                this.SendStartingMessages(isDownloadOperation: true);
                RssItemDtosResult rssItemDtosResult = await this.downLoadRssItemsUseCase.ExecuteAsync(new Uri(this.Id), cancellationToken);
                if (rssItemDtosResult.CreatedRssItemDtos != null)
                {
                    ICollection<RssItemViewModel> rssItemViewModels = this.ConvertToRssItemViewModels(rssItemDtosResult.CreatedRssItemDtos);
                    this.InsertToBeginningIndexOfRssItemViewModels(rssItemViewModels);
                }
                this.SendEndingMessages(rssItemDtosResult, isDownloadOperation: true);
            }
            catch (Exception exception)
            {
                /// خطاهای XML/شبکه (که در Orchestrator گرفته نشدن) اینجا گرفته می‌شن
                WeakReferenceMessenger.Default.Send(exception, MessengerKey.Exception_ErrorKey);
            }
            finally
            {
                this.SendOperationEndedMessage();
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


        private void SendOperationStartedMessage()
        {
            WeakReferenceMessenger.Default.Send(new FeedOperationStateMessage(FeedOperationState.Started, this.Id), 
                MessengerKey.FeedOperationStateMessage_RunningStateKey);
        }


        private void SendOperationEndedMessage()
        {
            WeakReferenceMessenger.Default.Send(new FeedOperationStateMessage(FeedOperationState.Ended, this.Id),
                MessengerKey.FeedOperationStateMessage_RunningStateKey);
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


        private void SendStartingMessages(bool isDownloadOperation)
        {
            string operationName = isDownloadOperation ? "دریافت خبر جدید" : "بارگذاری";
            string startingMessage = $"عملیات {operationName} برای دسته‌ی خبریِ \"{this.Title}\" شروع شد .";
            WeakReferenceMessenger.Default.Send(startingMessage, MessengerKey.String_GeneralMessageKey);
        }


        private void SendEndingMessages(RssItemDtosResult result, bool isDownloadOperation)
        {
            string operationName = isDownloadOperation ? "دریافت خبر جدید" : "بارگذاری";
            string operationContext = $"عملیات {operationName} برای دسته‌ی خبریِ \"{this.Title}\"";
            /// ۱. لغو کاربر — بالاترین اولویت
            if (result.WasUserCancelled)
            {
                string cancelMessage = (result.CreatedRssItemDtos.Count > 0)
                    ? $"{operationContext} لغو شد.\n{result.CreatedRssItemDtos.Count} خبر در کش موقت ذخیره شد."
                    : $"{operationContext} لغو شد.";
                WeakReferenceMessenger.Default.Send(cancelMessage, MessengerKey.String_GeneralMessageKey);
                return;
            }

            int createdCount = result.CreatedRssItemDtos.Count;
            int errorCount = result.CreationErrors.Count;
            /// ۲. حالت بدون خطا: پیام موفقیت/خالی
            if (errorCount == 0)
            {
                if (createdCount > 0)
                    WeakReferenceMessenger.Default.Send($"{operationContext} با موفقیت تمام شد.\n{createdCount} خبر جدید دریافت شد.",
                        MessengerKey.String_GeneralMessageKey);
                else
                    WeakReferenceMessenger.Default.Send($"{operationContext} تمام شد .\nهیچ خبر جدیدی وجود ندارد.",
                        MessengerKey.String_GeneralMessageKey);
                return;
            }

            /// ۳. حالت با خطا: پیام واحد و کامل (نه دو پیام متناقض)
            string errorSummary;
            if (createdCount > 0)
            {
                // موفقیت نسبی — خطا + موفقیت
                errorSummary = $"{operationContext} تمام شد .\n{createdCount} خبر با موفقیت دریافت شد، ولی دریافتِ {errorCount} خبر با شکست مواجه شد.\n" +
                    $"دلیل خطای اول:\n\n{result.CreationErrors[0].Message}";
            }
            else
            {
                // هیچ خبری + خطا (منبع خالی نبود؛ خطا داد)
                errorSummary = $"{operationContext} با شکست مواجه شد .\nهیچ خبری دریافت نشد.\n" +
                    $"{errorCount} خطا رخ داد. دلیل خطای اول:\n\n{result.CreationErrors[0].Message}";
            }
            Exception firstError = new(errorSummary, result.CreationErrors[0]);
            WeakReferenceMessenger.Default.Send(firstError, MessengerKey.Exception_ErrorKey);
        }

    }
}
