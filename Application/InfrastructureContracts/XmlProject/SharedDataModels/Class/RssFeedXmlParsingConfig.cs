namespace Application.InfrastructureContracts.XmlProject.SharedDataModels.Class
{
    public class RssFeedXmlParsingConfig : XmlParsingConfig
    {

        public Uri RssFeedId { get; set; }


        ////////////////////////////////////////////////////////////////


        public RssFeedXmlParsingConfig()
        {

        }


        public RssFeedXmlParsingConfig(Uri rssFeedId, string parentElementName, IList<string> childElementNames, IList<string> attributeNames = null) :
            base(parentElementName, childElementNames, attributeNames)
        {
            this.RssFeedId = rssFeedId;
        }

    }
}
