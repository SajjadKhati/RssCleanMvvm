using Application.OverallDataModels.OverallDto.CompositeDto;
using Application.OverallDataModels.OverallDto.EntityDto;
using Application.UseCases.Entry.SaveRssItemDataToDraftCache.DataModels;
using Application.UseCases.Entry.SaveRssItemDataToDraftCache.Interface;
using Application.UseCases.Shared.DraftRssItemDtoFactoryOrch.Interface;
using Application.UseCases.Shared.SaveRssItemToDraftCache.Interface;
using Domain.DomainEntities.RssEntity.Class;

namespace Application.UseCases.Entry.SaveRssItemDataToDraftCache.Class
{
    public class SaveRssItemDataToDraftCacheUseCase : ISaveRssItemDataToDraftCacheUseCase
    {

        private ISaveRssItemToDraftCacheOrchestrator saveRssItemToDraftCacheOrchestrator;

        private IDraftRssItemDtoFactoryOrchestrator draftRssItemDtoFactoryOrchestrator;


        /////////////////////////////////////////////////////////////////////


        public SaveRssItemDataToDraftCacheUseCase(ISaveRssItemToDraftCacheOrchestrator saveRssItemToDraftCacheOrchestrator,
            IDraftRssItemDtoFactoryOrchestrator draftRssItemDtoFactoryOrchestrator)
        {
            this.saveRssItemToDraftCacheOrchestrator = saveRssItemToDraftCacheOrchestrator;
            this.draftRssItemDtoFactoryOrchestrator = draftRssItemDtoFactoryOrchestrator;
        }


        /////////////////////////////////////////////////////////////////////


        public async Task<RssItemDto> ExecuteAsync(RssItemDataEditableInput rssItemDataEditableInput)
        {
            return await Task.Run(() => this.Execute(rssItemDataEditableInput));
        }


        /////////////////////////////////////////////////////////////////////


        private RssItemDto Execute(RssItemDataEditableInput rssItemDataEditableInput)
        {
            return this.saveRssItemToDraftCacheOrchestrator.SaveToDraftCache(
                rssItemDataEditableInput.Id,
                (RssItemWithDto currentRssItemWithDto) => this.BuildRssItemWithUpdatedDescription(rssItemDataEditableInput, currentRssItemWithDto));
        }


        /////////////////////////////////////////////////////////////////////


        private RssItemWithDto BuildRssItemWithUpdatedDescription(
            RssItemDataEditableInput rssItemDataEditableInput, RssItemWithDto currentRssItemWithDto)
        {
            this.ValidateRssItemWithDto(currentRssItemWithDto);

            RssItem updatedRssItem = new RssItem(currentRssItemWithDto.RssItem);
            updatedRssItem.Description = rssItemDataEditableInput.Description;

            /// ⚠️ چرا از CreateRssItemDtoWithSaveImage استفاده می‌کنیم و نه new RssItemDto(کپی)؟
            ///
            /// اگر DTO فعلی را کپی می‌کردیم، ImageFullFilePath همان قبلی می‌ماند. اما آیتم
            /// ممکن است از دیتابیس بارگذاری شده باشد و ImageFullFilePath آن به پوشه‌ی
            /// دائمی (persistent) اشاره کند — در حالی که طبق قرارداد DraftCache، تصویر
            /// هر آیتمی که در DraftCache قرار می‌گیرد باید در پوشه‌ی موقت (temp) باشد.
            ///
            /// CreateRssItemDtoWithSaveImage این قرارداد را تضمین می‌کند:
            ///   ۱. تصویر را از updatedRssItem.Argb8BitImage (که از current.RssItem کپی
            ///      شده) در پوشه‌ی موقت می‌نویسد (با مسیر hash-based بر اساس Id).
            ///   ۲. RssItemDto را با ImageFullFilePath مسیر موقت می‌سازد.
            ///
            /// اگر آیتم از قبل در DraftCache بود و فقط Description عوض شده، این متد فایل
            /// را با همان مسیر موقت (hash-based بر اساس Id) بازنویسی می‌کند — هزینه‌ی
            /// ناچیز، اما تضمین می‌کند که مسیر موقت همیشه درست است.
            RssItemDto updatedRssItemDto = this.draftRssItemDtoFactoryOrchestrator.CreateRssItemDtoWithSaveImage(updatedRssItem, isUnsaved: true);
            updatedRssItemDto.Description = rssItemDataEditableInput.Description;

            return new RssItemWithDto(updatedRssItem, updatedRssItemDto);
        }


        private void ValidateRssItemWithDto(RssItemWithDto currentRssItemWithDto)
        {
            if (currentRssItemWithDto == null || currentRssItemWithDto.RssItem == null || currentRssItemWithDto.RssItemDto == null)
                throw new ArgumentNullException($"مقدار {nameof(currentRssItemWithDto)} یا اعضاهایش نباید خالی باشد .");
        }

    }
}
