using Application.OverallDataModels.OverallDto.EntityDto;
using Application.UseCases.Entry.DownLoadRssItems.DataModels;

namespace Application.UseCases.Entry.LoadRssItems.DataModels
{
    public class LoadRssItemsResult
    {

        /// <summary>
        /// همه‌ی آیتم‌ها (DB + download)، مرتب‌شده بر اساس PublishDate نزولی.
        /// </summary>
        public IList<RssItemDto> AllRssItemDtos { get; init; }


        /// <summary>
        /// نتیجه‌ی بخش دانلود (شامل CreatedRssItemDtos, Errors, WasUserCancelled).
        /// </summary>
        public RssItemDtosResult DownloadedResult { get; init; }

    }
}
