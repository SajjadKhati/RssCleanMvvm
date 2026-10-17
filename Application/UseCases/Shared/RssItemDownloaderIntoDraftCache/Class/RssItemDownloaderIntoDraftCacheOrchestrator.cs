using Application.InfrastructureContracts.CacheProject.AppDraftCacheManager.RssItemDraftCacheManager.Interface;
using Application.InfrastructureContracts.XmlProject.SharedDataModels.Class;
using Application.OverallDataModels.OverallDto.CompositeDto;
using Application.OverallDataModels.OverallDto.EntityDto;
using Application.UseCases.Shared.DraftRssItemDtoFactoryOrch.Interface;
using Application.UseCases.Shared.RssItemDownloaderIntoDraftCache.DataModels;
using Application.UseCases.Shared.RssItemDownloaderIntoDraftCache.Interface;
using Application.UseCases.Shared.RssItemsDownLoader.Interface;
using Application.UseCases.Shared.SharedDataModels.Class;
using Domain.DomainEntities.RssEntity.Class;

namespace Application.UseCases.Shared.RssItemDownloaderIntoDraftCache.Class
{
    public class RssItemDownloaderIntoDraftCacheOrchestrator : IRssItemDownloaderIntoDraftCacheOrchestrator
    {

        private IRssItemsDownLoaderOrchestrator rssItemsDownLoaderOrchestrator;

        private IDraftRssItemDtoFactoryOrchestrator draftRssItemDtoFactoryOrchestrator;

        private IRssItemDraftCacheWriter rssItemDraftCacheWriter;


        //////////////////////////////////////////////////////////////////////////////


        public RssItemDownloaderIntoDraftCacheOrchestrator(IRssItemsDownLoaderOrchestrator rssItemsDownLoaderOrchestrator,
            IDraftRssItemDtoFactoryOrchestrator draftRssItemDtoFactoryOrchestrator, IRssItemDraftCacheWriter rssItemDraftCacheWriter)
        {
            this.rssItemsDownLoaderOrchestrator = rssItemsDownLoaderOrchestrator;
            this.draftRssItemDtoFactoryOrchestrator = draftRssItemDtoFactoryOrchestrator;
            this.rssItemDraftCacheWriter = rssItemDraftCacheWriter;
        }


        //////////////////////////////////////////////////////////////////////////////


        public async Task<RssItemsWithDtoReport> DownloadAndStoreInDraftCacheAsync(RssFeedConfig rssFeedConfig)
        {
            RssItemsReport rssItemsBuildingResult = await this.rssItemsDownLoaderOrchestrator.DownLoadRssItemsAsync(rssFeedConfig);
            if (rssItemsBuildingResult.CreatedRssItems == null || rssItemsBuildingResult.CreatedRssItems.Count < 1)
                return new RssItemsWithDtoReport(null, null, rssItemsBuildingResult.CreationErrors);

            IList<RssItemDto> rssItemDtos = this.CreateDtosAndStoreInDraftCache(rssItemsBuildingResult.CreatedRssItems);
            return new RssItemsWithDtoReport(rssItemsBuildingResult.CreatedRssItems, rssItemDtos, rssItemsBuildingResult.CreationErrors);
        }


        //////////////////////////////////////////////////////////////////////////////


        /// <summary>
        /// ساخت DTOها برای آیتم‌های دانلودشده و ذخیره در کش موقت.
        /// 
        /// ⚠️ چرا قفل ندارد؟
        /// 
        /// دلیل اول: عملیات کند (CreateRssItemDto) قبل از دسترسی به داده‌ی مشترک 
        /// انجام می‌شود، نه بین دو دسترسی. قفل فقط وقتی لازم است که یک عملیات کند، 
        /// وسط دو دسترسی به همان داده باشد.
        /// 
        /// دلیل دوم: هر فراخوانی MarkAsAddedInDraftCache خودش اتمی است (write lock 
        /// داخلی‌اش). پس نیازی به قفل بیرونی ندارد.
        /// 
        /// دلیل سوم: هیچ قانونی بین آیتم‌های حلقه نیست — هر RssItem با Id خودش 
        /// مستقل است. پس حتی اگر بین iterateها، نخ دیگری چیزی را تغییر دهد، 
        /// هیچ چیزی در این حلقه خراب نمی‌شود.
        /// </summary>
        /// <param name="rssItems"></param>
        /// <returns></returns>
        private IList<RssItemDto> CreateDtosAndStoreInDraftCache(IList<RssItem> rssItems)
        {
            List<RssItemDto> rssItemDtos = new List<RssItemDto>();
            foreach (RssItem rssItem in rssItems)
            {
                RssItemDto rssItemDto = this.draftRssItemDtoFactoryOrchestrator.CreateRssItemDtoWithSaveImage(rssItem);
                RssItemWithDto rssItemWithDto = new RssItemWithDto(rssItem, rssItemDto);
                /// ⚠️ چرا از متد MarkAsAddedInDraftCache ساده استفاده می‌کنیم و نه از متد نسخه‌ی Try (مثل TryMarkAsAddedIfDraftEntryUnchanged) ؟
                /// 
                /// دلیل اول: کاربر نمی‌تواند آیتمی را ویرایش کند که هنوز در cache نیست.
                /// چون در CreateRssItemAsync (در RssItemBuilder)، قبل از دانلود، متد 
                /// IsRssItemValid(id) چک می‌شود؛ اگر id در cache یا DB موجود باشد، 
                /// آیتم اصلاً دانلود نمی‌شود. پس ویرایشِ کاربر در این فاصله ممکن نیست.
                /// 
                /// دلیل دوم: دو دانلود همزمان از یک feed (CommunityToolkit در VM جلوی 
                /// double-execution در کامند را می‌گیرد)، یا Load + Download همزمان، هر دو از 
                /// همان XML می‌خوانند — پس اگر overwrite رخ دهد، داده‌ی یکسان است و 
                /// ضرری ندارد.
                /// 
                /// دلیل سوم: اگر از Try استفاده می‌کردیم و شکست می‌خورد، rssItemDto 
                /// به لیست rssItemDtos اضافه نمی‌شد؛ در حالی که CreatedRssItems (از 
                /// دانلود) شامل آن آیتم بود. این باعث ناسازگاری در RssItemsWithDtoReport 
                /// می‌شد — که بدتر از یک race بی‌ضرر است.
                this.rssItemDraftCacheWriter.MarkAsAddedInDraftCache(rssItem.Id, rssItemWithDto);
                rssItemDtos.Add(rssItemDto);
            }
            return rssItemDtos;
        }

    }
}
