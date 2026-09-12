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
                if(this.SetProperty(ref this._imageFullFilePath, value) == true)
                {

                }
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
