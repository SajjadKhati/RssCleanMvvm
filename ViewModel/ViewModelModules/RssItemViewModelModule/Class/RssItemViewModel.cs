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
using ViewModel.Services.SharedMessengerKeyService.Class;

namespace ViewModel.ViewModelModules.RssItemViewModelModule.Class
{
    public class RssItemViewModel : ObservableObject
    {

        private ISaveRssItemDataToDraftCacheUseCase saveRssItemDataToDraftCacheUseCase;


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
            set => this.SetProperty(ref this._isUnsaved, value);
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


        public IAsyncRelayCommand SaveRssItemDataToDraftCacheCommand { get; }

        public IAsyncRelayCommand CommitRssItemCommand {  get; }


        /////////////////////////////////////////////////////////////////////////


        public RssItemViewModel(string id, string link, string category, string title, string description, string persianPublishDate, string imageFullFilePath,
            bool isUnsaved , ISaveRssItemDataToDraftCacheUseCase saveRssItemDataToDraftCacheUseCase)
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

            this.SaveRssItemDataToDraftCacheCommand = new AsyncRelayCommand(this.SaveRssItemDataToDraftCache);
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
                    throw new InvalidOperationException("آیتم مورد نظر ، در کش موقت ذخیره نشد.");

                this.UpdateDataFromDto(draftCacheRssItemDto);
                this.CommitRssItemCommand.NotifyCanExecuteChanged();
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

    }
}
