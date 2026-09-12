using Application.OverallDataModels.OverallDto.EntityDto;
using ViewModel.ViewModelModules.RssItemViewModelModule.Class;

namespace ViewModel.CompositionRootContracts.RssItemViewModelFactory.Interface
{
    /// <summary>
    /// پیاده سازیِ این رابط ، در لایه ی Composition Root انجام میشه .
    /// تا لازم نباشه شی ای از RssItemViewModel را درون این لایه بسازیم . 
    /// وگرنه باید شی ای از وابستگی هاش که لایه های بعد هستند را هم بسازیم که این ، وظیفه ی Composition Root هست .
    /// </summary>
    public interface IRssItemViewModelFactory
    {
        RssItemViewModel Create(RssItemDto rssItemDto);
    }
}
