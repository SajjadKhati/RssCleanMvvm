namespace Domain.ApplicationServiceContracts.RssItemAllSourcesCoordinator.Interface
{
    public interface IRssItemExistenceChecker
    {
        bool IsRssItemExist(Uri rssItemId);
    }
}
