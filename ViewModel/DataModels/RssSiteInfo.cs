using ViewModel.Services.RangeObservableCollectionService.Class;

namespace ViewModel.DataModels
{
    public class RssSiteInfo
    {

        public string Id { get; }

        public RangeObservableCollection<RssFeedInfo> RssFeedInfos { get; }

        public string SiteName { get; }


        ////////////////////////////////////////////////


        public RssSiteInfo(string id, IEnumerable<RssFeedInfo> rssFeedInfos, string siteName)
        {
            this.Id = id;
            this.RssFeedInfos = new RangeObservableCollection<RssFeedInfo>(rssFeedInfos);
            this.SiteName = siteName;
        }

    }
}
