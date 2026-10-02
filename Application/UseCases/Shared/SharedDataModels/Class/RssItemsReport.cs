using Domain.DomainEntities.RssEntity.Class;

namespace Application.UseCases.Shared.SharedDataModels.Class
{
    public class RssItemsReport
    {

        public IList<RssItem> CreatedRssItems { get; init; }

        public IList<Exception> CreationErrors { get; init; }


        ////////////////////////////////////////////////////////////////


        public RssItemsReport(IList<RssItem> createdRssItems)
        {
            this.CreatedRssItems = createdRssItems;
        }


        public RssItemsReport(IList<RssItem> createdRssItems, IList<Exception> creationErrors) : this(createdRssItems)
        {
            this.CreationErrors = creationErrors;
        }

    }
}
