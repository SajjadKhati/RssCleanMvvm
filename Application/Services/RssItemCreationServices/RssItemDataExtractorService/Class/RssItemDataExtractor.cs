using Application.Services.RssItemCreationServices.RssItemDataExtractorService.Interface;
using Application.InfrastructureContracts.ImageProject.ImageDataExtractor.Interfaces;
using Application.InfrastructureContracts.SharedProject.DateTimes.DateTimeParser.Interface;
using Application.Services.ImageNormalizerServices._0_ImageNormalizerService.Interface;
using Application.InfrastructureContracts.HttpProject.HttpRequest.Interfaces;
using Application.InfrastructureContracts.HttpProject.HttpRequest.Factory.Interfaces;
using Application.InfrastructureContracts.ImageProject.SharedDataModels.Class;

namespace Application.Services.RssItemCreationServices.RssItemDataExtractorService.Class
{
    public class RssItemDataExtractor : IRssItemDataExtractor
    {

        private IDateTimeParser dateTimeParser;

        private IHttpRequestFactory httpRequestFactory;

        private IImageDataExtractor imageDataExtractor;

        private IImageNormalizer<byte> imageNormalizer;


        ///////////////////////////////////////////////////////////////////


        public RssItemDataExtractor(IDateTimeParser dateTimeParser, IHttpRequestFactory httpRequestFactory,
            IImageDataExtractor imageDataExtractor, IImageNormalizer<byte> imageNormalizer)
        {
            this.dateTimeParser = dateTimeParser;
            this.httpRequestFactory = httpRequestFactory;
            this.imageDataExtractor = imageDataExtractor;
            this.imageNormalizer = imageNormalizer;
        }


        ///////////////////////////////////////////////////////////////////


        public Uri GetId(string idData)
        {
            return new Uri(idData);
        }


        public Uri GetLink(string linkData)
        {
            return new Uri(linkData);
        }


        public DateTime GetPublishDate(string publishDateData)
        {
            return this.dateTimeParser.ParseDateTime(publishDateData);
        }


        public async Task<byte[,,]> GetArgb8BitImageAsync(string imageLinkData)
        {
            return await Task.Run(() => this.GetArgb8BitImage(imageLinkData));
        }


        ///////////////////////////////////////////////////////////////////


        private async Task<byte[,,]> GetArgb8BitImage(string imageLinkData)
        {
            Uri imageLink = new Uri(imageLinkData);
            byte[] rawImage = await this.DownloadRawImage(imageLink);
            ImageData<byte> imageData = this.imageDataExtractor.Get8BitImageData(rawImage);
            ImageData<byte> normalizedImageData = this.imageNormalizer.NormalizeImage(imageData);
            return normalizedImageData.Data;
        }


        private async Task<byte[]> DownloadRawImage(Uri imageLink)
        {
            using IHttpRequest httpRequest = this.httpRequestFactory.Create();
            return await httpRequest.GetByteArrayAsync(imageLink);
        }

    }
}
