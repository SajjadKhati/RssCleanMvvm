namespace Application.InfrastructureContracts.XmlProject.SharedDataModels.Class
{
    public class RssFeedXmlParsingConfig : XmlParsingConfig
    {

        public Uri RssFeedLink { get; set; }


        ////////////////////////////////////////////////////////////////


        public RssFeedXmlParsingConfig()
        {

        }


        public RssFeedXmlParsingConfig(Uri rssFeedLink, string parentElementName, IList<string> childElementNames, IList<string> attributeNames = null) :
            base(parentElementName, childElementNames, attributeNames)
        {
            this.RssFeedLink = rssFeedLink;
        }

    }
}
