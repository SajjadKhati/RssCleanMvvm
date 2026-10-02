using Application.OverallDataModels.OverallDto.EntityDto;
using Application.UseCases.Entry.SaveRssItemDataToDraftCache.Interface;
using ViewModel.Contracts.CompositionRootContracts.RssItemViewModelFactory.Interface;
using ViewModel.ViewModelModules.RssItemViewModelModule.Class;

namespace CompositionRoot.Shared.ClassLibrary.Services.RssItemViewModelFactoryService.Class
{
    public class RssItemViewModelFactory : IRssItemViewModelFactory
    {

        private ISaveRssItemDataToDraftCacheUseCase saveRssItemDataToDraftCacheUseCase;


        /////////////////////////////////////////////////////////////////////////


        public RssItemViewModelFactory(ISaveRssItemDataToDraftCacheUseCase saveRssItemDataToDraftCacheUseCase)
        {
            this.saveRssItemDataToDraftCacheUseCase = saveRssItemDataToDraftCacheUseCase;
        }


        /////////////////////////////////////////////////////////////////////////


        public RssItemViewModel Create(RssItemDto rssItemDto)
        {
            return new RssItemViewModel(
                rssItemDto.Id,
                rssItemDto.Link,
                rssItemDto.Category,
                rssItemDto.Title,
                rssItemDto.Description,
                rssItemDto.PersianPublishDate,
                rssItemDto.ImageFullFilePath, 
                this.saveRssItemDataToDraftCacheUseCase);
        }

    }
}
