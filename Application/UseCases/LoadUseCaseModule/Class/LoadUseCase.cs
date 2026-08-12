using Application.OverallDataModels.OverallDto.EntityDto;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Application.InfrastructureContracts.HttpProject.HttpRequest.Interfaces;
using Application.InfrastructureContracts.HttpProject.HttpRequest.Factory.Interfaces;
using Application.InfrastructureContracts.XmlProject.XmlParser.Interfaces;
using Application.InfrastructureContracts.XmlProject.SharedDataModels.Class;

namespace Application.UseCases.LoadUseCaseModule.Class
{
    public class LoadUseCase
    {

        private IXmlParser xmlParser;

        private IHttpRequestFactory httpRequestFactory;


        ////////////////////


        public LoadUseCase(IXmlParser xmlParser, IHttpRequestFactory httpRequestFactory)
        {
            this.xmlParser = xmlParser;
            this.httpRequestFactory = httpRequestFactory;
        }


        ////////////////////


        public async Task<RssItemDto> ExecuteAsync(Uri rssFeedLink, string xmlParentElementName,
            IList<string> xmlChildElementNames, IList<string> xmlAttributeNames)
        {
            using IHttpRequest httpRequest = httpRequestFactory.Create();
            string rssFeedXmlData = await httpRequest.GetStringAsync(rssFeedLink);

            XmlParsingConfig xmlParsingConfig = new XmlParsingConfig
            {
                xmlData = rssFeedXmlData,
                ParentElementName = xmlParentElementName,
                ChildElementNames = xmlChildElementNames,
                AttributeNames = xmlAttributeNames
            };
            IList<IDictionary<string, string>> rssParsedXml = this.xmlParser.GetXmlData(xmlParsingConfig);

            /// تصاویر را دانلود و مقدار آرایه ی خام تصویر را درون شی دیکشنری که کلیدش همان guid متد GetXmlDataAsync هست ، برمیگرداند .
            /// "مقدار کلید شناسه ی guid" : تصویر خام
            /// مثال :
            /// "https://tasnimnews.ir/3580074"  :  تصویر خام بصورت byte[]
            //IList<IDictionary<string, byte[]>> rawImages = await this.GetByteDataFromLink(tasnimRssParsedXml, guid, url);

            //byte[] imageRawData = await httpRequest.GetByteArrayAsync(new Uri("https://newsmedia.tasnimnews.ir/Tasnim/Uploaded/Image/1405/01/29/1405012916564487036443874.jpg"));



            /////
            return null;
        }


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
