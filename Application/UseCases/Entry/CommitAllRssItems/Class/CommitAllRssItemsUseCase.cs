using Application.OverallDataModels.OverallDto.EntityDto;
using Application.UseCases.Entry.CommitAllRssItems.Interface;
using Application.InfrastructureContracts.CacheProject.AppDraftCacheManager.RssItemDraftCacheManager.Interface;
using Application.InfrastructureContracts.DataAccessProject.AppQuery.RssItemDtoQuery.Factory.Interface;
using Application.InfrastructureContracts.DataAccessProject.AppQuery.RssItemDtoQuery.Interface;
using Application.UseCases.Shared.RssItemAllSources.Interface;

namespace Application.UseCases.Entry.CommitAllRssItems.Class
{
    public class CommitAllRssItemsUseCase : ICommitAllRssItemsUseCase, ICommitAllRssItemsNoResultUseCase, IDisposable
    {

        public event EventHandler<bool> HasUnsavedItemsChanged;


        //////////////////////////////////////////////////////////////////////////////


        private IRssItemDraftCacheReader rssItemDraftCacheReader;

        private IRssItemAllSourcesOrchestrator rssItemAllSourcesOrchestrator;

        private IRssItemDtoQueryFactory rssItemDtoQueryFactory;


        //////////////////////////////////////////////////////////////////////////////


        private bool hasUnsaveLast;

        private bool isDisposed;


        //////////////////////////////////////////////////////////////////////////////


        public CommitAllRssItemsUseCase(IRssItemDraftCacheReader rssItemDraftCacheReader, 
            IRssItemAllSourcesOrchestrator rssItemAllSourcesOrchestrator, IRssItemDtoQueryFactory rssItemDtoQueryFactory)
        {
            this.rssItemDraftCacheReader = rssItemDraftCacheReader;
            this.rssItemDraftCacheReader.ItemsCountChanged += this.RssItemDraftCacheReader_ItemsCountChanged;
            this.rssItemAllSourcesOrchestrator = rssItemAllSourcesOrchestrator;
            this.rssItemDtoQueryFactory = rssItemDtoQueryFactory;
            this.isDisposed = false;
        }


        //////////////////////////////////////////////////////////////////////////////


        public bool HasUnsavedItems() => this.rssItemDraftCacheReader.HasAnyItems();
        

        async Task<IList<RssItemDto>> ICommitAllRssItemsUseCase.ExecuteAsync()
        {
            return await Task.Run(() => this.ICommitAllRssItemsUseCase_Execute());
        }


        async Task ICommitAllRssItemsNoResultUseCase.ExecuteAsync()
        {
            await Task.Run(() => this.ICommitAllRssItemsNoResultUseCase_Execute());
        }


        protected void OnHasUnsavedItemsChanged(bool hasUnsavedItems)
        {
            this.HasUnsavedItemsChanged?.Invoke(this, hasUnsavedItems);
        }


        public void Dispose()
        {
            this.Dispose(true);
        }


        //////////////////////////////////////////////////////////////////////////////


        protected virtual void Dispose(bool disposing)
        {
            if (disposing == false || this.isDisposed == true)
                return;
            this.isDisposed = true;
            this.rssItemDraftCacheReader.ItemsCountChanged -= this.RssItemDraftCacheReader_ItemsCountChanged;
            this.HasUnsavedItemsChanged = null;
        }


        private void RssItemDraftCacheReader_ItemsCountChanged(object? sender, int draftCacheItemsCount)
        {
            bool hasUnsaveCurrent = draftCacheItemsCount > 0;
            if (hasUnsaveCurrent == this.hasUnsaveLast)
                return;

            this.hasUnsaveLast = hasUnsaveCurrent;
            this.OnHasUnsavedItemsChanged(hasUnsaveCurrent);
        }


        private IList<RssItemDto> ICommitAllRssItemsUseCase_Execute()
        {
            IList<Uri> commitedRssItemIds = this.rssItemAllSourcesOrchestrator.CommitAllDraftCacheItemsToDatabase();
            return this.FindRssItemDtos(commitedRssItemIds);
        }


        private void ICommitAllRssItemsNoResultUseCase_Execute()
        {
            this.rssItemAllSourcesOrchestrator.CommitAllDraftCacheItemsToDatabase();
        }


        private IList<RssItemDto> FindRssItemDtos(IList<Uri> commitedRssItemIds)
        {
            IList<RssItemDto> rssItemDtos = new List<RssItemDto>();
            using IRssItemDtoQuery rssItemDtoQuery = this.rssItemDtoQueryFactory.Create();

            foreach (Uri commitedRssItemId in commitedRssItemIds)
            {
                RssItemDto rssItemDto = rssItemDtoQuery.FindByKey(commitedRssItemId.AbsoluteUri);
                if (rssItemDto == null)
                    continue;
                rssItemDtos.Add(rssItemDto);
            }
            return rssItemDtos;
        }

    }
}
