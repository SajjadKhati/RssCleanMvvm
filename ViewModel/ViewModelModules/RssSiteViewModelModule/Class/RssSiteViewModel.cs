using CommunityToolkit.Mvvm.ComponentModel;
using ViewModel.Services.RangeObservableCollectionService.Class;
using ViewModel.ViewModelModules.RssFeedViewModelModule.Class;

namespace ViewModel.ViewModelModules.RssSiteViewModelModule.Class
{
    public class RssSiteViewModel : ObservableObject
    {

        private RssFeedViewModel _selectedRssFeedViewModel;


        /////////////////////////////////////////////////////////////////////////////


        public string Id { get; }

        public RangeObservableCollection<RssFeedViewModel> RssFeedViewModels { get; }

        public string SiteName { get; }


        /////////////////////////////////////////////////////////////////////////////


        public RssFeedViewModel SelectedRssFeedViewModel
        {
            get => this._selectedRssFeedViewModel;
            set => this.SetProperty(ref this._selectedRssFeedViewModel, value);
        }


        /////////////////////////////////////////////////////////////////////////////


        public RssSiteViewModel(string id, IEnumerable<RssFeedViewModel> rssFeedInfos, string siteName)
        {
            this.Id = id;
            this.RssFeedViewModels = new RangeObservableCollection<RssFeedViewModel>(rssFeedInfos);
            this.SiteName = siteName;
        }

    }
}
