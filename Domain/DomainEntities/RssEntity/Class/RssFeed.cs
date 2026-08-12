using Domain.Utility.Validators;

namespace Domain.DomainEntities.RssEntity.Class
{
    public class RssFeed
    {

        private Uri _id;

        private Uri _rssSiteId;


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

        public Uri RssSiteId
        {
            get
            {
                return this._rssSiteId;
            }
            set
            {
                if (this._rssSiteId == value)
                    return;

                Validator.ValidateUri(value);
                this._rssSiteId = value;
            }
        }

        public string Title { get; set; }


        /////////////////////////////////////////////////////////


        public RssFeed(Uri id, Uri rssSiteId)
        {
            this.Id = id;
            this.RssSiteId = rssSiteId;
        }

        public RssFeed(Uri id, Uri rssSiteId, string title) : this(id, rssSiteId)
        {
            this.Title = title;
        }

    }
}
