using Application.OverallDataModels.OverallDto.EntityDto;
using Application.UseCases.Entry.ConvertRssItemImageToGrayscale.Interface;
using Application.UseCases.Entry.SaveRssItemDataToDraftCache.Interface;
using ViewModel.Contracts.CompositionRootContracts.RssItemViewModelFactory.Interface;
using ViewModel.ViewModelModules.RssItemViewModelModule.Class;

namespace CompositionRoot.Shared.ClassLibrary.Services.RssItemViewModelFactoryService.Class
{
    public class RssItemViewModelFactory : IRssItemViewModelFactory
    {

        private ISaveRssItemDataToDraftCacheUseCase saveRssItemDataToDraftCacheUseCase;

        private IConvertRssItemImageToGrayscaleUseCase convertRssItemImageToGrayscaleUseCase;


        /////////////////////////////////////////////////////////////////////////


        public RssItemViewModelFactory(ISaveRssItemDataToDraftCacheUseCase saveRssItemDataToDraftCacheUseCase,
            IConvertRssItemImageToGrayscaleUseCase convertRssItemImageToGrayscaleUseCase)
        {
            this.saveRssItemDataToDraftCacheUseCase = saveRssItemDataToDraftCacheUseCase;
            this.convertRssItemImageToGrayscaleUseCase = convertRssItemImageToGrayscaleUseCase;
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
                rssItemDto.IsUnsaved,
                this.saveRssItemDataToDraftCacheUseCase,
                this.convertRssItemImageToGrayscaleUseCase);
        }

    }
}
