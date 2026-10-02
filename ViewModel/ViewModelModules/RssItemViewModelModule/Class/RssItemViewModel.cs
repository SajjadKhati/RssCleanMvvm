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


        /////////////////////////////////////////////////////////////////////////


        public string Id { get; }

        public string Link { get; }

        public string Category { get; }

        public string Title { get; }

        public string Description
        {
            get => this._description;
            set
            {
                this.SetProperty(ref this._description, value);
            }
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
            ISaveRssItemDataToDraftCacheUseCase saveRssItemDataToDraftCacheUseCase)
        {
            this.Id = id;
            this.Link = link;
            this.Category = category;
            this.Title = title;
            this.Description = description;
            this.PersianPublishDate = persianPublishDate;
            this.ImageFullFilePath = imageFullFilePath;

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
                await this.saveRssItemDataToDraftCacheUseCase.ExecuteAsync(new RssItemDataEditableInput(new Uri(this.Id), this.Description));
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

    }
}
