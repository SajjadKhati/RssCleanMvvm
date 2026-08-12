using Application.Services.RssItemFactoryService.Interface;
using Domain.DomainEntities.RssEntity.Class;
using System.Globalization;
using Application.Services.ImageNormalizerServices._0_ImageNormalizerService.Interface;
using Application.InfrastructureContracts.HttpProject.HttpRequest.Interfaces;
using Application.InfrastructureContracts.HttpProject.HttpRequest.Factory.Interfaces;
using Application.InfrastructureContracts.ImageProject.ImageDataExtractor.Interfaces;
using Application.InfrastructureContracts.ImageProject.SharedDataModels.Class;

namespace Application.Services.RssItemFactoryService.Class
{
    public class RssItemFactory : IRssItemFactory
    {

        private IHttpRequestFactory httpRequestFactory;

        private IImageDataExtractor imageDataExtractor;

        private IImageNormalizer<byte> imageNormalizer;


        //////////////////////////////////////////////////////////


        public RssItemFactory(IHttpRequestFactory httpRequestFactory, IImageDataExtractor imageDataExtractor,
            IImageNormalizer<byte> imageNormalizer)
        {
            this.httpRequestFactory = httpRequestFactory;
            this.imageDataExtractor = imageDataExtractor;
            this.imageNormalizer = imageNormalizer;
        }


        //////////////////////////////////////////////////////////


        public RssItem CreateRssItem(IDictionary<string, string> rssItemDictionary)
        {
            //string id = rssItemDictionary.ContainsKey()

            //RssItem rssItem = new RssItem();
            //return rssItem;

            throw new NotImplementedException();
        }


        private Uri ParseLink(string link)
        {
            return new Uri(link);
        }


        /// <summary>
        /// فرمت رشته ی زمان ، باید به شکل زیر باشد :
        /// Fri, 01 May 2026 22:04:00 +0330
        /// </summary>
        /// <param name="dateTimeString"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        private DateTime ParsePublishDate(string dateTimeString)
        {
            /// حذف بخش آخر ، یعنی بخش +0330 از زمان .
            var dateTimePartStrings = dateTimeString.Split(' ');
            if (dateTimePartStrings.Length < 5)
                throw new ArgumentNullException($"پارامتر {nameof(dateTimeString)} در الگوی معتبر وارد نشده .");

            dateTimeString = string.Join(" ", dateTimePartStrings, 0, 5);
            // حالا dateTimeString = "Fri, 01 May 2026 20:38:00"

            DateTime dateTime = DateTime.ParseExact(dateTimeString, "ddd, dd MMM yyyy HH:mm:ss",
                CultureInfo.InvariantCulture, DateTimeStyles.None);

            return dateTime;
        }


        private async Task<byte[,,]> CreateAndNormalizeArgb8BitImageFromUrl(Uri imageLink)
        {
            byte[] rawImage = await this.DownloadRawImage(imageLink);
            if (rawImage == null)
                throw new InvalidOperationException("داده ی تصویر ، null هست و مقداری ندارد . دانلود ، شکست خورد .");

            ImageData<byte> normalizedImageData = this.GetNormalizedImageData(rawImage);
            if (normalizedImageData == null || normalizedImageData.Data == null)
                throw new InvalidOperationException("تصویر ، نرمال نشد .");

            return normalizedImageData.Data;
        }


        private async Task<byte[]> DownloadRawImage(Uri imageLink)
        {
            using IHttpRequest httpRequest = this.httpRequestFactory.Create();
            byte[] rawImage = await httpRequest.GetByteArrayAsync(imageLink);
            return rawImage;
        }


        private ImageData<byte> GetNormalizedImageData(byte[] rawImage)
        {
            ImageData<byte> imageData = this.imageDataExtractor.Get8BitImageData(rawImage);
            return imageNormalizer.NormalizeImage(imageData);
        }


    }
}
