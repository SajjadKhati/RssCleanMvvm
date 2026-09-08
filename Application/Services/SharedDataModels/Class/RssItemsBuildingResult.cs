using Domain.DomainEntities.RssEntity.Class;

namespace Application.Services.SharedDataModels.Class
{
    public class RssItemsBuildingResult
    {

        public IList<RssItem> CreatedRssItems { get; init; }

        public IList<Exception> CreationErrors { get; init; }


        ////////////////////////////////////////////////////////////////


        public RssItemsBuildingResult(IList<RssItem> createdRssItems)
        {
            this.CreatedRssItems = createdRssItems;
        }


        public RssItemsBuildingResult(IList<RssItem> createdRssItems, IList<Exception> creationErrors) : this(createdRssItems)
        {
            this.CreationErrors = creationErrors;
        }

    }
}
