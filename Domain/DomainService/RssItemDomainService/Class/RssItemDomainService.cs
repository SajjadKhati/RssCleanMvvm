using Domain.DomainService.RssItemDomainService.Interface;
using Domain.ApplicationServiceContracts.RssItemAllSourcesCoordinator.Interface;

namespace Domain.DomainService.RssItemDomainService.Class
{
    public class RssItemDomainService : IRssItemDomainService
    {

        private IRssItemExistenceChecker rssItemExistenceChecker;


        //////////////////////////////////////////////////////////////////


        public RssItemDomainService(IRssItemExistenceChecker rssItemExistenceChecker)
        {
            this.rssItemExistenceChecker = rssItemExistenceChecker;
        }


        //////////////////////////////////////////////////////////////////


        public bool IsRssItemValid(Uri rssItemId)
        {
            return this.rssItemExistenceChecker.IsRssItemExist(rssItemId) == false;
        }

    }
}
