using Application.InfrastructureContracts.DataAccessProject.AppQuery.RssItemDtoQuery.Factory.Interface;
using Application.InfrastructureContracts.DataAccessProject.AppQuery.RssItemDtoQuery.Interface;
using Application.OverallDataModels.OverallDto.EntityDto;
using Application.UseCases.Entry.DownLoadRssItems.DataModels;
using Application.UseCases.Entry.DownLoadRssItems.Interface;
using Application.UseCases.Entry.LoadRssItems.DataModels;
using Application.UseCases.Entry.LoadRssItems.Interface;

namespace Application.UseCases.Entry.LoadRssItems.Class
{
    public class LoadRssItemsUseCase : ILoadRssItemsUseCase
    {

        //////////////////////////////////////////////////////////////////////////////////////////////

        private IRssItemDtoQueryFactory rssItemDtoQueryFactory;

        private IDownLoadRssItemsUseCase downLoadRssItemsUseCase;


        //////////////////////////////////////////////////////////////////////////////////////////////


        public LoadRssItemsUseCase(IRssItemDtoQueryFactory rssItemDtoQueryFactory, IDownLoadRssItemsUseCase downLoadRssItemsUseCase)
        {
            this.rssItemDtoQueryFactory = rssItemDtoQueryFactory;
            this.downLoadRssItemsUseCase = downLoadRssItemsUseCase;
        }


        //////////////////////////////////////////////////////////////////////////////////////////////


        public async Task<LoadRssItemsResult> ExecuteAsync(Uri rssFeedId, CancellationToken cancellationToken)
        {
            return await Task.Run(() => this.ExecuteAsyncCore(rssFeedId, cancellationToken), cancellationToken);
        }


        //////////////////////////////////////////////////////////////////////////////////////////////


        private async Task<LoadRssItemsResult> ExecuteAsyncCore(Uri rssFeedId, CancellationToken cancellationToken)
        {
            /// ۱. شروع موازی: DB (sync، در thread pool) + Download (async).
            ///    هر دو Task بدون await شروع می‌شوند (موازی).
            ///    خطای یکی، روی دیگری اثر نمی‌گذارد (چون هر کدوم مستقل await می‌شوند).
            Task<IList<RssItemDto>> loadingFromDatabaseTask = Task.Run(() => this.LoadRssItemDtosFromDatabase(rssFeedId));
            Task<RssItemDtosResult> downloadTask = this.DownloadRssItemsSafelyAsync(rssFeedId, cancellationToken);

            IList<RssItemDto> rssItemDtosFromDb = await loadingFromDatabaseTask;
            // ۳. Download (خطا → wrap در RssItemDtosResult)
            RssItemDtosResult downloadedRssItemDtosResult = await downloadTask;
            // ۴. ترکیب و مرتب‌سازی بر اساس PublishDate (نزولی)
            IList<RssItemDto> mergeAndSortedRssItemDtos = this.MergeAndSortByPublishDate(rssItemDtosFromDb, downloadedRssItemDtosResult);

            return new LoadRssItemsResult
            {
                DownloadedResult = downloadedRssItemDtosResult,
                AllRssItemDtos = mergeAndSortedRssItemDtos
            };
        }


        /// <summary>
        /// Load آیتم‌های فید از دیتابیس (sync)، تا در thread pool اجرا شود.
        /// </summary>
        /// <param name="rssFeedId"></param>
        /// <returns></returns>
        private IList<RssItemDto> LoadRssItemDtosFromDatabase(Uri rssFeedId)
        {
            using IRssItemDtoQuery rssItemDtoQuery = this.rssItemDtoQueryFactory.Create();
            return (IList<RssItemDto>)rssItemDtoQuery.FindByFeedId(rssFeedId.AbsoluteUri);
        }


        private async Task<RssItemDtosResult> DownloadRssItemsSafelyAsync(Uri rssFeedId, CancellationToken cancellationToken)
        {
            try
            {
                return await this.downLoadRssItemsUseCase.ExecuteAsync(rssFeedId, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                // لغو قبل از شروع (Task.Run) → no items downloaded، کاربر لغو کرد
                return new RssItemDtosResult(Array.Empty<RssItemDto>(), new List<Exception>(), wasUserCancelled: true);
            }
            catch (Exception exception)
            {
                return new RssItemDtosResult(Array.Empty<RssItemDto>(), new List<Exception> { exception }, wasUserCancelled: false);
            }
        }


        /// <summary>
        /// ادغام DB + Download و مرتب‌سازی نزولی بر اساس PublishDate.
        /// ترتیب نهایی، دقیقاً بر اساس تاریخ انتشار است — نه ترتیب append.
        /// </summary>
        /// <param name="fromDbDtos"></param>
        /// <param name="downloadedResult"></param>
        /// <returns></returns>
        private IList<RssItemDto> MergeAndSortByPublishDate(IList<RssItemDto> rssItemDtos, RssItemDtosResult rssItemDtosResult)
        {
            List<RssItemDto> allDtos = new();

            if (rssItemDtosResult.CreatedRssItemDtos != null)
                allDtos.AddRange(rssItemDtosResult.CreatedRssItemDtos);

            if (rssItemDtos != null)
                allDtos.AddRange(rssItemDtos);

            return allDtos
                .OrderByDescending(dto => dto.PublishDate)
                .ToList();
        }

    }
}
