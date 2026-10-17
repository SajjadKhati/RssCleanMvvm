using Application.InfrastructureContracts.CacheProject.AppDraftCacheManager.DraftCacheWriter.Interface;
using Application.InfrastructureContracts.CacheProject.CacheBase.Cache.Interfaces;
using Application.InfrastructureContracts.DataAccessProject.AppQuery.RssItemDtoQuery.Factory.Interface;
using Application.InfrastructureContracts.DataAccessProject.AppQuery.RssItemDtoQuery.Interface;
using Application.InfrastructureContracts.DataAccessProject.AppQuery.RssSiteQuery.Factory.Interface;
using Application.InfrastructureContracts.DataAccessProject.AppQuery.RssSiteQuery.Interface;
using Application.InfrastructureContracts.DataAccessProject.AppRepository.RssItemRepository.DataModels;
using Application.InfrastructureContracts.DataAccessProject.AppRepository.RssItemRepository.Factory.Interfaces;
using Application.InfrastructureContracts.DataAccessProject.AppRepository.RssItemRepository.Interfaces;
using Application.InfrastructureContracts.DataAccessProject.DataAccessBase.DatabaseInitialization.Interface;
using Application.InfrastructureContracts.DataAccessProject.DataAccessBase.UnitOfWork.Interfaces;
using Application.InfrastructureContracts.DataAccessProject.UnitOfWorkFactory.Interfaces;
using Application.InfrastructureContracts.HttpProject.HttpRequest.Factory.Interfaces;
using Application.InfrastructureContracts.HttpProject.HttpRequest.Interfaces;
using Application.InfrastructureContracts.ImageProject.ImageFileReader.Interfaces;
using Application.InfrastructureContracts.ImageProject.ImageFileSaver.DataModels.Class;
using Application.InfrastructureContracts.ImageProject.ImageFileSaver.DataModels.Enum;
using Application.InfrastructureContracts.ImageProject.ImageFileSaver.Interfaces;
using Application.InfrastructureContracts.ImageProject.SharedDataModels.Class;
using Application.InfrastructureContracts.ImageProject.SharedDataModels.Enum;
using Application.InfrastructureContracts.SharedProject.DateTimes.DateTimeParser.Interface;
using Application.InfrastructureContracts.SharedProject.DateTimes.PersianCalenderConvertor.Interface;
using Application.InfrastructureContracts.SharedProject.FilePathMaker.DataModels;
using Application.InfrastructureContracts.SharedProject.FilePathMaker.Interfaces;
using Application.InfrastructureContracts.SharedProject.SharedMapper.RssItemDtoMapper.Interface;
using Application.InfrastructureContracts.XmlProject.SharedDataModels.Class;
using Application.InfrastructureContracts.XmlProject.XmlParser.Interfaces;
using Application.OverallDataModels.OverallDto.EntityDto;
using Application.Services.ImageNormalizerServices._0_ImageNormalizerService.Class;
using Application.Services.ImageNormalizerServices._0_ImageNormalizerService.Interface;
using Application.Services.ImageNormalizerServices._1_ImageChannelCountNormalizerService.Factory.Class;
using Application.Services.ImageNormalizerServices._2_ImageChannelSequenceNormalizerService.Factory.Class;
using Application.Services.ImageNormalizerServices.ImageInfoValidatorService.Class;
using Domain.DomainEntities.RssEntity.Class;
using Infra.CacheService.AppDraftCacheManagerService.DraftCacheWriterService.Class;
using Infra.CacheService.CacheBaseServices.CacheServices.Class;
using Infra.DataAccess.AppQueryService.RssItemDtoQueryService.Factory.Class;
using Infra.DataAccess.AppQueryService.RssSiteQueryService.Class.Factory;
using Infra.DataAccess.AppRepositoryService.RssItemRepositoryService.Factory.Class;
using Infra.DataAccess.DbContextService.Class;
using Infra.DataAccess.EfEntities.Class;
using Infra.DataAccess.EfEntityMapperService.Class;
using Infra.DataAccess.ExpressionFactoryService.Class;
using Infra.DataAccess.RssItemDtoEfEntityFactoryService.Class;
using Infra.DataAccess.RssItemDtoEfEntityFactoryService.Interface;
using Infra.DataAccess.UnitOfWorkFactoryService.Class;
using Infra.HttpService.HttpRequestService.Class;
using Infra.HttpService.HttpRequestService.Factory.Class;
using Infra.ImageService.ImageDataExtractorService.Class;
using Infra.ImageService.ImageFileReaderService.Class;
using Infra.ImageService.ImageFileSaverService.Class;
using Infra.ImageService.InfraImageCreatorService.Class;
using Infra.ImageService.MappingService.Class;
using Infra.SharedService.DateTimesServices.DateTimeParserService.Class;
using Infra.SharedService.DateTimesServices.PersianCalenderConvertorService.Class;
using Infra.SharedService.FilePathMakerService.Class;
using Infra.SharedService.ImageFileExtensionResolverService.Class;
using Infra.SharedService.MapperService.ImageSettingsMapperService.Class;
using Infra.SharedService.StringHasherService.Class;
using Infra.XmlService.XamlParserService.Class;
using Shared.ImageServices.ImageUtilityService.Class;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace xUnitTest.Infrastructure.HttpServiceTest
{
    public class HttpRequestTest
    {

        private ICache<Uri, RssItemEfEntity> _persistenceCache;


        private ICache<Uri, RssItemEfEntity> PersistenceCache
        {
            get
            {
                if (this._persistenceCache == null)
                    this._persistenceCache = new Cache<Uri, RssItemEfEntity>();
                return this._persistenceCache;
            }
        }




        /// //////////////////////////////////////////////////////////////
        




        //[Fact]
        //public async Task LoadUseCaseTest()
        //{
        //    IHttpRequestFactory httpRequestFactory = new HttpRequestFactory();
        //    IXmlParser xmlParser = new XmlParser();
        //    LoadRssItemsUseCase loadUseCase = new LoadRssItemsUseCase(xmlParser, httpRequestFactory);

        //    Uri tasnimRssFeedLink = new Uri("https://www.tasnimnews.ir/fa/rss/feed/0/0/8/1/TopStories");
        //    string tasnimXmlParentElementName = "item";
        //    var tasnimXmlChildElementNames = new List<string>()
        //    {
        //        "guid",
        //        "link",
        //        "category",
        //        "title",
        //        "description",
        //        "pubDate",
        //        "{http://search.yahoo.com/mrss/}content"
        //    };
        //    string tasnimImageUrl = "url";
        //    var tasnimXmlAttributeNames = new List<string>() { tasnimImageUrl };

        //    var test = await loadUseCase.ExecuteAsync(tasnimRssFeedLink, tasnimXmlParentElementName, 
        //        tasnimXmlChildElementNames, tasnimXmlAttributeNames);

        //}


        [Fact]
        public async Task ImageTest()
        {
            //HttpRequest httpRequest = new HttpRequest(new HttpClient());
            //byte[] rawData = await httpRequest.GetByteArrayAsync(new Uri("https://newsmedia.tasnimnews.ir/Tasnim/Uploaded/Image/1405/01/29/1405012916564487036443874.jpg"));

            //E:\Tasavir\Shahidan\Emam Khameneee\1.jpg
            //E:\Tasavir\0 Behyar Sanat Sepahan\16bitImages\Boards 1600 Height.tif
            //E:\Tasavir\0 Behyar Sanat Sepahan\32Bit Image\Boards 1600 Height Tiff.tif
            byte[] rawData = File.ReadAllBytes("E:\\Tasavir\\Shahidan\\Emam Khameneee\\1.jpg");
            //File.WriteAllBytes("F:\\Laptop\\raw.jpg", rawData);

            /////
            var imageInfoMemberMapper = new ImageInfoMemberMapper();
            ImageDataExtractor imageExtractor = new ImageDataExtractor(new InfraImageCreator(), imageInfoMemberMapper);
            ImageData<byte> imageData = imageExtractor.Get8BitImageData(rawData);

            /////

            ImageUtility imageUtility = new ImageUtility();
            ImageFileSaver imageSaver = new ImageFileSaver(imageUtility, new ImageFileSettingsMemberMapper(imageInfoMemberMapper));
            imageSaver.Save8BitImageToFile(this.CreateArgb8BitImageSaverSettings(imageData.Data, "F:\\Laptop\\ConvertedImageData.png"));
        }


        [Fact]
        public void ImageNormalizeTest_1()
        {
            byte[,,] imgData = new byte[2,3,3]
            {
                //y =0 قرمز و سبز و آبی
                {
                    {255, 0, 0 },
                    { 0, 255, 0},
                    { 0, 0, 255}
                },
                // y = 1  سیاه و خاکستری و سفید
                {
                    { 0, 0, 0},
                    { 128, 128, 128},
                    { 255, 255, 255}
                }
            };

            ImageInfo imageInfo = new ImageInfo
            {
                Height = imgData.GetLength(0),
                Width = imgData.GetLength(1),
                ChannelsCount = imgData.GetLength(2),
                ColorDepth = 8,
                ColorSpace = ColorSpace.RGB,
                ChannelsSequence = (new List<PixelChannel> { PixelChannel.Red, PixelChannel.Green, PixelChannel.Blue }).AsReadOnly()
            };
            ImageData<byte> imageData = new ImageData<byte>(imageInfo, imgData);

            IImageNormalizer<byte> imageNormalizer = new Argb8BitImageNormalizer(
                new ImageInfoValidator(), new ImageChannelCountNormalizerFactory(), new ImageChannelSequenceNormalizerFactory());

            var normalizedImageData = imageNormalizer.NormalizeImage(imageData);

            ///
            var imageInfoMemberMapper = new ImageInfoMemberMapper();
            ImageUtility imageUtility = new ImageUtility();
            ImageFileSaver imageSaver = new ImageFileSaver(imageUtility, new ImageFileSettingsMemberMapper(imageInfoMemberMapper));
            imageSaver.Save8BitImageToFile(this.CreateArgb8BitImageSaverSettings(normalizedImageData.Data, "F:\\Laptop\\ConvertedImageData.png"));
        }


        [Fact]
        public void ImageNormalizeTest_2()
        {
            byte[,,] imgData = new byte[2, 3, 3]
            {
                //y =0 قرمز و سبز و آبی
                {
                    {0, 0, 255},
                    { 0, 255, 0},
                    { 255, 0, 0}
                },
                // y = 1  سیاه و خاکستری و سفید
                {
                    { 0, 0, 0},
                    { 128, 128, 128},
                    { 255, 255, 255}
                }
            };

            ImageInfo imageInfo = new ImageInfo
            {
                Height = imgData.GetLength(0),
                Width = imgData.GetLength(1),
                ChannelsCount = imgData.GetLength(2),
                ColorDepth = 8,
                ColorSpace = ColorSpace.RGB,
                ChannelsSequence = (new List<PixelChannel> {PixelChannel.Blue, PixelChannel.Green, PixelChannel.Red}).AsReadOnly()
            };
            ImageData<byte> imageData = new ImageData<byte>(imageInfo, imgData);

            IImageNormalizer<byte> imageNormalizer = new Argb8BitImageNormalizer(
                new ImageInfoValidator(), new ImageChannelCountNormalizerFactory(), new ImageChannelSequenceNormalizerFactory());

            var normalizedImageData = imageNormalizer.NormalizeImage(imageData);

            ///

            var imageInfoMemberMapper = new ImageInfoMemberMapper();
            ImageUtility imageUtility = new ImageUtility();
            ImageFileSaver imageSaver = new ImageFileSaver(imageUtility, new ImageFileSettingsMemberMapper(imageInfoMemberMapper));
            imageSaver.Save8BitImageToFile(this.CreateArgb8BitImageSaverSettings(normalizedImageData.Data, "F:\\Laptop\\ConvertedImageData.png"));
        }



        [Fact]
        public void ImageNormalizeTest_3()
        {
            //F:\Laptop\1.png
            //E:\Tasavir\Shahidan\Emam Khameneee\1.jpg
            byte[] rawData = File.ReadAllBytes("E:\\Tasavir\\Shahidan\\Emam Khameneee\\1.jpg");

            /////
            
            var imageInfoMemberMapper = new ImageInfoMemberMapper();
            ImageDataExtractor imageExtractor = new ImageDataExtractor(new InfraImageCreator(), imageInfoMemberMapper);
            ImageData<byte> imageData = imageExtractor.Get8BitImageData(rawData);

            IImageNormalizer<byte> imageNormalizer = new Argb8BitImageNormalizer(
                new ImageInfoValidator(), new ImageChannelCountNormalizerFactory(), new ImageChannelSequenceNormalizerFactory());

            var normalizedImageData = imageNormalizer.NormalizeImage(imageData);

            ///

            ImageUtility imageUtility = new ImageUtility();
            ImageFileSaver imageSaver = new ImageFileSaver(imageUtility, new ImageFileSettingsMemberMapper(imageInfoMemberMapper));
            imageSaver.Save8BitImageToFile(this.CreateArgb8BitImageSaverSettings(normalizedImageData.Data, "F:\\Laptop\\ConvertedImageData.png"));
        }


        [Fact]
        public void DatabaseTest_4()
        {
            var database = this.GetRssItemRepository();
            IRssItemRepository rssItemRepository = database.rssItemRepository;
            using IUnitOfWork unitOfWork = database.unitOfWork;

            rssItemRepository.Add(this.GetRssItem(), this.GetImageSaveOptions(@"D:\Tasavir\Test"));
            unitOfWork.SaveChanges();
        }


        [Fact]
        public void RssItemFactoryTest_5()
        {
            var abc = this.ParsePublishDate("Fri, 01 May 2026 22:04:00 +0330");
        }


        [Fact]
        public void DatabaseTest_6()
        {
            IRssSiteQueryFactory rssSiteQueryFactory = this.GetRssSiteQueryFactory();
            using IRssSiteQuery rssSiteQuery = rssSiteQueryFactory.Create();
            IList<RssSiteDto> rssSiteDtos = rssSiteQuery.FindAllEagerLoading();
        }


        [Fact]
        public void DatabaseTest_7()
        {
            var database = this.GetRssItemRepository();
            IRssItemRepository rssItemRepository = database.rssItemRepository;
            using (IUnitOfWork unitOfWork = database.unitOfWork)
            {
                rssItemRepository.Add(this.GetRssItem(), this.GetImageSaveOptions(@"D:\Tasavir\Test"));
                unitOfWork.SaveChanges();
            }

            using (IRssItemDtoQuery rssItemDtoQuery = this.GetRssItemDtoQuery())
            {
                //IEnumerable<RssItemDto> rssItemDtos = rssItemDtoQuery.FindByFeedId();
            }
        }


        [Fact]
        public async Task RssFactoryTest_8()
        {
            IXmlParser xmlParser = new XmlParser();
            using IHttpRequest httpRequest = new HttpRequest(new HttpClient());
            string rssFeedXmlData = await httpRequest.GetStringAsync(new Uri("https://www.tasnimnews.ir/fa/rss/feed/1/0/7/0/MostPupolar"));

            string tasnimXmlParentElementName = "item";
            var tasnimXmlChildElementNames = new List<string>()
            {
                "guid",
                "link",
                "category",
                "title",
                "description",
                "pubDate",
                "{http://search.yahoo.com/mrss/}content"
            };
            string tasnimImageUrl = "url";
            var tasnimXmlAttributeNames = new List<string>() { tasnimImageUrl };

            XmlParsingData xmlParsingConfig = new XmlParsingData
            {
                XmlData = rssFeedXmlData,
                ParentElementName = tasnimXmlParentElementName,
                ChildElementNames = tasnimXmlChildElementNames,
                AttributeNames = tasnimXmlAttributeNames
            };
            IList<IDictionary<string, string>> rssParsedXml = xmlParser.GetXmlData(xmlParsingConfig);
            //Uri tasnimRssFeedLink = new Uri("https://www.tasnimnews.ir/fa/rss/feed/0/0/8/1/TopStories");
        }


        [Fact]
        public async Task DateTimeTest_9()
        {
            IDateTimeParser dateTimeParser = new DateTimeParser();
            var dateTime = dateTimeParser.ParseDateTime("Mon, 31 Aug 2026 15:31:00 +0330");
            IPersianCalenderConvertor persianCalenderConvertor = new PersianCalenderConvertor();
            string persianCalenderDate = persianCalenderConvertor.ToPersianDateString(dateTime);
        }


        [Fact]
        public async Task DateTimeTest_10()
        {
            IDateTimeParser dateTimeParser = new DateTimeParser();
            var dateTime = dateTimeParser.ParseDateTime("13 Sep 2026 13:28:59 +0330");
            IPersianCalenderConvertor persianCalenderConvertor = new PersianCalenderConvertor();
            string persianCalenderDate = persianCalenderConvertor.ToPersianDateString(dateTime);
        }


        [Fact]
        public async Task FilePathMakerTest_11()
        {
            FilePathMaker filePathMaker = new FilePathMaker(new StringHasher());
            FullFilePath fullFilePath = new FullFilePath(
                "C:\\Users\\sajja\\AppData\\Local\\Temp",
                new Uri("https://www.yjc.ir/fa/news/9128684/رزمایش-و-اجتماع-بزرگ-مردم-جان-فدای-ایران-روز-جمعه-برگزار-می‌شود").AbsoluteUri,
                "png"
                );
            DestinationPath destinationPath = filePathMaker.CreateDestinationPathWithHashedFileName(fullFilePath);
        }

        [Fact]
        public async Task OtherNewsBaseTest_12()
        {


            //IRssItemDownloaderIntoDraftCache draftCache = this.GetRssItemDownloaderIntoDraftCache();
            //ICache<Uri, RssFeedConfig> rssFeedConfigCache = this.GetRssFeedConfigCache();
            //DownLoadRssItemsUseCase downLoadRssItemsUseCase = new DownLoadRssItemsUseCase(draftCache, rssFeedConfigCache);

            //var abc = await downLoadRssItemsUseCase.ExecuteAsync();
        }




        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////





        //private IRssItemDownloaderIntoDraftCache GetRssItemDownloaderIntoDraftCache()
        //{
        //    return new RssItemDownloaderIntoDraftCache()
        //}

        private ICache<Uri, RssFeedConfig> GetRssFeedConfigCache()
        {
            return new Cache<Uri, RssFeedConfig>();
        }



        private IRssItemDtoQuery GetRssItemDtoQuery()
        {
            throw new NotImplementedException();
            //IRssItemDtoQueryFactory rssItemDtoQueryFactory = new RssItemDtoQueryFactory(
            //    this.GetConnectionString(), this.PersistenceCache, new RssItemDtoEfEntityMapper(new PersianCalenderConvertor()), this.GetExpressionFactory());
            //return rssItemDtoQueryFactory.Create();
        }


        private RssItemEfEntityExpressionFactory GetExpressionFactory()
        {
            return new RssItemEfEntityExpressionFactory();
        }


        private DateTime ParsePublishDate(string dateStr)
        {
            if (string.IsNullOrWhiteSpace(dateStr))
                return DateTime.MinValue;

            // بخش تاریخ-زمان (بدون offset) را استخراج کن
            var parts = dateStr.Split(' ');
            if (parts.Length < 5)
                return DateTime.MinValue;

            string dateOnly = string.Join(" ", parts, 0, 5); // 5 بخش اول
            // حالا dateOnly = "Fri, 01 May 2026 20:38:00"

            if (DateTime.TryParseExact(dateOnly, "ddd, dd MMM yyyy HH:mm:ss",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result))
            {
                return result;
            }

            return DateTime.MinValue; // یا هر مقدار پیش‌فرض/لاگ خطا
        }


        private IRssSiteQueryFactory GetRssSiteQueryFactory()
        {
            return new RssSiteQueryFactory(this.GetConnectionString(), new RssSiteEfEntityMapper(new RssFeedEfEntityMapper()));
        }


        private (IRssItemRepository rssItemRepository, IUnitOfWork unitOfWork) GetRssItemRepository()
        {
            IRssItemRepositoryFactory rssItemRepositoryFactory = this.GetRssItemRepositoryFactory();
            IUnitOfWorkFactory unitOfWorkFactory = this.GetUnitOfWorkFactory();
            IUnitOfWork unitOfWork = unitOfWorkFactory.Create(false);
            return (rssItemRepositoryFactory.Create(unitOfWork), unitOfWork);
        }


        private IRssItemRepositoryFactory GetRssItemRepositoryFactory()
        {
            throw new NotImplementedException();

            //RssItemEfEntityMapper efEntityMapper = new RssItemEfEntityMapper();
            //IDraftCacheWriter draftCacheWriter = new DraftCacheWriter();
            //ImageUtility imageUtility = new ImageUtility();
            //IImageFileSaver imageFileSaver = new ImageFileSaver(imageUtility, new ImageFileSettingsMemberMapper(new ImageInfoMemberMapper()));
            //IFilePathMaker filePathMaker = new FilePathMaker(new StringHasher());
            //ImageSettingsMapper imageSettingsMapper = new ImageSettingsMapper();
            //IImageFileSaverCoordinator imageFileSaverCoordinator = new ImageFileSaverCoordinator(imageFileSaver, 
            //    new ImageFileExtensionResolver(), filePathMaker, imageSettingsMapper);
            //IImageFileReader imageFileReader = new ImageFileReader(new ImageDataExtractor(new InfraImageCreator(), new ImageInfoMemberMapper()));
            //IRssItemDtoMapper rssItemDtoMapper = new Application.Services.Mapper.RssItemDtoMapper(new PersianCalenderConvertor());
            //IRssItemEfEntityFactory rssItemEfEntityFactory = new RssItemDtoEfEntityFactory(imageFileSaverCoordinator, rssItemDtoMapper, efEntityMapper);
            //return new RssItemRepositoryFactory(efEntityMapper, this.PersistenceCache, draftCacheWriter, imageFileReader, 
            //    rssItemEfEntityFactory, this.GetExpressionFactory());
        }


        private IUnitOfWorkFactory GetUnitOfWorkFactory()
        {
            string connectionString = this.GetConnectionString();
            return new UnitOfWorkFactory(connectionString);
        }


        private string GetConnectionString()
        {
            return @"Server=(localdb)\MSSQLLocalDB;Database=RssDb;Trusted_Connection=True;Encrypt=True;TrustServerCertificate=True;";
        }


        private RssItem GetRssItem()
        {
            Uri id = new Uri("https://tasnimnews.ir/3660402");
            Uri rssFeedId = new Uri("https://www.tasnimnews.ir/fa/rss/feed/1127/0/7/0/MostPupolar");
            Uri link = new Uri("https://downloadly.ir/software/audio-video-editors/powerdirector-1/");
            string category = "دسته بندی خبر";
            string title = "عنوان خبر";
            string description = "توضیح خبر";
            DateTime publishDate = DateTime.Now;
            byte[,,] argb8bitImageData = new byte[2, 3, 4]
            {
                //y =0 قرمز و سبز و آبی
                {
                    {255, 255, 0, 0}, // x = 0
                    { 255, 0, 255, 0}, // x = 1
                    { 255, 0, 0, 255} // x = 2
                },
                // y = 1  سیاه و خاکستری و سفید
                {
                    { 255, 0, 0, 0}, // x = 0
                    { 255, 128, 128, 128}, // x = 1
                    { 255, 255, 255, 255} // x = 2
                }
            };

            return new RssItem(id, rssFeedId, link, argb8bitImageData, category, title, description, publishDate);
        }





        //////////


        private ImageFileSaveSettings CreateArgb8BitImageSaverSettings(byte[,,] image, string fullFilePath)
        {
            return new ImageFileSaveSettings
            {
                FullFilePath = fullFilePath,
                ImageFileSettings = this.GetImageDataSettings(image)
            };
        }


        private ImageSaveOptions GetImageSaveOptions(string folderPath)
        {
            return new ImageSaveOptions
            {
                ImageInfoSettings = this.GetImageInfoSettings(),
                FolderPath = folderPath
            };
        }


        private ImageSettings<ImageData<byte>> GetImageDataSettings(byte[,,] image)
        {
            return new ImageSettings<ImageData<byte>>(this.GetImageSettingsBase(), this.GetImageData(image));
        }


        private ImageSettings<ImageInfo> GetImageInfoSettings()
        {
            return new ImageSettings<ImageInfo>(this.GetImageSettingsBase(), this.GetImageInfo());
        }


        private ImageSettingsBase GetImageSettingsBase()
        {
            return new ImageSettingsBase
            {
                StorageType = StorageType.Byte,
                Mapping = PixelMapping.ARGB,
                HaveAlphaChannel = AlphaOption.On,
                Format = MagickFormat.Png32
            };
        }

        private ImageData<byte> GetImageData(byte[,,] image)
        {
            return new ImageData<byte>(this.GetImageInfo(), image);
        }


        private ImageInfo GetImageInfo()
        {
            return new ImageInfo
            {
                ColorSpace = ColorSpace.sRGB,
                ColorDepth = 8
            };
        }


    }


}
