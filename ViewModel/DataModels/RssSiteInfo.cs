using System.Collections.ObjectModel;

namespace ViewModel.DataModels
{
    public class RssSiteInfo
    {

        public string Id { get; }

        public ObservableCollection<RssFeedInfo> RssFeedInfos { get; }

        public string SiteName { get; }


        ////////////////////////////////////////////////


        public RssSiteInfo(string id, IEnumerable<RssFeedInfo> rssFeedInfos, string siteName)
        {
            this.Id = id;
            this.RssFeedInfos = new ObservableCollection<RssFeedInfo>(rssFeedInfos);
            this.SiteName = siteName;
        }

    }
}
