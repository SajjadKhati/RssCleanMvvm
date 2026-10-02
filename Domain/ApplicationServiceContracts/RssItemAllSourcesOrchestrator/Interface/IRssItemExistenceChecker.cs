namespace Domain.ApplicationServiceContracts.RssItemAllSourcesOrchestrator.Interface
{
    public interface IRssItemExistenceChecker
    {
        bool IsRssItemExist(Uri rssItemId);
    }
}
