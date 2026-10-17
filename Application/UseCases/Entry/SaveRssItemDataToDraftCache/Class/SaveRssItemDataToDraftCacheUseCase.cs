using Application.OverallDataModels.OverallDto.CompositeDto;
using Application.OverallDataModels.OverallDto.EntityDto;
using Application.UseCases.Entry.SaveRssItemDataToDraftCache.DataModels;
using Application.UseCases.Entry.SaveRssItemDataToDraftCache.Interface;
using Application.UseCases.Shared.SaveRssItemToDraftCache.Interface;
using Domain.DomainEntities.RssEntity.Class;

namespace Application.UseCases.Entry.SaveRssItemDataToDraftCache.Class
{
    public class SaveRssItemDataToDraftCacheUseCase : ISaveRssItemDataToDraftCacheUseCase
    {

        private ISaveRssItemToDraftCacheOrchestrator saveRssItemToDraftCacheOrchestrator;


        /////////////////////////////////////////////////////////////////////


        public SaveRssItemDataToDraftCacheUseCase(ISaveRssItemToDraftCacheOrchestrator saveRssItemToDraftCacheOrchestrator)
        {
            this.saveRssItemToDraftCacheOrchestrator = saveRssItemToDraftCacheOrchestrator;
        }


        /////////////////////////////////////////////////////////////////////


        public async Task<bool> ExecuteAsync(RssItemDataEditableInput rssItemDataEditableInput)
        {
            return await Task.Run(() => this.Execute(rssItemDataEditableInput));
        }


        /////////////////////////////////////////////////////////////////////


        private bool Execute(RssItemDataEditableInput rssItemDataEditableInput)
        {
            return this.saveRssItemToDraftCacheOrchestrator.SaveToDraftCache(rssItemDataEditableInput.Id,
                (RssItemWithDto currentRssItemWithDto) =>
                {
                    if (currentRssItemWithDto ==null || currentRssItemWithDto.RssItem == null || currentRssItemWithDto.RssItemDto == null)
                        throw new ArgumentNullException($"مقدار {nameof(currentRssItemWithDto)} یا اعضاهایش نباید خالی باشد .");

                    RssItem updatedRssItem = new RssItem(currentRssItemWithDto.RssItem);
                    updatedRssItem.Description = rssItemDataEditableInput.Description;
                    RssItemDto updatedRssItemDto = new RssItemDto(currentRssItemWithDto.RssItemDto);
                    updatedRssItemDto.Description = rssItemDataEditableInput.Description;
                    return new RssItemWithDto(updatedRssItem, updatedRssItemDto);
                });
        }

    }
}
