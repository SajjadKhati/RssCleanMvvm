using Application.OverallDataModels.OverallDto.EntityDto;
using ViewModel.Contracts.CompositionRootContracts.RssFeedViewModelFactory.Interface;
using ViewModel.ViewModelModules.RssFeedViewModelModule.Class;
using ViewModel.ViewModelModules.RssSiteViewModelModule.Class;

namespace ViewModel.Services.RssSiteViewModelMapperService.Class
{
    public class RssSiteInfoMapper
    {

        private IRssFeedViewModelFactory rssFeedViewModelFactory;


        /////////////////////////////////////////////////////////////////////////////


        public RssSiteInfoMapper(IRssFeedViewModelFactory rssFeedViewModelFactory)
        {
            this.rssFeedViewModelFactory = rssFeedViewModelFactory;
        }


        /////////////////////////////////////////////////////////////////////////////


        public RssSiteViewModel ToRssSiteViewModel(RssSiteDto rssSiteDto)
        {
            IList<RssFeedViewModel> rssFeedViewModels = rssSiteDto.RssFeedDtos?.Select(
                rssFeedDto => this.rssFeedViewModelFactory.Create(rssFeedDto.Id, rssFeedDto.Title))?.ToList();
            return new RssSiteViewModel(rssSiteDto.Id, rssFeedViewModels, rssSiteDto.SiteName);
        }

    }
}
