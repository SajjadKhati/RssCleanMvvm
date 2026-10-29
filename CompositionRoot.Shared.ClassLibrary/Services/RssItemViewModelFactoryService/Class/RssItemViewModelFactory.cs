using Application.OverallDataModels.OverallDto.EntityDto;
using Application.UseCases.Entry.CommitRssItem.Interface;
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

        private ICommitRssItemUseCase commitRssItemUseCase;


        /////////////////////////////////////////////////////////////////////////


        public RssItemViewModelFactory(ISaveRssItemDataToDraftCacheUseCase saveRssItemDataToDraftCacheUseCase,
            IConvertRssItemImageToGrayscaleUseCase convertRssItemImageToGrayscaleUseCase, 
            ICommitRssItemUseCase commitRssItemUseCase)
        {
            this.saveRssItemDataToDraftCacheUseCase = saveRssItemDataToDraftCacheUseCase;
            this.convertRssItemImageToGrayscaleUseCase = convertRssItemImageToGrayscaleUseCase;
            this.commitRssItemUseCase = commitRssItemUseCase;
        }


        /////////////////////////////////////////////////////////////////////////


        public ICollection<RssItemViewModel> CreateRange(ICollection<RssItemDto> rssItemDtos)
        {
            if (rssItemDtos == null)
                return null;

            ICollection<RssItemViewModel> rssItemViewModels = new List<RssItemViewModel>();
            foreach (RssItemDto rssItemDto in rssItemDtos)
            {
                RssItemViewModel rssItemViewModel = this.Create(rssItemDto);
                rssItemViewModels.Add(rssItemViewModel);
            }
            return rssItemViewModels;
        }


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
                this.convertRssItemImageToGrayscaleUseCase,
                this.commitRssItemUseCase);
        }

    }
}
