namespace ViewModel.DataModels
{
    public class RssFeedInfo
    {

        public string Id { get; }

        public string Title { get; }


        ////////////////////////////////////////////////


        public RssFeedInfo(string id, string title)
        {
            this.Id = id;
            this.Title = title;
        }

    }
}
