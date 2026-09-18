using Application.OverallDataModels.OverallDto.EntityDto;
using ViewModel.Contracts.CompositionRootContracts.RssItemViewModelFactory.Interface;
using ViewModel.ViewModelModules.RssItemViewModelModule.Class;

namespace CompositionRoot.Shared.ClassLibrary.Services.RssItemViewModelFactoryService.Class
{
    public class RssItemViewModelFactory : IRssItemViewModelFactory
    {

        public RssItemViewModel Create(RssItemDto rssItemDto)
        {
            return new RssItemViewModel(
                rssItemDto.Id,
                rssItemDto.Link,
                rssItemDto.Category,
                rssItemDto.Title,
                rssItemDto.Description,
                rssItemDto.PersianPublishDate,
                rssItemDto.ImageFullFilePath);
        }

    }
}
