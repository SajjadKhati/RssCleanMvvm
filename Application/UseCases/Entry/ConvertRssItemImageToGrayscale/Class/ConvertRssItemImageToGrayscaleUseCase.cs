using Application.OverallDataModels.OverallDto.CompositeDto;
using Application.OverallDataModels.OverallDto.EntityDto;
using Application.UseCases.Entry.ConvertRssItemImageToGrayscale.Interface;
using Application.UseCases.Shared.DraftRssItemDtoFactoryOrch.Interface;
using Application.UseCases.Shared.SaveRssItemToDraftCache.Interface;
using Core.ImageServices.ImageFilteringService.Interface;
using Domain.DomainEntities.RssEntity.Class;

namespace Application.UseCases.Entry.ConvertRssItemImageToGrayscale.Class
{
    public class ConvertRssItemImageToGrayscaleUseCase : IConvertRssItemImageToGrayscaleUseCase
    {

        private IImageFilter imageFilter;

        private ISaveRssItemToDraftCacheOrchestrator saveRssItemToDraftCacheOrchestrator;

        private IDraftRssItemDtoFactoryOrchestrator draftRssItemDtoFactoryOrchestrator;


        //////////////////////////////////////////////////////////////////////////////////////////


        public ConvertRssItemImageToGrayscaleUseCase(IImageFilter imageFilter, 
            ISaveRssItemToDraftCacheOrchestrator saveRssItemToDraftCacheOrchestrator, 
            IDraftRssItemDtoFactoryOrchestrator draftRssItemDtoFactoryOrchestrator)
        {
            this.imageFilter = imageFilter;
            this.saveRssItemToDraftCacheOrchestrator = saveRssItemToDraftCacheOrchestrator;
            this.draftRssItemDtoFactoryOrchestrator = draftRssItemDtoFactoryOrchestrator;
        }


        //////////////////////////////////////////////////////////////////////////////////////////


        public async Task<RssItemDto> ExecuteAsync(Uri rssItemId)
        {
            return await Task.Run(() => this.Execute(rssItemId));
        }


        //////////////////////////////////////////////////////////////////////////////////////////
        

        private RssItemDto Execute(Uri rssItemId)
        {
            return this.saveRssItemToDraftCacheOrchestrator.SaveToDraftCache(rssItemId, this.BuildRssItemWithGrayscaledImage);
        }


        private RssItemWithDto BuildRssItemWithGrayscaledImage(RssItemWithDto currentRssItemWithDto)
        {
            this.ValidateRssItemWithDto(currentRssItemWithDto);

            byte[,,] grayscaledImage = this.imageFilter.ToGrayScale(currentRssItemWithDto.RssItem.Argb8BitImage);

            RssItem updatedRssItem = new RssItem(currentRssItemWithDto.RssItem);
            updatedRssItem.Argb8BitImage = grayscaledImage;

            RssItemDto updatedRssItemDto = this.draftRssItemDtoFactoryOrchestrator.CreateRssItemDtoWithSaveImage(updatedRssItem, isUnsaved : true);

            return new RssItemWithDto(updatedRssItem, updatedRssItemDto);
        }


        private void ValidateRssItemWithDto(RssItemWithDto currentRssItemWithDto)
        {
            if (currentRssItemWithDto == null || currentRssItemWithDto.RssItem == null || currentRssItemWithDto.RssItemDto == null
                || currentRssItemWithDto.RssItem.Argb8BitImage == null)
                throw new ArgumentNullException($"مقدار {nameof(currentRssItemWithDto)} یا اعضاهایش و یا تصویرش نباید خالی باشد .");
        }

    }
}
