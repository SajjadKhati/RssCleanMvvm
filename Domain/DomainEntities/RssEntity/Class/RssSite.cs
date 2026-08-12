using Domain.Utility.Validators;

namespace Domain.DomainEntities.RssEntity.Class
{
    /// <summary>
    /// این کلاس ، یک Aggregate Root هست .
    /// </summary>
    public class RssSite
    {

        private Uri _id;


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

        public IList<RssFeed> RssFeeds {  get; set; }

        public string SiteName { get; set; }


        /////////////////////////////////////////////////////////


        public RssSite(Uri id)
        {
            this.Id = id;
        }

        public RssSite(Uri id, IList<RssFeed> rssFeeds, string siteName) : this(id)
        {
            this.RssFeeds = rssFeeds;
            this.SiteName = siteName;
        }

    }
}
