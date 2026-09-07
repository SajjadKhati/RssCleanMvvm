namespace Application.Services.RssItemCreationServices.RssItemDataExtractorService.Interface
{
    public interface IRssItemDataExtractor
    {

        Uri GetId(string idData);

        Uri GetLink(string linkData);

        DateTime GetPublishDate(string publishDateData);

        Task<byte[,,]> GetArgb8BitImageAsync(string imageLinkData);

    }
}
