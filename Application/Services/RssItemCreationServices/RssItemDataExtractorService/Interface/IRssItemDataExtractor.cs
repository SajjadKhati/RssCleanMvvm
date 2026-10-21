using Application.InfrastructureContracts.ImageProject.SharedDataModels.Class;

namespace Application.Services.RssItemCreationServices.RssItemDataExtractorService.Interface
{
    public interface IRssItemDataExtractor
    {

        Uri GetId(string idData);

        Uri GetLink(string linkData);

        DateTime GetPublishDate(string publishDateData);

        Task<ImageData<byte>> GetArgb8BitImageDataAsync(string imageLinkData, CancellationToken cancellationToken = default);

    }
}
