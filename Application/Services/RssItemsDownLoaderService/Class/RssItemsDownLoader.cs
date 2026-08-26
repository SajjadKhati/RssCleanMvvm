using Application.InfrastructureContracts.HttpProject.HttpRequest.Factory.Interfaces;
using Application.InfrastructureContracts.HttpProject.HttpRequest.Interfaces;
using Application.InfrastructureContracts.JsonProject.Json.Interface;
using Application.InfrastructureContracts.XmlProject.SharedDataModels.Class;
using Application.InfrastructureContracts.XmlProject.XmlParser.Interfaces;
using Application.OverallDataModels.OverallDto.EntityDto;
using Application.Services.RssItemsDownLoaderService.Interface;
using Domain.DomainEntities.RssEntity.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.RssItemsDownLoaderService.Class
{
    public class RssItemsDownLoader //: IRssItemsDownLoader
    {

        private IJsonFile jsonFile;

        private IXmlParser xmlParser;

        private IHttpRequestFactory httpRequestFactory;


        ////////////////////


        public RssItemsDownLoader(IJsonFile jsonFile, IXmlParser xmlParser, IHttpRequestFactory httpRequestFactory)
        {
            this.jsonFile = jsonFile;
            this.xmlParser = xmlParser;
            this.httpRequestFactory = httpRequestFactory;
        }


        ////////////////////


        //public RssItem DownLoadRssItems(Uri rssFeedLink, XmlParsingConfig xmlParsingConfig)
        //{
        //    //this.jsonFile.ReadJson<>
        //    using IHttpRequest httpRequest = httpRequestFactory.Create();
        //    string rssFeedXmlData = await httpRequest.GetStringAsync(rssFeedLink);

        //    XmlParsingData xmlParsingConfig = new XmlParsingData
        //    {
        //        XmlData = rssFeedXmlData,
        //        ParentElementName = xmlParentElementName,
        //        ChildElementNames = xmlChildElementNames,
        //        AttributeNames = xmlAttributeNames
        //    };
        //    IList<IDictionary<string, string>> rssParsedXml = this.xmlParser.GetXmlData(xmlParsingConfig);

        //    /// تصاویر را دانلود و مقدار آرایه ی خام تصویر را درون شی دیکشنری که کلیدش همان guid متد GetXmlDataAsync هست ، برمیگرداند .
        //    /// "مقدار کلید شناسه ی guid" : تصویر خام
        //    /// مثال :
        //    /// "https://tasnimnews.ir/3580074"  :  تصویر خام بصورت byte[]
        //    //IList<IDictionary<string, byte[]>> rawImages = await this.GetByteDataFromLink(tasnimRssParsedXml, guid, url);

        //    //byte[] imageRawData = await httpRequest.GetByteArrayAsync(new Uri("https://newsmedia.tasnimnews.ir/Tasnim/Uploaded/Image/1405/01/29/1405012916564487036443874.jpg"));



        //    /////
        //    return null;
        //}


        //public async Task<RssItemDto> ExecuteAsync(Uri rssFeedLink, string xmlParentElementName,
        //    IList<string> xmlChildElementNames, IList<string> xmlAttributeNames)
        //{

        //}

        //private async Task<IList<IDictionary<string, byte[]>>> GetByteDataFromLink(
        //    IList<IDictionary<string, string>> dataLinkContainDictionaries, string guidKey, string dataLinkKey)
        //{
        //    IList<IDictionary<string, byte[]>> rawDataList = new List<IDictionary<string, byte[]>>();

        //    foreach (var dataLinkContainDictionary in dataLinkContainDictionaries)
        //    {
        //        if (dataLinkContainDictionary.ContainsKey(dataLinkKey) == true &&
        //            dataLinkContainDictionary.ContainsKey(guidKey) == true)
        //        {
        //            string dataLink = dataLinkContainDictionary[dataLinkKey];
        //            string guid = dataLinkContainDictionary[guidKey];
        //            byte[] rawData = await this.httpRequest.GetByteArrayAsync(new Uri(dataLink));

        //            if (rawData != null)
        //            {
        //                IDictionary<string, byte[]> rawDataDictionary = new Dictionary<string, byte[]>();
        //                rawDataDictionary.Add(guid, rawData);
        //                rawDataList.Add(rawDataDictionary);
        //            }
        //        }
        //    }

        //    return rawDataList?.Count > 0 ? rawDataList : null;
        //}

    }
}
