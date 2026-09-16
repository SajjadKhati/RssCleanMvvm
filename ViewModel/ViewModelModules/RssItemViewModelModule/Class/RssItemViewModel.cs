using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.ViewModelModules.RssItemViewModelModule.Class
{
    public class RssItemViewModel : ObservableObject
    {

        /////////////////////////////////////////////////////////////////////////


        private string _description;

        private string _imageFullFilePath;


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
                if(this.SetProperty(ref this._description, value) == true)
                {

                }
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


        public RssItemViewModel(string id, string link, string category, string title, string description, string persianPublishDate, string imageFullFilePath)
        {
            this.Id = id;
            this.Link = link;
            this.Category = category;
            this.Title = title;
            this.Description = description;
            this.PersianPublishDate = persianPublishDate;
            this.ImageFullFilePath = imageFullFilePath;
        }


        /////////////////////////////////////////////////////////////////////////


    }
}
