using Application.InfrastructureContracts.DataAccessProject.AppRepository.RssItemRepository.DataModels;

namespace Application.Services.RssItemAllSourcesCoordinatorService.Interface
{
    public interface IRssItemAllSourcesCoordinator
    {
        bool CommitDraftCacheItemToDatabase(Uri rssItemId, ImageSaveOptions imageSaveOptions);
    }
}
