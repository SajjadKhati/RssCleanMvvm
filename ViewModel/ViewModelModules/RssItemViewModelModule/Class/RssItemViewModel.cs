using Application.OverallDataModels.OverallDto.EntityDto;
using Application.UseCases.Entry.SaveRssItemDataToDraftCache.DataModels;
using Application.UseCases.Entry.SaveRssItemDataToDraftCache.Interface;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.UseCases.Entry.ConvertRssItemImageToGrayscale.Interface;
using ViewModel.Services.SharedMessengerKeyService.Class;

namespace ViewModel.ViewModelModules.RssItemViewModelModule.Class
{
    public class RssItemViewModel : ObservableObject
    {

        private ISaveRssItemDataToDraftCacheUseCase saveRssItemDataToDraftCacheUseCase;

        private IConvertRssItemImageToGrayscaleUseCase convertRssItemImageToGrayscaleUseCase;


        /////////////////////////////////////////////////////////////////////////


        private string _description;

        private string _imageFullFilePath;

        private bool _isInEditMode;

        private bool _isUnsaved;


        /////////////////////////////////////////////////////////////////////////


        public string Id { get; }

        public string Link { get; }

        public string Category { get; }

        public string Title { get; }

        public string Description
        {
            get => this._description;
            set => this.SetProperty(ref this._description, value);
        }

        public string PersianPublishDate { get; }

        public string ImageFullFilePath
        {
            get => this._imageFullFilePath;
            set
            {
                /// دقت کنید که برای این پروپرتی ، نباید شرط بذاریم که اگه مقدارش تغییر نکرد ، رویداد OnPropertyChanged اجرا بشه .
                /// چون تصویر گاها ممکنه که همون مسیر فایل قبل باشه اما محتوای تصویر ، تغییر کنه (مثل زمانی که تصویر ، تبدیل به سیاه و سفید میشه) .
                /// متد SetProperty را هم فراخوانی نکردیم چون این متد ، شرط تغییر مقدار را هم اجرا میکنه . بجایش از OnPropertyChanged استفاده کردیم .
                this._imageFullFilePath = value;
                this.OnPropertyChanged();
            }
        }

        public bool IsUnsaved
        {
            get => this._isUnsaved;
            set
            {
                if (this.SetProperty(ref _isUnsaved, value) == true && this.CommitRssItemCommand != null)
                    this.CommitRssItemCommand.NotifyCanExecuteChanged();
            }
        }


        /////////////////////////////////////////////////////////////////////////


        public bool IsInEditMode
        {
            get => this._isInEditMode;
            set
            {
                this.SetProperty(ref this._isInEditMode, value);
            }
        }


        /////////////////////////////////////////////////////////////////////////


        /// <summary>
        /// ذخیره‌ی داده‌های ویرایش‌شده (متن) در کش موقت.
        ///
        /// Trigger: Binding.SourceUpdated روی TextBox متن خبر.
        /// یعنی وقتی کاربر متن Description را ویرایش می‌کند و از TextBox خارج می‌شود
        /// (LostFocus)، این کامند به‌طور خودکار اجرا می‌شود.
        ///
        /// چرا ذخیره در DraftCache و نه دیتابیس؟
        /// چون تغییرات هنوز نهایی نشده‌اند. کاربر ممکن است چند خبر را ویرایش کند
        /// و در نهایت با دکمه‌ی «ذخیره همه» همه را به دیتابیس منتقل کند.
        /// DraftCache، محل نگهداری موقت تغییرات ذخیره‌نشده است.
        /// </summary>
        public IAsyncRelayCommand SaveRssItemDataToDraftCacheCommand { get; }

        /// <summary>
        /// تبدیل تصویر خبر به سیاه و سفید، و ذخیره‌ی نتیجه در کش موقت.
        ///
        /// Trigger: کلیک کاربر روی دکمه‌ی «تبدیل به سیاه و سفید».
        /// برخلاف ویرایش متن (که trigger آن نامرئی است)، این یک اقدام صریح
        /// از طرف کاربر است.
        ///
        /// جریان:
        ///   ۱. تصویر فعلی خبر از DraftCache یا دیتابیس خوانده می‌شود.
        ///   ۲. تبدیل به grayscale روی آرایه‌ی پیکسل‌ها انجام می‌شود.
        ///   ۳. فایل تصویر (با همان مسیر hash-based، چون شناسه تغییر نمی‌کند) بازنویسی می‌شود.
        ///   ۴. RssItem و RssItemDto به‌روزشده در DraftCache ذخیره می‌شوند
        ///      (با وضعیت Modified یا Added).
        ///
        /// ⚠️ چرا CanExecute ندارد؟
        ///
        /// تشخیص «آیا این تصویر قبلاً سیاه‌و‌سفید شده؟» از دو راه ممکن است و هر دو
        /// در این نسخه انجام نمی‌شوند:
        ///
        ///   ۱. یک ستون جداگانه در دیتابیس (مثلاً IsGrayscaled) که فقط با
        ///      بارگذاری از DB قابل بررسی است. در این نسخه چنین ستونی اضافه نشده.
        ///
        ///   ۲. مقایسه‌ی تمام کانال‌های R/G/B هر پیکسل برای برابری. این کار برای
        ///      هر بار hover یا بررسی، هزینه‌ی سنگین O(width × height) دارد و
        ///      در این نسخه انجام نمی‌شود.
        ///
        /// بنابراین CanExecute همیشه true است و کاربر (حتی روی تصویر رنگی) می‌تواند
        /// دکمه را بزند. اگر تصویر از قبل grayscale باشد، تبدیل یک no-op است (همه‌ی
        /// مقادیر R=G=B می‌مانند) و صرفاً فایل با همان محتوا بازنویسی می‌شود —
        /// که هزینه‌ی ناچیزی است.
        ///
        /// چرا ذخیره در DraftCache و نه دیتابیس؟
        /// چون مثل ویرایش متن، این تغییر هم هنوز نهایی نشده. کاربر ممکن است
        /// چند تصویر را تبدیل کند و بعد با «ذخیره همه» به دیتابیس منتقل کند.
        /// </summary>
        public IAsyncRelayCommand ConvertRssItemImageToGrayscaleCommand { get; }

        public IAsyncRelayCommand CommitRssItemCommand {  get; }


        /////////////////////////////////////////////////////////////////////////


        public RssItemViewModel(string id, string link, string category, string title, string description, string persianPublishDate, string imageFullFilePath, 
            bool isUnsaved , ISaveRssItemDataToDraftCacheUseCase saveRssItemDataToDraftCacheUseCase,
            IConvertRssItemImageToGrayscaleUseCase convertRssItemImageToGrayscaleUseCase)
        {
            this.Id = id;
            this.Link = link;
            this.Category = category;
            this.Title = title;
            this.Description = description;
            this.PersianPublishDate = persianPublishDate;
            this.ImageFullFilePath = imageFullFilePath;
            this.IsUnsaved = isUnsaved;

            this.IsInEditMode = false;
            this.saveRssItemDataToDraftCacheUseCase = saveRssItemDataToDraftCacheUseCase;
            this.convertRssItemImageToGrayscaleUseCase = convertRssItemImageToGrayscaleUseCase;

            this.SaveRssItemDataToDraftCacheCommand = new AsyncRelayCommand(this.SaveRssItemDataToDraftCache);
            this.ConvertRssItemImageToGrayscaleCommand = new AsyncRelayCommand(this.ConvertRssItemImageToGrayscale);
            this.CommitRssItemCommand = new AsyncRelayCommand(this.CommitRssItem, this.CanExecuteCommitRssItem);
        }


        /////////////////////////////////////////////////////////////////////////


        private async Task SaveRssItemDataToDraftCache()
        {
            try
            {
                RssItemDto draftCacheRssItemDto = await this.saveRssItemDataToDraftCacheUseCase.ExecuteAsync(
                    new RssItemDataEditableInput(new Uri(this.Id), this.Description));
                if (draftCacheRssItemDto == null)
                    throw new InvalidOperationException($"آیتم در کش موقت پیدا نشد — احتمالاً همزمان، تغییر دیگری روی آن انجام شده است. لطفاً دوباره تلاش کنید.");

                this.UpdateDataFromDto(draftCacheRssItemDto);
            }
            catch (Exception exception)
            {
                WeakReferenceMessenger.Default.Send(exception, SharedMessengerKey.Exception_ErrorKey);
            }
        }


        private async Task ConvertRssItemImageToGrayscale()
        {
            try
            {
                RssItemDto draftCacheRssItemDto = await this.convertRssItemImageToGrayscaleUseCase.ExecuteAsync(new Uri(this.Id));
                if (draftCacheRssItemDto == null)
                    throw new InvalidOperationException($"آیتم در کش موقت پیدا نشد — احتمالاً همزمان، تغییر دیگری روی آن انجام شده است. لطفاً دوباره تلاش کنید.");

                this.UpdateImageFromDto(draftCacheRssItemDto);
            }
            catch (Exception exception)
            {
                WeakReferenceMessenger.Default.Send(exception, SharedMessengerKey.Exception_ErrorKey);
            }
        }


        private bool CanExecuteCommitRssItem()
        {
            /// یه usecase را فراخوانی کنه که ببینه آیا شناسه ی این آیتم ، درون کش موقت وجود داره یا نه؟
            throw new NotImplementedException();
        }


        private Task CommitRssItem()
        {
            throw new NotImplementedException();
        }


        /////////////////////////////////////////////////////////////////////////


        /// <summary>
        /// داده‌های ViewModel را با مقادیر DTO هم‌گام می‌کند (به‌جز تصویر، که متد جدا دارد).
        /// </summary>
        public void UpdateDataFromDto(RssItemDto rssItemDto)
        {
            /// چرا Description را هم دوباره set می‌کنیم، با اینکه در ۹۹.۹٪ موارد یکسان است؟
            ///
            /// در حالت عادی، Description همان است که کاربر ویرایش کرده (نخ VM آن را
            /// قبلاً نوشته) — پس SetProperty هیچ کاری نمی‌کند و PropertyChanged هم
            /// fire نمی‌شود (بدون overhead، بدون تغییر UI).
            ///
            /// اما در سناریوی نادر conflict (اگر نخ دیگری زودتر در DraftCache نوشته
            /// باشد)، DTO برگشتی نسخه‌ی cache را حمل می‌کند. در آن حالت نادر، این
            /// set کردن باعث می‌شود VM با واقعیت cache هم‌گام بماند و کاربر متوجه
            /// تفاوت شود — در غیر این صورت، UI متنی را نشان می‌داد که در cache نیست
            /// (ناهم‌گامی خاموش).
            ///
            /// این سناریو در این اپ تقریباً غیرممکن است (چون AsyncRelayCommand جلوی
            /// double-execution را می‌گیرد و برای هر آیتم یک VM وجود دارد)، ولی
            /// هزینه‌ی این دفاع صفر است و در آینده از data loss خاموش جلوگیری می‌کند.
            /// 
            /// SetProperty خودش no-op می‌کنه اگه یکسان باشه
            this.Description = rssItemDto.Description;
            this.IsUnsaved = rssItemDto.IsUnsaved;
        }


        public void UpdateImageFromDto(RssItemDto rssItemDto)
        {
            this.ImageFullFilePath = rssItemDto.ImageFullFilePath;
            this.IsUnsaved = rssItemDto.IsUnsaved;
        }

    }
}
