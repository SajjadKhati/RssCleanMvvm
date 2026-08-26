using Application.InfrastructureContracts.XmlProject.SharedDataModels.Class;
using Application.InfrastructureContracts.XmlProject.XmlParser.Interfaces;
using Shared.CollectionService.Class;
using System.Xml.Linq;

namespace Infra.XmlParserService.XamlParserService.Class
{
    public class XmlParser : IXmlParser
    {

        /// <summary>
        /// مثال هر شی دیکشنری ، مجموعه ای از کلید و مقدار هست . مثلا یک شی دیکشنری شامل موارد زیر هست :
        /// 
        /// کلید : مقدار
        /// "category" : "https://tasnimnews.ir/3580074"
        /// "category" : "حوزه امام و رهبری"
        /// "content" : "https://newsmedia.tasnimnews.ir/Tasnim/Uploaded/Image/1405/01/29/1405012916564487036443874.jpg"
        /// 
        /// </summary>
        /// <param name="xmlParsingConfig"></param>
        /// <returns></returns>
        public IList<IDictionary<string, string>> GetXmlData(XmlParsingData xmlParsingConfig)
        {
            XDocument xmlDocument = XDocument.Parse(xmlParsingConfig.XmlData);
            IEnumerable<XElement> parentElements = xmlDocument?.Descendants(xmlParsingConfig.ParentElementName);
            if (parentElements == null)
                return null;

            IList<IDictionary<string, string>> parentElementsParsed = new List<IDictionary<string, string>>();

            foreach (XElement parentElement in parentElements)
            {
                var xmlDataValues = new Dictionary<string, string>();

                var parentAttributes = this.GetXmlAttributes(parentElement, xmlParsingConfig.AttributeNames);
                /// متد AddRange ، از لایه ی Shared استفاده میشه . و بصورت مستقیم استفاده میشه .
                /// چون اون ، یک ماژول سطح بالاست و با احتمال تغییر بسیار کم هست .
                if (parentAttributes != null)
                    xmlDataValues.AddRange(parentAttributes);

                var childElements = this.GetXmlChildElements(parentElement, xmlParsingConfig.ChildElementNames, xmlParsingConfig.AttributeNames);
                if (childElements != null)
                    xmlDataValues.AddRange(childElements);

                if (xmlDataValues.Count > 0)
                    parentElementsParsed.Add(xmlDataValues);
            }

            return parentElementsParsed.Count > 0 ? parentElementsParsed : null;
        }


        public IDictionary<string, string> GetXmlChildElements(XElement parentElement, IList<string> childElementNames,
            IList<string> attributeNames = null)
        {
            if (parentElement == null || childElementNames == null || childElementNames.Count < 0)
                return null;

            var xmlDataValues = new Dictionary<string, string>();

            foreach (string childElementName in childElementNames)
            {
                if (string.IsNullOrEmpty(childElementName) == true || string.IsNullOrWhiteSpace(childElementName) == true)
                    continue;

                XElement childElement = parentElement.Element(childElementName);
                if (string.IsNullOrEmpty(childElement?.Value) == false)
                    xmlDataValues[childElementName] = childElement.Value;

                var childAttributes = this.GetXmlAttributes(childElement, attributeNames);
                if (childAttributes != null)
                    xmlDataValues.AddRange(childAttributes);
            }

            return xmlDataValues.Count > 0 ? xmlDataValues : null;
        }


        public IDictionary<string, string> GetXmlAttributes(XElement element, IList<string> attributeNames)
        {
            if (element == null || attributeNames == null || attributeNames.Count < 1)
                return null;

            var attributeDictionary = new Dictionary<string, string>();

            foreach (string attributeName in attributeNames)
            {
                if (string.IsNullOrEmpty(attributeName) == true || string.IsNullOrWhiteSpace(attributeName) == true)
                    continue;

                XAttribute attribute = element.Attribute(attributeName);
                if (string.IsNullOrEmpty(attribute?.Value) == false)
                    attributeDictionary[attributeName] = attribute?.Value;
            }

            return attributeDictionary.Count > 0 ? attributeDictionary : null;
        }

    }
}
