using Application.OverallDataModels.OverallDto.EntityDto;
using Application.UseCases.Entry.CommitAllRssItems.Interface;
using Application.UseCases.Entry.LoadRssSite.Interface;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ViewModel.Contracts.ViewContracts.ConfirmationDialog.Interface;
using ViewModel.Contracts.ViewContracts.WaitingDialog.Interface;
using ViewModel.DataModels.MessengerDataModels.FeedOperationStateMessage.Class;
using ViewModel.DataModels.MessengerDataModels.FeedOperationStateMessage.Enum;
using ViewModel.DataModels.MessengerKey.Class;
using ViewModel.Services.RangeObservableCollectionService.Class;
using ViewModel.Services.RssSiteViewModelMapperService.Class;
using ViewModel.ViewModelModules.RssFeedViewModelModule.Class;
using ViewModel.ViewModelModules.RssItemViewModelModule.Class;
using ViewModel.ViewModelModules.RssSiteViewModelModule.Class;

namespace ViewModel.ViewModelModules.RssUserControlViewModelModule.Class
{
    public class RssUserControlViewModel : ObservableObject, IDisposable
    {

        private ILoadRssSiteUseCase loadRssSiteUseCase;

        private ICommitAllRssItemsUseCase commitAllRssItemsUseCase;

        private ICommitAllRssItemsNoResultUseCase commitAllRssItemsNoResultUseCase;

        private IWaitingDialog waitingDialog;

        private IConfirmationDialog confirmationDialog;

        private RssSiteInfoMapper rssSiteInfoMapper;


        /////////////////////////////////////////////////////////////////////////////


        private RssSiteViewModel _selectedRssSiteViewModel;

        /// <summary>
        /// SynchronizationContext نخ UI که در زمان ساخت ViewModel (روی UI thread) کپچر می‌شود.
        ///
        /// چرا لازم است؟
        /// رویداد HasUnsavedItemsChanged از نخ پس‌زمینه fire می‌شود (چون ریشه در تغییرات
        /// کش موقت دارد که داخل Task.Run اجرا می‌شوند). handler این رویداد،
        /// NotifyCanExecuteChanged را صدا می‌زند که فقط از UI thread قابل دسترسی است.
        /// پس با Post (نه Send، برای جلوگیری از deadlock)، آن را به UI thread می‌فرستیم.
        ///
        /// چرا فقط یک‌بار در Constructor کپچر می‌شود؟
        /// SynchronizationContext.Current به نخ فعلی وابسته است. اگر دیرتر (در نخ
        /// پس‌زمینه) خوانده شود، context اشتباه را می‌گیرد. ضمناً در WPF همیشه غیر-null
        /// است؛ null بودن یعنی ViewModel روی نخ غیر-UI ساخته شده (باگ در CompositionRoot).
        /// </summary>
        private readonly SynchronizationContext uiSynchronizationContext;

        /// <summary>
        /// تعداد عملیات‌هایی (Load یا Download در RssFeedViewModelها) که در حال اجرا هستند.
        /// تا وقتی این مقدار > 0 است، کامند CommitAllRssItems غیرفعال می‌ماند
        /// (چون ذخیره‌ی همه در حین دانلود/لود، می‌تواند نتیجه‌ی ناقص بدهد).
        /// با پیام‌های FeedOperationStateMessage (Started/Ended) به‌روزرسانی می‌شود.
        /// </summary>
        private int saveAllBlockingOperationsCount;

        private bool isDisposed;


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


        public IAsyncRelayCommand LoadRssSiteCommand {  get; }

        /// <summary>
        /// کامند ذخیره‌ی همه‌ی آیتم‌های ذخیره‌نشده.
        ///
        /// ⚠️ چرا IAsyncRelayCommand نیست و IRelayCommand است؟
        ///
        /// این کامند، برخلاف Load/Download، خودش منطق async اجرا نمی‌کند.
        /// فقط یک عملیات synchronous شروع می‌کند: «باز کردن پنجره‌ی انتظار و
        /// اجرای منطق داخل آن». کل کار async (ExecuteCommitAllRssItemsAsync)
        /// در دل WaitingWindow.Loaded اجرا می‌شود (نه در بدنه‌ی این کامند).
        ///
        /// پس از دید WPF، این کامند فوراً برمی‌گردد (بدون await) — و IRelayCommand
        /// (نه IAsyncRelayCommand) دقیقاً همین را توصیف می‌کند.
        ///
        /// ⚠️ تفاوت با LoadRssItemsCommand/DownLoadRssItemsCommand:
        ///   - آن‌ها async هستند (خودشان await دارند) → IAsyncRelayCommand.
        ///   - این کامند sync است (کار async را به WaitingWindow.Loaded می‌سپارد) → IRelayCommand.
        ///
        /// ⚠️ پیامد: IsRunning این کامند وجود ندارد. ولی چون WaitingWindow modal
        /// است، کاربر تا بسته‌شدن آن نمی‌تواند دوباره روی کامند کلیک کند — پس
        /// خطر double-execution وجود ندارد.
        /// </summary>
        public IRelayCommand CommitAllRssItemsCommand { get; }

        /// <summary>
        /// کامند ذخیره‌ی همه‌ی آیتم‌های ذخیره‌نشده، بدون برگرداندن DTO (برای هنگام خروج).
        ///
        /// ⚠️ چرا در این ViewModel (RssUserControlViewModel) و نه MainWindowViewModel؟
        ///
        /// ۱. مالکیت منطق:
        ///    این کامند، عملیات commit آیتم‌های RSS را انجام می‌دهد — که کاملاً مربوط
        ///    به دامنه‌ی RssUserControl است. MainWindowViewModel نباید درگیر جزئیات
        ///    commit آیتم‌های RSS شود؛ وظیفه‌ی آن، مدیریت پنجره (منو، StatusBar، تم، ...) است.
        /// </summary>
        public IAsyncRelayCommand CommitAllRssItemsNoResultCommand { get; }


        /////////////////////////////////////////////////////////////////////////////


        public RssUserControlViewModel(ILoadRssSiteUseCase loadRssSiteUseCase, ICommitAllRssItemsUseCase commitAllRssItemsUseCase,
            ICommitAllRssItemsNoResultUseCase commitAllRssItemsNoResultUseCase,  IWaitingDialog waitingDialog, 
            IConfirmationDialog confirmationDialog, RssSiteInfoMapper rssSiteInfoMapper)
        {
            this.loadRssSiteUseCase = loadRssSiteUseCase;
            this.commitAllRssItemsUseCase = commitAllRssItemsUseCase;
            this.commitAllRssItemsUseCase.HasUnsavedItemsChanged += this.CommitAllRssItemsUseCase_HasUnsavedItemsChanged;
            this.commitAllRssItemsNoResultUseCase = commitAllRssItemsNoResultUseCase;
            this.waitingDialog = waitingDialog;
            this.confirmationDialog = confirmationDialog;
            this.rssSiteInfoMapper = rssSiteInfoMapper;

            this.uiSynchronizationContext = SynchronizationContext.Current?? 
                throw new InvalidOperationException($"{nameof(RssUserControlViewModel)} باید روی نخ اصلی ساخته شود.");
            this.isDisposed = false;

            this.RssSiteViewModels = new RangeObservableCollection<RssSiteViewModel>();
            this.LoadRssSiteCommand = new AsyncRelayCommand(this.LoadRssSite);
            this.CommitAllRssItemsCommand = new RelayCommand(this.CommitAllRssItems, this.CanCommitAllRssItems);
            this.CommitAllRssItemsNoResultCommand = new AsyncRelayCommand(CommitAllRssItemsNoResult, CanCommitAllRssItemsNoResult);
            this.RegisterMessageHandlers();
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
                WeakReferenceMessenger.Default.Send(exception, MessengerKey.Exception_ErrorKey);
            }
        }


        /// <summary>
        /// شرط فعال بودن کامند CommitAllRssItems:
        ///   ۱. هیچ عملیات Load/Download در حال اجرا نباشد.
        ///   ۲. آیتم ذخیره‌نشده‌ای در کش موقت وجود داشته باشد.
        /// </summary>
        private bool CanCommitAllRssItems()
        {
            return this.saveAllBlockingOperationsCount == 0 && this.commitAllRssItemsUseCase.HasUnsavedItems() == true;
        }


        /// <summary>
        /// نقطه‌ی ورود کامند «ذخیره‌ی همه» (bind به CommitAllRssItemsCommand).
        ///
        /// این متد (sync)، پنجره‌ی انتظار را باز می‌کند و منطق واقعی
        /// (ExecuteCommitAllRssItemsAsync) را به آن می‌سپارد. سپس فوراً برمی‌گردد.
        ///
        /// ⚠️ نکته‌ی مهم برای درک جریان اجرا:
        /// این متد، ExecuteCommitAllRssItemsAsync را «مستقیماً» اجرا نمی‌کند.
        /// فقط یک delegate (اشاره‌گر به ExecuteCommitAllRssItemsAsync) را به
        /// WaitingDialog می‌دهد. اجرای واقعی، بعداً و از مسیر دیگری رخ می‌دهد
        /// (توضیح در RunLogicInWaitingDialog و WaitingWindow.Loaded).
        /// </summary>
        private void CommitAllRssItems()
        {
            this.waitingDialog.RunLogicInWaitingDialog(this.ExecuteCommitAllRssItemsAsync);
        }


        /// <summary>
        /// منطق تجاری commit همه‌ی آیتم‌های ذخیره‌نشده.
        ///
        /// ⚠️ این متد، مستقیماً از CommitAllRssItems اجرا نمی‌شود!
        ///
        /// جریان واقعی اجرا:
        ///   ۱. CommitAllRssItems (sync) → RunLogicInWaitingDialog(delegate) → برمی‌گردد
        ///   ۲. RunLogicInWaitingDialog → WaitingWindow بساز، Loaded += handler، ShowDialog()
        ///   ۳. ShowDialog → UI thread رو بلاک می‌کند (ولی nested pump دارد)
        ///   ۴. WaitingWindow رندر می‌شود → Loaded event fire می‌شود
        ///   ۵. Loaded delegate (که در RunLogicInWaitingDialog تعریف شده) اجرا می‌شود
        ///   ۶. داخل Loaded delegate: await logicFuncAsync()
        ///      ← اینجاست که ExecuteCommitAllRssItemsAsync واقعاً شروع می‌شود
        ///
        /// یعنی: اجرای ExecuteCommitAllRssItemsAsync، از Loaded event شروع می‌شود
        /// (نه از فراخوانی RunLogicInWaitingDialog). بین این دو، یک بار رندر/نمایش
        /// WaitingWindow فاصله است.
        /// </summary>
        /// <returns></returns>
        private async Task ExecuteCommitAllRssItemsAsync()
        {
            try
            {
                IList<RssItemDto> rssItemDtos = await this.commitAllRssItemsUseCase.ExecuteAsync();
                this.UpdateViewModelsWithCommittedDtos(rssItemDtos);
                this.SendGeneralMessage( $"تعداد {rssItemDtos.Count} خبر در دیتابیس با موفقیت ذخیره شد.");
            }
            catch (Exception exception)
            {
                WeakReferenceMessenger.Default.Send(exception, MessengerKey.Exception_ErrorKey);
            }
        }


        private bool CanCommitAllRssItemsNoResult()
        {
            return this.commitAllRssItemsNoResultUseCase.HasUnsavedItems() == true;
        }


        private async Task CommitAllRssItemsNoResult()
        {
            try
            {
                string message = "آیتم‌های ذخیره‌نشده‌ای در کش موقت وجود دارد. آیا می‌خواهید قبل از خروج، آن‌ها را در پایگاه داده ذخیره کنید؟";
                string title = "وجود خبرهای ذخیره نشده";
                if (this.confirmationDialog.ShowConfirmation(message, title) == false)
                    return;
                await this.commitAllRssItemsNoResultUseCase.ExecuteAsync();
            }
            catch (Exception exception)
            {
                WeakReferenceMessenger.Default.Send(exception, MessengerKey.Exception_ErrorKey);
            }    
        }


        public void Dispose()
        {
            this.Dispose(true);
        }


        /////////////////////////////////////////////////////////////////////////////


        /// <summary>
        /// ثبت‌نام این ViewModel به‌عنوان گیرنده‌ی پیام‌های لازم:
        ///   • FeedOperationStateMessage_RunningStateKey → برای ردیابی عملیات‌های در حال اجرا.
        ///
        /// نکته: چون WeakReferenceMessenger از weak reference استفاده می‌کند،
        /// نیازی به Unregister در Dispose نیست.
        /// </summary>
        private void RegisterMessageHandlers()
        {
            WeakReferenceMessenger.Default.Register<FeedOperationStateMessage, string>(this,
                MessengerKey.FeedOperationStateMessage_RunningStateKey,
                (object recipient, FeedOperationStateMessage message) => this.TrackSaveAllBlockingOperations(message));
        }


        /// <summary>
        /// با دریافت پیام Started/Ended از هر RssFeedViewModel، شمارنده‌ی
        /// عملیات‌های در حال اجرا را به‌روزرسانی می‌کند و سپس CanExecute
        /// کامند CommitAllRssItems را دوباره ارزیابی می‌کند.
        ///
        /// اگر شمارنده منفی شود، به معنی یک Ended بدون Started متناظر است (باگ).
        /// مقدار به صفر reset می‌شود (برای بازیابی) و سپس استثنا پرتاب می‌شود
        /// تا باگ در دیباگ دیده شود.
        /// </summary>
        /// <param name="feedOperationStateMessage"></param>
        /// <exception cref="Exception"></exception>
        private void TrackSaveAllBlockingOperations(FeedOperationStateMessage feedOperationStateMessage)
        {
            if (feedOperationStateMessage.State == FeedOperationState.Started)
                this.saveAllBlockingOperationsCount++;
            else if(feedOperationStateMessage.State == FeedOperationState.Ended)
                this.saveAllBlockingOperationsCount--;

            if (this.saveAllBlockingOperationsCount < 0)
            {
                this.saveAllBlockingOperationsCount = 0;
                throw new Exception($"مقدار {this.saveAllBlockingOperationsCount} نباید منفی شود .");
            }
            this.CommitAllRssItemsCommand.NotifyCanExecuteChanged();
        }


        /// <summary>
        /// هر بار وضعیت «وجود آیتم ذخیره‌نشده» در کش موقت تغییر کند (true ↔ false)،
        /// CanExecute کامند CommitAllRssItems دوباره ارزیابی می‌شود.
        /// (پارامتر hasUnsavedItems استفاده نمی‌شود — خود CanExecute مجدداً می‌پرسد.)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="hasUnsavedItems"></param>
        private void CommitAllRssItemsUseCase_HasUnsavedItemsChanged(object sender, bool hasUnsavedItems)
        {
            /// اگه روی UI thread هستیم، مستقیم ، یعنی در همون نخ اصلی اجرا بشه؛ 
            if (this.uiSynchronizationContext == SynchronizationContext.Current)
            {
                this.CommitAllRssItemsCommand.NotifyCanExecuteChanged();
                return;
            }
            /// وگرنه Post به UI thread
            this.uiSynchronizationContext.Post(_ => this.CommitAllRssItemsCommand.NotifyCanExecuteChanged(), null);
        }


        protected virtual void Dispose(bool disposing)
        {
            if (disposing == false || this.isDisposed == true)
                return;
            this.isDisposed = true;
            this.commitAllRssItemsUseCase.HasUnsavedItemsChanged -= this.CommitAllRssItemsUseCase_HasUnsavedItemsChanged;
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


        /// <summary>
        /// DTOهای commit‌شده را روی RssItemViewModelهای متناظر اعمال می‌کند.
        ///
        /// چرا این کار لازم است؟
        /// در فرآیند commit، فایل تصویر از پوشه‌ی موقت (temp) به پوشه‌ی دائمی (persistent)
        /// منتقل می‌شود، پس ImageFullFilePath عوض می‌شود. VM باید مسیر جدید را بداند،
        /// وگرنه UI به فایل حذف‌شده (temp) اشاره می‌کند و تصویر broken می‌شود.
        /// </summary>
        /// <param name="committedDtos"></param>
        private void UpdateViewModelsWithCommittedDtos(IList<RssItemDto> committedDtos)
        {
            if (committedDtos == null || committedDtos.Count == 0)
                return;

            /// lookup سریع: Id → DTO
            /// چرا Dictionary؟
            /// RssItemViewModelها در سه سطح تودرتو هستند (Site → Feed → Item). جستجوی خطی
            /// برای هر آیتم، O(n*m) می‌شود. با Dictionary، O(n+m).
            /// و کلا پیدا کردن در دیکشنری ، O(1) هست خیلی سریعتر هست . هر چند ، برای اولین بار ، برای نوشتن در دیکشنری ، یک دور چرخه نیاز هست .
            Dictionary<string, RssItemDto> dtosById = new(committedDtos.Count);
            foreach (RssItemDto dto in committedDtos)
                dtosById[dto.Id] = dto;

            foreach (RssSiteViewModel site in this.RssSiteViewModels)
            {
                foreach (RssFeedViewModel feed in site.RssFeedViewModels)
                {
                    foreach (RssItemViewModel item in feed.RssItemViewModels)
                    {
                        if (dtosById.TryGetValue(item.Id, out RssItemDto? dto))
                            item.UpdateImage(dto.ImageFullFilePath, dto.IsUnsaved);
                    }
                }
            }
        }


        private void SendGeneralMessage(string message)
        {
            WeakReferenceMessenger.Default.Send(message, MessengerKey.String_GeneralMessageKey);
        }

    }
}
