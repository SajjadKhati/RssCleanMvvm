using Domain.Utility.Validators;

namespace Domain.DomainEntities.RssEntity.Class
{
    /// <summary>
    /// این کلاس ، یک Aggregate Root هست .
    /// </summary>
    public class RssItem
    {
        private Uri _id;

        private Uri _rssFeedId;

        private Uri _link;

        private string _title;

        private byte[,,] _argb8BitImage;


        /////////////////////////////////////////////////////////


        public Uri Id
        {
            get 
            { 
                return this._id; 
            }
            set
            {
                if (this._id == value)
                    return;

                Validator.ValidateUri(value);
                this._id = value; 
            }
        }

        public Uri RssFeedId
        {
            get
            {
                return this._rssFeedId;
            }
            set
            {
                if (this._rssFeedId == value)
                    return;

                Validator.ValidateUri(value);
                this._rssFeedId = value;
            }
        }

        public Uri Link
        {
            get
            {
                return this._link;
            }
            set
            {
                if (this._link == value)
                    return;

                Validator.ValidateUri(value);
                this._link = value;
            }
        }

        public string? Category { get; set; }

        public string Title
        {
            get
            {
                return this._title;
            }
            set
            {
                if(this._title  == value)
                    return;

                this.ValidateTitle(value);
                this._title = value;
            }
        }

        public string Description { get; set; }

        public DateTime PublishDate {  get; set; }

        public byte[,,] Argb8BitImage
        {
            get
            {
                return this._argb8BitImage;
            }
            set
            {
                if (this._argb8BitImage == value)
                    return;

                this.ValidatePicture(value);
                this._argb8BitImage = value;
            }
        }


        /////////////////////////////////////////////////////////


        public RssItem(Uri id, Uri rssFeedId, Uri link, byte[,,] argb8BitImage)
        {
            this.Id = id;
            this.RssFeedId = rssFeedId;
            this.Link = link;
            this.Argb8BitImage = argb8BitImage;
        }


        public RssItem(Uri id, Uri rssFeedId, Uri link, byte[,,] argb8BitImage, string? category,string title, 
            string description, DateTime publishDate) : this(id, rssFeedId, link, argb8BitImage)
        {
            this.Category = category;
            this.Title = title;
            this.Description = description;
            this.PublishDate = publishDate;
        }


        /////////////////////////////////////////////////////////


        private void ValidatePicture(byte[,,] argb8BitImage)
        {
            if (argb8BitImage.GetLength(0) == 0 || argb8BitImage.GetLength(1) == 0)
                throw new ArgumentException("ابعاد تصویر باید بزرگتر از صفر باشد .");

            if (argb8BitImage.GetLength(2) != 4)
                throw new ArgumentException("تصویر باید 4 کانال داشته باشد .");
        }


        private void ValidateTitle(string title)
        {
            if (title != null && title.Length > 2048)
                throw new ArgumentNullException(nameof(title), $"{nameof(title)} در فیلد دامنه ، نباید اندازه ی رشته اش بیشتر از 2048 کاراکتر باشد .");
        }

    }
}
