using Application.UseCases.Entry.DownLoadRssItems.Interface;
using Application.UseCases.Entry.LoadRssItems.Interface;
using ViewModel.Contracts.CompositionRootContracts.RssFeedViewModelFactory.Interface;
using ViewModel.Contracts.CompositionRootContracts.RssItemViewModelFactory.Interface;
using ViewModel.ViewModelModules.RssFeedViewModelModule.Class;

namespace CompositionRoot.Shared.ClassLibrary.Services.RssFeedViewModelFactoryService.Class
{
    public class RssFeedViewModelFactory : IRssFeedViewModelFactory
    {

        private ILoadRssItemsUseCase loadRssItemsUseCase;

        private IDownLoadRssItemsUseCase downLoadRssItemsUseCase;

        private IRssItemViewModelFactory rssItemViewModelFactory;


        /////////////////////////////////////////////////////////////////////////////


        public RssFeedViewModelFactory(ILoadRssItemsUseCase loadRssItemsUseCase, 
            IDownLoadRssItemsUseCase downLoadRssItemsUseCase, IRssItemViewModelFactory rssItemViewModelFactory)
        {
            this.loadRssItemsUseCase = loadRssItemsUseCase;
            this.downLoadRssItemsUseCase = downLoadRssItemsUseCase;
            this.rssItemViewModelFactory = rssItemViewModelFactory;
        }


        /////////////////////////////////////////////////////////////////////////////


        public RssFeedViewModel Create(string id, string title)
        {
            return new RssFeedViewModel(id, title, this.loadRssItemsUseCase, this.downLoadRssItemsUseCase, this.rssItemViewModelFactory);
        }

    }
}
