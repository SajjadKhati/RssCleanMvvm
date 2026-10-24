using Application.InfrastructureContracts.DataAccessProject.AppQuery.RssItemDtoQuery.Factory.Interface;
using Application.InfrastructureContracts.DataAccessProject.AppQuery.RssItemDtoQuery.Interface;
using Application.OverallDataModels.OverallDto.EntityDto;
using Application.UseCases.Entry.CommitRssItem.Interface;
using Application.UseCases.Shared.RssItemAllSources.Interface;

namespace Application.UseCases.Entry.CommitRssItem.Class
{
    public class CommitRssItemUseCase : ICommitRssItemUseCase
    {

        private IRssItemAllSourcesOrchestrator rssItemAllSourcesOrchestrator;

        private IRssItemDtoQueryFactory RssItemDtoQueryFactory;


        //////////////////////////////////////////////////////////////////////////////////


        public CommitRssItemUseCase(IRssItemAllSourcesOrchestrator rssItemAllSourcesOrchestrator, 
            IRssItemDtoQueryFactory rssItemDtoQueryFactory)
        {
            this.rssItemAllSourcesOrchestrator = rssItemAllSourcesOrchestrator;
            this.RssItemDtoQueryFactory = rssItemDtoQueryFactory;
        }


        //////////////////////////////////////////////////////////////////////////////////


        public async Task<RssItemDto> ExecuteAsync(Uri rssItemId)
        {
            return await Task.Run(()=>this.Execute(rssItemId));
        }


        //////////////////////////////////////////////////////////////////////////////////


        private RssItemDto Execute(Uri rssItemId)
        {
            /// لازم به چک کردن وجود یا عدم وجود شناسه ی rssItemId قبل از فراخوانی متد CommitDraftCacheItemToDatabase نیست . 
            /// چون این متد درون خودش ، این بررسی را انجام میده .
            bool isCommitedToDb = this.rssItemAllSourcesOrchestrator.CommitDraftCacheItemToDatabase(rssItemId);
            if (isCommitedToDb == false)
                return null;

            using IRssItemDtoQuery rssItemDtoQuery = this.RssItemDtoQueryFactory.Create();
            return rssItemDtoQuery.FindByKey(rssItemId.AbsoluteUri);
        }

    }
}
