using Application.OverallDataModels.OverallDto.EntityDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.LoadRssItemsUseCaseModule.Interface
{
    public interface ILoadRssItemsUseCase
    {

        Task<RssItemDto> ExecuteAsync(Uri rssFeedLink, string xmlParentElementName,
            IList<string> xmlChildElementNames, IList<string> xmlAttributeNames);

    }
}
