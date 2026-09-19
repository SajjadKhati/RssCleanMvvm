using ViewModel.ViewModelModules.RssFeedViewModelModule.Class;

namespace ViewModel.Contracts.CompositionRootContracts.RssFeedViewModelFactory.Interface
{
    public interface IRssFeedViewModelFactory
    {
        /// <summary>
        /// پیاده سازیِ این رابط ، در لایه ی Composition Root انجام میشه .
        /// تا لازم نباشه شی ای از RssFeedViewModel را درون این لایه بسازیم . 
        /// وگرنه باید شی ای از وابستگی هاش که لایه های بعد هستند را هم بسازیم که این ، وظیفه ی Composition Root هست .
        /// </summary>
        RssFeedViewModel Create(string id, string title);
    }
}
