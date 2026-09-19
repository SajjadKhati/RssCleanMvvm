using Application.UseCases.DownLoadRssItemsUseCaseModule.Interface;
using ViewModel.Contracts.CompositionRootContracts.RssFeedViewModelFactory.Interface;
using ViewModel.Contracts.CompositionRootContracts.RssItemViewModelFactory.Interface;
using ViewModel.ViewModelModules.RssFeedViewModelModule.Class;

namespace CompositionRoot.Shared.ClassLibrary.Services.RssFeedViewModelFactoryService.Class
{
    public class RssFeedViewModelFactory : IRssFeedViewModelFactory
    {

        private IDownLoadRssItemsUseCase downLoadRssItemsUseCase;

        private IRssItemViewModelFactory rssItemViewModelFactory;


        /////////////////////////////////////////////////////////////////////////////


        public RssFeedViewModelFactory(IDownLoadRssItemsUseCase downLoadRssItemsUseCase, IRssItemViewModelFactory rssItemViewModelFactory)
        {
            this.downLoadRssItemsUseCase = downLoadRssItemsUseCase;
            this.rssItemViewModelFactory = rssItemViewModelFactory;
        }


        /////////////////////////////////////////////////////////////////////////////


        public RssFeedViewModel Create(string id, string title)
        {
            return new RssFeedViewModel(id, title, this.downLoadRssItemsUseCase, this.rssItemViewModelFactory);
        }

    }
}
