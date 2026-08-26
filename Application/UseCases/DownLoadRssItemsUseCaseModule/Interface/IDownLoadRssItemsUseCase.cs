using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.OverallDataModels.OverallDto.EntityDto;

namespace Application.UseCases.DownLoadRssItemsUseCaseModule.Interface
{
    public interface IDownLoadRssItemsUseCase
    {

        Task<RssItemDto> ExecuteAsync(Uri rssFeedLink, string xmlParentElementName,
            IList<string> xmlChildElementNames, IList<string> xmlAttributeNames);

    }
}
