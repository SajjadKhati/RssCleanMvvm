using Application.InfrastructureContracts.XmlProject.SharedDataModels.Class;
using Domain.DomainEntities.RssEntity.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.RssItemsDownLoaderService.Interface
{
    public interface IRssItemsDownLoader
    {

        RssItem DownLoadRssItems(Uri rssFeedLink, XmlParsingConfig xmlParsingConfig);

    }
}
